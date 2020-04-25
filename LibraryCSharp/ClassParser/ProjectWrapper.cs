using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace Library.ClassParser
{
	public class ProjectWrapper
	{
		private Mutex m_ClassMutex = new Mutex();
		private Mutex m_EnumMutex = new Mutex();
		private Mutex m_DefinesMutex = new Mutex();
		private Dictionary<string, ClassStructure> m_ClassStructures = new Dictionary<string, ClassStructure>();
		private Dictionary<string, EnumList> m_EnumLists = new Dictionary<string, EnumList>();
		private Dictionary<string, string> m_Defines = new Dictionary<string, string>();
		public ProjectWrapper()
		{
		}

        public List<ClassStructure> getClassesInheritingFromClass(string strClass)
        {
            ClassStructure mStructure = getClassStructByName(strClass);
            if( mStructure == null)
            {
                return null;
            }

            List<ClassStructure> mList = new List<ClassStructure>();
            mList.Add(mStructure);
            foreach(ClassStructure classStructure in m_ClassStructures.Values)
            {
                if( classStructure.classStructuresInheritingFrom.Contains(strClass))
                {
                    mList.Add(classStructure);
                }
            }
            return mList;
        }


        public ClassStructure getClassStructByName(string strName)
		{
			if(m_ClassStructures.ContainsKey(strName.ToUpper()))
			{
				return m_ClassStructures[strName.ToUpper()];
			}
			return null;
		}
		public EnumList getEnumListByName(string strName)
		{
			if (m_EnumLists.ContainsKey(strName.ToUpper()))
			{
				return m_EnumLists[strName.ToUpper()];
			}
			return null;
		}
		public bool addClassStructure(ClassStructure mStructure )
		{
			if( mStructure == null ||
				mStructure.name == null ||
				mStructure.name == "" )
			{
				return false;
			}
			m_ClassMutex.WaitOne();
			m_ClassStructures[mStructure.name.ToUpper()] = mStructure;
			foreach( EnumList mList in mStructure.enums)
			{
				addEnumList(mList);
			}
			foreach (KeyValuePair<string, string> mDefine in mStructure.defines)
			{
				addDefine(mDefine.Key, mDefine.Value);
			}
			m_ClassMutex.ReleaseMutex();
			return true;
		}

		public Dictionary<string, ClassStructure> classStructures { get { return m_ClassStructures; } }

		public bool addEnumList(EnumList mEnum)
		{
			if (mEnum == null ||
				mEnum.enumName == null ||
				mEnum.enumName == "")
			{
				return false;
			}
			m_EnumMutex.WaitOne();
			m_EnumLists[mEnum.enumName.ToUpper()] = mEnum;
			m_EnumMutex.ReleaseMutex();
			return true;
		}

		public Dictionary<string, EnumList> enums { get { return m_EnumLists; } }


		public bool addDefine(string strName, string strValue)
		{
			if (strName == null ||
				strValue == null ||
				strName == "")
			{
				return false;
			}
			m_DefinesMutex.WaitOne();
			m_Defines[strName.ToUpper()] = strValue;
			m_DefinesMutex.ReleaseMutex();
			return true;
		}

		public Dictionary<string, string> defines { get { return m_Defines; } }

		public int removeAllInsideFile(string strFile)
		{
			int iRemoved = 0;
			List<string> mRemove = new List<string>();
			foreach (KeyValuePair<string, ClassStructure> mEntry in m_ClassStructures)
			{
				if( mEntry.Value.file == strFile )
				{
					mRemove.Add(mEntry.Key);
				}
			}
			iRemoved = iRemoved + mRemove.Count;
			foreach (string strRemove in mRemove)
			{
				m_ClassStructures.Remove(strRemove);
			}
			mRemove.Clear();
			
			foreach (KeyValuePair<string, EnumList> mEnum in m_EnumLists)
			{
				if (mEnum.Value.file == strFile)
				{
					mRemove.Add(mEnum.Key);
				}
			}
			foreach (string strRemove in mRemove)
			{
				m_EnumLists.Remove(strRemove);
			}
			iRemoved = iRemoved + mRemove.Count;
			return iRemoved;
		}
	}

}
