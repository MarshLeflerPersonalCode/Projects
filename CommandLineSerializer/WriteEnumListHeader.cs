using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.ClassParser;
using System.IO;

namespace CommandLineSerializer
{
    class WriteEnumListHeader
    {        
        public static bool writeEnumListHeader(string strEnumHeaderFile, SerializerController mController)
        {
            ProjectWrapper mProjectWrapper = mController.getProjectWrapper();

            //First - lets get the list of enums because it's a map it might not be in order and we want to make sure the file doesn't change.
            List<EnumList> mEnumOrderedList = new List<EnumList>();
            foreach (EnumList mEnum in mProjectWrapper.enums.Values)
            {
                while(mEnumOrderedList.Count <= mEnum.uniqueID)
                {
                    mEnumOrderedList.Add(null);
                }
                mEnumOrderedList[mEnum.uniqueID] = mEnum;
            }
            int iClassEnumID = mEnumOrderedList.Count;

            StringWriter mHeaderWriter = new StringWriter();
            mHeaderWriter.WriteLine("#pragma once" + Environment.NewLine + Environment.NewLine);
            mHeaderWriter.WriteLine("#include \"KCIncludes.h\"" + Environment.NewLine + Environment.NewLine);
            /////////////////////enum the enums!!
            mHeaderWriter.WriteLine("enum class ESERIALIZED_ENUMS");
            mHeaderWriter.WriteLine("{");            
            foreach (EnumList mEnum in mEnumOrderedList)
            {
                mHeaderWriter.WriteLine("     " + mEnum.enumName + " = " + mEnum.uniqueID.ToString() + ",");
            }
            mHeaderWriter.WriteLine("     ESERIALIZED_CLASSES = " + iClassEnumID.ToString());
            mHeaderWriter.WriteLine("};" + Environment.NewLine + Environment.NewLine);
            /////////////////////enum the enums!
            /////////////////////enum the classes!
            mHeaderWriter.WriteLine("enum class ESERIALIZED_CLASSES");
            mHeaderWriter.WriteLine("{");
            int iClassIndex = 0;
            foreach (ClassStructure mClass in mProjectWrapper.classStructures.Values)
            {
                if (mClass.isSerialized)
                {
                    mHeaderWriter.WriteLine("     " + mClass.name + " = " + iClassIndex.ToString() + ",");
                    iClassIndex++;
                }
            }
            mHeaderWriter.WriteLine("};" + Environment.NewLine + Environment.NewLine);
            /////////////////////enum the classes!

            mHeaderWriter.WriteLine("class _SERIALIZER_ENUMS_");
            mHeaderWriter.WriteLine("{");
            mHeaderWriter.WriteLine("public:");
            mHeaderWriter.WriteLine("static int32 _getEnumValueByName(int32 iEnumID, const KCString &strEnumItemName)");
            mHeaderWriter.WriteLine("{");
            mHeaderWriter.WriteLine("     static std::unordered_map<int32, std::unordered_map<KCString, int32>> m_EnumLookUpByIDAndName;");
            mHeaderWriter.WriteLine("     if(m_EnumLookUpByIDAndName.size() > 0 )");
            mHeaderWriter.WriteLine("     {");
            mHeaderWriter.WriteLine("          auto mValue = m_EnumLookUpByIDAndName.find(iEnumID);");
            mHeaderWriter.WriteLine("          if (mValue != m_EnumLookUpByIDAndName.end())");
            mHeaderWriter.WriteLine("          {");
            mHeaderWriter.WriteLine("               return -1;");
            mHeaderWriter.WriteLine("          }");
            mHeaderWriter.WriteLine("          auto mValue2 = mValue->second.find(strEnumItemName);");
            mHeaderWriter.WriteLine("          if (mValue2 != mValue->second.end())");
            mHeaderWriter.WriteLine("          {");
            mHeaderWriter.WriteLine("              return -1;");
            mHeaderWriter.WriteLine("          }");
            mHeaderWriter.WriteLine("          return mValue2->second;");            
            mHeaderWriter.WriteLine("     }");

            ////////////////////////write the enums found
            foreach (EnumList mEnumList in mEnumOrderedList)
            {
                mHeaderWriter.WriteLine("     {");
                mHeaderWriter.WriteLine("          m_EnumLookUpByIDAndName[" + mEnumList.uniqueID + "] = std::unordered_map<KCString, int32>();");
                mHeaderWriter.WriteLine("          std::unordered_map<KCString, int32> &mNameToValue = m_EnumLookUpByIDAndName[" + mEnumList.uniqueID + "];");
                mHeaderWriter.Write("          ");
                int iIndex = 0;
                foreach (EnumItem mItem in mEnumList.enumItems)
                {
                    mHeaderWriter.Write("mNameToValue[\"" + mItem.name + "\"] = " + iIndex.ToString() + ";");
                    iIndex++;
                }
                mHeaderWriter.WriteLine("");
                mHeaderWriter.WriteLine("     }");

            }
            ///////////////////////end write the enums
            ///////////////////////write the enums for classes!
            mHeaderWriter.WriteLine("     {");
            mHeaderWriter.WriteLine("          ///////////////////////CLASS LOOKUP////////////////////////////");
            mHeaderWriter.WriteLine("          m_EnumLookUpByIDAndName[" + iClassEnumID + "] = std::unordered_map<KCString, int32>();");
            mHeaderWriter.WriteLine("          std::unordered_map<KCString, int32> &mNameToValue = m_EnumLookUpByIDAndName[" + iClassEnumID + "];");
            mHeaderWriter.Write("          ");
            iClassIndex = 0;
            foreach (ClassStructure mClass in mProjectWrapper.classStructures.Values)
            {
                if (mClass.isSerialized)
                {
                    mHeaderWriter.Write("mNameToValue[\"" + mClass.name + "\"] = " + iClassIndex.ToString() + ";");
                    iClassIndex++;
                }
            }
            mHeaderWriter.WriteLine("");
            mHeaderWriter.WriteLine("          ///////////////////////CLASS LOOKUP////////////////////////////");
            mHeaderWriter.WriteLine("     }");
            /////////////////////end write the enums for classes!


            mHeaderWriter.WriteLine("    return _getEnumValueByName(iEnumID, strEnumItemName);");
            mHeaderWriter.WriteLine("}");



            mHeaderWriter.WriteLine("static const KCString & _getEnumItemNameByValue(int32 iEnumID, int32 iValue)");
            mHeaderWriter.WriteLine("{");
            mHeaderWriter.WriteLine("     static std::unordered_map<int32, std::unordered_map<int32, KCString>> m_EnumLookUpByIDAndValue;");
            mHeaderWriter.WriteLine("     if(m_EnumLookUpByIDAndValue.size() > 0 )");
            mHeaderWriter.WriteLine("     {");
            mHeaderWriter.WriteLine("          static KCString g_Empty;");
            mHeaderWriter.WriteLine("          auto mValue = m_EnumLookUpByIDAndValue.find(iEnumID);");
            mHeaderWriter.WriteLine("          if (mValue != m_EnumLookUpByIDAndValue.end())");
            mHeaderWriter.WriteLine("          {");
            mHeaderWriter.WriteLine("               return g_Empty;");
            mHeaderWriter.WriteLine("          }");
            mHeaderWriter.WriteLine("          auto mValue2 = mValue->second.find(iValue);");
            mHeaderWriter.WriteLine("          if (mValue2 != mValue->second.end())");
            mHeaderWriter.WriteLine("          {");
            mHeaderWriter.WriteLine("              return g_Empty;");
            mHeaderWriter.WriteLine("          }");
            mHeaderWriter.WriteLine("          return mValue2->second;");
            mHeaderWriter.WriteLine("     }");
            //////////////////////Enum look up by ID//////////////////////////
            foreach (EnumList mEnumList in mEnumOrderedList)
            {
                mHeaderWriter.WriteLine("     {");
                mHeaderWriter.WriteLine("          m_EnumLookUpByIDAndValue[" + mEnumList.uniqueID + "] = std::unordered_map<int32,KCString>();");
                mHeaderWriter.WriteLine("          std::unordered_map<int, KCString> &mValueToName = m_EnumLookUpByIDAndValue[" + mEnumList.uniqueID + "];");
                mHeaderWriter.Write("          ");
                int iIndex = 0;
                foreach (EnumItem mItem in mEnumList.enumItems)
                {
                    mHeaderWriter.Write("mValueToName[" + iIndex.ToString() + "] = \"" + mItem.name + "\";");
                    iIndex++;
                }
                mHeaderWriter.WriteLine("");
                mHeaderWriter.WriteLine("     }");

            }
            //////////////////////end Enum look up by ID//////////////////////////
            ///////////////////////write the enums for classes look up by ID!
            mHeaderWriter.WriteLine("     {");
            mHeaderWriter.WriteLine("          ///////////////////////CLASS LOOKUP////////////////////////////");
            mHeaderWriter.WriteLine("          m_EnumLookUpByIDAndValue[" + iClassEnumID + "] = std::unordered_map<int32,KCString>();");
            mHeaderWriter.WriteLine("          std::unordered_map<int, KCString> &mValueToName = m_EnumLookUpByIDAndValue[" + iClassEnumID + "];");
            mHeaderWriter.Write("          ");
            iClassIndex = 0;
            foreach (ClassStructure mClass in mProjectWrapper.classStructures.Values)
            {
                if (mClass.isSerialized)
                {
                    mHeaderWriter.Write("mValueToName[" + iClassIndex.ToString() + "] = \"" + mClass.name + "\";");
                    iClassIndex++;
                }
            }
            mHeaderWriter.WriteLine("");
            mHeaderWriter.WriteLine("          ///////////////////////CLASS LOOKUP////////////////////////////");
            mHeaderWriter.WriteLine("     }");
            /////////////////////end write the enums for classes look up by ID!
            mHeaderWriter.WriteLine("     return _getEnumItemNameByValue(iEnumID,iValue);");
            mHeaderWriter.WriteLine("}");



            mHeaderWriter.WriteLine("static const TArray<KCString> & _getEnumItemNamesByEnumName(const KCString &strEnumName)");
            mHeaderWriter.WriteLine("{");
            mHeaderWriter.WriteLine("    static std::unordered_map<KCString, TArray<KCString>> m_EnumLookUpByName;");
            mHeaderWriter.WriteLine("    if(m_EnumLookUpByName.size() > 0 )");
            mHeaderWriter.WriteLine("    {");
            mHeaderWriter.WriteLine("          static TArray<KCString> g_ReturnEmpty;");
            mHeaderWriter.WriteLine("          auto mValue = m_EnumLookUpByName.find(strEnumName);");
            mHeaderWriter.WriteLine("          if (mValue != m_EnumLookUpByName.end())");
            mHeaderWriter.WriteLine("          {");
            mHeaderWriter.WriteLine("               return g_ReturnEmpty;");
            mHeaderWriter.WriteLine("          }");
            mHeaderWriter.WriteLine("          return mValue->second;");
            mHeaderWriter.WriteLine("    }");
            //////////////////////Enum list by enum name look up//////////////////////////
            foreach (EnumList mEnumList in mEnumOrderedList)
            {
                mHeaderWriter.WriteLine("     {");
                mHeaderWriter.WriteLine("          m_EnumLookUpByName[\"" + mEnumList.enumName + "\"] = TArray<KCString>();");
                mHeaderWriter.WriteLine("          TArray<KCString> &mArray1 = m_EnumLookUpByName[\"" + mEnumList.enumName + "\"];");
                mHeaderWriter.WriteLine("          mArray1.Reserve(" + mEnumList.enumItems.Count.ToString() + ");");
                mHeaderWriter.Write("          ");
                int iIndex = 0;
                foreach (EnumItem mItem in mEnumList.enumItems)
                {
                    mHeaderWriter.Write("mArray1.Add(\"" + mItem.name + "\");");
                    iIndex++;
                }
                mHeaderWriter.WriteLine("");
                mHeaderWriter.WriteLine("     }");

            }
            //////////////////////end Enum list by enum name look up//////////////////////////
            ///////////////////////write class list look up by the enum name 
            mHeaderWriter.WriteLine("     {");
            mHeaderWriter.WriteLine("          ///////////////////////CLASS LOOKUP////////////////////////////");
            mHeaderWriter.WriteLine("          m_EnumLookUpByName[\"ESERIALIZED_CLASSES\"] = TArray<KCString>();");
            mHeaderWriter.WriteLine("          TArray<KCString> &mArray1 = m_EnumLookUpByName[\"ESERIALIZED_CLASSES\"];");
            mHeaderWriter.WriteLine("          mArray1.Reserve(" + iClassIndex.ToString() + ");");
            mHeaderWriter.Write("          ");
            iClassIndex = 0;
            foreach (ClassStructure mClass in mProjectWrapper.classStructures.Values)
            {
                if (mClass.isSerialized)
                {
                    mHeaderWriter.Write("mArray1.Add(\"" + mClass.name + "\");");
                    iClassIndex++;
                }
            }
            mHeaderWriter.WriteLine("");
            mHeaderWriter.WriteLine("          ///////////////////////CLASS LOOKUP////////////////////////////");
            mHeaderWriter.WriteLine("     }");
            ///////////////////////end write class list look up by the enum name 

            mHeaderWriter.WriteLine("     return _getEnumItemNamesByEnumName(strEnumName);");
            mHeaderWriter.WriteLine("}");
            mHeaderWriter.WriteLine("}; //end SERIALIZER_ENUMS");
            string strFile = mHeaderWriter.ToString();
            try
            {



                string strOldFile = File.ReadAllText(strEnumHeaderFile);
                if (strFile == strOldFile)
                {
                    mController.log("EnumsByName.h - didn't change. Not resaving.");
                    return false;
                }
            }
            catch
            {

            }
            File.WriteAllText(strEnumHeaderFile, mHeaderWriter.ToString());
            mController.log("resaving EnumsByName.h.");
            return true;
        }

    }//end class
}//end name space
