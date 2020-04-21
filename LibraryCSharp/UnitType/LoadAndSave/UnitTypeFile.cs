using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;


namespace Library.UnitType.LoadAndSave
{
	public class UnitTypeFile
	{
		public enum UNITTYPE_VERSION
		{
			ONE,
			LATEST
		}

		public UnitTypeFile()
		{
			version = UNITTYPE_VERSION.LATEST;
			categoryConfigs = new List<CategoryConfig>();
		}


		[DisplayName("Version")]
		public UNITTYPE_VERSION version { get; set; }

		[DisplayName("Category Configs")]
		public List<CategoryConfig> categoryConfigs { get; set; }

		private bool _createBackup(string strValidPath)
		{
			string strFileNameBackUp = strValidPath + ".backup";
			if( File.Exists(strValidPath) == false )
			{
				return true;	//we can't create a back up if the file isn't there.
			}
			File.Delete(strFileNameBackUp);
			File.Copy(strValidPath, strFileNameBackUp);
			return File.Exists(strFileNameBackUp);
		}
		static public string getValidFileAndPath(string strFileAndPath)
		{
			if(strFileAndPath == null ||
				strFileAndPath == "")
			{
				return "";
			}
			string strFileName = Path.GetFileName(strFileAndPath);
			string strPath = strFileAndPath.Substring(0, strFileAndPath.Length - strFileName.Length);
			if( strPath.Contains(":\\") == false )
			{
				if (strPath == "")
				{
					strPath = ".\\";
				}
				strPath = Path.GetFullPath(strPath);
			}
			if (Directory.Exists(strPath) == false)
			{
				return "";
			}
			if( strPath.EndsWith("\\") == false )
			{
				strPath = strPath + "\\";
			}
			return strPath + strFileName;
		}


		public bool _saveHeaderFiles(UnitTypeManager mManager)
		{
			List<UnitTypeCategory> mCategories = mManager.getCategories();
			foreach (UnitTypeCategory mCategory in mCategories)
			{
				if( mCategory.categoryConfig.enumHeaderFile == null ||
					mCategory.categoryConfig.enumHeaderFile == "" )
				{
					continue;
				}
				UnitTypeSaveToHeader.saveToHeader(mCategory);					
			}
			return true;
		}

		public bool save(UnitTypeManager mManager, string strUnitTypeFile)
		{
			categoryConfigs.Clear();
			string strLocationToSaveFile = getValidFileAndPath(strUnitTypeFile);			
			if ( strLocationToSaveFile == "" )
			{
				return false;
			}
			_createBackup(strLocationToSaveFile);			
			List<UnitTypeCategory> mCategories = mManager.getCategories();
			foreach(UnitTypeCategory mCategory in mCategories)
			{
				if (mCategory.configureForSave())
				{
					categoryConfigs.Add(mCategory.categoryConfig);
				}
			}

			try
			{
				File.WriteAllText(strLocationToSaveFile, JsonConvert.SerializeObject(this, Formatting.Indented), Encoding.ASCII);
				_saveHeaderFiles(mManager);
				UnitTypeBinaryWriter mBinaryWriter = new UnitTypeBinaryWriter();
				mBinaryWriter.save(mManager);
				

			}
			catch (Exception e)
			{
				// Let the user know what went wrong.
				Console.WriteLine("The file could not be saved:");
				Console.WriteLine(e.Message);
				return false;
			}
			return true;


		}

		static public UnitTypeFile load(UnitTypeManager mManager, string strUnitTypeFile)
		{
			
			string strLocationToLoadFile = getValidFileAndPath(strUnitTypeFile);
			if (strLocationToLoadFile == "")
			{
				return null;
			}
			
			try
			{
				mManager.resetUnitTypesAndCategories();
				
				string jsonFile = File.ReadAllText(strLocationToLoadFile);
				UnitTypeFile mUnitTypeFile = JsonConvert.DeserializeObject<UnitTypeFile>(jsonFile);
				return mUnitTypeFile;
			}
			catch (Exception e)
			{
				// Let the user know what went wrong.
				Console.WriteLine("The file could not be saved:");
				Console.WriteLine(e.Message);
				return null;
			}
			
		}

		static public string getRelativePath(string strRelativeorAbsolutePath)
		{
			try
			{
				string strFolderRelativeTo = AppDomain.CurrentDomain.BaseDirectory;
				string strFullFilePath = UnitTypeFile.getValidFileAndPath(strRelativeorAbsolutePath);
				Uri pathUri = new Uri(strFullFilePath);
				// Folders must end in a slash
				if (!strFolderRelativeTo.EndsWith(Path.DirectorySeparatorChar.ToString()))
				{
					strFolderRelativeTo += Path.DirectorySeparatorChar;
				}
				Uri folderUri = new Uri(strFolderRelativeTo);
				return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
			}
			catch
			{

			}
			return strRelativeorAbsolutePath;
		}

	}
}
