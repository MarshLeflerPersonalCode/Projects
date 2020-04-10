using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.ComponentModel;
using Library.ClassParser;
using Library.IO;
namespace Library.ClassCreator
{
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
		public int getPropertyValueChar(string strProperty, ref char iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				char.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}
		public sbyte getPropertyValueSByte(string strProperty, ref sbyte iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				sbyte.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}
		public short getPropertyValueShort(string strProperty, ref short iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				short.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}
		public int getPropertyValueInt(string strProperty, ref int iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				int.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}
		public long getPropertyValueLong(string strProperty, ref long iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				long.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}

		public byte getPropertyValueByte(string strProperty, ref byte iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				byte.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}
		public ushort getPropertyValueUShort(string strProperty, ref ushort iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				ushort.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}
		public uint getPropertyValueUInt(string strProperty, ref uint iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				uint.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}
		public ulong getPropertyValueULong(string strProperty, ref ulong iDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				ulong.TryParse(strReturnValue, out iDefaultValue);
			}
			return iDefaultValue;
		}

		public float getPropertyValueFloat(string strProperty, ref float fDefaultValue)
		{
			string strReturnValue = "";
			if (getPropertyValue(strProperty, ref strReturnValue))
			{
				float.TryParse(strReturnValue, out fDefaultValue);
			}
			return fDefaultValue;
		}
		public double getPropertyValueDouble(string strProperty, ref double fDefaultValue)
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
					m_bIsDirty = true;
				}
				catch(Exception e)
				{
					log("ERROR - attempting to set property " + strProperty + " on object type " + GetType().Name + ". Error was: " + e.Message);
					return false;
				}
			}
			return true;
		}

		private bool setProperty(string strProperty, bool bValue) { return _setPropertyValue(strProperty, bValue); }
		private bool setProperty(string strProperty, char iValue) { return _setPropertyValue(strProperty, iValue); }
		private bool setProperty(string strProperty, sbyte iValue) { return _setPropertyValue(strProperty, iValue); }
		private bool setProperty(string strProperty, byte iValue) { return _setPropertyValue(strProperty, iValue); }
		private bool setProperty(string strProperty, short iValue) { return _setPropertyValue(strProperty, iValue); }
		private bool setProperty(string strProperty, ushort iValue) { return _setPropertyValue(strProperty, iValue); }
		private bool setProperty(string strProperty, int iValue) { return _setPropertyValue(strProperty, iValue); }
		private bool setProperty(string strProperty, uint iValue) { return _setPropertyValue(strProperty, iValue); }
		private bool setProperty(string strProperty, long iValue) { return _setPropertyValue(strProperty, iValue); }
		private bool setProperty(string strProperty, ulong iValue) { return _setPropertyValue(strProperty, iValue); }
		private bool setProperty(string strProperty, float fValue) { return _setPropertyValue(strProperty, fValue); }
		#endregion
		/////////////////////////////////////////////////////////////////
		//End set property
		/////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	}
}
