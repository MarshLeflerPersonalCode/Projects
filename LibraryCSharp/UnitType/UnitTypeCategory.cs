using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.UnitType.LoadAndSave;
namespace Library.UnitType
{
	public class UnitTypeCategory
	{
		private CategoryConfig m_CategoryConfig = null;
		private UnitTypeManager m_Manager = null;
		private List<UnitType> m_UnitTypes = new List<UnitType>();		
		private Dictionary<string, int> m_UnitTypesByName = new Dictionary<string, int>();
		private bool m_bDictionaryLookUpForUnitTypesDirty = true;
		private bool m_bConfigured = true;
		private bool m_bBuildHieararchyValid = true;
		public UnitTypeCategory(UnitTypeManager mManager)
		{
			m_Manager = mManager;
			m_CategoryConfig = new CategoryConfig();
			m_CategoryConfig.m_OwningCategory = this;
		}

		public CategoryConfig categoryConfig
		{
			get{ return m_CategoryConfig; }
			set
			{
				m_CategoryConfig = value;
			}
		}		


		public string categoryName
		{
			get { return m_CategoryConfig.categoryName; }
			set { m_CategoryConfig.categoryName = value; }
		}


		public UnitTypeManager unitTypeManager { get { return m_Manager; } }

		//adds a new unit type
		public UnitType createUnitType(string strUnitType)
		{
			if( getUnitTypeByUnsafeName(strUnitType) != null)
			{
				return null;
			}			
			m_UnitTypes.Add(new UnitType(this, strUnitType));
			m_bDictionaryLookUpForUnitTypesDirty = true;
			m_Manager.needsToSave = true;
			m_Manager.requestRefresh();
			return m_UnitTypes[m_UnitTypes.Count-1];
		}

		//removes a unit type by unsafe name. It converts it to a code safe name
		public bool deleteUnitTypeByUnsafeName(string strUnitTypeName)
		{
			return delteUnitTypeByCodeName(UnitType.convertToCodeName(strUnitTypeName));
		}
		//removes a unit type by code name.
		public bool delteUnitTypeByCodeName(string strUnitTypeName)
		{
			return deleteUnitType(getUnitTypeByCodeName(strUnitTypeName));
		}
		//deletes a unit type
		public bool deleteUnitType(UnitType mUnitType)
		{
			if (mUnitType != null &&
				mUnitType.unitTypeName != "ANY" &&
				m_UnitTypes.Contains(mUnitType))
			{
				m_UnitTypes.Remove(mUnitType);
				m_bDictionaryLookUpForUnitTypesDirty = true;
				_buildHierarchy();
				m_Manager.needsToSave = true;
				m_Manager.requestRefresh();
				return true;
			}
			return false;
		}


		//returns the unit type by it's name - note it will convert the unittype name into a code name and pass it along. better to call getUnitTypeByCodeName
		public UnitType getUnitTypeByUnsafeName(string strUnitTypeName)
		{
			return getUnitTypeByCodeName(UnitType.convertToCodeName(strUnitTypeName));
		}
		//returns the unit type by it's code name
		public UnitType getUnitTypeByCodeName(string strUnitTypeCodeName)
		{
			_reconfigureDictionaryOfUnitTypesByName(false);
			if(m_UnitTypesByName.ContainsKey(strUnitTypeCodeName))
			{
				int iIndex = m_UnitTypesByName[strUnitTypeCodeName];
				return m_UnitTypes[iIndex];
			}
			return null;
		}

		//fills out the dictionary of unit types by name.
		private void _reconfigureDictionaryOfUnitTypesByName(bool bForceReconfigure)
		{
			if (bForceReconfigure == false)	//if I'm not forcing it to reconfigure, check to see if it's dirty and I need to reconfigure.
			{
				if (m_bDictionaryLookUpForUnitTypesDirty == false)  //nothing is dirty. Don't need to do it.
				{
					return;
				}
			}
			m_bDictionaryLookUpForUnitTypesDirty = false;
			m_UnitTypesByName.Clear();
			for(int iIndex = 0; iIndex < m_UnitTypes.Count; iIndex++) 
			{
				UnitType mUnitType = m_UnitTypes[iIndex];
				m_UnitTypesByName.Add(mUnitType.unitTypeName, iIndex);
			}
		}


		//recursively goes through a single unittype making adding all the children assigned to it.
		private void _buildHiearchyForUnitType(UnitType mUnitType)
		{
			if( mUnitType == null)
			{
				return;
			}
			List<string> mAllChildren = new List<string>();
			if ( mUnitType.immediateChildren.Count == 0 )
			{
				mUnitType.setFullHierarchy(mAllChildren);
				return; //we are done here
			}
			if(mUnitType.getFullHierarchyConfigured())
			{
				return; //we have already been configured. This can happen a lot, especially with unittypes that share a specific unit type
			}
			foreach( string strChildName in mUnitType.immediateChildren)
			{
				UnitType mChildUnitType = getUnitTypeByCodeName(strChildName);
				if(mChildUnitType == null)
				{
					//probably should log here
					continue;					
				}
				if( mChildUnitType.getFullHierarchyConfigured() == false )
				{
					_buildHiearchyForUnitType(mChildUnitType);
				}
				if( mAllChildren.Contains(mChildUnitType.unitTypeName) == false )
				{
					mAllChildren.Add(mChildUnitType.unitTypeName);
				}
				foreach(string strChildrenOfChild in mChildUnitType.fullHierarchyOfChildren)
				{
					if (mAllChildren.Contains(strChildrenOfChild) == false)
					{
						mAllChildren.Add(strChildrenOfChild);
					}
				}

			}
			mUnitType.setFullHierarchy(mAllChildren);
		}

		//rebuilds the hierarchy. First by reseting all the unittypes, then rebuilding it through a recursive function
		private void _buildHierarchy()
		{
			if(m_bBuildHieararchyValid == false)
			{
				return;
			}
			m_bBuildHieararchyValid = false;
			m_bDictionaryLookUpForUnitTypesDirty = true;
			//reset all the hierarchies
			foreach(UnitType mUnitType in m_UnitTypes)
			{
				mUnitType.resetFullHierarchy();
			}
			//now rebuild the hierarchies
			for (int iIndex = 0; iIndex < m_UnitTypes.Count; iIndex++)
			{
				_buildHiearchyForUnitType(m_UnitTypes[iIndex]);
			}			
			m_bBuildHieararchyValid = true;
		}

		//gets called from a unittype to notify that we need to update the other unittypes of the change
		public void childAddedToUnitType(UnitType mType)
		{
			m_Manager.needsToSave = true;
			_buildHierarchy();
			m_Manager.requestRefresh();
		}

		//gets called from a unittype to notify that we need to update the other unittypes of the change
		public void childRemovedFromUnitType(UnitType mType)
		{
			m_Manager.needsToSave = true;
			_buildHierarchy();
			m_Manager.requestRefresh();
		}

		//this should be called from a unit type when it has it's name changed. It will then go through all the other unit types and update them.
		public void notifyOfUnitTypeNameChange(string strOldName, string strNewName)
		{
			if( m_bConfigured == false )
			{
				return;
			}
			m_bDictionaryLookUpForUnitTypesDirty = true;
			m_Manager.needsToSave = true;
			m_Manager.requestRefresh();
			/*foreach (UnitType mUnitType in m_UnitTypes)
			{
				mUnitType._notifyOfChildNameChange(strOldName, strNewName);
			}*/
		}

		//returns the unit types in an array
		public List<UnitType> getUnitTypes()
		{
			return m_UnitTypes;
		}
		//returns the unit types in an array
		public List<UnitType> unitTypes { get { return m_UnitTypes; } }

		//returns a list of unit types of a specific unit type
		public List<UnitType> getUnitTypesOfType(string strUnitTypeCodeName)
		{	
			if( strUnitTypeCodeName == null ||
				strUnitTypeCodeName == "" )
			{
				return m_UnitTypes;
			}
			List<UnitType> mList = new List<UnitType>();
			UnitType mUnitTypeToTestWith = getUnitTypeByCodeName(strUnitTypeCodeName);
			if (mUnitTypeToTestWith == null)
			{
				return mList;
			}
			foreach( UnitType mUnitType in m_UnitTypes)
			{
				if( mUnitType.isa(strUnitTypeCodeName))
				{
					mList.Add(mUnitType);
				}
			}

			return mList;
		}

		public bool configureForSave()
		{
			if( m_UnitTypes.Count == 0 )
			{
				return false;
			}
			m_CategoryConfig.unitTypes = m_UnitTypes;
			return true;
		}

		public bool loadFromConfig(CategoryConfig mConfig)
		{
			if( mConfig == null )
			{
				return false;
			}
			m_CategoryConfig = mConfig;
			m_CategoryConfig.m_OwningCategory = this;
			m_UnitTypes = new List<UnitType>(m_CategoryConfig.unitTypes);
			foreach( UnitType mUnitType in m_UnitTypes)
			{
				mUnitType._setUnitTypeCategoryOwner(this);
			}
			m_bConfigured = true;
			m_bDictionaryLookUpForUnitTypesDirty = true;
			_buildHierarchy();
			return true;
		}

		public void notifyOfCategoryNameChange()
		{
			m_Manager.requestCategoryRefresh();
		}

	}
}
