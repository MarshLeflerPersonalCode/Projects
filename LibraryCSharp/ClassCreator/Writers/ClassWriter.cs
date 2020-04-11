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
			strClass = strClass + "    public class " + mClass.name + ": ClassInstance" + Environment.NewLine;


			strClass = strClass + "    {" + Environment.NewLine;
			//todo add properties in.
			foreach(ClassVariable mVariable in mClass.variables)
			{
				if (mVariable.isPrivateVariable == false &&
					mVariable.isSerialized == true)
				{
					string strReplace = _writeVariable(mManager, mProjectWrapper, mClass, mVariable);
					strReplace = "        " + strReplace.Replace(Environment.NewLine, Environment.NewLine + "        ");
					strClass = strClass + strReplace;
				}
			}
			strClass = strClass + Environment.NewLine + "    } //end of " + mClass.name + Environment.NewLine;

			return strClass;
		}

		private static string _writeVariable(ClassCreatorManager mManager, ProjectWrapper mProjectWrapper, ClassStructure mClass, ClassVariable mVariable)
		{
			VariableDefinitionHandler mVariableTypes = mManager.variableDefinitionHandler;
			if(mProjectWrapper.enums.ContainsKey(mVariable.variableType.ToUpper()) )
			{
				//it's an enum.
				return _writeVariableInfo(mClass, mVariable) + _writeEnumVariable(mManager, mProjectWrapper, mClass, mVariable);
			}


			foreach(VariableDefinition mVariableDef in mVariableTypes.getVariableDefinitions())
			{
				if(mVariable.variableType !=  mVariableDef.variableName)
				{
					
					continue;
				}
				if(mVariableDef.eCSharpVariable == EVARIABLE_CSHARP_TYPES.COUNT ||
					mVariableDef.eCSharpVariable == EVARIABLE_CSHARP_TYPES.NOT_DEFINED)
				{
					mManager.log("ERROR - found variable " + mVariable.variableName + " of type " + mVariable.variableType + " in class " + mClass.name + " but the definition for that variable is not defined. Unable to create that variable");
				}
				if(mVariableDef.isPrimitiveType)
				{
					return _writeVariableInfo( mClass, mVariable) + _writePrimitiveVariable(mProjectWrapper, mVariableDef, mVariable);
				}
				

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
			if ( mClassVariable.variableProperties.Count > 0)
			{
				string strProperties = "";
				foreach(KeyValuePair<string, string> mData in mClassVariable.variableProperties)
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
			switch(mVariableDefinition.eCSharpVariable)
			{
				default:
					{
						
						string strValue = mClassVariable.variableValue.Trim().Replace("\"", "");
						if(strValue.Length > 0 )
						{
							bool bFound = false;
							while (mProjectWrapper.defines.ContainsKey(strValue))
							{
								bFound = true;
								strValue = mProjectWrapper.defines[strValue];
							}
							if(bFound == false)
							{
								strValue = strValue.Replace("f", ""); //fixes issue with defining 0.0f
							}
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
			if(mClassVariable.variableProperties.ContainsKey("ClampMin") ||
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
				strClass = strClass + "    }" + Environment.NewLine;
			}
			else
			{
				strClass = strClass + "    set{ _" + mClassVariable.variableName + " = value; }" + Environment.NewLine;
			}
			strClass = strClass + "}" + Environment.NewLine;
			return strClass;
		}

		private static string _writeVaraibleComponentModelDetails(ClassVariable mClassVariable)
		{			
			if( mClassVariable.variableComment == "" && mClassVariable.variableProperties.Count == 0 )
			{
				return "";
			}
			string strDetails = "";
			
			if(mClassVariable.variableProperties.ContainsKey("DISPLAYNAME"))
			{
				strDetails = strDetails + "DisplayName(\"" + mClassVariable.variableProperties["DISPLAYNAME"] + "\"), ";
			}
			if(mClassVariable.variableProperties.ContainsKey("CATEGORY"))
			{
				strDetails = strDetails + "Category(\"" + mClassVariable.variableProperties["CATEGORY"] + "\"), ";
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
			else if( mClassVariable.variableComment != "")
			{
				strDescription = mClassVariable.variableComment;				
			}

			if(strDescription.Length != 0)
			{
				while(strDescription.StartsWith("//")) { strDescription = strDescription.Substring(2, strDescription.Length - 2); }
				if(strDescription.StartsWith("/*")) { strDescription = strDescription.Substring(2, strDescription.Length - 2); }
				if(strDescription.StartsWith("\n//")) { strDescription = strDescription.Replace("\n//", "\n"); }
				if (strDescription.StartsWith(Environment.NewLine + "//")) { strDescription = strDescription.Replace(Environment.NewLine + "//", Environment.NewLine); }
				if (strDescription.StartsWith("\n/*")) { strDescription = strDescription.Replace("\n/*", "\n"); }
				if (strDescription.StartsWith(Environment.NewLine + "/*")) { strDescription = strDescription.Replace(Environment.NewLine + "/*", Environment.NewLine); }

				strDetails = strDetails + "Description(\"" + strDescription + "\"), ";
			}
			if( strDetails.Length == 0 )
			{
				return "";
			}
			strDetails = strDetails.Substring(0, strDetails.Length - 2); //remove ", "
			return "[" + strDetails + "]" + Environment.NewLine;
		}


		public static string _writeEnumVariable(ClassCreatorManager mManager, ProjectWrapper mProjectWrapper, ClassStructure mClass, ClassVariable mVariable)
		{
			string strEnumLine = "";
			string strValue = mVariable.variableValue;
			if( strValue != "")
			{
				strValue = " = " + strValue.Replace("::", ".") + ";";
			}
			else
			{
				strValue = ";";
			}
			strEnumLine = strEnumLine + "private " + mVariable.variableType + " _" + mVariable.variableName + strValue + Environment.NewLine;
			strEnumLine = strEnumLine + _writeVaraibleComponentModelDetails(mVariable);
			strEnumLine = strEnumLine + "public " + mVariable.variableType + " " + mVariable.variableName + Environment.NewLine;
			strEnumLine = strEnumLine + "{" + Environment.NewLine;
			strEnumLine = strEnumLine + "    get{ return _" + mVariable.variableName + "; }" + Environment.NewLine;
			strEnumLine = strEnumLine + "    set{ _" + mVariable.variableName + " = value; }" + Environment.NewLine;
			strEnumLine = strEnumLine + "}" + Environment.NewLine;
			return strEnumLine;
		}

	}//end class

} //end namespace
