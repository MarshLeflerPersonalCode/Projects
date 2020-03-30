using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


//NOTE based on the work done here: https://www.codeproject.com/Articles/13342/Filtering-properties-in-a-PropertyGrid

namespace CustomControls
{
	public partial class FilteredPropertyGrid : PropertyGrid
	{
		List<PropertyDescriptor> m_PropertyDescriptors = new List<PropertyDescriptor>();
		private AttributeCollection m_HiddenAttributes = null;
		private AttributeCollection m_BrowsableAttributes = null;
		private string[] m_BrowsableProperties = null;
		private string[] m_HiddenProperties = null;
		private ObjectWrapper m_Wrapper = null;
		private string m_strFilterString = "";
		public FilteredPropertyGrid()
		{
			InitializeComponent();
			base.SelectedObject = m_Wrapper;
			FilterString = "";
			FilterIsCaseSensitive = false;
		}

		//sets the filter for the property grid
		public string FilterString
		{
			set
			{
				if(m_strFilterString != value)
				{
					m_strFilterString = value;
					RefreshProperties();
				}
			}
			get { return m_strFilterString; }
		}
		
		//sets if filtering is case sensitive
		public bool FilterIsCaseSensitive
		{
			set;
			get;
		}

		public new AttributeCollection BrowsableAttributes
		{
			get { return m_BrowsableAttributes; }
			set
			{
				if (m_BrowsableAttributes != value)
				{
					m_HiddenAttributes = null;
					m_BrowsableAttributes = value;
					RefreshProperties();
				}
			}
		}

		/// <summary>Get or set the categories to hide.</summary>
		public AttributeCollection HiddenAttributes
		{
			get { return m_HiddenAttributes; }
			set
			{
				if (value != m_HiddenAttributes)
				{
					m_HiddenAttributes = value;
					m_BrowsableAttributes = null;
					RefreshProperties();
				}
			}
		}

		/// <summary>Get or set the properties to show.</summary>
		/// <exception cref="ArgumentException">if one or several properties don't exist.</exception>
		public string[] BrowsableProperties
		{
			get { return m_BrowsableProperties; }
			set
			{
				if (value != m_BrowsableProperties)
				{
					m_BrowsableProperties = value;
					//m_HiddenProperties = null;
					RefreshProperties();
				}
			}
		}

		/// <summary>Get or set the properties to hide.</summary>
		public string[] HiddenProperties
		{
			get { return m_HiddenProperties; }
			set
			{
				if (value != m_HiddenProperties)
				{
					//m_BrowsableProperties = null;
					m_HiddenProperties = value;
					RefreshProperties();
				}
			}
		}

		/// <summary>Overwrite the PropertyGrid.SelectedObject property.</summary>
		/// <remarks>The object passed to the base PropertyGrid is the wrapper.</remarks>
		public new object SelectedObject
		{
			get { return m_Wrapper != null ? ((ObjectWrapper)base.SelectedObject).SelectedObject : null; }
			set
			{
				// Set the new object to the wrapper and create one if necessary.
				if (value == null)
				{
					m_Wrapper = null;
					RefreshProperties();
				}
				else if (m_Wrapper == null)
				{
					m_Wrapper = new ObjectWrapper(value);
					RefreshProperties();
				}
				else if (m_Wrapper.SelectedObject != value)
				{
					bool bNeedrefresh = value.GetType() != m_Wrapper.SelectedObject.GetType();
					m_Wrapper.SelectedObject = value;
					if (bNeedrefresh)
					{
						RefreshProperties();
					}
				}
				if (m_Wrapper != null)
				{
					// Set the list of properties to the wrapper.
					m_Wrapper.PropertyDescriptors = m_PropertyDescriptors;
					// Link the wrapper to the parent PropertyGrid.
					base.SelectedObject = m_Wrapper;
				}
			}
		}

		/// <summary>Allows to hide a set of properties to the parent PropertyGrid.</summary>
		/// <param name="propertyname">A set of attributes that filter the original collection of properties.</param>
		/// <remarks>For better performance, include the BrowsableAttribute with true value.</remarks>
		private void HideAttribute(Attribute attribute)
		{
			//this gets all the properties that have the attribute passed in - this is the category.
			//It then removes those properties to show
			PropertyDescriptorCollection filteredoriginalpropertydescriptors = TypeDescriptor.GetProperties(m_Wrapper.SelectedObject, new Attribute[] { attribute });
			if (filteredoriginalpropertydescriptors == null || filteredoriginalpropertydescriptors.Count == 0)
			{
				throw new ArgumentException("Attribute not found", attribute.ToString());
			}
			foreach (PropertyDescriptor propertydescriptor in filteredoriginalpropertydescriptors)
			{
				HideProperty(propertydescriptor);
			}
		}
		/// <summary>Add all the properties that match an attribute to the list of properties to be displayed in the PropertyGrid.</summary>
		/// <param name="property">The attribute to be added.</param>
		private void ShowAttribute(Attribute attribute)
		{
			//this gets all the properties that have the attribute passed in - this is the category.
			//It then adds it to the properties to show
			PropertyDescriptorCollection filteredoriginalpropertydescriptors = TypeDescriptor.GetProperties(m_Wrapper.SelectedObject, new Attribute[] { attribute });
			if (filteredoriginalpropertydescriptors == null || filteredoriginalpropertydescriptors.Count == 0)
			{
				throw new ArgumentException("Attribute not found", attribute.ToString());
			}
			foreach (PropertyDescriptor propertydescriptor in filteredoriginalpropertydescriptors)
			{
				ShowProperty(propertydescriptor);
			}
		}
		/// <summary>Add a property to the list of properties to be displayed in the PropertyGrid.</summary>
		/// <param name="property">The property to be added.</param>
		private void ShowProperty(PropertyDescriptor property)
		{
			if (!m_PropertyDescriptors.Contains(property))
			{
				m_PropertyDescriptors.Add(property);
			}
		}
		/// <summary>Allows to hide a property to the parent PropertyGrid.</summary>
		/// <param name="propertyname">The name of the property to be hidden.</param>
		private void HideProperty(PropertyDescriptor property)
		{
			if (m_PropertyDescriptors.Contains(property))
			{
				m_PropertyDescriptors.Remove(property);
			}
		}

		/// <summary>Build the list of the properties to be displayed in the PropertyGrid, following the filters defined the Browsable and Hidden properties.</summary>
		private void RefreshProperties()
		{
			if (m_Wrapper == null)
			{
				base.SelectedObject = null;
				Refresh();
				return;
			}
			base.SelectedObject = m_Wrapper;
			// Clear the list of properties to be displayed.
			m_PropertyDescriptors.Clear();
			// Check whether the list is filtered 
			if (m_BrowsableAttributes != null && m_BrowsableAttributes.Count > 0)
			{
				// Add to the list the attributes that need to be displayed.
				foreach (Attribute attribute in m_BrowsableAttributes)
				{
					ShowAttribute(attribute);
				}
			}
			else
			{
				// Fill the collection with all the properties.
				PropertyDescriptorCollection originalpropertydescriptors = TypeDescriptor.GetProperties(m_Wrapper.SelectedObject);
				foreach (PropertyDescriptor propertydescriptor in originalpropertydescriptors)
				{
					m_PropertyDescriptors.Add(propertydescriptor);
				}
				// Remove from the list the attributes that mustn't be displayed.
				if (m_HiddenAttributes != null)
				{
					foreach (Attribute attribute in m_HiddenAttributes)
					{
						HideAttribute(attribute);
					}
				}
			}

			// Get all the properties of the SelectedObject
			PropertyDescriptorCollection allproperties = TypeDescriptor.GetProperties(m_Wrapper.SelectedObject);
			// Hide if necessary, some properties
			if (m_HiddenProperties != null && m_HiddenProperties.Length > 0)
			{
				// Remove from the list the properties that mustn't be displayed.
				foreach (string propertyname in m_HiddenProperties)
				{
					try
					{
						PropertyDescriptor property = allproperties[propertyname];
						// Remove from the list the property
						HideProperty(property);
					}
					catch (Exception ex)
					{
						throw new ArgumentException(ex.Message);
					}
				}
			}

			// filter out properties
			if( FilterString != null &&
				FilterString != "" )
			{
				string strFilterString = FilterString;
				if(FilterIsCaseSensitive == false)
				{
					strFilterString = strFilterString.ToLower();
				}
				foreach (PropertyDescriptor mProperty in allproperties)
				{
					try
					{						
						if( mProperty != null)
						{
							if (FilterIsCaseSensitive)
							{
								if(mProperty.DisplayName.Contains(strFilterString) == false)
								{
									// Remove from the list the property
									HideProperty(mProperty);
								}
							}
							else if ( mProperty.DisplayName.ToLower().Contains(strFilterString)  == false)
							{
								// Remove from the list the property
								HideProperty(mProperty);
							}
						}
						
					}
					catch (Exception ex)
					{
						throw new ArgumentException(ex.Message);
					}
				}
			}

			// Display if necessary, some properties
			if (m_BrowsableProperties != null && m_BrowsableProperties.Length > 0)
			{
				foreach (string mPropertyname in m_BrowsableProperties)
				{
					try
					{
						ShowProperty(allproperties[mPropertyname]);
					}
					catch
					{
						throw new ArgumentException("Property not found", mPropertyname);
					}
				}
			}
			Refresh();	//refreshes the property grid
		}


	}
}
