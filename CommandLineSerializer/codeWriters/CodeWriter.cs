using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.IO;
using Library.ClassParser;
namespace CommandLineSerializer.codeWriters
{
	public class CodeWriter
	{
		public virtual bool attemptByteWriterCode(HeaderFile mHeaderFile, ClassVariable mVariable) { return false; }
		public virtual bool attemptByteReaderCode(HeaderFile mHeaderFile, ClassVariable mVariable) { return false; }
		public virtual bool attemptDataGroupWriteCode(HeaderFile mHeaderFile, ClassVariable mVariable, string strDataGroupName) { return false; }
		public virtual bool attemptDataGroupReadCode(HeaderFile mHeaderFile, ClassVariable mVariable, string strDataGroupName) { return false; }
	}
}
