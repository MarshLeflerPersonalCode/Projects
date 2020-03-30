using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace UnitTypeCore.LoadAndSave
{
	public class CategoryConfig
	{
		[NonSerialized] public UnitTypeCategory m_OwningCategory = null;
		[NonSerialized] private string m_strCategoryName = "";
		public CategoryConfig()
		{			
		}

		private void _dirty()
		{
			if( m_OwningCategory != null)
			{
				m_OwningCategory.unitTypeManager.needsToSave = true;
			}
		}

		[DisplayName("Category Name"),
		 Category("STANDARD PROPERTIES"),
		 Description("The name of the category.")]
		public string categoryName 
		{ 
			get
			{
				return m_strCategoryName;
			}
			set
			{
				m_strCategoryName = UnitType.convertToCodeName(value);
				if(m_OwningCategory != null)
				{
					m_OwningCategory.notifyOfCategoryNameChange();
					_dirty();
					
				}
			}
		}


		[NonSerialized] private string m_strHeaderDirectory = "";
		[DisplayName("Folder For Enum Header File"),
		 Category("C++"),
		 Description("The folder which the enum header file will be saved at.")]
		[EditorAttribute(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string enumHeaderDirectory
		{
			get { return m_strHeaderDirectory; }
			set { m_strHeaderDirectory = UnitTypeFile.getRelativePath(value); _dirty(); }
		}

		[NonSerialized] private string m_strHeaderFile = "";
		[DisplayName("Enum Header File"),
		 Category("C++"),
		 Description("The name of the header file.")]		
		public string enumHeaderFile
		{
			get { return m_strHeaderFile; }
			set { m_strHeaderFile = UnitTypeFile.getRelativePath(value); _dirty(); }
		}

		[NonSerialized] private string m_strEnumName = "";
		[DisplayName("Enum Name"),
		 Category("C++"),
		 Description("The name of the enum. If blank it will use the category name. Example: enum class ECATEGORY_NAME")]
		public string enumName
		{
			get { return m_strEnumName; }
			set { m_strEnumName = value; _dirty(); }
		}



		[NonSerialized] private List<UnitType> m_UnitTypes = null;
		[DisplayName("Unit Types"),
		 Category("C++"),
		 Description("The unit types being saved"),
		 Browsable(false)]
		public System.Collections.Generic.List<UnitTypeCore.UnitType> unitTypes
		{
			get { return m_UnitTypes; }
			set { m_UnitTypes = value; _dirty(); }
		}


	}
}
