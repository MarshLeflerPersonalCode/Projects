using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using UnitTypeCore;
using UnitTypeCore.LoadAndSave;
namespace UnitTypeForm
{
	public partial class unitTypeForm : Form
	{
		enum EFILTER_TYPES
		{
			NONE,
			CATEGORIES_STRING,
			UNIT_TYPE_STRING,
			UNIT_TYPE_DROPDOWN,
			PROPERTIES_STRING	
		}

		enum EPROPERTIES_SHOWING
		{
			UNITTYPE,
			CATEGORY
		}

		private EPROPERTIES_SHOWING m_ePropertiesShowing = EPROPERTIES_SHOWING.CATEGORY;
		private bool m_bUnitTypePropertiesShowing = true;
		private int m_iUnitTypeProperitesPanelWith = 0;
		private bool m_bNeedsToSave = false;
		UnitTypeManager m_UnitTypeManager = null;
		UnitTypeTreeViewManager m_TreeViewManager = null;
		UnitTypeCategoryListBoxManager m_CategoryManager = null;
		List<string> m_FilterStrings = new List<string>();		
		EFILTER_TYPES m_FilterModifying = EFILTER_TYPES.NONE;
		public unitTypeForm()
		{
			InitializeComponent();

			m_iUnitTypeProperitesPanelWith = splitContainerUnitType.SplitterDistance;
			_toggleUnitTypeProperties();
			foreach (int iIndex in Enum.GetValues(typeof(EFILTER_TYPES)))
			{
				m_FilterStrings.Add("");
			}
			filteredPropertyGrid.FilterIsCaseSensitive = false;
			_constuctUnitTypeManager();
			_updateColumns();
		}

		private void _constuctUnitTypeManager()
		{
			m_UnitTypeManager = new UnitTypeManager();
			m_TreeViewManager = new UnitTypeTreeViewManager(m_UnitTypeManager, "", treeViewUnitTypes, true, true, true);
			m_CategoryManager = new UnitTypeCategoryListBoxManager(m_UnitTypeManager, lstBoxCategories, true);
			m_UnitTypeManager.registerTreeViewManager(m_TreeViewManager);
			m_UnitTypeManager.registerCategoryListBoxManager(m_CategoryManager);
			m_UnitTypeManager.registerComboBox(cmbBoxFilterUnittypes, "", true);

		}

		private void _updateColumns()
		{
			_updateCategoriesColumn();
			_updateUnitTypeColumn();
			_updatePropertiesColumn();
		}

		private void _updateCategoriesColumn()
		{
			//TODO
		}
		private void _updateUnitTypeColumn()
		{

		}
		private void _updatePropertiesColumn()
		{

		}


		private void txtBoxFilterPropertiesByString_TextChanged(object sender, EventArgs e)
		{
			m_FilterModifying = EFILTER_TYPES.PROPERTIES_STRING;
			filterTimer.Enabled = true;
		}

		private void filterTimer_Tick(object sender, EventArgs e)
		{
			_handleFilters();

			if( m_FilterModifying == EFILTER_TYPES.NONE)
			{
				filterTimer.Enabled = false;
			}
		}

		private void _handleFilters()
		{
			switch (m_FilterModifying)
			{
				default:
					return;
				case EFILTER_TYPES.PROPERTIES_STRING:
					{
						if( m_FilterStrings[(int)EFILTER_TYPES.PROPERTIES_STRING] != txtBoxFilterPropertiesByString.Text)
						{
							m_FilterStrings[(int)EFILTER_TYPES.PROPERTIES_STRING] = txtBoxFilterPropertiesByString.Text;							
							return;
						}
						_filterProperties(txtBoxFilterPropertiesByString.Text);
					}
					break;
				case EFILTER_TYPES.UNIT_TYPE_DROPDOWN:
					m_FilterStrings[(int)EFILTER_TYPES.UNIT_TYPE_DROPDOWN] = cmbBoxFilterUnittypes.Text;						
					m_TreeViewManager.unitTypeFilterByUnitType = cmbBoxFilterUnittypes.Text;
					break;
				case EFILTER_TYPES.UNIT_TYPE_STRING:
					{
						if (m_FilterStrings[(int)EFILTER_TYPES.UNIT_TYPE_STRING] != txtBoxFilterUnitTypesByString.Text)
						{
							m_FilterStrings[(int)EFILTER_TYPES.UNIT_TYPE_STRING] = txtBoxFilterUnitTypesByString.Text;
							return;
						}
						m_TreeViewManager.unitTypeFilterString = txtBoxFilterUnitTypesByString.Text;						
					}
					break;
				case EFILTER_TYPES.CATEGORIES_STRING:
					{
						if (m_FilterStrings[(int)EFILTER_TYPES.CATEGORIES_STRING] != txtBoxFilterCategoriesByString.Text)
						{
							m_FilterStrings[(int)EFILTER_TYPES.CATEGORIES_STRING] = txtBoxFilterCategoriesByString.Text;
							return;
						}
						m_CategoryManager.unitTypeFilterString = txtBoxFilterCategoriesByString.Text;
					}
					break;
			}
			//must return from above else default is none
			m_FilterModifying = EFILTER_TYPES.NONE;
		}

		private void _filterProperties(string strFilter)
		{
			filteredPropertyGrid.FilterString = strFilter;
			
		}

		private void propertyGrid_SelectedObjectsChanged(object sender, EventArgs e)
		{
			_filterProperties(m_FilterStrings[(int)EFILTER_TYPES.PROPERTIES_STRING]);
		}

		private void txtBoxFilterUnitTypesByString_TextChanged(object sender, EventArgs e)
		{
			m_FilterModifying = EFILTER_TYPES.UNIT_TYPE_STRING;
			filterTimer.Enabled = true;
		}

		private void cmbBoxFilterUnittypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_FilterModifying = EFILTER_TYPES.UNIT_TYPE_DROPDOWN;
			filterTimer.Enabled = true;
		}

		private void logicTimer_Tick(object sender, EventArgs e)
		{
			if( m_UnitTypeManager.needsToSave == true &&
				m_bNeedsToSave == false )
			{
				m_bNeedsToSave = true;
				this.Text = this.Text + "*";
			}
			else if(m_UnitTypeManager.needsToSave == false &&
				    m_bNeedsToSave)
			{
				m_bNeedsToSave = false;
				this.Text = this.Text.Replace("*", "");
			}
			_handleShowingProperties();
			
		}

		private bool _saveAs()
		{
			if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
			{
				return false;
			}
			return m_UnitTypeManager.saveAs(Path.GetFullPath(saveFileDialog.FileName));
		}

		private bool save()
		{
			if (m_UnitTypeManager.save() == false)
			{
				return _saveAs();
			}
			return true;
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			save();
			
		}

		private void loadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if( m_UnitTypeManager.needsToSave)
			{
				DialogResult mResult = MessageBox.Show("You have unsaved changes. Do you want ot save first?", "Save Changes?", MessageBoxButtons.YesNoCancel);
				if(mResult == DialogResult.OK)
				{
					if(save() == false )
					{
						return;
					}
				}
				else if( mResult == DialogResult.Cancel)
				{
					return;
				}
			}
			openFileDialog.FileName = m_UnitTypeManager.unitTypeSaveLocation;
			
			if(openFileDialog.ShowDialog() == DialogResult.OK)
			{
				m_UnitTypeManager.load(openFileDialog.FileName, true);
			}
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_saveAs();
		}

		private void btnShowAndHideUnitTypeProperties_Click(object sender, EventArgs e)
		{
			_toggleUnitTypeProperties();
		}

		private void _toggleUnitTypeProperties()
		{
			m_bUnitTypePropertiesShowing = !m_bUnitTypePropertiesShowing;
			if( m_bUnitTypePropertiesShowing)
			{
				splitContainerUnitType.SplitterDistance = m_iUnitTypeProperitesPanelWith;
				btnShowAndHideUnitTypeProperties.Text = ">";
				unitTypePanelProperties.Show();
			}
			else
			{
				m_iUnitTypeProperitesPanelWith = Math.Max(splitContainerUnitType.SplitterDistance, m_iUnitTypeProperitesPanelWith);
				splitContainerUnitType.SplitterDistance = splitContainerUnitType.Width;
				btnShowAndHideUnitTypeProperties.Text = "<";
				unitTypePanelProperties.Hide();
			}
		}
		private void _handleShowingProperties()
		{

			switch (m_ePropertiesShowing)
			{
				case EPROPERTIES_SHOWING.CATEGORY:
					{
						if (lstBoxCategories.SelectedItem != null)
						{
							UnitTypeCategory mActiveCategory = m_UnitTypeManager.getCategory(lstBoxCategories.SelectedItem.ToString());
							if (mActiveCategory != null &&
								filteredPropertyGrid.SelectedObject != mActiveCategory.categoryConfig)
							{
								filteredPropertyGrid.SelectedObject = mActiveCategory.categoryConfig;
							}
							else if( mActiveCategory == null )
							{
								filteredPropertyGrid.SelectedObject = null;
							}
						}
						else
						{
							filteredPropertyGrid.SelectedObject = null;
						}
					}

					break;
				case EPROPERTIES_SHOWING.UNITTYPE:
					{
						UnitType mSelectedUnitType = m_TreeViewManager.getSelectedUnitType(true);
						if (filteredPropertyGrid.SelectedObject != mSelectedUnitType)
						{
							filteredPropertyGrid.SelectedObject = mSelectedUnitType;
						}
					}
					break;
			}
		}
		private void lstBoxCategories_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_TreeViewManager.setCategory(m_CategoryManager.getSelectedCategory());
			m_ePropertiesShowing = EPROPERTIES_SHOWING.CATEGORY;
			_handleShowingProperties();
		}
		private void treeViewUnitTypes_AfterSelect(object sender, TreeViewEventArgs e)
		{
			m_ePropertiesShowing = EPROPERTIES_SHOWING.UNITTYPE;
			_handleShowingProperties();
		}

		private void lstBoxCategories_DoubleClick(object sender, EventArgs e)
		{
			if(m_CategoryManager.getSelectedCategory() != "")
			{
				_toggleUnitTypeProperties();
			}
		}

		private void txtBoxFilterCategoriesByString_TextChanged(object sender, EventArgs e)
		{
			m_FilterModifying = EFILTER_TYPES.CATEGORIES_STRING;
			filterTimer.Enabled = true;
		}

		private void treeViewUnitTypes_DoubleClick(object sender, EventArgs e)
		{
			if(treeViewUnitTypes.SelectedNode != null)
			{
				_toggleUnitTypeProperties();
			}
		}

		private void configToolStripMenuItem_Click(object sender, EventArgs e)
		{
			m_UnitTypeManager.showUnitTypeManagerConfigForm(this);
		}

		private void quitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void unitTypeForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (m_UnitTypeManager.needsToSave)
			{
				DialogResult mResult = MessageBox.Show("You have unsaved changes. Do you want to save first before exiting?", "Save Changes?", MessageBoxButtons.YesNoCancel);
				if (mResult == DialogResult.Yes)
				{
					save();
				}
				else if(mResult == DialogResult.Cancel)
				{
					e.Cancel = true;
				}
			}

		}
	}
}
