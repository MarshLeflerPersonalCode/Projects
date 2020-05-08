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




        public static bool writeClassCreationHeader(string strIntermediateFolder, SerializerController mController)
        {
            bool bModifiedFiles = false;
            ProjectWrapper mProjectWrapper = mController.getProjectWrapper();
            string strHeaderFileName = strIntermediateFolder + "ClassCreation.h";
            StringWriter mHeaderWriter = new StringWriter();
            mHeaderWriter.WriteLine("#pragma once" + Environment.NewLine + Environment.NewLine);
            mHeaderWriter.WriteLine("#include \"EnumsByName.h\"");
            mHeaderWriter.WriteLine("");
            mHeaderWriter.WriteLine("class _SERIALIZE_CLASS_CREATION_");
            mHeaderWriter.WriteLine("{");
            mHeaderWriter.WriteLine("public:");
            mHeaderWriter.WriteLine("static void * createObject(const KCString &strClassName, const KCString &strParentClass);");            
            mHeaderWriter.WriteLine("}; //end _SERIALIZE_CLASS_CREATION_");
            
            try
            {
                string strHeaderFileText = mHeaderWriter.ToString();
                string strOldFile = "";
                if (File.Exists(strHeaderFileName))
                {
                    strOldFile = File.ReadAllText(strHeaderFileName);
                }
                if (strHeaderFileText == strOldFile)
                {
                    mController.log("ClassCreation.h - didn't change. Not resaving.");                    
                }
                else
                {
                    bModifiedFiles = true;
                    File.WriteAllText(strHeaderFileName, strHeaderFileText);
                    mController.log("Modified ClassCreation.h.");
                }
            }
            catch
            {
                mController.log("unable to update ClassCreation.h.");
            }
            
            

            ////////////////////////////////////////////////////
            ////write the class code file
            ////////////////////////////////////////////////////
            string strClassEnumName = "(int32)ESERIALIZED_ENUMS::ESERIALIZED_CLASSES";
            string strCodeFileName = strIntermediateFolder + "ClassCreation.cpp";
            StringWriter mCodeWriter = new StringWriter();
            mCodeWriter.WriteLine("#include \"ClassCreation.h\"");
            List<string> mHeadersAdded = new List<string>();
            foreach (ClassStructure mClass in mProjectWrapper.classStructures.Values)
            {
                if (mClass.isSerialized &&
                    mHeadersAdded.Contains(mClass.file) == false)
                {
                    mCodeWriter.WriteLine("#include \"" + Path.GetFullPath(mClass.file) + "\"");
                    mHeadersAdded.Add(mClass.file);
                }
            }

            mCodeWriter.WriteLine("void* _SERIALIZE_CLASS_CREATION_::createObject(const KCString &strClassName, const KCString &strParentClass)");
            mCodeWriter.WriteLine("{");
            mCodeWriter.WriteLine("    int32 iLookUp = _SERIALIZER_ENUMS_::_getEnumValueByName(" + strClassEnumName + ", strClassName);");
            mCodeWriter.WriteLine("    switch (iLookUp)");
            mCodeWriter.WriteLine("    {");
            mCodeWriter.WriteLine("        default:");
            mCodeWriter.WriteLine("            {");
            mCodeWriter.WriteLine("                 KCEnsureAlwaysMsg(false, \"ERROR - Class Name wasn't found in serialized objects: \" + strClassName + \". It probably was either deleted or renamed.\");");
            mCodeWriter.WriteLine("                 if(strClassName != strParentClass)");
            mCodeWriter.WriteLine("                 {");
            mCodeWriter.WriteLine("                     return _SERIALIZE_CLASS_CREATION_::createObject(strParentClass, strParentClass);");
            mCodeWriter.WriteLine("                 }");
            mCodeWriter.WriteLine("            }");
            mCodeWriter.WriteLine("            return nullptr; ");
            int iClassIndex = 0;
            foreach (ClassStructure mClass in mProjectWrapper.classStructures.Values)
            {
                if (mClass.isSerialized == false)
                {
                    continue;
                }
                mCodeWriter.WriteLine("            case " + iClassIndex.ToString() + ":    //" + mClass.name);
                mCodeWriter.WriteLine("                return KC_NEW " + mClass.name + ";");
                iClassIndex++;
            }


            mCodeWriter.WriteLine("   }");
            mCodeWriter.WriteLine("   return nullptr;");
            mCodeWriter.WriteLine("} //end createObject");

            try
            {
                string strCodeFileText = mCodeWriter.ToString();
                string strOldFile = "";
                if (File.Exists(strCodeFileText))
                {
                    strOldFile = File.ReadAllText(strCodeFileText);
                }

                if (strCodeFileText == strOldFile)
                {
                    mController.log("ClassCreation.cpp - didn't change. Not resaving.");
                }
                else
                {
                    bModifiedFiles = true;
                    File.WriteAllText(strCodeFileName, strCodeFileText);
                    mController.log("Modified ClassCreation.cpp.");
                }
            }
            catch
            {
                mController.log("unable to update ClassCreation.cpp.");
            }


            return bModifiedFiles;
        }






























        /*
        public static bool writeClassCreationHeaders(string strIntermediateFolder, SerializerController mController)
        {

            ProjectWrapper mProjectWrapper = mController.getProjectWrapper();
            foreach (ClassStructure mClass in mProjectWrapper.classStructures.Values)
            {
                if (mClass.isSerialized)
                {
                    _writeHeaderFile(mClass, strIntermediateFolder, mController);
                }
            }
            return true;

        }
        private static void _writeHeaderFile(ClassStructure mClassWriting, string strIntermediateFolder, SerializerController mController)
        {
            string strClassEnumName = "(int32)ESERIALIZED_ENUMS::ESERIALIZED_CLASSES";
            ProjectWrapper mProjectWrapper = mController.getProjectWrapper();
            string strHeaderFileName = strIntermediateFolder + "ClassCreation_" + mClassWriting.name + ".h";
            StringWriter mCodeWriter = new StringWriter();
            mCodeWriter.WriteLine("#pragma once" + Environment.NewLine + Environment.NewLine);
            mCodeWriter.WriteLine("#include \"EnumsByName.h\"");
            if( mClassWriting.isClass)
            {
                mCodeWriter.WriteLine("class " + mClassWriting.name + ";");
            }
            else
            {
                mCodeWriter.WriteLine("struct " + mClassWriting.name + ";");
            }
            
            ///////////////////////////write headers///////////////////////////////////            
            List<ClassStructure> mClassesUsing = new List<ClassStructure>();
            foreach (ClassStructure mClass in mProjectWrapper.classStructures.Values)
            {
                if( mClass.isSerialized == false )
                {
                    continue;
                }
                if(mClass == mClassWriting ||
                   mClass.classStructuresInheritingFrom.Contains(mClassWriting.name))
                {
                    mClassesUsing.Add(mClass);
                }
            }
            List<string> mHeadersAdded = new List<string>();
            foreach (ClassStructure mClass in mClassesUsing)
            {
                if (mHeadersAdded.Contains(mClass.file) == false)
                {                                                
                    //mCodeWriter.WriteLine("#include \"" + Path.GetFullPath(mClass.file) + "\"");
                    mHeadersAdded.Add(mClass.file);
                }
            }
            ///////////////////////////writing new function////////////////////////////
            mCodeWriter.WriteLine(@"
class _SERIALIZE_CLASS_CREATION_" + mClassWriting.name + @"
{
public:
    static void * createObject(KCString strClassName)
    {
        int32 iLookUp = _SERIALIZER_ENUMS_::_getEnumValueByName(" + strClassEnumName + @", strClassName);
        switch(iLookUp)
        {
            default:
                return nullptr;");
            int iClassIndex = 0;
            foreach (ClassStructure mClass in mClassesUsing)
            {
                mCodeWriter.WriteLine("            case " + iClassIndex.ToString() + ":    //" + mClass.name);
                mCodeWriter.WriteLine("                return KC_NEW " + mClass.name + ";");
                iClassIndex++;
            }

            mCodeWriter.WriteLine(@"

        }
        return nullptr;
    } //end createObject");
            ///////////////////////////writing template functions////////////////////////////
            mCodeWriter.WriteLine("    static bool addObject(TArray<" + mClassWriting.name + "> &mList, KCString strClassName)");
            mCodeWriter.WriteLine("    {");
            mCodeWriter.WriteLine("       int32 iLookUp = _SERIALIZER_ENUMS_::_getEnumValueByName(" + strClassEnumName + ", strClassName);");
            mCodeWriter.WriteLine("       if(iLookUp < 0 || iLookUp >= " + strClassEnumName + "){return false;}");
            mCodeWriter.WriteLine("       switch((ESERIALIZED_CLASSES)iLookUp)");
            mCodeWriter.WriteLine("       {");
            mCodeWriter.WriteLine("           default:");
            mCodeWriter.WriteLine("               mList.Add(" + mClassWriting.name + "());");
            mCodeWriter.WriteLine("               return true;");
            foreach (ClassStructure mChildClass in mClassesUsing)
            {               
                mCodeWriter.WriteLine("           case ESERIALIZED_CLASSES::" + mChildClass.name + ":");
                mCodeWriter.WriteLine("               mList.Add(" + mChildClass.name + "());");
                mCodeWriter.WriteLine("               return true;");
            }
            mCodeWriter.WriteLine("       }");
            mCodeWriter.WriteLine("    } //end addObject " + mClassWriting.name);

            ///////////////////////////writing template functions////////////////////////////
            mCodeWriter.WriteLine("    static " + mClassWriting.name + " createObjectRef(KCString strClassName)");
            mCodeWriter.WriteLine("    {");
            mCodeWriter.WriteLine("       int32 iLookUp = _SERIALIZER_ENUMS_::_getEnumValueByName(" + strClassEnumName + ", strClassName);");
            mCodeWriter.WriteLine("       if(iLookUp < 0 || iLookUp >= " + strClassEnumName + "){return " + mClassWriting.name + "();}");
            mCodeWriter.WriteLine("       switch((ESERIALIZED_CLASSES)iLookUp)");
            mCodeWriter.WriteLine("       {");
            mCodeWriter.WriteLine("           default:");
            mCodeWriter.WriteLine("               return " + mClassWriting.name + "();");
            foreach (ClassStructure mChildClass in mClassesUsing)
            {
                mCodeWriter.WriteLine("           case ESERIALIZED_CLASSES::" + mChildClass.name + ":");
                mCodeWriter.WriteLine("               return (" + mClassWriting.name + ")" + mChildClass.name + "();");
            }
            mCodeWriter.WriteLine("       }");
            mCodeWriter.WriteLine("    } //end createObjectRef");
        

            //end the class
            mCodeWriter.WriteLine("}; //end class _SERIALIZE_CLASS_CREATION_");

            string strFile = mCodeWriter.ToString();
            try
            {



                string strOldFile = File.ReadAllText(strHeaderFileName);
                if (strFile == strOldFile)
                {
                    mController.log(strHeaderFileName + " - didn't change. Not resaving.");
                    return;
                }
            }
            catch
            {

            }
            File.WriteAllText(strHeaderFileName, mCodeWriter.ToString());

        }*/


    } //end class

} //end namespace
