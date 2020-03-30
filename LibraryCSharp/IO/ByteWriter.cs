using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Library.IO
{
	public class ByteWriter
	{
		MemoryStream m_MemoryStream = null;
		BinaryWriter m_Writer = null;

		public ByteWriter(Encoding eEncoding )
		{
			m_MemoryStream = new MemoryStream();
			m_Writer = new BinaryWriter(m_MemoryStream, eEncoding);
		}

		public MemoryStream getMemoryStream() { return m_MemoryStream; }

		public void write(bool bValue)
		{
			m_Writer.Write(bValue);

		}
		//char character
		public void write(char mChar)
		{
			m_Writer.Write(mChar);

		}
		//an signed 8bit int
		public void write(byte iValue)
		{
			m_Writer.Write(iValue);

		}
		//an unsigned 16bit int
		public void write(ushort iValue)
		{
			m_Writer.Write(iValue);

		}
		//an signed 16bit int
		public void write(short iValue)
		{
			m_Writer.Write(iValue);

		}
		//an signed 32bit int
		public void write(int iValue)
		{
			m_Writer.Write(iValue);

		}

		//an unsigned 32bit int
		public void write(uint iValue)
		{
			m_Writer.Write(iValue);

		}

		//an signed 64bit int
		public void write(long iValue)
		{
			m_Writer.Write(iValue);

		}
		//an unsigned 64bit int
		public void write(ulong iValue)
		{
			m_Writer.Write(iValue);

		}

		//writes a 32bit float
		public void write(float fValue)
		{
			m_Writer.Write(fValue);
		}

		//writes a 64big double
		public void write(double dValue)
		{
			m_Writer.Write(dValue);
		}

		//writes a char array
		public void write(char[] mCharArray)
		{
			ushort iCount = 0;
			if ( mCharArray == null )
			{
				m_Writer.Write(iCount);
				return;
			}
			iCount = (ushort)mCharArray.Count();
			m_Writer.Write(iCount);
			if( iCount  == 0 )
			{
				return;
			}
			m_Writer.Write(mCharArray);
		}
		//writes a char array
		public void write(int[] intArray)
		{
			
			char[] mCharArray = Array.ConvertAll<int, char>(intArray, new Converter<int, char>(delegate(int iValue) { return Convert.ToChar(iValue); }));
			m_Writer.Write(mCharArray);
		}

		//writes a string first by saving the count as a ushort
		public void write(string strValue)
		{
			
			if( strValue == null ||
				strValue == "")
			{
				ushort iCount = 0;
				m_Writer.Write(iCount);
				return;
			}
			write(strValue.ToCharArray());
		}

	}
}
