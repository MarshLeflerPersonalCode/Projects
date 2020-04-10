using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.IO;
using Library.ClassParser;

namespace CommandLineSerializer.codeWriters
{
	public class ClassOrStructWriter : CodeWriter
	{


		public override bool attemptByteWriterCode(HeaderFile mHeaderFile, ClassVariable mVariable) 
		{
			if (_isPrimaryVariableType(mHeaderFile, mVariable))
			{
				return false;
			}
			ClassStructure mClassStruct = getClassStructure(mHeaderFile, mVariable);
			if (mClassStruct == null)
			{
				return false;
			}

			if( mVariable.isPointer)
			{
				string strBoolName = "_b" + mVariable.variableName;
				mHeaderFile.addLine("bool " + strBoolName + "(" + mVariable.variableName + " != nullptr);");
				mHeaderFile.addLine("mByteWriter << " + strBoolName + ";");
				mHeaderFile.addLine("if(" + strBoolName + ")");
				mHeaderFile.addLine("{");
				mHeaderFile.addLine(mVariable.variableName + "->serialize(mByteWriter);");
				mHeaderFile.addLine("}");
			}
			else
			{
				mHeaderFile.addLine(mVariable.variableName + ".serialize(mByteWriter);");
			}

			return true; 
		}
		public override bool attemptByteReaderCode(HeaderFile mHeaderFile, ClassVariable mVariable)
		{
			if (_isPrimaryVariableType(mHeaderFile, mVariable))
			{
				return false;
			}
			ClassStructure mClassStruct = getClassStructure(mHeaderFile, mVariable);
			if (mClassStruct == null)
			{
				return false;
			}

			if (mVariable.isPointer)
			{
				string strBoolName = "_b" + mVariable.variableName;
				mHeaderFile.addLine("DELETE_SAFELY(" + mVariable.variableName + ");");
				mHeaderFile.addLine("bool " + strBoolName + "(false);");
				mHeaderFile.addLine("mByteReader << " + strBoolName + ";");
				mHeaderFile.addLine("if(" + strBoolName + ")");
				mHeaderFile.addLine("{");
				mHeaderFile.addLine(mVariable.variableName + " = new " + mVariable.variableType + "();");
				mHeaderFile.addLine(mVariable.variableName + "->deserialize(mByteReader);");
				mHeaderFile.addLine("}");
			}
			else
			{
				mHeaderFile.addLine(mVariable.variableName + ".deserialize(mByteReader);");
			}

			return true;
		}
		public override bool attemptDataGroupWriteCode(HeaderFile mHeaderFile, ClassVariable mVariable, string strDataGroupName)
		{
			if (_isPrimaryVariableType(mHeaderFile, mVariable))
			{
				return false;
			}
			ClassStructure mClassStruct = getClassStructure(mHeaderFile, mVariable);
			if (mClassStruct == null)
			{
				return false;
			}

			if (mVariable.isPointer)
			{
				string strBoolName = "_b" + mVariable.variableName;
				mHeaderFile.addLine("bool " + strBoolName + "(" + mVariable.variableName + " != nullptr);");
				mHeaderFile.addLine(strDataGroupName + ".setProperty(\"" + strBoolName +"\", " + strBoolName + ");");
				mHeaderFile.addLine("if(" + strBoolName + ")");
				mHeaderFile.addLine("{");
				mHeaderFile.addLine(mVariable.variableName + "->serialize(" + strDataGroupName + ", \""+ mVariable.variableName + "\");");
				mHeaderFile.addLine("}");
			}
			else
			{
				mHeaderFile.addLine(mVariable.variableName + ".serialize(" + strDataGroupName + ", \"" + mVariable.variableName + "\");");
			}

			return true;
		}
		public override bool attemptDataGroupReadCode(HeaderFile mHeaderFile, ClassVariable mVariable, string strDataGroupName)
		{
			if (_isPrimaryVariableType(mHeaderFile, mVariable))
			{
				return false;
			}
			ClassStructure mClassStruct = getClassStructure(mHeaderFile, mVariable);
			if (mClassStruct == null)
			{
				return false;
			}

			if (mVariable.isPointer)
			{
				string strBoolName = "_b" + mVariable.variableName;
				mHeaderFile.addLine("DELETE_SAFELY(" + mVariable.variableName + ");");
				mHeaderFile.addLine("bool " + strBoolName + "(false);");
				mHeaderFile.addLine(strBoolName + " = " + strDataGroupName + ".getProperty(\"" + strBoolName + "\", " + strBoolName + ");");
				mHeaderFile.addLine("if(" + strBoolName + ")");
				mHeaderFile.addLine("{");
				mHeaderFile.addLine(mVariable.variableName + " =  new " + mVariable.variableType + "();");
				mHeaderFile.addLine(mVariable.variableName + "->deserialize(" + strDataGroupName + ");");
				mHeaderFile.addLine("}");
			}
			else
			{
				mHeaderFile.addLine(mVariable.variableName + ".deserialize(" + strDataGroupName + ");");
			}

			return true;
		}

		private ClassStructure getClassStructure(HeaderFile mHeaderFile, ClassVariable mVariable)
		{
			string strVariableType = mVariable.variableType;
			if (strVariableType.Contains("<"))
			{
				strVariableType = strVariableType.Substring(0, strVariableType.IndexOf("<"));
			}
			return mHeaderFile.getClassStructure(strVariableType);
		}

		private bool _isPrimaryVariableType(HeaderFile mHeaderFile, ClassVariable mVariable)
		{
			return mHeaderFile.getTypeDefs().ContainsKey(mVariable.variableType.ToLower());
		}
	}
}
