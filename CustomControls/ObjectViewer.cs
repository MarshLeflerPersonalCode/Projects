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
using System.Reflection;

namespace CustomControls
{
	

	public partial class ObjectViewer : UserControl
	{
		private List<object> m_ObjectsList = new List<object>();
		private List<ClassObjectViewerData> m_SubObjects = new List<ClassObjectViewerData>();
		private List<ArrayViewerData> m_SubArrays = new List<ArrayViewerData>();
		private int m_iArrayIndexSelected = -1;		
		private bool m_bObjectListDirty = false;
		private bool m_bRefreshArrayItems = false;
		private List<List<object>> m_ArrayList = new List<List<object>>();
		//private bool m_bObjectsDirty 
		public ObjectViewer()
		{
			InitializeComponent();
			objectFilteredPropertyGrid.FilterIsCaseSensitive = false;
			objectFilteredPropertyGrid.showObjects = false;
			objectFilteredPropertyGrid.showArrays = false;

		}

		//sets the object you want to see
		public void setObjectViewing(object obj)
		{
            if (obj == null)
            {
                return;
            }
            m_ObjectsList.Clear();
			m_ObjectsList.Add(obj);
			m_bObjectListDirty = true;
			_invalidateObjectsModifying();
		}
		public bool addObjectViewing(object obj)
		{			
            if(obj == null)
            {
                return false;
            }
			if( m_ObjectsList.Count > 0 )
			{
				if( m_ObjectsList[0].GetType() != obj.GetType())
				{
					return false;
				}
			}
            if (m_ObjectsList.Contains(obj) == false)
            {
                m_ObjectsList.Add(obj);
                m_bObjectListDirty = true;
            }
			return true;
		}
		//sets all the objects wanting to see
		public void setObjectsViewing(List<object> mObjects )
		{
			m_ObjectsList = new List<object>(mObjects);			
			m_bObjectListDirty = true;
			_invalidateObjectsModifying();
		}
		//sets all the objects wanting to see
		public void setObjectsViewing(object[] mObjects)
		{
			m_ObjectsList = new List<object>(mObjects);
			m_bObjectListDirty = true;
			_invalidateObjectsModifying();
		}

		private void timerUpdate_Tick(object sender, EventArgs e)
		{
			if( m_bObjectListDirty )
			{
				_updatePropertyGrid();
                m_bRefreshArrayItems = true;
                

            }
			if(txtBoxFilterPropertyGrid.Text != objectFilteredPropertyGrid.FilterString)
			{
				objectFilteredPropertyGrid.FilterString = txtBoxFilterPropertyGrid.Text;
			}
			_updateListViews();
		}

		private void _invalidateObjectsModifying()
		{
			m_ArrayList.Clear();
			m_bRefreshArrayItems = false;
			ListViewSplitter.Panel2.Enabled = false;
			btnDeleteArrayItem.Enabled = false;
			m_iArrayIndexSelected = -1;
			m_SubArrays.Clear();
			m_SubObjects.Clear();
			objectListView.Items.Clear();
			arrayListView.Items.Clear();
			objectFilteredPropertyGrid.SelectedObjects = null;
		}


		private void _updatePropertyGrid()
		{

			m_bObjectListDirty = false;
			if ( m_ObjectsList != null )
			{
				for (int iObjectsIndex = 0; iObjectsIndex < m_ObjectsList.Count; iObjectsIndex++)
				{
					if (m_ObjectsList[iObjectsIndex] == null)
					{
						m_ObjectsList.RemoveAt(iObjectsIndex);
						iObjectsIndex--;
					}
				}
			}
			if( m_ObjectsList == null ||
				m_ObjectsList.Count == 0 )
			{
				objectFilteredPropertyGrid.SelectedObjects = null;
			}
			else
			{
				Type mType = m_ObjectsList[0].GetType();
				foreach(object mObject  in m_ObjectsList)
				{
					if( mObject.GetType() != mType )
					{
						_setObjectViewerEnabled(false);
						return;
					}
				}
				_setObjectViewerEnabled(true);
				objectFilteredPropertyGrid.SelectedObjects = m_ObjectsList.ToArray();
				_configureMemberInfo();
				_updateObjectsList();
				
			}
			
		}

		private void _setObjectViewerEnabled(bool bEnabled)
		{
			if(this.Enabled == bEnabled)
			{
				return;
			}
			this.Enabled = bEnabled;
			if(bEnabled == false )
			{
				objectFilteredPropertyGrid.SelectedObjects = null;
			}
		}

		private void _configureMemberInfo()
		{
			
			if(objectFilteredPropertyGrid.SelectedObjects == null ||
			   objectFilteredPropertyGrid.SelectedObjects.Length == 0 ) 
			{ 
				return; 
			}
			Type mType = m_ObjectsList[0].GetType();
			MemberInfo[] mMembers = mType.GetMembers();
			
			Type mStringType = Type.GetType("System.String");			
			foreach ( MemberInfo mMember in mMembers)
			{
				MemberTypes mMemberTypeInfo = mMember.MemberType;
				if (mMemberTypeInfo != MemberTypes.Property)
				{
					continue;
				}
                BrowsableAttribute mBrowsable = mMember.GetCustomAttribute(typeof(BrowsableAttribute)) as BrowsableAttribute;
                if(mBrowsable != null &&
                    mBrowsable.Browsable == false)
                {
                    continue;
                }
                PropertyInfo mProperty = (System.Reflection.PropertyInfo)mMember;
				Type mMemberType = mProperty.PropertyType;
                

				if( mMemberType.IsPrimitive ||
					mMemberType.IsPublic == false ||
					mMemberType.IsVisible == false ||
					mMemberType == mStringType)
				{
					continue;
				}

				

				if(mMemberType.IsClass)
				{
					string strNameToDisplay = mMember.Name;
					object[] attribute = mMember.GetCustomAttributes(typeof(DisplayNameAttribute), true);
					if( attribute != null &&
						attribute.Length != 0)
					{
						DisplayNameAttribute mDisplayAttribute = attribute.Cast<DisplayNameAttribute>().Single();
						if (mDisplayAttribute != null)
						{
							strNameToDisplay = mDisplayAttribute.DisplayName;
						}
					}
					if( strNameToDisplay == "")
					{
						strNameToDisplay = "";
					}
	  
					
					if ( mMemberType.GetInterface(nameof(IEnumerable)) != null )
					{
						if (mMemberType.Name.StartsWith("List") &&
							mMemberType.GenericTypeArguments != null &&
							mMemberType.GenericTypeArguments.Count() == 1)
						{
							//this is a collection or an array of some sort.
							ArrayViewerData mArrayData = new ArrayViewerData();
							mArrayData.type = mMemberType;
							mArrayData.name = strNameToDisplay;
							mArrayData.memberInfo = mMember;
							mArrayData.objectType = mMemberType.GenericTypeArguments[0];
							m_SubArrays.Add(mArrayData);
						}
					}
					else
					{
						//handle class data
						ClassObjectViewerData mNewObjectView = new ClassObjectViewerData();
						mNewObjectView.type = mMemberType;
						mNewObjectView.memberInfo = mMember;
						mNewObjectView.name = strNameToDisplay;
						mNewObjectView.isRootObject = false;
						m_SubObjects.Add(mNewObjectView);

					}
				}
			}
			
		}

		private void _updateObjectsList()
		{
            objectListView.Items.Clear();

            if (m_ObjectsList.Count > 0)
			{
				Type mType = m_ObjectsList[0].GetType();
                ListViewItem mNewItem = new ListViewItem("*");                
				mNewItem.SubItems.Add(mType.Name);
				mNewItem.SubItems.Add("Root Object");
				mNewItem.Font = new Font(mNewItem.Font, FontStyle.Bold);
				objectListView.Items.Add(mNewItem);
			}

			foreach (ClassObjectViewerData mData in m_SubObjects)
			{
                bool bFound = false;
                foreach (ListViewItem mCurrentItem in objectListView.Items)
                {
                    if (mCurrentItem.SubItems[1].Text == mData.name)
                    {
                        bFound = true;
                        break;
                    }
                }
                if (bFound) { continue; }
                ListViewItem mNewItem = new ListViewItem("*");
				mNewItem.SubItems.Add(mData.name);
				mNewItem.SubItems.Add("Object");
				if(mData.isRootObject)
				{
					mNewItem.Font = new Font(mNewItem.Font, FontStyle.Bold);


				}
				objectListView.Items.Add(mNewItem);
			}
            _updateArrayCountOnObjectList();

        }

        private void _updateArrayCountOnObjectList()
        {
            foreach (ArrayViewerData mData in m_SubArrays)
            {
                ListViewItem mListItemToUpdate = null;
                
                foreach (ListViewItem mCurrentItem in objectListView.Items)
                {
                    if (mCurrentItem.SubItems[1].Text == mData.name)
                    {
                        mListItemToUpdate = mCurrentItem;
                        break;
                    }
                }
                
                if( mListItemToUpdate == null )
                {
                    mListItemToUpdate = new ListViewItem("");
                    mListItemToUpdate.SubItems.Add(mData.name);
                    if (mData.objectType != null)
                    {
                        mListItemToUpdate.SubItems.Add(mData.objectType.Name);
                    }
                    else
                    {
                        mListItemToUpdate.SubItems.Add("unknown");
                    }

                    objectListView.Items.Add(mListItemToUpdate);
                }

                PropertyInfo mPropertyInfo = (PropertyInfo)mData.memberInfo;

                int iMaxCount = 0;
                foreach (object mObj in m_ObjectsList)
                {
                    IList mItemList = mPropertyInfo.GetValue(mObj, null) as IList;
                    if (mItemList != null &&
                        mItemList.Count > iMaxCount)
                    {
                        iMaxCount = mItemList.Count;
                    }
                }
                mListItemToUpdate.SubItems[0].Text = iMaxCount.ToString();
                
            }
        }


		private void objectListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			m_iArrayIndexSelected = -1;
			if (objectListView.SelectedItems.Count == 0 ||
				m_ObjectsList.Count == 0)
			{
				return;
			}
			ListViewItem mItem = objectListView.SelectedItems[0];
			m_bRefreshArrayItems = true;

			//check to see if i'm an array
			foreach (ArrayViewerData mData in m_SubArrays)
			{
				if (mItem.SubItems[1].Text == mData.name)
				{
					PropertyInfo mPropertyInfo = (PropertyInfo)mData.memberInfo;

					List<object> mArraysSelected = new List<object>();
					foreach (object mObj in m_ObjectsList)
					{
						mArraysSelected.Add(mPropertyInfo.GetValue(mObj, null));
					}
					objectFilteredPropertyGrid.SelectedObjects = mArraysSelected.ToArray();
					m_iArrayIndexSelected = m_SubArrays.IndexOf(mData);
					return;
				}
			}
			foreach (ClassObjectViewerData mData in m_SubObjects)
			{
				if (mItem.SubItems[1].Text == mData.name)
				{
					PropertyInfo mPropertyInfo = (PropertyInfo)mData.memberInfo;

					List<object> mArraysSelected = new List<object>();
					foreach (object mObj in m_ObjectsList)
					{
						mArraysSelected.Add(mPropertyInfo.GetValue(mObj, null));
					}
					objectFilteredPropertyGrid.SelectedObjects = mArraysSelected.ToArray();
					return;
				}
			}

			objectFilteredPropertyGrid.SelectedObjects = m_ObjectsList.ToArray();

		}

		private void _updateListViews()
		{
			if(objectListView.SelectedItems == null ||
				objectListView.SelectedItems.Count == 0 )
			{
				if (m_ObjectsList.Count > 0)
				{
					foreach (ListViewItem mItem in objectListView.Items)
					{
						if (mItem.SubItems[1].Text == m_ObjectsList[0].GetType().Name)
						{
							mItem.Focused = true;
							mItem.Selected = true;
							return;
						}
					}
				}
				return;
			}

			_updateArrayListView();


		}

		private void _updateArrayListView()
		{
            m_bRefreshArrayItems = m_bRefreshArrayItems | (arrayListView.Items.Count != m_ArrayList.Count) ? true : false;

            if (m_iArrayIndexSelected < 0 ||
				m_iArrayIndexSelected >= m_SubArrays.Count)
			{
				if (arrayListView.Items.Count > 0)
				{
					arrayListView.Items.Clear();
				}
				m_iArrayIndexSelected = -1;
				if (ListViewSplitter.Panel2.Enabled)
				{
					ListViewSplitter.Panel2.Enabled = false;
				}
				return; //we are done.
			}
			if (ListViewSplitter.Panel2.Enabled == true &&
				m_bRefreshArrayItems == false)
			{
				return; //we don't need to refresh
			}

			//refresh time
			int iSelectedItem = -1;
			if( arrayListView.SelectedItems.Count > 0 )
			{
				iSelectedItem = arrayListView.Items.IndexOf(arrayListView.SelectedItems[0]);
			}
			arrayListView.Items.Clear();
			m_bRefreshArrayItems = false;
			ListViewSplitter.Panel2.Enabled = true;
			m_ArrayList.Clear();
			PropertyInfo mPropertyInfo = getSelectedArrayPropertyInfo();
			foreach (object mObjectModifying in m_ObjectsList)
			{
				IList mArray = mPropertyInfo.GetValue(mObjectModifying) as IList;
                if (mArray != null)
                {
                    int iIndex = 0;
                    foreach (object mObjectInArray in mArray)
                    {
                        while (iIndex >= m_ArrayList.Count)
                        {
                            m_ArrayList.Add(new List<object>());
                        }
                        m_ArrayList[iIndex].Add(mObjectInArray);
                        iIndex++;
                    }
                }
			}
			btnUpArrayItem.Enabled = (m_ArrayList.Count >= 1) ? true : false;
			btnDownArrayItem.Enabled = (m_ArrayList.Count >= 1) ? true : false;
			if ( m_ArrayList.Count == 0 )
			{
				btnDeleteArrayItem.Enabled = false;
				arrayListView.Enabled = false;
				return;
			}
			arrayListView.Enabled = true;
			Type subObjectType = m_ArrayList[0][0].GetType();
			int iIndexOf = 0;
			foreach (List<object> mObjectArray in m_ArrayList)
			{
				ListViewItem mNewItem = new ListViewItem(iIndexOf.ToString());
                string strType = "";
                foreach(object mObj in mObjectArray)
                {
                    if(mObj == null )
                    {
                        continue;
                    }
                    if(strType == "" ||
                       strType.Contains(mObj.GetType().Name) == false)
                    {
                        strType = strType + mObj.GetType().Name + ", ";
                    }
                }
                if( strType != "" )
                {
                    strType = strType.Substring(0, strType.Length - 2);
                }
                else
                {
                    strType = subObjectType.Name;
                }
				mNewItem.SubItems.Add(strType);
				mNewItem.SubItems.Add(mObjectArray.Count.ToString());
				arrayListView.Items.Add(mNewItem);
				iIndexOf++;
			}

			if(iSelectedItem >= 0 &&
				iSelectedItem < arrayListView.Items.Count)
			{
				arrayListView.Items[iSelectedItem].Focused = true;
				arrayListView.Items[iSelectedItem].Selected = true;
			}
			else if( arrayListView.Items.Count > 0)
			{
				arrayListView.Items[arrayListView.Items.Count - 1].Focused = true;
				arrayListView.Items[arrayListView.Items.Count - 1].Selected = true;
			}

            if(objectListView.SelectedItems != null &&
                objectListView.SelectedItems.Count == 1 )
            {
                objectListView.SelectedItems[0].SubItems[0].Text = m_ArrayList.Count.ToString();
            }
		}
		private ArrayViewerData getSelectedArray()
		{
			if (m_iArrayIndexSelected < 0 ||
				m_iArrayIndexSelected >= m_SubArrays.Count)
			{
				return null;
			}
			return m_SubArrays[m_iArrayIndexSelected];
		}
		private PropertyInfo getSelectedArrayPropertyInfo()
		{
			if (m_iArrayIndexSelected < 0 ||
				m_iArrayIndexSelected >= m_SubArrays.Count)
			{
				return null;
			}
			if(m_SubArrays[m_iArrayIndexSelected].memberInfo == null )
			{
				return null;
			}
			return (PropertyInfo)m_SubArrays[m_iArrayIndexSelected].memberInfo;
		}

		private void btnNewArrayItem_Click(object sender, EventArgs e)
		{

			PropertyInfo mPropertyInfo = getSelectedArrayPropertyInfo();
			if( mPropertyInfo == null ||
				m_ObjectsList.Count == 0 )
			{
				return;
			}
			foreach (object mObjectModifying in m_ObjectsList)
			{
				IList mList = mPropertyInfo.GetValue(mObjectModifying) as IList;				
                if(mList == null)
                {
                    continue;
                }
				Type typeOfObject = mList.GetType().GetGenericArguments().Single();
				if(typeOfObject == typeof(System.String) )
				{
					mList.Add("");
				}
				else if( typeOfObject.IsPrimitive)
				{
					mList.Add(0);
				}
				else if( typeOfObject.IsEnum)
				{

				}
				else
				{

                    typeOfObject = RequestTypeOverride(typeOfObject);
                    if (typeOfObject != null)
                    {
                        object mNewObject = Activator.CreateInstance(typeOfObject);
                        if (mNewObject != null)
                        {
                            mList.Add(mNewObject);
                        }
                    }

				}
			}
            SelectedObjectsHaveChangedProperties(m_ObjectsList);
            m_bRefreshArrayItems = true;
            _updateArrayCountOnObjectList();

        }

		private void arrayListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if(arrayListView.Items.Count == 0 ||
				arrayListView.SelectedIndices.Count == 0 )
			{
				return;
			}
            List<object> mSelectedObjects = new List<object>();
            foreach(int iSelectedIndex in arrayListView.SelectedIndices)
            {
                if (iSelectedIndex > m_ArrayList.Count)
                {
                    return;
                }
                List<object> mObjList = m_ArrayList[iSelectedIndex];
                foreach( object mObj in mObjList)
                {
                    mSelectedObjects.Add(mObj);
                }

            }
            btnDeleteArrayItem.Enabled = (mSelectedObjects.Count > 0) ? true : false;
            btnUpArrayItem.Enabled = (mSelectedObjects.Count > 0) ? true : false;
            btnDownArrayItem.Enabled = (mSelectedObjects.Count > 0) ? true : false;
            if (mSelectedObjects.Count == 1)
            {
                objectFilteredPropertyGrid.SelectedObject = mSelectedObjects[0];
            }
            else if (mSelectedObjects.Count > 1)
            {
                objectFilteredPropertyGrid.SelectedObjects = mSelectedObjects.ToArray();
            }


        }

        private int m_iArrayReplaceIndex1 = -1;
        private int m_iArrayReplaceIndex2 = -1;
        public void _arraySwitchIndexes<T>(List<T> mList)
        {
            if (mList == null ||
                m_iArrayReplaceIndex1 < 0 ||
                m_iArrayReplaceIndex2 < 0 ||
                m_iArrayReplaceIndex2 >= mList.Count ||
                m_iArrayReplaceIndex1 >= mList.Count )
            {
                return;
            }

            T mListOfObjectAfter = mList[m_iArrayReplaceIndex1];
            mList[m_iArrayReplaceIndex1] = mList[m_iArrayReplaceIndex2];
            mList[m_iArrayReplaceIndex2] = mListOfObjectAfter;

        }
        
        private void btnUpArrayItem_Click(object sender, EventArgs e)
        {
            if(arrayListView.SelectedIndices.Count != 1) { return; }
            if(arrayListView.SelectedIndices[0] == 0 )
            {
                return;
            }
            PropertyInfo mPropertyInfo = getSelectedArrayPropertyInfo();
            if (mPropertyInfo == null)
            {
                return;
            }
            m_iArrayReplaceIndex1 = arrayListView.SelectedIndices[0];
            m_iArrayReplaceIndex2 = arrayListView.SelectedIndices[0] - 1;

            IEnumerable mEnumerable = (IEnumerable)mPropertyInfo.GetValue(m_ObjectsList[0]);
            if(mEnumerable == null)
            {
                return;
            }
            Type mArrayType = mEnumerable.GetType();
            Type mDataType = mArrayType.GetGenericArguments()[0];
            Type mClassType = GetType(); // The class DoSomething is in
            MethodInfo methodToCall = mClassType.GetMethod("_arraySwitchIndexes"); // DoSomething above
            var genMethod = methodToCall.MakeGenericMethod(new[] { mDataType });

            foreach (object mObjectModifying in m_ObjectsList)
            {
                IEnumerable mArray = (IEnumerable)mPropertyInfo.GetValue(mObjectModifying);
                if (mArray != null)
                {
                    genMethod.Invoke(this, new[] { mArray });
                }

            }


            SelectedObjectsHaveChangedProperties(m_ObjectsList);
            m_bRefreshArrayItems = true;
            _updateArrayListView();



            arrayListView.Items[m_iArrayReplaceIndex1].Focused = false;
            arrayListView.Items[m_iArrayReplaceIndex1].Selected = false;
            arrayListView.Items[m_iArrayReplaceIndex2].Focused = true;
            arrayListView.Items[m_iArrayReplaceIndex2].Selected = true;
        }

        private void btnDownArrayItem_Click(object sender, EventArgs e)
        {
            if (arrayListView.SelectedIndices.Count != 1 || m_ObjectsList.Count == 0) { return; }
            if (arrayListView.SelectedIndices[0] == m_ArrayList.Count - 1)
            {
                return;
            }
            PropertyInfo mPropertyInfo = getSelectedArrayPropertyInfo();
            if (mPropertyInfo == null)
            {
                return;
            }
            m_iArrayReplaceIndex1 = arrayListView.SelectedIndices[0];
            m_iArrayReplaceIndex2 = arrayListView.SelectedIndices[0] + 1;
            
            IEnumerable mEnumerable = (IEnumerable)mPropertyInfo.GetValue(m_ObjectsList[0]);
            if(mEnumerable == null)
            {
                return;
            }
            Type mArrayType = mEnumerable.GetType();
            Type mDataType = mArrayType.GetGenericArguments()[0];
            Type mClassType = GetType(); // The class DoSomething is in
            MethodInfo methodToCall = mClassType.GetMethod("_arraySwitchIndexes"); // DoSomething above
            var genMethod = methodToCall.MakeGenericMethod(new[] { mDataType });

            foreach (object mObjectModifying in m_ObjectsList)
            {
                IEnumerable mArray = (IEnumerable)mPropertyInfo.GetValue(mObjectModifying);
                if (mArray != null)
                {
                    genMethod.Invoke(this, new[] { mArray });
                }

            }


            SelectedObjectsHaveChangedProperties(m_ObjectsList);
            m_bRefreshArrayItems = true;
            _updateArrayListView();



            arrayListView.Items[m_iArrayReplaceIndex1].Focused = false;
            arrayListView.Items[m_iArrayReplaceIndex1].Selected = false;
            arrayListView.Items[m_iArrayReplaceIndex2].Focused = true;
            arrayListView.Items[m_iArrayReplaceIndex2].Selected = true;
            

        }
        public void _deleteArrayObjects<T>(List<T> mList)
        {
            if (mList == null)
            {
                return;
            }
            
            for (int iIndex = mList.Count - 1; iIndex >= 0 ; iIndex--)
            {
                if (arrayListView.SelectedIndices.Contains(iIndex) )//== false)
                {
                    mList.RemoveAt(iIndex);                    
                }
            }

        }
        private void btnDeleteArrayItem_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to delete the select item(s)?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                PropertyInfo mPropertyInfo = getSelectedArrayPropertyInfo();
                if (mPropertyInfo == null)
                {
                    return;
                }

                IEnumerable mEnumerable = (IEnumerable)mPropertyInfo.GetValue(m_ObjectsList[0]);
                if(mEnumerable == null)
                {
                    return;
                }
                Type mArrayType = mEnumerable.GetType();
                Type mDataType = mArrayType.GetGenericArguments()[0];
                Type mClassType = GetType(); // The class DoSomething is in
                MethodInfo methodToCall = mClassType.GetMethod("_deleteArrayObjects"); // DoSomething above
                var genMethod = methodToCall.MakeGenericMethod(new[] { mDataType });

                foreach (object mObjectModifying in m_ObjectsList)
                {
                    IEnumerable mArray = (IEnumerable)mPropertyInfo.GetValue(mObjectModifying);
                    if (mArray != null)
                    {
                        genMethod.Invoke(this, new[] { mArray });
                    }

                }

                SelectedObjectsHaveChangedProperties(m_ObjectsList);
                m_bRefreshArrayItems = true;
                
                _updateArrayListView();
                _updateArrayCountOnObjectList();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        #region events
        ///////////////////////////////////////////////////////////////////////////////        
        public event PropertyValueChangedHandler PropertyValueChanged;
        public delegate void PropertyValueChangedHandler(Object m, EventArgs e);

        public event SelectedObjectsHaveChangedPropertiesHandler SelectedObjectsHaveChangedProperties;
        public delegate void SelectedObjectsHaveChangedPropertiesHandler(List<object> mObjects);

        //gets called when a new item is added to an array. The object it returns is the object it will create.
        public event RequestTypeOverrideHandler RequestTypeOverride;
        public delegate Type RequestTypeOverrideHandler(Type mTypeCreating);

        private void objectFilteredPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if(PropertyValueChanged != null)
            {
                PropertyValueChanged(s, e);
                SelectedObjectsHaveChangedProperties(m_ObjectsList);
            }
        }




        ///////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////



    }   //end class

    public class ClassObjectViewerData
	{
		public string name { get; set; }
		public bool isRootObject { get; set; }
		public MemberInfo memberInfo { get; set; }
		public Type type { get; set; }
	}

	public class ArrayViewerData
	{
		public string name { get; set; }
		public MemberInfo memberInfo { get; set; }
		public Type type { get; set; }
		public Type objectType { get; set; }
	}

}//end namespace
