using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using UnitTypeCore.LoadAndSave;

namespace UnitTypeCore.Forms
{
	public partial class UnitTypeManagerConfigForm : Form
	{
		UnitTypeManagerConfig m_Config = null;
		string m_strOriginalConfig = "";
		string m_strHeader = "";
		public UnitTypeManagerConfigForm()
		{
			InitializeComponent();
		}



		public void configure(UnitTypeManagerConfig mConfig)
		{
			m_Config = mConfig;
			propertyGrid.SelectedObject = m_Config;
			m_strOriginalConfig = JsonConvert.SerializeObject(mConfig);
			m_strHeader = mConfig.enumHeaderLayout;
			txtBoxHeader.Text = m_Config.enumHeaderLayout;
		}

		private void UnitTypeManagerConfigForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(m_strOriginalConfig != JsonConvert.SerializeObject(m_Config) ||
				m_strHeader != txtBoxHeader.Text)
			{
				this.DialogResult = MessageBox.Show("You have unsaved changes. Do you want to save first?", "Save Changes?", MessageBoxButtons.YesNoCancel);
				if( this.DialogResult == DialogResult.Yes)
				{
					m_Config.enumHeaderLayout = txtBoxHeader.Text;
				}
			}
			else
			{
				this.DialogResult = DialogResult.No;
			}
		}
		private void btnAddEnum_Click(object sender, EventArgs e)
		{
			txtBoxHeader.Paste("[ENUM]");
		}

		private void btnFunctions_Click(object sender, EventArgs e)
		{
			txtBoxHeader.Paste("[FUNCTIONS]");
		}
	}
}
