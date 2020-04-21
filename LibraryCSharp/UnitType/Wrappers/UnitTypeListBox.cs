using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library.UnitType.Wrappers
{
	public class UnitTypeListBox
	{
		
		private string m_strUnitTypeFilter = "";
		private string m_strFilterString = "";
		private string m_strCategory = "";
		private List<string> m_UnitTypesToHide = new List<string>();
		bool m_bConfigured = false;
		public UnitTypeListBox(UnitTypeManager mManager, string strCategory, ListBox mListBox, string strUnitTypeFilter, string strFilterString, List<string> mUnitTypesToHide)
		{
			unitTypeManager = mManager;
			
			filterUnitType = strUnitTypeFilter;
			filterString = strFilterString;
			listBoxControlling = mListBox;
			if( mUnitTypesToHide != null &&
				mUnitTypesToHide.Count > 0 )
			{
				m_UnitTypesToHide = mUnitTypesToHide.ToList();
			}
			m_bConfigured = true;
			setCategory(strCategory); //set category requests a refresh
			//requestRefresh();
			
		}
		public void setCategory(string strCategory)
		{
			if (m_strCategory != strCategory)
			{
				m_strCategory = strCategory;
				requestRefresh();
			}
		}


		//sets or gets the list box controlling
		public ListBox listBoxControlling { get; set; }


		//the unit type to filter by
		public UnitTypeManager unitTypeManager { get; set; }

		//the unit type to filter by a unittype name
		public string filterUnitType 
		{
			get { return m_strUnitTypeFilter; }
			set
			{
				m_strUnitTypeFilter = (value != null) ? value : "";
				requestRefresh();
			}
		}

		//filters the unittypes selected by the string
		public string filterString
		{
			get { return m_strFilterString; }
			set
			{
				m_strFilterString = (value != null) ? value.ToUpper() : "";
				requestRefresh();
			}
		}

		public void setUnitTypeAndStringFilters( string strUnitTypeFilter, string strFilterString )
		{
			m_strUnitTypeFilter = (strUnitTypeFilter != null) ? strUnitTypeFilter : "";
			m_strFilterString = (strFilterString != null) ? strFilterString.ToUpper() : "";
			requestRefresh();
		}

		//refreshes the list box
		public void requestRefresh()
		{
			if( m_bConfigured == false )
			{
				return;
			}
			UnitTypeCategory mCategory = unitTypeManager.getCategory(m_strCategory);
			string strSelectedItem = (listBoxControlling.SelectedItem != null) ? listBoxControlling.SelectedItem.ToString() : "";
			listBoxControlling.Items.Clear();
			if(mCategory == null )
			{
				return;
			}

			List<UnitType> mUnitTypes = mCategory.getUnitTypes();
			
			foreach (UnitType mUnitType in mUnitTypes)
			{
				if(m_strUnitTypeFilter != "" )
				{
					if( mUnitType.isa(m_strUnitTypeFilter) == false )
					{
						continue;
					}
				}
				if( m_strFilterString != "" )
				{
					if( mUnitType.unitTypeName.Contains(m_strFilterString) == false )
					{
						continue;
					}
				}
				if(m_UnitTypesToHide.Contains(mUnitType.unitTypeName))
				{
					continue;
				}

				listBoxControlling.Items.Add(mUnitType.unitTypeName);
			}

			if( strSelectedItem != "" )
			{
				int iIndexOf = listBoxControlling.Items.IndexOf(strSelectedItem);
				if (iIndexOf >= 0)
				{
					listBoxControlling.SelectedItem = strSelectedItem;
					return;
				}
			}
			if (listBoxControlling.Items.Count > 0)
			{
				listBoxControlling.SelectedIndex = 0;
			}

		}
	}
}
