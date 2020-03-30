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
			_writeHeader(mWriter);						
			_writeCategoryData(mManager, mWriter);

			File.WriteAllBytes(strLocationToSaveFile, mWriter.getMemoryStream().ToArray());
			
			return true;
		}





		private bool _writeHeader(ByteWriter mWriter)
		{
			byte iVersion = 1;
			mWriter.write(iVersion);
			return true;
		}

		private bool _writeCategoryData(UnitTypeManager mManager, ByteWriter mWriter)
		{
			List<UnitTypeCategory> mCategories = mManager.getCategories();
			ushort iCountOfCategories = (ushort)mCategories.Count;			
			mWriter.write(iCountOfCategories);
			foreach(UnitTypeCategory mCategory in mCategories)
			{
				
				ushort iCountOfUnitTypes = (ushort)mCategory.unitTypes.Count;				
				byte iCountOfBinaryLookupInts = (byte)((mCategory.unitTypes.Count > 0) ? mCategory.unitTypes[0].bitLookupArray.Count : 0);
				
				mWriter.write(mCategory.categoryName);
				mWriter.write(iCountOfUnitTypes);
				mWriter.write(iCountOfBinaryLookupInts);
				foreach (UnitType mUnitType in mCategory.unitTypes)
				{
					mWriter.write(mUnitType.unitTypeName);
					foreach(int iBitLookUp in mUnitType.bitLookupArray)
					{
						mWriter.write(iBitLookUp);
					}
				}
			}
			return true;
		}
	}
}
