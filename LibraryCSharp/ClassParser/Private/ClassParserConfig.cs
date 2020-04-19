using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Library.ClassParser.Private
{
	public class FileStamp
	{
		public string file { get; set; }
		public DateTime writeTime { get; set; }
	}
	public class ClassParserConfig
	{
		public ClassParserConfig()
		{
			files = new List<FileStamp>();
		}
		public List<FileStamp> files { get; set; }


		public bool save(string strPathAndFile)
		{
			string strFileContents = "";
			foreach (FileStamp mFile in files)
			{
				strFileContents = strFileContents + mFile.writeTime.Ticks.ToString() + "|" + mFile.file + Environment.NewLine;
			}
            try
            {
                File.WriteAllText(strPathAndFile, strFileContents);
            }
            catch
            {
                return false;
            }
			return File.Exists(strPathAndFile);
		}

		public bool load(string strPathAndFile)
		{
			if(File.Exists(strPathAndFile) == false )
			{
				return false;
			}
			files.Clear();
			
			string[] mLines = File.ReadAllLines(strPathAndFile);
			foreach( string strLine in mLines)
			{
				string[] mWriteTimeAndFile = strLine.Split('|');
				if( mWriteTimeAndFile.Length == 2 )
				{					
					if (Int64.TryParse(mWriteTimeAndFile[0], out long iTicks))
					{
						FileStamp mFileStamp = new FileStamp();
						mFileStamp.file = mWriteTimeAndFile[1];
						mFileStamp.writeTime = new DateTime(iTicks);
						files.Add(mFileStamp);
					}
				}
			}
			return (files.Count > 0) ? true : false;
		}

		public bool fileAlreadyAdded(string strFile)
		{
			foreach(FileStamp mFile in files)
			{
				if(mFile.file == strFile)
				{
					return true;
				}
			}
			return false;
		}

		private FileStamp getFile(string strFile)
		{
			foreach (FileStamp mFile in files)
			{
				if (mFile.file == strFile)
				{
					return mFile;
				}
			}
			return null;
		}

		public bool removeFile(string strFile)
		{
			for(int iIndex = 0; iIndex < files.Count; iIndex++) 
			{
				FileStamp mFile = files[iIndex];
				if (mFile.file == strFile)
				{
					files.RemoveAt(iIndex);
					return true;
				}
			}
			return false;
		}
		
		public bool addFile(string strFile)
		{
			if( File.Exists(strFile) == false )
			{
				return false;
			}
			removeFile(strFile);
			FileStamp mFileStamp = new FileStamp();
			mFileStamp.file = strFile;
			mFileStamp.writeTime = File.GetLastWriteTime(strFile);
			files.Add(mFileStamp);
			return true;
		}

		public ClassParserConfig getNewerFiles(ClassParserConfig mOldParserToCompareTo)
		{
			ClassParserConfig mConfigWithChanges = new ClassParserConfig();
			foreach (FileStamp mFile in files)
			{
				FileStamp mStamp = mOldParserToCompareTo.getFile(mFile.file);
				if( mStamp == null ||
					mStamp.writeTime != mFile.writeTime)
				{
					mConfigWithChanges.addFile(mFile.file);
				}
			}
			return mConfigWithChanges;
		}
	}
}
