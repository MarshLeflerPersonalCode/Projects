using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnitTypeCore.Wrappers
{
	public class UnitTypeListComboBox
	{
		private string m_strCategory = "";
		private UnitTypeManager m_UnitTypeManager = null;
		private ComboBox m_ComboBox = null;
		private string m_strFilterUnitTypeBy = "";
		private bool m_bAddEmptySlot = false;
		public UnitTypeListComboBox(UnitTypeManager mManager, string strCategory, ComboBox mComboBox, string strFilterByUnitType, bool bAddEmptySlot)
		{			
			m_UnitTypeManager = mManager;
			m_ComboBox = mComboBox;
			m_strFilterUnitTypeBy = strFilterByUnitType;
			m_bAddEmptySlot = bAddEmptySlot;
			setCategory(strCategory); //set category does a refrsh
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

		public string getUnitTypeFilter() { return m_strFilterUnitTypeBy; }
		public ComboBox getComboBox() { return m_ComboBox; }

		//this can be null
		public void resetFilterByUnitType(UnitType mNewFilterUnitType)
		{
			string strOldUnitType = m_strFilterUnitTypeBy;
			if( mNewFilterUnitType == null)
			{
				m_strFilterUnitTypeBy = "";
			}
			else
			{
				m_strFilterUnitTypeBy = mNewFilterUnitType.unitTypeName;
			}
			if( strOldUnitType != m_strFilterUnitTypeBy)
			{
				requestRefresh();
			}

		}
		public void requestRefresh()
		{
			string strSelectedItem = m_ComboBox.Text;
			m_ComboBox.Items.Clear();
			if (m_strFilterUnitTypeBy == null)
			{
				m_strFilterUnitTypeBy = "";
			}
			if( m_bAddEmptySlot)
			{
				m_ComboBox.Items.Add("");
			}
			UnitTypeCategory mCategory = m_UnitTypeManager.getCategory(m_strCategory);
			if(mCategory == null)
			{
				m_ComboBox.Text = "";
				return;
			}
			List<UnitType> mUnitTypes = mCategory.getUnitTypesOfType(m_strFilterUnitTypeBy);
			foreach (UnitType unitType in mUnitTypes)
			{
				m_ComboBox.Items.Add(unitType.unitTypeName);
			}
			int iSelectedIndex = m_ComboBox.Items.IndexOf(strSelectedItem);
			if( iSelectedIndex < 0 )
			{
				m_ComboBox.SelectedIndex = 0;
			}
			else
			{
				m_ComboBox.SelectedIndex = iSelectedIndex;
			}
		}
	}
}
