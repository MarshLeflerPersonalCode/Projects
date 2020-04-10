using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.IO;
using Library.ClassParser;

namespace CommandLineSerializer.codeWriters
{
	public class PrimaryVariableTypeWriter : CodeWriter
	{
		public override bool attemptByteWriterCode(HeaderFile mHeaderFile, ClassVariable mVariable)
		{
			mHeaderFile.addLine("mByteWriter << " + mVariable.variableName + ";");
			return true; 
		}
		public override bool attemptByteReaderCode(HeaderFile mHeaderFile, ClassVariable mVariable)
		{
			mHeaderFile.addLine("mByteReader << " + mVariable.variableName + ";");
			return true;
		}
		public override bool attemptDataGroupWriteCode(HeaderFile mHeaderFile, ClassVariable mVariable, string strDataGroupName)
		{
			mHeaderFile.addLine(strDataGroupName + ".setProperty(\"" + mVariable.variableName + "\", " + mVariable.variableName + ");");
			return true;
		}
		public override bool attemptDataGroupReadCode(HeaderFile mHeaderFile, ClassVariable mVariable, string strDataGroupName)
		{
			mHeaderFile.addLine(mVariable.variableName + " = " + strDataGroupName + ".getProperty(\"" + mVariable.variableName + "\", " + mVariable.variableName + ");");
			return true;
		}
	}
}
