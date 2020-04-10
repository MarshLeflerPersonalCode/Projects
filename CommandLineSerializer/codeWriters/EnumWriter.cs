using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Library.IO;
using Library.ClassParser;
namespace CommandLineSerializer.codeWriters
{
	public class EnumWriter : CodeWriter
	{
		public override bool attemptByteWriterCode(HeaderFile mHeaderFile, ClassVariable mVariable)
		{
			EnumList mEnum = mHeaderFile.getEnumList(mVariable.variableType);
			if (mEnum == null)
			{
				return false;
			}

			string strType = mEnum.type;
			if( strType == "")
			{
				strType = "uint16";
			}
			mHeaderFile.addLine("mByteWriter << (" + strType + ")" + mVariable.variableName + ";");
			return true;
		}
		public override bool attemptByteReaderCode(HeaderFile mHeaderFile, ClassVariable mVariable)
		{
			EnumList mEnum = mHeaderFile.getEnumList(mVariable.variableType);
			if (mEnum == null)
			{
				return false;
			}

			string strType = mEnum.type;
			if (strType == "")
			{
				strType = "uint16";
			}
			string strNewVariable = "m_e" + mVariable.variableName + "Convert";
			mHeaderFile.addLine("uint16 " + strNewVariable + "(0);");
			mHeaderFile.addLine("mByteReader << " + strNewVariable + ";");
			mHeaderFile.addLine(mVariable.variableName + " = (" + mEnum.enumName + ")" + strNewVariable + ";");
			return true;
		}
		
		public override bool attemptDataGroupWriteCode(HeaderFile mHeaderFile, ClassVariable mVariable, string strDataGroupName)
		{
			EnumList mEnum = mHeaderFile.getEnumList(mVariable.variableType);
			if (mEnum == null)
			{
				return false;
			}

			mHeaderFile.addLine(strDataGroupName + ".setProperty(\"" + mVariable.variableName + "\", (uint16)" + mVariable.variableName + ");");			
			return true;
		}
		public override bool attemptDataGroupReadCode(HeaderFile mHeaderFile, ClassVariable mVariable, string strDataGroupName)
		{
			EnumList mEnum = mHeaderFile.getEnumList(mVariable.variableType);
			if (mEnum == null)
			{
				return false;
			}
			mHeaderFile.addLine(mVariable.variableName + " = (" + mEnum.enumName + ")" + strDataGroupName  + ".getProperty(\"" + mVariable.variableName + "\", (uint16)" + mVariable.variableName + ");");
			return true; 
		}

	}
}
