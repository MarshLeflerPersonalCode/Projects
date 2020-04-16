using System;
using System.Collections.Generic;
using System.Collections;
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
        private List<Type> m_TypesTested = new List<Type>();
		//private ObjectWrapper m_Wrapper = null;
        private List<ObjectWrapper> m_ObjectWrappers = new List<ObjectWrapper>();
        private List<object> m_ObjectsSelected = new List<object>();
        private string m_strFilterString = "";
		private Type m_StringType = null;
		public FilteredPropertyGrid()
		{
			InitializeComponent();
			showObjects = true;
			showArrays = true;
			m_StringType = Type.GetType("System.String");
            base.SelectedObjects = m_ObjectWrappers.ToArray();// m_Wrapper;
			FilterString = "";
			FilterIsCaseSensitive = false;
		}


		//sets if objects/class show in the property gird
		public bool showObjects { get; set; }

		//sets if arrays(iEnumerator) show in the property gird
		public bool showArrays { get; set; }
		public bool showEnumerators { get { return showArrays; } set { showArrays = value; } }
		//sets the filter for the property grid
		public string FilterString
		{
			set
			{
				if(m_strFilterString != value)
				{
					m_strFilterString = value;
					_RefreshProperties();
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
					_RefreshProperties();
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
					_RefreshProperties();
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
					_RefreshProperties();
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
					_RefreshProperties();
				}
			}
		}
		public new object[] SelectedObjects 
		{ 
			get
			{
                //return (m_ObjectWrappers != null)? ((ObjectWrapper)base.SelectedObject).SelectedObjects.ToArray() : null;
                return m_ObjectsSelected.ToArray();

            }
			set
			{
                m_ObjectsSelected.Clear();
                m_ObjectWrappers.Clear();
                if (value != null)
                {
                    foreach (object mObj in value)
                    {
                        m_ObjectsSelected.Add(mObj);
                        ObjectWrapper mWrapper = new ObjectWrapper(mObj);
                        m_ObjectWrappers.Add(mWrapper);
                    }
                }
             
                _RefreshProperties();

            }
		}

		public void addObject(object mObj)
		{
            if( m_ObjectsSelected.Contains(mObj))
            {
                return;
            }
            m_ObjectsSelected.Add(m_ObjectsSelected);
            ObjectWrapper mNewWrapper = new ObjectWrapper(mObj);
            m_ObjectWrappers.Add(mNewWrapper);


            _RefreshProperties();

            // Link the wrapper to the parent PropertyGrid.
            //base.SelectedObjects = m_ObjectWrappers.ToArray();
        }
		public void addObjects(List<object> mObjects)
		{
            foreach (object mObj in mObjects)
            {
                if (m_ObjectsSelected.Contains(mObj))
                {
                    continue;
                }
                m_ObjectsSelected.Add(m_ObjectsSelected);
                ObjectWrapper mWrapper = new ObjectWrapper(mObj);
                m_ObjectWrappers.Add(mWrapper);
            }
            _RefreshProperties();

            // Link the wrapper to the parent PropertyGrid.
           // base.SelectedObjects = m_ObjectWrappers.ToArray();
        }

		/// <summary>Overwrite the PropertyGrid.SelectedObject property.</summary>
		/// <remarks>The object passed to the base PropertyGrid is the wrapper.</remarks>
		//
        //public new object SelectedObject
        public new object SelectedObject
        {
            get { return (m_ObjectsSelected.Count > 0) ? m_ObjectsSelected[0] : null; }
            set
            {
                m_ObjectsSelected.Clear();
                m_ObjectWrappers.Clear();
                if (value != null)
                {

                    m_ObjectsSelected.Add(value);
                    ObjectWrapper mWrapper = new ObjectWrapper(value);
                    m_ObjectWrappers.Add(mWrapper);
                    m_ObjectWrappers[0].PropertyDescriptors = m_PropertyDescriptors;
                }

                _RefreshProperties();
                // Set the list of properties to the wrapper.
                //m_ObjectWrappers[0].PropertyDescriptors = m_PropertyDescriptors;
                // Link the wrapper to the parent PropertyGrid.
                //base.SelectedObject = m_ObjectWrappers[0];

            }

		}

		/// <summary>Allows to hide a set of properties to the parent PropertyGrid.</summary>
		/// <param name="propertyname">A set of attributes that filter the original collection of properties.</param>
		/// <remarks>For better performance, include the BrowsableAttribute with true value.</remarks>
		private void _HideAttribute(object mObj, Attribute attribute)
		{
			//this gets all the properties that have the attribute passed in - this is the category.
			//It then removes those properties to show
			PropertyDescriptorCollection filteredoriginalpropertydescriptors = TypeDescriptor.GetProperties(mObj, new Attribute[] { attribute });
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
		private void _ShowAttribute(object mObj, Attribute attribute)
		{
			//this gets all the properties that have the attribute passed in - this is the category.
			//It then adds it to the properties to show
			PropertyDescriptorCollection filteredoriginalpropertydescriptors = TypeDescriptor.GetProperties(mObj, new Attribute[] { attribute });
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
        private void _RefreshProperties()
        {
            m_TypesTested.Clear();
            foreach (object mObj in m_ObjectsSelected)
            {
                if (m_TypesTested.Contains(mObj.GetType()) == false)
                {
                    _updateObjectWrapperProperties(mObj);
                    m_TypesTested.Add(mObj.GetType());
                }
            }
            foreach (ObjectWrapper mWrapper in m_ObjectWrappers)
            {
                // Set the list of properties to the wrapper.
                mWrapper.PropertyDescriptors = m_PropertyDescriptors;

            }
            if ( m_ObjectsSelected.Count == 0)
            {
                base.SelectedObject = null;
            }
            else if(m_ObjectsSelected.Count == 1)
            {
                base.SelectedObject = m_ObjectWrappers[0];
            }
            else
            {
                base.SelectedObjects = m_ObjectWrappers.ToArray();
            }
            
            Refresh();	//refreshes the property grid
        }
        private void _updateObjectWrapperProperties(object mObject)
        { 
			if (mObject == null)
			{
				return;
			}
			
			// Clear the list of properties to be displayed.
			m_PropertyDescriptors.Clear();
			// Check whether the list is filtered 
			if (m_BrowsableAttributes != null && m_BrowsableAttributes.Count > 0)
			{
				// Add to the list the attributes that need to be displayed.
				foreach (Attribute attribute in m_BrowsableAttributes)
				{
					_ShowAttribute(mObject, attribute);
				}
			}
			else
			{
				// Fill the collection with all the properties.
				PropertyDescriptorCollection originalpropertydescriptors = TypeDescriptor.GetProperties(mObject);
				foreach (PropertyDescriptor mProperty in originalpropertydescriptors)
				{
					if (_canPropertyBeShown(mProperty))
					{
						m_PropertyDescriptors.Add(mProperty);
					}
				}
				// Remove from the list the attributes that mustn't be displayed.
				if (m_HiddenAttributes != null)
				{
					foreach (Attribute attribute in m_HiddenAttributes)
					{
                        _HideAttribute(mObject, attribute);
					}
				}
			}

			// Get all the properties of the SelectedObject
			PropertyDescriptorCollection allproperties = TypeDescriptor.GetProperties(mObject);
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

							if(_canPropertyBeShown(mProperty) == false)
							{
								HideProperty(mProperty);
							}
							else if (FilterIsCaseSensitive)
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
			
		}

		private bool _canPropertyBeShown(PropertyDescriptor mProperty)
		{
			if( mProperty != null )
			{
				if (showObjects != true ||
					showArrays != true)
				{

					Type mPropertyType = mProperty.PropertyType;
					if (mPropertyType.IsPrimitive == false &&
						mPropertyType.IsClass == true &&
						mPropertyType != m_StringType)
					{

						bool bIsArray = false;
						if (mPropertyType.GetInterface(nameof(IEnumerable)) != null)
						{
							bIsArray = true;
						}
						if (showObjects == false && bIsArray == false)
						{
							return false;
						}
						else if (showArrays == false && bIsArray == true)
						{
							return false;
						}
					}
				}
				return true;
			}
			return false;
		}

	} //end class
}//end namespace
