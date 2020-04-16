using System;
using System.Collections.Generic;
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

		public string getPropertyValue(string strProperty, string strDefaultValue)
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
