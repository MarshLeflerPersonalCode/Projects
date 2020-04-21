using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Library.UnitType.LoadAndSave;

namespace Library.UnitType
{
	public class UnitTypeCategoryListBoxManager
	{
		ContextMenu m_CategoriesContextMenu = new ContextMenu();
		private UnitTypeManager m_UnitTypeManager = null;		
		private ListBox m_ListBoxControlling = null;
		private string m_strLastSelectedCategory = "";
		private bool m_bCanEdit = false;
		private string m_strFilterUpperCase = "";
		public UnitTypeCategoryListBoxManager(UnitTypeManager mUnitTypeManager, ListBox mListBox, bool bCanEdit)
		{			
			m_UnitTypeManager = mUnitTypeManager;
			m_ListBoxControlling = mListBox;
			m_bCanEdit = bCanEdit;
			_registerFunctions();
		}
		public string unitTypeFilterString 
		{
			get
			{
				return m_strFilterUpperCase;
			}
			set
			{
				m_strFilterUpperCase = (value != null) ? value.ToUpper() : "";
				requestRefresh();
			}
		}
		public string getSelectedCategory()
		{
			if (m_ListBoxControlling.SelectedItem != null)
			{
				m_strLastSelectedCategory = m_ListBoxControlling.SelectedItem.ToString();
				return m_strLastSelectedCategory;
			}
			if(m_strLastSelectedCategory != "" )
			{
				int iIndexOf = m_ListBoxControlling.Items.IndexOf(m_strLastSelectedCategory);
				if( iIndexOf < 0 )
				{
					m_strLastSelectedCategory = "";
				}
				else
				{
					m_ListBoxControlling.SelectedIndex = iIndexOf;
					return m_strLastSelectedCategory;
				}
			}
			return "";

		}
		

		private void _registerFunctions()
		{
			m_ListBoxControlling.MouseUp += new System.Windows.Forms.MouseEventHandler(_listBoxMouseClick);			
		}
		private void _listBoxMouseClick(object sender, MouseEventArgs e)
		{
			Form mForm = m_ListBoxControlling.FindForm();
			Point mControlLocation = mForm.PointToClient(mForm.PointToScreen(m_ListBoxControlling.Location));
			Point mousePoint = new Point(e.X, e.Y);// + mControlLocation.X, e.Y + mControlLocation.Y);
			
			

			if (e.Button == MouseButtons.Right &&
				m_bCanEdit )
			{
				//context menu
				_createContextMenu();
				if (m_CategoriesContextMenu.MenuItems.Count != 0)
				{
					mousePoint.X += 8;
					mousePoint.Y -= 5;
					m_CategoriesContextMenu.Show(m_ListBoxControlling, mousePoint);
				}
			}
			
		}

		private void _createContextMenu()
		{
			
			if (m_bCanEdit == false )
			{
				return; //we can't do anything so lets not add anything to do
			}
			m_CategoriesContextMenu.MenuItems.Clear();
			m_CategoriesContextMenu.MenuItems.Add("Create Category", _contextMenuNewCategory);

			string strCategoryName = getSelectedCategory();

			if (strCategoryName != "")
			{
				m_CategoriesContextMenu.MenuItems.Add("-");
				m_CategoriesContextMenu.MenuItems.Add("Delete " + strCategoryName, _contextMenuDeleteCategory);
			}
		}

		private void _contextMenuNewCategory(object sender, EventArgs e)
		{
			string strValidName = "CATEGORY";
			int iCount = 1;
			while (m_UnitTypeManager.getCategory(strValidName) != null)
			{
				strValidName = "CATEGORY" + iCount.ToString();
				iCount++;
			}
			m_UnitTypeManager.createCategory(strValidName);		
			

		}
		private void _contextMenuDeleteCategory(object sender, EventArgs e)
		{
			m_UnitTypeManager.deleteCategory(getSelectedCategory());
		}
		public void requestRefresh()
		{
			string strCurrentSelected = getSelectedCategory();
			m_ListBoxControlling.Items.Clear();

			List<UnitTypeCategory> mListOfCategories = m_UnitTypeManager.getCategories();
			foreach( UnitTypeCategory mCategory in mListOfCategories)
			{
				if (unitTypeFilterString == "" ||
					mCategory.categoryName.Contains(unitTypeFilterString))
				{
					m_ListBoxControlling.Items.Add(mCategory.categoryName);
				}
			}
			if (strCurrentSelected != "")
			{
				int iIndexOf = m_ListBoxControlling.Items.IndexOf(strCurrentSelected);
				if (iIndexOf >= 0)
				{
					m_ListBoxControlling.SelectedIndex = iIndexOf;					
				}
			}
			else if( m_ListBoxControlling.Items.Count > 0)
			{
				m_ListBoxControlling.SelectedIndex = 0;
			}
		}

	}
}
