using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using UnitTypeCore.LoadAndSave;
using UnitTypeCore.Wrappers;
using UnitTypeCore.Forms;
namespace UnitTypeCore
{
	public class UnitTypeManager
	{
		private UnitTypeManagerConfigForm m_ConfigForm = null;
		private UnitTypeManagerConfig m_UnitTypeManagerConfig = null;
		private bool m_bConfigured = false;
		private List<UnitTypeTreeViewManager> m_TreeViewManagers = new List<UnitTypeTreeViewManager>();
		private List<UnitTypeListComboBox> m_UnitTypeComboBoxes = new List<UnitTypeListComboBox>();
		private List<UnitTypeListBox> m_UnitTypeListBoxes = new List<UnitTypeListBox>();
		private List<UnitTypeCategoryListBoxManager> m_CategoryListBoxManager = new List<UnitTypeCategoryListBoxManager>();
		private List<UnitTypeCategory> m_Categories = new List<UnitTypeCategory>();
		private UnitTypeCategory m_LastCategory = null;
		public UnitTypeManager()
		{
			m_UnitTypeManagerConfig = UnitTypeManagerConfig.createFromConfigFile();			
			_handleInitialConfiguration();
			m_bConfigured = true;
			requestRefresh();
			needsToSave = false;
		}

		public bool needsToSave { get; set; }

		public string unitTypeSaveLocation
		{
			get
			{
				return m_UnitTypeManagerConfig.unittypeConfigFile;
			}
			set
			{
				m_UnitTypeManagerConfig.unittypeConfigFile = value;
			}
		}

		public UnitTypeManagerConfig getUnitTypeManagerConfig() { return m_UnitTypeManagerConfig;  }

		public bool configured() { return m_bConfigured; }

		public void _handleInitialConfiguration()
		{
			if( getHasValidSavePath())
			{
				load(unitTypeSaveLocation, true);
			}
		}

		public void registerTreeViewManager(UnitTypeTreeViewManager mManager)
		{
			if (m_TreeViewManagers.Contains(mManager) == false)
			{
				m_TreeViewManagers.Add(mManager);
				if (m_bConfigured)
				{
					mManager.requestRefresh();
				}
			}
		}

		public void registerCategoryListBoxManager(UnitTypeCategoryListBoxManager mCategoryListBoxManager)
		{
			if(m_CategoryListBoxManager.Contains(mCategoryListBoxManager) == false)
			{
				m_CategoryListBoxManager.Add(mCategoryListBoxManager);
				if(m_bConfigured)
				{
					mCategoryListBoxManager.requestRefresh();
				}
			}
		}

		public bool registerComboBox(ComboBox mComboBox, string strCategory, bool bEmptySlot)
		{
			return registerComboBox(mComboBox, strCategory, null, bEmptySlot);
		}
		
		public bool setComboBoxCategory(ComboBox mComboBox, string strCategory)
		{
			foreach (UnitTypeListComboBox mUnitTypeCombo in m_UnitTypeComboBoxes)
			{
				if (mUnitTypeCombo.getComboBox() == mComboBox)
				{
					mUnitTypeCombo.setCategory(strCategory);
					return true;
				}
			}
			return false;
		}

		public bool registerComboBox(ComboBox mComboBox, string strCategory, UnitType mUnitType, bool bEmptySlot)
		{
			foreach( UnitTypeListComboBox mUnitTypeCombo in m_UnitTypeComboBoxes)
			{
				if(mUnitTypeCombo.getComboBox() == mComboBox)
				{
					if( mUnitType != null )
					{
						changeComboBoxFilter(mComboBox, mUnitType);
					}
					return true;
				}
			}
			UnitTypeListComboBox mUnitTypeComboBox = new UnitTypeListComboBox(this, strCategory, mComboBox, (mUnitType != null)?mUnitType.unitTypeName:"", bEmptySlot);
			m_UnitTypeComboBoxes.Add(mUnitTypeComboBox);
			return true;
		}

		public bool removeComboBox(ComboBox mComboBox)
		{
			for (int iIndex = 0; iIndex > m_UnitTypeComboBoxes.Count; iIndex++)
			{
				UnitTypeListComboBox mListComboBox = m_UnitTypeComboBoxes[iIndex];
				if (mListComboBox.getComboBox() == mComboBox)
				{
					m_UnitTypeComboBoxes.RemoveAt(iIndex);
					return true;
				}
			}
			return false;
		}

		//registers a list box
		public bool registerListBox(ListBox mListBox, string strCategory, UnitType mUnitTypeFilter, string strFilterString, List<string> mUnitTypesToHide)
		{
			removeListBox(mListBox);
			UnitTypeListBox mUnitTypeListBox = new UnitTypeListBox(this, 
																   strCategory,
																   mListBox,
																   (mUnitTypeFilter != null) ? mUnitTypeFilter.unitTypeName : "", 
																   strFilterString,
																   mUnitTypesToHide);
			m_UnitTypeListBoxes.Add(mUnitTypeListBox);
			return true;
		}
		public bool setListBoxCategory(ListBox mListBox, string strCategory)
		{
			for (int iIndex = 0; iIndex < m_UnitTypeListBoxes.Count; iIndex++)
			{
				UnitTypeListBox mListBoxTesting = m_UnitTypeListBoxes[iIndex];
				if (mListBoxTesting.listBoxControlling == mListBox)
				{
					mListBoxTesting.setCategory(strCategory);
					return true;
				}
			}
			return false;
		}
		//removes a list box
		public bool removeListBox(ListBox mListBox)
		{
			for (int iIndex = 0; iIndex < m_UnitTypeListBoxes.Count; iIndex++)
			{
				UnitTypeListBox mListBoxTesting = m_UnitTypeListBoxes[iIndex];
				if (mListBoxTesting.listBoxControlling == mListBox)
				{
					m_UnitTypeListBoxes.RemoveAt(iIndex);
					return true;
				}
			}
			return false;
		}

		public void updateListBoxFilters(ListBox mListBox, UnitType mUnitType, string strFilterString)
		{
			for (int iIndex = 0; iIndex < m_UnitTypeListBoxes.Count; iIndex++)
			{
				UnitTypeListBox mListBoxTesting = m_UnitTypeListBoxes[iIndex];
				if (mListBoxTesting.listBoxControlling == mListBox)
				{
					mListBoxTesting.setUnitTypeAndStringFilters( strFilterString,
																(mUnitType != null) ? mUnitType.unitTypeName : "");
					return;
				}
			}
		}

		


		//resets to show all the unit type
		public bool changeComboBoxFilter(ComboBox mComboBox)
		{
			return changeComboBoxFilter(mComboBox, null);
		}
		public bool changeComboBoxFilter(ComboBox mComboBox, UnitType mUnitType)
		{
			if (mComboBox != null)
			{
				foreach (UnitTypeListComboBox mUnitTypeCombo in m_UnitTypeComboBoxes)
				{
					if (mUnitTypeCombo.getComboBox() == mComboBox)
					{
						mUnitTypeCombo.resetFilterByUnitType(mUnitType);
						return true;
					}
				}
			}
			return false;
		}
		

		

		public void requestRefresh()
		{
			if( m_bConfigured == false )
			{
				return;
			}
			requestComboBoxesRefresh();
			requestListBoxesRefresh();
			requestTreeViewManagersRefresh();
			requestCategoryRefresh();
			


		}

		public void requestCategoryRefresh()
		{
			if (m_bConfigured == false)
			{
				return;
			}
			requestCategoryListBoxManagersRefresh();
		}

		public void requestListBoxesRefresh()
		{
			foreach (UnitTypeListBox mUnitTypeListBox in m_UnitTypeListBoxes)
			{
				mUnitTypeListBox.requestRefresh();
			}
		}

		public void requestComboBoxesRefresh()
		{
			foreach(UnitTypeListComboBox mUnitTypeComboBox in m_UnitTypeComboBoxes)
			{
				mUnitTypeComboBox.requestRefresh();
			}
		}

		public void requestTreeViewManagersRefresh()
		{
			if( m_bConfigured == false)
			{
				return;
			}
			foreach(UnitTypeTreeViewManager mManager in m_TreeViewManagers)
			{
				mManager.requestRefresh();
			}
		}

		public void requestCategoryListBoxManagersRefresh()
		{
			if (m_bConfigured == false)
			{
				return;
			}
			foreach (UnitTypeCategoryListBoxManager mManager in m_CategoryListBoxManager)
			{
				mManager.requestRefresh();
			}
		}

		//returns a list of all the unit types
		public List<UnitType> getUnitTypes(string strCategoryName) 
		{
			UnitTypeCategory mCategory = getCategory(strCategoryName);
			if (mCategory != null)
			{
				return mCategory.getUnitTypes();
			}
			return null;
		}

		//returns the unit type by it's name - note it will convert the unittype name into a code name and pass it along. better to call getUnitTypeByCodeName
		public UnitType getUnitTypeByUnsafeName(string strCategoryName, string strUnitTypeName)
		{
			UnitTypeCategory mCategory = getCategory(strCategoryName);
			if (mCategory != null)
			{
				return mCategory.getUnitTypeByUnsafeName(strUnitTypeName);
			}
			return null;
		}
		//returns the unit type by it's code name
		public UnitType getUnitTypeByCodeName(string strCategoryName, string strUnitTypeCodeName)
		{
			UnitTypeCategory mCategory = getCategory(strCategoryName);
			if (mCategory != null)
			{
				return mCategory.getUnitTypeByCodeName(strUnitTypeCodeName);
			}
			return null;
		}

		//returns a list of unit types of a specific unit type
		public List<UnitType> getUnitTypesOfType(string strCategoryName, string strUnitTypeCodeName)
		{
			UnitTypeCategory mCategory = getCategory(strCategoryName);
			if (mCategory != null)
			{
				return mCategory.getUnitTypesOfType(strUnitTypeCodeName);
			}
			return null;		
		}

		//this function can return null if the unit type is already created with that name
		public UnitType createUnitType(string strCategoryName, string strUnitTypeName)
		{
			UnitTypeCategory mCategory = getCategory(strCategoryName);
			if (mCategory != null)
			{
				return mCategory.createUnitType(strUnitTypeName);
			}
			return null;			
		}

		//deletes a unit type
		public bool deleteUnitType(UnitType mUnitType)
		{
			if(mUnitType != null)
			{
				return mUnitType.deleteUnitType();				
			}
			return false;
		}
		public bool getHasValidSavePath()
		{
			return (UnitTypeFile.getValidFileAndPath(unitTypeSaveLocation) != "") ? true : false;
		}

		public bool save()
		{
			if( getHasValidSavePath() == false )
			{
				return false;
			}
			return saveAs(unitTypeSaveLocation);
		}
		public bool saveAs(string strUnitTypeFile)
		{
			
			UnitTypeFile mUnitTypeFile = new UnitTypeFile();
			if( mUnitTypeFile.save(this, strUnitTypeFile) == true )
			{
				
				unitTypeSaveLocation = UnitTypeFile.getRelativePath(strUnitTypeFile);
				m_UnitTypeManagerConfig.save();
				needsToSave = false;
				return true;
			}
			return false;
		}

		public bool load(string strUnitTypeFile, bool bOkayToWipeCurrentState)
		{
			if(needsToSave == true &&
				bOkayToWipeCurrentState == false )
			{
				return false;
			}
			UnitTypeFile mUnitTypeFile = UnitTypeFile.load(this, strUnitTypeFile);
			if (mUnitTypeFile != null)
			{
				m_bConfigured = false;
				foreach(CategoryConfig mConfig in mUnitTypeFile.categoryConfigs)
				{
					UnitTypeCategory mCategory = new UnitTypeCategory(this);
					if (mCategory.loadFromConfig(mConfig))
					{
						m_Categories.Add(mCategory);
					}					
				}
				m_bConfigured = true;
				requestRefresh();
				return true;
			}
			return false;
		}

		//wipes out all the categories and unit types
		public void resetUnitTypesAndCategories()
		{
			m_LastCategory = null;
			m_Categories.Clear();
		}
		public UnitTypeCategory getCategoryUnsafeName(string strCategoryName)
		{
			return getCategory(UnitType.convertToCodeName(strCategoryName));			
		}
		public UnitTypeCategory getCategory(string strCategoryName)
		{			
			if(m_LastCategory != null &&
				m_LastCategory.categoryName == strCategoryName)
			{
				return m_LastCategory;
			}
			foreach(UnitTypeCategory mCategory in m_Categories)
			{
				if(strCategoryName == mCategory.categoryName)
				{
					m_LastCategory = mCategory;
					return mCategory;
				}
			}
			return null;
		}

		public UnitTypeCategory createCategory(string strCategoryName)
		{
			UnitTypeCategory mCategory = getCategoryUnsafeName(strCategoryName);
			if( mCategory == null)
			{
				mCategory = new UnitTypeCategory(this);
				mCategory.categoryName = strCategoryName;
				m_Categories.Add(mCategory);
				requestCategoryRefresh();
			}
			return mCategory;
		}
		public bool deleteCategory(string strCategoryToDelete)
		{
			return deleteCategory(getCategoryUnsafeName(strCategoryToDelete));
		}
		public bool deleteCategory(UnitTypeCategory mCategoryToDelete)
		{
			if(mCategoryToDelete == null)
			{
				return false;
			}
			if( m_Categories.Contains(mCategoryToDelete) )
			{
				m_LastCategory = null;
				m_Categories.Remove(mCategoryToDelete);
				requestRefresh();
				return true;
			}
			return false;
		}
		public List<UnitTypeCategory> getCategories()
		{
			return m_Categories;
		}

		public void showUnitTypeManagerConfigForm(Form mParentForm)
		{
			if( m_ConfigForm  == null)
			{
				m_ConfigForm = new UnitTypeManagerConfigForm();
			}
			m_ConfigForm.configure(m_UnitTypeManagerConfig);
			if ( m_ConfigForm.ShowDialog(mParentForm) == DialogResult.Yes)
			{
				m_UnitTypeManagerConfig.save();
			}
		}
	}
}
