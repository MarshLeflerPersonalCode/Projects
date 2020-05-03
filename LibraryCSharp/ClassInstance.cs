using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.ComponentModel;
using Library.ClassParser;
using Library.IO;
namespace Library
{
    public interface IClassInstanceCallbacks
    {
        void _notifyOfPropertySetOnClassInstance(ClassInstance mInstance, string strProperty);
    }

    public class ClassInstance
	{
		[Browsable(false)]
		[NonSerialized] public ClassStructure m_ClassStructure = null;
		[Browsable(false)]
		[NonSerialized] public string m_strClassName = "";
		[Browsable(false)]
		[NonSerialized] public LogFile m_LogFile = null;
		[Browsable(false)]
		[NonSerialized] public bool m_bIsDirty = false;
        [Browsable(false)]
        [NonSerialized] public List<IClassInstanceCallbacks> m_CallBacks = new List<IClassInstanceCallbacks>();
        [Browsable(false)]
        [NonSerialized] protected ClassInstance m_OwningClass = null;

        public virtual object _getAs(Type mType)
        {
            if (mType == this.GetType())
            {
                return (ClassInstance)this;
            }
            return null;
        }

        public void setOwningClass(ClassInstance mOwner)
        {
            if( mOwner == m_OwningClass )
            {
                return;
            }
            if( mOwner == this )
            {
                mOwner = null;
            }
            m_OwningClass = mOwner;
            try
            {
                Type mType = GetType();                //mDataGroup.setProperty("CSHARP", mType.AssemblyQualifiedName);
                MemberInfo[] mMembers = mType.GetMembers();

                Type mStringType = Type.GetType("System.String");
                foreach (MemberInfo mMember in mMembers)
                {
                    MemberTypes mMemberTypeInfo = mMember.MemberType;
                    if (mMemberTypeInfo == MemberTypes.Property)
                    {
                        PropertyInfo mPropertyInfo = mMember as PropertyInfo;
                        if(mPropertyInfo == null ||
                            mPropertyInfo.CanWrite == false ||
                            mPropertyInfo.PropertyType.IsPrimitive )
                        {
                            continue;
                        }
                        try
                        {
                            object mObject = mPropertyInfo.GetValue(this);
                            ClassInstance mObjectClassInstance = mObject as ClassInstance;
                            if (mObjectClassInstance != null)
                            {
                                ClassInstance mNewParent = null;
                                if (m_OwningClass != null) //if m_OwningClass == null then we no longer have an owning class
                                {
                                    mNewParent = mPropertyInfo.GetValue(m_OwningClass) as ClassInstance;
                                }
                                mObjectClassInstance.setOwningClass(mNewParent);
                            }
                            else 
                            {
                                IList iList = mObject as IList;
                                IList iParentList = null;
                                if( m_OwningClass != null )
                                {
                                    iParentList = mPropertyInfo.GetValue(m_OwningClass) as IList;
                                }
                                if( iList != null)
                                {
                                    if( iParentList != null)
                                    {
                                        for (int iParentIndex = 0; iParentIndex < iParentList.Count; iParentIndex++)
                                        {
                                            ClassInstance mParentInstanceInList = iParentList[iParentIndex] as ClassInstance;
                                            if( mParentInstanceInList != null )
                                            {
                                                if(iParentIndex >= iList.Count )
                                                {

                                                    iList.Add(Activator.CreateInstance(mParentInstanceInList.GetType()));
                                                }
                                                else if(iList[iParentIndex] == null )
                                                {
                                                    iList[iParentIndex] = Activator.CreateInstance(mParentInstanceInList.GetType());
                                                }
                                                ClassInstance mCurrentInstanceInList = iList[iParentIndex] as ClassInstance;
                                                mCurrentInstanceInList.setOwningClass(mParentInstanceInList);
                                            }
                                        }
                                    }
                                    Type mTypeForList = null;
                                    if (iList.Count > 0)
                                    {
                                        Type[] mTypes = iList.GetType().GetGenericArguments();
                                        if (mTypes != null &&
                                            mTypes.Length > 0)
                                        {
                                            mTypeForList = mTypes.Single();
                                        }
                                    }
                                    //this usually happens when the parent was set to null
                                    for(int iIndex = 0; iIndex < iList.Count; iIndex++)
                                    {
                                        if( iList[iIndex] == null &&
                                            mTypeForList != null )
                                        {
                                            try
                                            {
                                                iList[iIndex] = Activator.CreateInstance(mTypeForList);
                                            }
                                            catch
                                            {
                                                log("Error in attempting to create instance for array object. Type was: " + mTypeForList.Name);
                                                iList[iIndex] = null;
                                            }
                                        }
                                        ClassInstance mInstanceInList = iList[iIndex] as ClassInstance;
                                        if( mInstanceInList != null )
                                        {
                                            ClassInstance mNewParent = null;
                                            if (iParentList != null &&
                                                iIndex < iParentList.Count)
                                            {
                                                mNewParent = iParentList[iIndex] as ClassInstance;
                                                if( mNewParent == null ||
                                                    mNewParent.GetType() != mInstanceInList.GetType())
                                                {
                                                    mNewParent = null;
                                                }
                                            }
                                            mInstanceInList.setOwningClass(mNewParent);
                                        }                                        
                                    }
                                    
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            log("ERROR - attempting to get property. from object type " + mPropertyInfo.Name + ". Error was: " + e.Message);
                            
                        }
                        continue;
                    }
                    
                }

            }
            catch (Exception e)
            {
                log("ERROR - Unable to serialize object into Data Group. Exception was: " + e.Message);
            }
        }

        public ClassInstance getOwneringClass() { return m_OwningClass; }
        protected void _notifyOfPropertyChanged( string strProperty)
        {
            m_bIsDirty = true;
            foreach(IClassInstanceCallbacks iCallback in m_CallBacks)
            {
                if( iCallback != null )
                {
                    iCallback._notifyOfPropertySetOnClassInstance(this, strProperty);
                }
            }
        }

        public void log(string strLog)
		{
			if( m_LogFile != null)
			{
				m_LogFile.log(strLog);
			}
		}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////
        //start get property by ref
        /////////////////////////////////////////////////////////////////
        #region GetPropertyByRef
       
        public bool getPropertyValue(string strProperty, ref string strValue)
		{
			
			PropertyInfo mProperty = GetType().GetProperty(strProperty);
			if (mProperty != null)
			{
				try
				{
					strValue = mProperty.GetValue(this).ToString();
				}
				catch(Exception e)
				{
					log("ERROR - attempting to get property " + strProperty + " from object type " + GetType().Name + ". Error was: " + e.Message);
					strValue = "";
					return false;
				}
			}
			return true;
		}
		public bool getPropertyValue(string strProperty, ref bool bValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				if (bool.TryParse(strReturnValue, out bValue))
				{
					return true;
				}
			}
			return false;
		}
		public bool getPropertyValue(string strProperty, ref char iValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				if (char.TryParse(strReturnValue, out iValue))
				{
					return true;
				}
			}
			return false;
		}
		public bool getPropertyValue(string strProperty, ref sbyte iValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				if (sbyte.TryParse(strReturnValue, out iValue))
				{
					return true;
				}
			}
			return false;
		}
		public bool getPropertyValue(string strProperty, ref short iValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				if (short.TryParse(strReturnValue, out iValue))
				{
					return true;
				}
			}
			return false;
		}
		public bool getPropertyValue(string strProperty, ref int iValue )
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				if (int.TryParse(strReturnValue, out iValue))
				{
					return true;
				}
			}
			return false;
		}
		public bool getPropertyValue(string strProperty, ref long iValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				if (long.TryParse(strReturnValue, out iValue))
				{
					return true;
				}
			}
			return false;
		}

		public bool getPropertyValue(string strProperty, ref byte iValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				if (byte.TryParse(strReturnValue, out iValue))
				{
					return true;
				}
			}
			return false;
		}
		public bool getPropertyValue(string strProperty, ref ushort iValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				if (ushort.TryParse(strReturnValue, out iValue))
				{
					return true;
				}
			}
			return false;
		}
		public bool getPropertyValue(string strProperty, ref uint iValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				if (uint.TryParse(strReturnValue, out iValue))
				{
					return true;
				}
			}
			return false;
		}
		public bool getPropertyValue(string strProperty, ref ulong iValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				if (ulong.TryParse(strReturnValue, out iValue))
				{
					return true;
				}
			}
			return false;
		}

		public bool getPropertyValue(string strProperty, ref float fValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				if (float.TryParse(strReturnValue, out fValue))
				{
					return true;
				}
			}
			return false;
		}
        #endregion
        /////////////////////////////////////////////////////////////////
        //End get property by ref
        /////////////////////////////////////////////////////////////////
        #region GetProperty
        /////////////////////////////////////////////////////////////////
        //start get property
        /////////////////////////////////////////////////////////////////
        

        public bool hasProperty(string strProperty)
        {
            return (GetType().GetProperty(strProperty) != null) ? true : false;
        }
        public IList getPropertyValueList(string strProperty)
        {

            PropertyInfo mProperty = GetType().GetProperty(strProperty);
            if (mProperty != null)
            {
                try
                {
                    return mProperty.GetValue(this) as IList;
                }
                catch (Exception e)
                {
                    log("ERROR - attempting to get property " + strProperty + " from object type " + GetType().Name + ". Error was: " + e.Message);
                    return null;
                }
            }
            return null;
        }
        public string getPropertyValueString(string strProperty, string strDefaultValue)
		{
			getPropertyValue(strProperty, ref strDefaultValue);
			return strDefaultValue;
		}
		public bool getPropertyValueBool(string strProperty, bool bDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				bool.TryParse(strReturnValue, out bDefaultValue);
			}
			return bDefaultValue;
		}
		public char getPropertyValueChar(string strProperty, char iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				char.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}
		public sbyte getPropertyValueSByte(string strProperty, sbyte iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				sbyte.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}
		public short getPropertyValueShort(string strProperty, short iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				short.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}
		public int getPropertyValueInt(string strProperty,  int iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				int.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}
		public long getPropertyValueLong(string strProperty, long iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				long.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}

		public byte getPropertyValueByte(string strProperty, byte iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				byte.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}
		public ushort getPropertyValueUShort(string strProperty, ushort iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				ushort.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}
		public uint getPropertyValueUInt(string strProperty, uint iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				uint.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}
		public ulong getPropertyValueULong(string strProperty, ulong iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				ulong.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}

		public float getPropertyValueFloat(string strProperty, float fDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				float.TryParse(strReturnValue, out fDefaultValue);
			}
			return fDefaultValue;
		}
		public double getPropertyValueDouble(string strProperty, double fDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				double.TryParse(strReturnValue, out fDefaultValue);
			}
			return fDefaultValue;
		}

        public string getAnyPropertyAsString(string strProperty)
        {
            PropertyInfo mProperty = GetType().GetProperty(strProperty);
            if (mProperty != null)
            {
                try
                {
                    object mObject = mProperty.GetValue(this);
                    Type mType = mObject.GetType();
                    if(mType.IsPrimitive ||
                       mType == Type.GetType("System.String"))
                    {
                        return mObject.ToString();
                    }

                    return mObject.ToString();
                }
                catch (Exception e)
                {
                    log("ERROR - attempting to get property " + strProperty + " from object type " + GetType().Name + ". Error was: " + e.Message);
                    
                    return "";
                }
            }
            return "";
        }
		#endregion
		/////////////////////////////////////////////////////////////////
		//End get property
		/////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////
		//start setting property
		/////////////////////////////////////////////////////////////////
		#region SetProperty
		private bool _setPropertyValue(string strProperty, object mValue)
		{
			PropertyInfo mProperty = GetType().GetProperty(strProperty);
			if (mProperty != null)
			{
				try
				{
					mProperty.SetValue(this, mValue);
                    _notifyOfPropertyChanged(strProperty);

                }
				catch(Exception e)
				{
					log("ERROR - attempting to set property " + strProperty + " on object type " + GetType().Name + ". Error was: " + e.Message);
					return false;
				}
			}
			return true;
		}
        public bool setProperty(string strProperty, string strValue) { return _setPropertyValue(strProperty, strValue); }
        public bool setProperty(string strProperty, bool bValue) { return _setPropertyValue(strProperty, bValue); }
		public bool setProperty(string strProperty, char iValue) { return _setPropertyValue(strProperty, iValue); }
		public bool setProperty(string strProperty, sbyte iValue) { return _setPropertyValue(strProperty, iValue); }
		public bool setProperty(string strProperty, byte iValue) { return _setPropertyValue(strProperty, iValue); }
		public bool setProperty(string strProperty, short iValue) { return _setPropertyValue(strProperty, iValue); }
		public bool setProperty(string strProperty, ushort iValue) { return _setPropertyValue(strProperty, iValue); }
		public bool setProperty(string strProperty, int iValue) { return _setPropertyValue(strProperty, iValue); }
		public bool setProperty(string strProperty, uint iValue) { return _setPropertyValue(strProperty, iValue); }
		public bool setProperty(string strProperty, long iValue) { return _setPropertyValue(strProperty, iValue); }
		public bool setProperty(string strProperty, ulong iValue) { return _setPropertyValue(strProperty, iValue); }
		public bool setProperty(string strProperty, float fValue) { return _setPropertyValue(strProperty, fValue); }
		#endregion
		/////////////////////////////////////////////////////////////////
		//End set property
		/////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        

	}
}
