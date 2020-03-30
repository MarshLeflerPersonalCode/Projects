using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Library.IO;

namespace UnitTypeCore.LoadAndSave
{
	public class UnitTypeBinaryWriter
	{
		public bool save(UnitTypeManager mManager)
		{
			string strLocationToSaveFile = UnitTypeFile.getValidFileAndPath(Path.Combine(mManager.getUnitTypeManagerConfig().binaryDirectory, mManager.getUnitTypeManagerConfig().binaryFile) );
			if (strLocationToSaveFile == "")
			{
				return false;
			}
			ByteWriter mWriter = new ByteWriter(Encoding.ASCII);
			//MemoryStream mMemoryStream = new MemoryStream();			
			//BinaryWriter mWriter = new BinaryWriter(mMemoryStream, Encoding.ASCII);
			
			byte iByte = 13;
			int iInt = 13;
			Int64 iInt64 = 13;
			float fFloat = 13.0f;
			double fDouble = 13.0;
			string strString = "13131313131313131313";
			
			mWriter.write(true);
			mWriter.write(iByte);
			mWriter.write(iInt);
			mWriter.write(iInt64);
			mWriter.write(fFloat);
			mWriter.write(fDouble);
			mWriter.write(strString);
			File.WriteAllBytes(strLocationToSaveFile, mWriter.getMemoryStream().ToArray());
			/*
			_writeHeader(mWriter);
			_createStringLookup(mWriter);
			_writeStringLookup(mWriter);
			*/
			return true;
		}


		private bool _writeHeader(BinaryWriter mWriter)
		{
			return true;
		}

		private bool _createStringLookup(BinaryWriter mWriter)
		{
			return true;
		}
		private bool _writeStringLookup(BinaryWriter mWriter)
		{
			return true;
		}
	}
}
