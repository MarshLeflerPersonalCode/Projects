using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.IO;
using Library.ClassParser;
namespace CommandLineSerializer.codeWriters
{
	public class TArrayWriter : CodeWriter
	{

		public override bool attemptByteWriterCode(HeaderFile mHeaderFile, ClassVariable mVariable)
		{
			string strType = "";
			if(_parseType(mVariable, ref strType) == false ) { return false; }

			
			
			

			string strCountVar = "iCount_" + mVariable.variableName;
			string strLoopVar = "iLoop_" + mVariable.variableName;
			mHeaderFile.addLine("uint16 " + strCountVar + " = (uint16)" + mVariable.variableName + ".Num();");
			mHeaderFile.addLine("mByteWriter << " + strCountVar + ";");
			if (_isPrimitive(mHeaderFile, strType))
			{
				mHeaderFile.addLine("for(uint16 " + strLoopVar + " = 0; " + strLoopVar + " < " + strCountVar + "; " + strLoopVar + "++)");
				mHeaderFile.addLine("{");
				mHeaderFile.addLine("mByteWriter << " + mVariable.variableName + "[" + strLoopVar+ "];");
				mHeaderFile.addLine("}");
			}
			else
			{
				string strTmpVar = "_tmp" + mVariable.variableName;
				ClassVariable mNewVariable = _createNewVariable(mHeaderFile, strTmpVar, strType);
				mHeaderFile.addLine("for(uint16 " + strLoopVar + " = 0; " + strLoopVar + " < " + strCountVar + "; " + strLoopVar + "++)");
				mHeaderFile.addLine("{");
				mHeaderFile.addLine(strType + ((mNewVariable.isPointer)?"": " &") + strTmpVar + " = " + mVariable.variableName + "[" + strLoopVar + "];");
				foreach(CodeWriter mWriter in mHeaderFile.getCodeWriters())
				{
					if(mWriter.attemptByteWriterCode(mHeaderFile, mNewVariable))
					{
						break;
					}
				}				
				mHeaderFile.addLine("}");
			}

			return true;
		}
		public override bool attemptByteReaderCode(HeaderFile mHeaderFile, ClassVariable mVariable)
		{
			string strType = "";
			if (_parseType(mVariable, ref strType) == false) { return false; }
			

			string strCountVar = "iCount_" + mVariable.variableName;
			string strLoopVar = "iLoop_" + mVariable.variableName;
			string strTmpVar = "_tmp" + mVariable.variableName;
			mHeaderFile.addLine("uint16 " + strCountVar + "(0);");
			mHeaderFile.addLine("mByteReader << " + strCountVar + ";");
			mHeaderFile.addLine(mVariable.variableName + ".Reset();");
			mHeaderFile.addLine(mVariable.variableName + ".Reserve(" + strCountVar + ");");
			if (_isPrimitive(mHeaderFile, strType))
			{
				mHeaderFile.addLine(_writePrimitiveTypeInitializer(mHeaderFile, strType, strTmpVar));// + " " + strTmpVar + ";");
				mHeaderFile.addLine("for(uint16 " + strLoopVar + " = 0; " + strLoopVar + " < " + strCountVar + "; " + strLoopVar + "++)");
				mHeaderFile.addLine("{");				
				mHeaderFile.addLine("mByteReader << " + strTmpVar + ";");
				mHeaderFile.addLine(mVariable.variableName + ".Add( " + strTmpVar + ");");
				mHeaderFile.addLine("}");
			}
			else
			{				
				ClassVariable mNewVariable = _createNewVariable(mHeaderFile, strTmpVar, strType);
				mHeaderFile.addLine("for(uint16 " + strLoopVar + " = 0; " + strLoopVar + " < " + strCountVar + "; " + strLoopVar + "++)");
				mHeaderFile.addLine("{");
				if (mNewVariable.isPointer == false)
				{
					mHeaderFile.addLine(mVariable.variableName + ".Add(" + mNewVariable.variableType + "() );");
					mHeaderFile.addLine(strType + " &" + strTmpVar + " = " + mVariable.variableName + ".Last();");
				}
				else
				{
					//just just need to define it.
					mHeaderFile.addLine(strType + " " + strTmpVar + " = nullptr;");
				}								
				foreach (CodeWriter mWriter in mHeaderFile.getCodeWriters())
				{
					if (mWriter.attemptByteReaderCode(mHeaderFile, mNewVariable))
					{
						break;
					}
				}
				if (mNewVariable.isPointer)
				{
					//we need to add it back in.
					mHeaderFile.addLine(mVariable.variableName + ".Add(" + strTmpVar + ");");
				}
				mHeaderFile.addLine("}");
			}
			
			return true;
		}
		public override bool attemptDataGroupWriteCode(HeaderFile mHeaderFile, ClassVariable mVariable, string strDataGroupName)
		{
			string strType = "";
			if (_parseType(mVariable, ref strType) == false) { return false; }
			EnumList mEnum = mHeaderFile.getEnumList(strType);

			string strCountVar = "iCount_" + mVariable.variableName;
			string strLoopVar = "iLoop_" + mVariable.variableName;
			string strTmpVar = "_tmp" + mVariable.variableName;
			mHeaderFile.addLine("uint16 " + strCountVar + " = (uint16)" + mVariable.variableName + ".Num();");
			mHeaderFile.addLine(strDataGroupName + ".setProperty(\"" + strCountVar + "\", " + strCountVar + ");");
			if (_isPrimitive(mHeaderFile, strType))
			{
				mHeaderFile.addLine("for(uint16 " + strLoopVar + " = 0; " + strLoopVar + " < " + strCountVar + "; " + strLoopVar + "++)");
				mHeaderFile.addLine("{");				
				mHeaderFile.addLine(strDataGroupName + ".setProperty( \"" + mVariable.variableName + "\" + std::to_string(" + strLoopVar + "), " + mVariable.variableName + "[" + strLoopVar + "]);");
				mHeaderFile.addLine("}");
			}
			else if (mEnum != null)
			{
				mHeaderFile.addLine("for(uint16 " + strLoopVar + " = 0; " + strLoopVar + " < " + strCountVar + "; " + strLoopVar + "++)");
				mHeaderFile.addLine("{");				
				mHeaderFile.addLine(strDataGroupName + ".setProperty( \"" + mVariable.variableName + "\" + std::to_string(" + strLoopVar + "), (int32)" + mVariable.variableName + "[" + strLoopVar + "]);");				
				mHeaderFile.addLine("}");
			}
			else
			{
				
				ClassVariable mNewVariable = _createNewVariable(mHeaderFile, strTmpVar, strType);
				mHeaderFile.addLine("for(uint16 " + strLoopVar + " = 0; " + strLoopVar + " < " + strCountVar + "; " + strLoopVar + "++)");
				mHeaderFile.addLine("{");				
				mHeaderFile.addLine(strType + ((mNewVariable.isPointer) ? "" : " &") + strTmpVar + " = " + mVariable.variableName + "[" + strLoopVar + "];");
				string strNewDataGroupName = "_mDataGroup" + mVariable.variableName;
				mHeaderFile.addLine("KCDataGroup &" + strNewDataGroupName + " = " + strDataGroupName + ".getOrCreateChildGroup(\"" + mVariable.variableName + "\" + std::to_string(" + strLoopVar + "));");

				foreach (CodeWriter mWriter in mHeaderFile.getCodeWriters())
				{

					if (mWriter.attemptDataGroupWriteCode(mHeaderFile, mNewVariable, strNewDataGroupName))
					{
						break;
					}
				}
				mHeaderFile.addLine("if(" + strNewDataGroupName + ".isEmpty()){ " + strDataGroupName + ".removeChildGroup(" + strNewDataGroupName + ");}");
				mHeaderFile.addLine("}");
			}
			//mHeaderFile.addLine("mDataGroup.setProperty(\"" + mVariable.variableName + "\", " + mVariable.variableName + ");");
			return true;
		}
		public override bool attemptDataGroupReadCode(HeaderFile mHeaderFile, ClassVariable mVariable, string strDataGroupName)
		{
			string strType = "";
			if (_parseType(mVariable, ref strType) == false) { return false; }
			
			EnumList mEnum = mHeaderFile.getEnumList(strType);

			string strCountVar = "iCount_" + mVariable.variableName;
			string strLoopVar = "iLoop_" + mVariable.variableName;
			string strTmpVar = "_tmp" + mVariable.variableName;
			mHeaderFile.addLine("uint16 " + strCountVar + "(0);");
			mHeaderFile.addLine(strCountVar + " = " + strDataGroupName + ".getProperty(\"" + strCountVar + "\", " + strCountVar + ");");
			mHeaderFile.addLine(mVariable.variableName + ".Reset();");
			mHeaderFile.addLine(mVariable.variableName + ".Reserve(" + strCountVar + ");");
			if (_isPrimitive(mHeaderFile, strType))
			{

				mHeaderFile.addLine(_writePrimitiveTypeInitializer(mHeaderFile, strType, strTmpVar));
				mHeaderFile.addLine("for(uint16 " + strLoopVar + " = 0; " + strLoopVar + " < " + strCountVar + "; " + strLoopVar + "++)");
				mHeaderFile.addLine("{");				
				mHeaderFile.addLine(strType + " " + strTmpVar + "_tmp = " + strDataGroupName + ".getProperty(\"" + mVariable.variableName + "\" + std::to_string(" + strLoopVar + "), " + strTmpVar + ");");
				mHeaderFile.addLine(mVariable.variableName + ".Add(" + strTmpVar + "_tmp);");
				mHeaderFile.addLine("}");
			}	
			else if(mEnum != null )
			{
				string strEnumTempVar = "_enum" + mVariable.variableName;
				mHeaderFile.addLine(_writePrimitiveTypeInitializer(mHeaderFile, "int32", strEnumTempVar));
				mHeaderFile.addLine("for(uint16 " + strLoopVar + " = 0; " + strLoopVar + " < " + strCountVar + "; " + strLoopVar + "++)");
				mHeaderFile.addLine("{");				
				mHeaderFile.addLine(strType + " " + strTmpVar + "_tmp = (" + strType + ")" + strDataGroupName + ".getProperty(\"" + mVariable.variableName + "\" + std::to_string(" + strLoopVar + "), " + strEnumTempVar + ");");
				mHeaderFile.addLine(mVariable.variableName + ".Add(" + strTmpVar + "_tmp);");
				mHeaderFile.addLine("}");
			}
			else
			{
				ClassVariable mNewVariable = _createNewVariable(mHeaderFile, strTmpVar, strType);
				mHeaderFile.addLine("for(uint16 " + strLoopVar + " = 0; " + strLoopVar + " < " + strCountVar + "; " + strLoopVar + "++)");
				mHeaderFile.addLine("{");
				if (mNewVariable.isPointer == false)
				{
					mHeaderFile.addLine(mVariable.variableName + ".Add(" + mNewVariable.variableType + "() );");
					mHeaderFile.addLine(strType + " &" + strTmpVar + " = " + mVariable.variableName + ".Last();");
				}
				else
				{
					//just just need to define it.
					mHeaderFile.addLine(strType + " " + strTmpVar + " = nullptr;");
				}
				string strNewDataGroupName = "_mDataGroup" + mVariable.variableName;
				mHeaderFile.addLine("KCDataGroup &" + strNewDataGroupName + " = " + strDataGroupName + ".getOrCreateChildGroup(\"" + mVariable.variableName + "\" + std::to_string(" + strLoopVar + "));");

				foreach (CodeWriter mWriter in mHeaderFile.getCodeWriters())
				{
					if (mWriter.attemptDataGroupReadCode(mHeaderFile, mNewVariable, strNewDataGroupName))
					{
						break;
					}
				}

				if (mNewVariable.isPointer )
				{
					//we need to add it back in.
					mHeaderFile.addLine(mVariable.variableName + ".Add(" + strTmpVar + ");");
				}
				mHeaderFile.addLine("if(" + strNewDataGroupName + ".isEmpty()){ " + strDataGroupName + ".removeChildGroup(" + strNewDataGroupName + ");}");
				mHeaderFile.addLine("}");
			}
			//mHeaderFile.addLine(mVariable.variableName + " = mDataGroup.getProperty(\"" + mVariable.variableName + "\", " + mVariable.variableName + ");");
			return true;
		}

		private bool _parseType(ClassVariable mVariable, ref string strType )
		{
			if( mVariable.variableType.Contains("TArray<") == false)
			{
				return false;
			}
			strType = mVariable.variableType;
			int iIndexOf = strType.IndexOf("<");
			strType = strType.Substring(iIndexOf + 1, strType.Length - iIndexOf - 2);
			
			return true;
		}

		private bool _isPrimitive(HeaderFile mHeaderFile, string strType)
		{
			if( mHeaderFile.getTypeDefs().ContainsKey(strType.ToLower()))
			{
				return true;
			}
			return false;
		}
		private string _writePrimitiveTypeInitializer(HeaderFile mHeaderFile, string strType, string varName)
		{
			string strTypeLower = strType.ToLower();
			if (mHeaderFile.getTypeDefs().ContainsKey(strTypeLower))
			{
				switch(mHeaderFile.getTypeDefs()[strTypeLower])
				{
					default:
					case ESERIALIZE_DATA_TYPE.NUMBER:
						return strType + " " + varName + "(0);";
					case ESERIALIZE_DATA_TYPE.STRING:
						return strType + " " + varName + "(\"\");";
						

				}
			}
			return strType + " " + varName + "(0);";
		}
		private ClassVariable _createNewVariable(HeaderFile mHeaderFile, string strVariableName, string strType)
		{
			ClassVariable mVariable = new ClassVariable();
			mVariable.variableName = strVariableName;
			if (strType.EndsWith("*"))
			{
				strType = strType.Substring(0, strType.Length - 1).Trim(); ;
				mVariable.isPointer = true;
			}
			mVariable.variableType = strType;
			return mVariable;
		}

	}
}
