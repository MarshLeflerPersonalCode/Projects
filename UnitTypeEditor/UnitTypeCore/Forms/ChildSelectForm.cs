using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnitTypeCore.Forms
{
	public partial class ChildSelectForm : Form
	{
		UnitTypeManager m_Manager = null;
		UnitTypeCategory m_Category = null;
		UnitType m_UnitTypeToFilterOn = null;
		List<string> m_ListToHide = new List<string>();
		public ChildSelectForm()
		{
			InitializeComponent();						
		}

		public void configure(UnitTypeManager mManager, UnitTypeCategory mCategory)
		{
			m_Manager = mManager;
			m_Category = mCategory;
		}

		public UnitTypeCategory unitTypeCategory
		{
			get { return m_Category; }
			set { m_Category = value; m_Manager = (value != null)?value.unitTypeManager:null; }
		}

		public string unitTypeToFilterOn
		{ 
			get 
			{ 
				if(m_UnitTypeToFilterOn != null)
				{
					return m_UnitTypeToFilterOn.unitTypeName;
				}
				return "";
			} 
			set 
			{
				m_UnitTypeToFilterOn = m_Category.getUnitTypeByUnsafeName(value);
			}
		}

		public string filterByString
		{
			get
			{
				return txtBoxFilter.Text;
			}
			set
			{
				txtBoxFilter.Text = (value != null) ? value : "";
			}
		}

		public void addUnitTypeToHide(UnitType mUnitType)
		{
			if( mUnitType == null )
			{
				return;
			}
			foreach(string strUnitTypeName in m_ListToHide)
			{
				if( strUnitTypeName == mUnitType.unitTypeName)
				{
					return;
				}
			}
			m_ListToHide.Add(mUnitType.unitTypeName);
		}
		


		public List<string> getSelectedUnitTypes() { return lstBoxUnitTypes.SelectedItems.Cast<string>().ToList(); }
		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btnOkay_Click(object sender, EventArgs e)
		{
			if (lstBoxUnitTypes.SelectedItems.Count > 0)
			{
				this.DialogResult = DialogResult.OK;
			}
			else
			{
				this.DialogResult = DialogResult.Cancel;
			}
			this.Close();

		}

		private void txtBoxFilter_TextChanged(object sender, EventArgs e)
		{
			m_Manager.updateListBoxFilters(lstBoxUnitTypes, m_UnitTypeToFilterOn, txtBoxFilter.Text);
		}

		private void ChildSelectForm_Shown(object sender, EventArgs e)
		{
			m_Manager.registerListBox(lstBoxUnitTypes, m_Category.categoryName, m_UnitTypeToFilterOn, "", m_ListToHide);
			m_Manager.updateListBoxFilters(lstBoxUnitTypes, m_UnitTypeToFilterOn, txtBoxFilter.Text);
			_updateOkayButton();
		}

		private void ChildSelectForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_Manager.removeListBox(lstBoxUnitTypes);
			m_UnitTypeToFilterOn = null;
			unitTypeCategory = null;
			m_ListToHide.Clear();
		}

		private void lstBoxUnitTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			_updateOkayButton();
		}

		private void _updateOkayButton()
		{
			if (lstBoxUnitTypes.SelectedIndex >= 0)
			{
				btnOkay.Enabled = true;
			}
			else
			{
				btnOkay.Enabled = false;
			}
		}
	}
}
