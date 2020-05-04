using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.ClassParser;
using System.IO;

namespace CommandLineSerializer
{
    public class WriteClassCreationHeader
    {
        public static bool writeClassCreationHeader(string strSourceDirectory, string strClassCreationHeaderFile, SerializerController mController)
        {
            ProjectWrapper mProjectWrapper = mController.getProjectWrapper();
            string strClassENumName = "(int32)ESERIALIZED_ENUMS::ESERIALIZED_CLASSES";
            StringWriter mHeaderWriter = new StringWriter();
            mHeaderWriter.WriteLine("#pragma once" + Environment.NewLine + Environment.NewLine);
            mHeaderWriter.WriteLine("#include \"EnumsByName.h\"");
            ///////////////////////////write headers///////////////////////////////////
            List<string> mHeadersAdded = new List<string>();
            foreach (ClassStructure mClass in mProjectWrapper.classStructures.Values)
            {
                if (mClass.isSerialized)
                {
                    if (mHeadersAdded.Contains(mClass.file) == false)
                    {
                        string strFullPath = Path.GetFullPath(mClass.file);
                        //mHeaderWriter.WriteLine("#include \"" + mClass.file.Substring(strSourceDirectory.Length, mClass.file.Length - strSourceDirectory.Length).Replace('\\', '/') + "\"\t\t\t//" + mClass.name);
                        mHeaderWriter.WriteLine("#include \"" + strFullPath + "\"\t\t\t//" + mClass.name);
                        mHeadersAdded.Add(mClass.file);
                    }
                }
            }
            ///////////////////////////writing new function////////////////////////////
            mHeaderWriter.WriteLine(@"



class _SERIALIZE_CLASS_CREATION_
{
public:
    static void * createObject(KCString strClassName)
    {
        int32 iLookUp = _SERIALIZER_ENUMS_::_getEnumValueByName(" + strClassENumName + @", strClassName);
        switch(iLookUp)
        {
            default:
                return nullptr;");
            int iClassIndex = 0;
            foreach (ClassStructure mClass in mProjectWrapper.classStructures.Values)
            {
                if (mClass.isSerialized)
                {
                    mHeaderWriter.WriteLine("            case " + iClassIndex.ToString() + ":    //" + mClass.name );
                    mHeaderWriter.WriteLine("                return KC_NEW " + mClass.name + ";");                    
                    iClassIndex++;
                }
            }

            mHeaderWriter.WriteLine(@"

        }
        return nullptr;
    } //end createObject");
            ///////////////////////////writing template functions////////////////////////////
            foreach (ClassStructure mClass in mProjectWrapper.classStructures.Values)
            {
                if( mClass.isSerialized == false )
                {
                    continue;
                }
                mHeaderWriter.WriteLine("    static bool addObject(TArray<" + mClass.name + "> &mList, KCString strClassName)");
                mHeaderWriter.WriteLine("    {");
                mHeaderWriter.WriteLine("       int32 iLookUp = _SERIALIZER_ENUMS_::_getEnumValueByName(" + strClassENumName + ", strClassName);");
                mHeaderWriter.WriteLine("       if(iLookUp < 0 || iLookUp >= " + strClassENumName + "){return false;}");
                mHeaderWriter.WriteLine("       switch((ESERIALIZED_ENUMS)iLookUp)");
                mHeaderWriter.WriteLine("       {");
                mHeaderWriter.WriteLine("           default:");
                mHeaderWriter.WriteLine("               mList.Add(" + mClass.name + "());");
                mHeaderWriter.WriteLine("               return true;");
                foreach (ClassStructure mChildClass in mProjectWrapper.classStructures.Values)
                {
                    if( mChildClass.isSerialized == false ||
                        mChildClass == mClass ||
                        mChildClass.classStructuresInheritingFrom.Contains(mClass.name) == false )
                    {
                        continue;
                    }
                    mHeaderWriter.WriteLine("           case ESERIALIZED_ENUMS::" + mChildClass.name + ":");
                    mHeaderWriter.WriteLine("               mList.Add(" + mChildClass.name + "());");
                    mHeaderWriter.WriteLine("               return true;");
                }
                mHeaderWriter.WriteLine("       }");
                mHeaderWriter.WriteLine("    } //end addObject " + mClass.name);
            }
            ///////////////////////////writing template functions////////////////////////////
            foreach (ClassStructure mClass in mProjectWrapper.classStructures.Values)
            {
                if (mClass.isSerialized == false)
                {
                    continue;
                }
                mHeaderWriter.WriteLine("    static " + mClass.name + " createObject(KCString strClassName)");
                mHeaderWriter.WriteLine("    {");
                mHeaderWriter.WriteLine("       int32 iLookUp = _SERIALIZER_ENUMS_::_getEnumValueByName(" + strClassENumName + ", strClassName);");
                mHeaderWriter.WriteLine("       if(iLookUp < 0 || iLookUp >= " + strClassENumName + "){return false;}");
                mHeaderWriter.WriteLine("       switch((ESERIALIZED_ENUMS)iLookUp)");
                mHeaderWriter.WriteLine("       {");
                mHeaderWriter.WriteLine("           default:");
                mHeaderWriter.WriteLine("               return " + mClass.name + "());");                
                foreach (ClassStructure mChildClass in mProjectWrapper.classStructures.Values)
                {
                    if (mChildClass.isSerialized == false ||
                        mChildClass == mClass ||
                        mChildClass.classStructuresInheritingFrom.Contains(mClass.name) == false)
                    {
                        continue;
                    }
                    mHeaderWriter.WriteLine("           case ESERIALIZED_ENUMS::" + mChildClass.name + ":");
                    mHeaderWriter.WriteLine("               return (" + mClass.name + ")" + mChildClass.name + "();");                    
                }
                mHeaderWriter.WriteLine("       }");
                mHeaderWriter.WriteLine("    } //end addObject " + mClass.name);
            }

            //end the class
            mHeaderWriter.WriteLine("}; //end class _SERIALIZE_CLASS_CREATION_");

            string strFile = mHeaderWriter.ToString();
            try
            {



                string strOldFile = File.ReadAllText(strClassCreationHeaderFile);
                if (strFile == strOldFile)
                {
                    mController.log(strClassCreationHeaderFile + " - didn't change. Not resaving.");
                    return false;
                }
            }
            catch
            {

            }
            File.WriteAllText(strClassCreationHeaderFile, mHeaderWriter.ToString());
            mController.log("resaving " + strClassCreationHeaderFile);
            return true;
        }

    } //end class
} //end namespace
