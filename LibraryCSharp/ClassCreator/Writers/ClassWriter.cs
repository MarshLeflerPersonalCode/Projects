using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.ClassParser;
namespace Library.ClassCreator.Writers
{
    public class ClassWriter
    {

        public static string writeClass(ClassCreatorManager mManager, ClassStructure mClass, ProjectWrapper mProjectWrapper)
        {
            if (mClass == null)
            {
                return "";
            }

            string strClass = Environment.NewLine;
            string strClassExtending = _getCLassInheritingFrom(mClass, mProjectWrapper);
            strClass = strClass + "    public class " + mClass.name + ": " + strClassExtending + Environment.NewLine;


            strClass = strClass + "    {" + Environment.NewLine;            
            strClass = strClass + _writeVariables(mManager, mClass, mProjectWrapper);           
            strClass = strClass + Environment.NewLine + "    } //end of " + mClass.name + Environment.NewLine;

            return strClass;
        }

        private static string _getCLassInheritingFrom(ClassStructure mClass, ProjectWrapper mProjectWrapper)
        {
            foreach( string strClass in mClass.classStructuresInheritingFrom)
            {
                if( mProjectWrapper.getClassStructByName(strClass) != null )
                {
                    return strClass;
                }
            }

            return "ClassInstance";
        }

        private static string _writeHierarchy(ClassCreatorManager mManager, ClassStructure mClass, ProjectWrapper mProjectWrapper)
        {
            string strHierarchyVariables = "";
            foreach(string strClass in mClass.classStructuresInheritingFrom)
            {
                ClassStructure mClassParent = mProjectWrapper.getClassStructByName(strClass);
                if(mClassParent == null )
                {
                    mManager.log("WARNING - class " + mClass.name + " is inheriting from " + strClass + ". But the class doesn't seem to be serialized. This might be okay depending on if those properties need to be serialized or not.");
                    continue;
                }
                strHierarchyVariables = strHierarchyVariables + _writeVariables(mManager, mClassParent, mProjectWrapper);
            }
            return strHierarchyVariables;
        }
        private static string _writeVariables(ClassCreatorManager mManager, ClassStructure mClass, ProjectWrapper mProjectWrapper)
        {
            string strClass = "";
            //strClass = strClass + _writeHierarchy(mManager, mClass, mProjectWrapper);
            foreach (ClassVariable mVariable in mClass.variables)
            {
                if (mVariable.isPrivateVariable == false &&
                    mVariable.isSerialized == true)
                {
                    string strReplace = _writeVariable(mManager, mProjectWrapper, mClass, mVariable);
                    strReplace = "        " + strReplace.Replace(Environment.NewLine, Environment.NewLine + "        ");
                    strClass = strClass + strReplace;
                }
            }
            return strClass;
        }

        private static string _writeVariable(ClassCreatorManager mManager, ProjectWrapper mProjectWrapper, ClassStructure mClass, ClassVariable mVariable)
        {
            VariableDefinitionHandler mVariableTypes = mManager.variableDefinitionHandler;


            foreach (VariableDefinition mVariableDef in mVariableTypes.getVariableDefinitions())
            {
                if( mVariableDef.eCSharpVariable == EVARIABLE_CSHARP_TYPES.LIST)
                {
                    if( mVariable.variableType.StartsWith(mVariableDef.variableName) == false )
                    {
                        continue;
                    }
                }
                else if (mVariable.variableType != mVariableDef.variableName)
                {

                    continue;
                }
                if (mVariableDef.eCSharpVariable == EVARIABLE_CSHARP_TYPES.COUNT ||
                    mVariableDef.eCSharpVariable == EVARIABLE_CSHARP_TYPES.NOT_DEFINED)
                {
                    mManager.log("ERROR - found variable " + mVariable.variableName + " of type " + mVariable.variableType + " in class " + mClass.name + " but the definition for that variable is not defined. Unable to create that variable");
                }
                if (mVariableDef.isPrimitiveType)
                {
                    return _writeVariableInfo(mClass, mVariable) + _writePrimitiveVariable(mProjectWrapper, mVariableDef, mVariable);
                }
                else if (mVariableDef.eCSharpVariable == EVARIABLE_CSHARP_TYPES.CLASS)
                {
                    if (mProjectWrapper.classStructures.ContainsKey(mVariableDef.variableClassName.ToUpper()))
                    {
                        //it's a class structure.
                        return _writeVariableInfo(mClass, mVariable) + _writeClassVariable(mManager, mProjectWrapper, mClass, mVariable, mVariableDef.variableClassName);
                    }
                }
                else if (mVariableDef.eCSharpVariable == EVARIABLE_CSHARP_TYPES.ENUM)
                {
                    if (mProjectWrapper.enums.ContainsKey(mVariableDef.variableEnumName.ToUpper()))
                    {
                        //it's an enum.
                        return _writeVariableInfo(mClass, mVariable) + _writeEnumVariable(mManager, mProjectWrapper, mClass, mVariable, mVariableDef.variableEnumName);
                    }
                }
                else if (mVariableDef.eCSharpVariable == EVARIABLE_CSHARP_TYPES.LIST)
                {
                    return _writeVariableInfo(mClass, mVariable) + _writeListVariable(mManager, mProjectWrapper, mClass, mVariable);
                }
            }


            if (mProjectWrapper.enums.ContainsKey(mVariable.variableType.ToUpper()))
            {
                //it's an enum.
                return _writeVariableInfo(mClass, mVariable) + _writeEnumVariable(mManager, mProjectWrapper, mClass, mVariable, "");
            }
            if (mProjectWrapper.classStructures.ContainsKey(mVariable.variableType.ToUpper()))
            {
                //it's a class structure.
                return _writeVariableInfo(mClass, mVariable) + _writeClassVariable(mManager, mProjectWrapper, mClass, mVariable, "");
            }



            mManager.log("ERROR - found variable " + mVariable.variableName + " of type " + mVariable.variableType + " in class " + mClass.name + ". No definition was found defining the type.");
            return "";
        }

        private static string _writeVariableInfo(ClassStructure mClass, ClassVariable mClassVariable)
        {

            string strClass = Environment.NewLine;
            strClass = strClass + "/".PadRight(100, '/') + Environment.NewLine;
            strClass = strClass + "//Class File:" + mClass.file + Environment.NewLine;
            strClass = strClass + "//Class Name:" + mClass.name + Environment.NewLine;
            strClass = strClass + "//Variable Name:" + mClassVariable.variableName + Environment.NewLine;
            strClass = strClass + "//Variable Type:" + mClassVariable.variableType + Environment.NewLine;
            strClass = strClass + "//Variable Value:" + mClassVariable.variableValue + Environment.NewLine;
            strClass = strClass + "//Variable Line Number:" + mClassVariable.lineNumber.ToString() + Environment.NewLine;
            if (mClassVariable.variableProperties.Count > 0)
            {
                string strProperties = "";
                foreach (KeyValuePair<string, string> mData in mClassVariable.variableProperties)
                {
                    strProperties = strProperties + mData.Key + " = " + mData.Value + ", ";
                }
                strProperties = strProperties.Substring(0, strProperties.Length - 2);
                strClass = strClass + "//Variable Properties: " + strProperties + Environment.NewLine;
            }
            return strClass;
        }

        private static string _writePrimitiveVariable(ProjectWrapper mProjectWrapper, VariableDefinition mVariableDefinition, ClassVariable mClassVariable)
        {
            string strClass = "";
            switch (mVariableDefinition.eCSharpVariable)
            {
                default:
                {

                    string strValue = mClassVariable.variableValue.Trim().Replace("\"", "");
                    if (strValue.Length > 0)
                    {
                        bool bFound = false;
                        while (mProjectWrapper.defines.ContainsKey(strValue))
                        {
                            bFound = true;
                            strValue = mProjectWrapper.defines[strValue];
                        }
                        /*if (bFound == false)
                        {
                            if(strValue.EndsWith("f"))
                            {
                                strValue = strValue.Replace("f", ""); //fixes issue with defining 0.0f
                            }
                            
                        }*/
                    }

                    strClass = strClass + "private " + EVARIABLE_CSHARP_TYPES_NAMES.g_Names[(int)mVariableDefinition.eCSharpVariable] + " _" + mClassVariable.variableName + " = " + ((strValue.Length != 0) ? strValue : "0") + ";" + Environment.NewLine;
                }
                break;
                case EVARIABLE_CSHARP_TYPES.STRING:
                {
                    string strValue = mClassVariable.variableValue.Trim();
                    while (mProjectWrapper.defines.ContainsKey(strValue))
                    {
                        strValue = mProjectWrapper.defines[strValue];
                    }
                    if (strValue.StartsWith("\""))
                    {
                        strValue = strValue.Substring(1, strValue.Length - 1);
                    }
                    if (strValue.EndsWith("\""))
                    {
                        strValue = strValue.Substring(0, strValue.Length - 1);
                    }
                    strClass = strClass + "private string _" + mClassVariable.variableName + " = \"" + ((strValue.Length != 0) ? strValue : "") + "\";" + Environment.NewLine;
                }
                break;
            }
            strClass = strClass + _writeVaraibleComponentModelDetails(mClassVariable);
            strClass = strClass + "public " + EVARIABLE_CSHARP_TYPES_NAMES.g_Names[(int)mVariableDefinition.eCSharpVariable] + " " + mClassVariable.variableName + Environment.NewLine;
            strClass = strClass + "{" + Environment.NewLine;
            strClass = strClass + "    get{ return _" + mClassVariable.variableName + "; }" + Environment.NewLine;
            if (mClassVariable.variableProperties.ContainsKey("ClampMin") ||
               mClassVariable.variableProperties.ContainsKey("ClampMax"))
            {
                strClass = strClass + "    set" + Environment.NewLine;
                strClass = strClass + "    {" + Environment.NewLine;
                if (mClassVariable.variableProperties.ContainsKey("ClampMin"))
                {
                    strClass = strClass + "        if(value < " + mClassVariable.variableProperties["ClampMin"] + "){value = " + mClassVariable.variableProperties["ClampMin"] + ";}" + Environment.NewLine;
                }
                if (mClassVariable.variableProperties.ContainsKey("ClampMax"))
                {
                    strClass = strClass + "        if(value > " + mClassVariable.variableProperties["ClampMax"] + "){value = " + mClassVariable.variableProperties["ClampMax"] + ";}" + Environment.NewLine;
                }
                strClass = strClass + "        _" + mClassVariable.variableName + " = value;" + Environment.NewLine;
                strClass = strClass + "        _notifyOfPropertyChanged(\"" + mClassVariable.variableName + "\");" + Environment.NewLine;
                strClass = strClass + "    }" + Environment.NewLine;
            }
            else
            {
                strClass = strClass + "    set{ _" + mClassVariable.variableName + " = value; _notifyOfPropertyChanged(\"" + mClassVariable.variableName + "\");}" + Environment.NewLine;
            }
            strClass = strClass + "}" + Environment.NewLine;
            return strClass;
        }

        private static string _writeVaraibleComponentModelDetails(ClassVariable mClassVariable)
        {
            if (mClassVariable.variableComment == "" && mClassVariable.variableProperties.Count == 0)
            {
                return "";
            }
            string strDetails = "";

            if (mClassVariable.variableProperties.ContainsKey("DISPLAYNAME"))
            {
                strDetails = strDetails + "DisplayName(\"" + mClassVariable.variableProperties["DISPLAYNAME"] + "\"), ";
            }
            if (mClassVariable.variableProperties.ContainsKey("CATEGORY"))
            {
                strDetails = strDetails + "Category(\"" + mClassVariable.variableProperties["CATEGORY"] + "\"), ";
            }
            if (mClassVariable.variableProperties.ContainsKey("READONLY"))
            {
                strDetails = strDetails + "ReadOnly(true), ";
            }
            if (mClassVariable.variableProperties.ContainsKey("HIDDEN"))
            {
                strDetails = strDetails + "Browsable(false), ";
            }
            else if (mClassVariable.variableProperties.ContainsKey("BROWSABLE"))
            {
                string strValue = mClassVariable.variableProperties["BROWSABLE"];
                if( strValue.ToUpper().Contains("FALSE"))
                {
                    strDetails = strDetails + "Browsable(false), ";
                }
            }

            string strDescription = "";
            if (mClassVariable.variableProperties.ContainsKey("TOOLTIP"))
            {
                strDescription = mClassVariable.variableProperties["TOOLTIP"];
            }
            else if (mClassVariable.variableProperties.ContainsKey("DESCRIPTION"))
            {
                strDescription = mClassVariable.variableProperties["DESCRIPTION"];
            }
            else if (mClassVariable.variableComment != "")
            {
                strDescription = mClassVariable.variableComment;
            }

            if (strDescription.Length != 0)
            {
                while (strDescription.StartsWith("//")) { strDescription = strDescription.Substring(2, strDescription.Length - 2); }
                if (strDescription.StartsWith("/*")) { strDescription = strDescription.Substring(2, strDescription.Length - 2); }
                if (strDescription.StartsWith("\n//")) { strDescription = strDescription.Replace("\n//", "\n"); }
                if (strDescription.StartsWith(Environment.NewLine + "//")) { strDescription = strDescription.Replace(Environment.NewLine + "//", Environment.NewLine); }
                if (strDescription.StartsWith("\n/*")) { strDescription = strDescription.Replace("\n/*", "\n"); }
                if (strDescription.StartsWith(Environment.NewLine + "/*")) { strDescription = strDescription.Replace(Environment.NewLine + "/*", Environment.NewLine); }

                strDetails = strDetails + "Description(\"" + strDescription + "\"), ";
            }
            if (strDetails.Length == 0)
            {
                return "";
            }
            strDetails = strDetails.Substring(0, strDetails.Length - 2); //remove ", "
            string strReturnString = "[" + strDetails + "]" + Environment.NewLine;
            if(mClassVariable.typeConverter != "")
            {
                return strReturnString + mClassVariable.typeConverter + Environment.NewLine;
            }
            return strReturnString;
        }


        public static string _writeEnumVariable(ClassCreatorManager mManager, ProjectWrapper mProjectWrapper, ClassStructure mClass, ClassVariable mVariable, string strTypeOverride)
        {
            string strEnumLine = "";
            string strValue = mVariable.variableValue;
            if (strValue != "")
            {
                strValue = " = " + strValue.Replace("::", ".") + ";";
            }
            else
            {
                strValue = ";";
            }
            string strType = (strTypeOverride == "") ? mVariable.variableType : strTypeOverride;
            strEnumLine = strEnumLine + "private " + strType + " _" + mVariable.variableName + strValue + Environment.NewLine;
            strEnumLine = strEnumLine + _writeVaraibleComponentModelDetails(mVariable);
            strEnumLine = strEnumLine + "public " + strType + " " + mVariable.variableName + Environment.NewLine;
            strEnumLine = strEnumLine + "{" + Environment.NewLine;
            strEnumLine = strEnumLine + "    get{ return _" + mVariable.variableName + "; }" + Environment.NewLine;
            strEnumLine = strEnumLine + "    set{ _" + mVariable.variableName + " = value; _notifyOfPropertyChanged(\"" + mVariable.variableName + "\");}" + Environment.NewLine;
            strEnumLine = strEnumLine + "}" + Environment.NewLine;
            return strEnumLine;
        }
        public static string _writeClassVariable(ClassCreatorManager mManager, ProjectWrapper mProjectWrapper, ClassStructure mClass, ClassVariable mVariable, string strTypeOverride)
        {
            string strClassLine = "";
            string strType = (strTypeOverride == "") ? mVariable.variableType : strTypeOverride;

            strClassLine = strClassLine + "private " + strType + " _" + mVariable.variableName + " = new " + strType + "();" + Environment.NewLine;
            strClassLine = strClassLine + _writeVaraibleComponentModelDetails(mVariable);
            strClassLine = strClassLine + "public " + strType + " " + mVariable.variableName + Environment.NewLine;
            strClassLine = strClassLine + "{" + Environment.NewLine;
            strClassLine = strClassLine + "    get{ return _" + mVariable.variableName + "; }" + Environment.NewLine;
            strClassLine = strClassLine + "    set{ _" + mVariable.variableName + " = value; _notifyOfPropertyChanged(\"" + mVariable.variableName + "\"); }" + Environment.NewLine;
            strClassLine = strClassLine + "}" + Environment.NewLine;
            return strClassLine;
        }

        public static string _writeListVariable(ClassCreatorManager mManager, ProjectWrapper mProjectWrapper, ClassStructure mClass, ClassVariable mVariable)
        {
            string strVariableType = "";
            int iIndexLessThan = mVariable.variableType.IndexOf("<");
            int iIndexGreaterThan = mVariable.variableType.IndexOf(">");
            if( iIndexGreaterThan < 0 ||
                iIndexLessThan < 0)
            {                
                mManager.log("ERROR - LIST VARIABLE TYPE IS FORMATED WRONG: " + mVariable.variableType);
                return "ERROR - CHECK LOG" + Environment.NewLine;
            }
            iIndexLessThan++;//removes the <
            VariableDefinitionHandler mVariableTypes = mManager.variableDefinitionHandler;
            string strTempVariableTypeName = mVariable.variableType.Substring(iIndexLessThan, iIndexGreaterThan - iIndexLessThan).Trim();
            strTempVariableTypeName = strTempVariableTypeName.Replace("*", "").Trim();
            bool bVariableTypeFound = false;
            foreach (VariableDefinition mVariableDef in mVariableTypes.getVariableDefinitions())
            {
                if (strTempVariableTypeName != mVariableDef.variableName)
                {

                    continue;
                }
                if (mVariableDef.eCSharpVariable == EVARIABLE_CSHARP_TYPES.COUNT ||
                    mVariableDef.eCSharpVariable == EVARIABLE_CSHARP_TYPES.NOT_DEFINED)
                {
                    mManager.log("ERROR - found variable " + strTempVariableTypeName + " inside list variable " + mVariable.variableType + " in class " + mClass.name + " but the definition for that variable is not defined. Unable to create that variable");
                    return "ERROR - CHECK LOG" + Environment.NewLine;
                }
                if (mVariableDef.isPrimitiveType)
                {
                    strVariableType = EVARIABLE_CSHARP_TYPES_NAMES.g_Names[(int)mVariableDef.eCSharpVariable];

                }
                else if (mVariableDef.eCSharpVariable == EVARIABLE_CSHARP_TYPES.CLASS)
                {
                    strVariableType = mVariableDef.variableClassName;
                }
                else if (mVariableDef.eCSharpVariable == EVARIABLE_CSHARP_TYPES.ENUM)
                {
                    strVariableType = mVariableDef.variableEnumName;
                }
                else if (mVariableDef.eCSharpVariable == EVARIABLE_CSHARP_TYPES.LIST)
                {

                    mManager.log("ERROR - List variable: " + mVariable.variableType + " appears to have a list inside it. This should be possible but I don't want to handle it right now - Marsh out.");
                    return "ERROR - CHECK LOG" + Environment.NewLine;
                }
                bVariableTypeFound = true;
                break;
            }
            if (bVariableTypeFound == false)
            {
                if (mProjectWrapper.enums.ContainsKey(strTempVariableTypeName.ToUpper()))
                {
                    //it's an enum.
                    strVariableType = strTempVariableTypeName;
                }
                if (mProjectWrapper.classStructures.ContainsKey(strTempVariableTypeName.ToUpper()))
                {
                    //it's a class structure.
                    strVariableType = strTempVariableTypeName;
                }
            }


            if (strVariableType == "")
            {
                mManager.log("ERROR - UNKNOWN VARIABLE TYPE FOR LIST: " + mVariable.variableType);
                return "ERROR - CHECK LOG" + Environment.NewLine;
            }



            string strClassLine = "";
            string strType = "List<" + strVariableType + ">";

            strClassLine = strClassLine + "private " + strType + " _" + mVariable.variableName + " = new " + strType + "();" + Environment.NewLine;
            strClassLine = strClassLine + _writeVaraibleComponentModelDetails(mVariable);
            strClassLine = strClassLine + "public " + strType + " " + mVariable.variableName + Environment.NewLine;
            strClassLine = strClassLine + "{" + Environment.NewLine;
            strClassLine = strClassLine + "    get{ return _" + mVariable.variableName + "; }" + Environment.NewLine;
            strClassLine = strClassLine + "    set{ _" + mVariable.variableName + " = value; _notifyOfPropertyChanged(\"" + mVariable.variableName + "\"); }" + Environment.NewLine;
            strClassLine = strClassLine + "}" + Environment.NewLine;
            return strClassLine;

        }

    }//end class

} //end namespace
