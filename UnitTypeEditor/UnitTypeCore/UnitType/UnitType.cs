using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;

namespace UnitTypeCore
{
	[Serializable]
	public class UnitType
	{
		private UnitTypeCategory m_UnitTypeCategory = null;
		private string m_strUnitTypeName = "";		
		private bool m_bFullHierarchyConfigured = true;
		private List<string> m_ImmediateChildren = new List<string>();
		private List<string> m_AllChildren = new List<string>();
		private int m_iHashCodeOfUnitType = 0;
		private List<byte> m_BitLookUp = new List<byte>();

		public UnitType() { }
		public UnitType(UnitTypeCategory pParent) { m_UnitTypeCategory = pParent; }
		public UnitType(UnitTypeCategory pParent, string strUnitTypeName)
		{ 
			m_UnitTypeCategory = pParent; 
			unitTypeName = strUnitTypeName;			
		}

		public void _setUnitTypeCategoryOwner(UnitTypeCategory mParent) { m_UnitTypeCategory = mParent; }
		public bool deleteUnitType()
		{
			if (m_UnitTypeCategory != null)
			{
				m_UnitTypeCategory.deleteUnitType(this);
				return true;
			}
			return false;
		}

		[DisplayName("Name"), Category("DETAILS"), Description("The name of the unit type.")]
		public string unitTypeName
		{
			get { return m_strUnitTypeName; }
			set
			{
				if (value == null)
				{
					return;
				}
				if (m_strUnitTypeName != "ANY")
				{
					string strOldName = m_strUnitTypeName;
					m_strUnitTypeName = convertToCodeName(value);
					if (m_strUnitTypeName == "")
					{
						m_strUnitTypeName = strOldName;
					}

					if (m_strUnitTypeName != strOldName &&
						m_UnitTypeCategory != null )
					{
						_calculateHashCodeForUnitType();
						m_UnitTypeCategory.notifyOfUnitTypeNameChange(strOldName, m_strUnitTypeName);
					}
				}
			}
		}

		[DisplayName("Description"), Category("DETAILS"), Description("The description of the unit type.")]
		public string description { get; set; }

		[DisplayName("Immediate Children"), Category("CHILDREN"), Description("The immediate children of this unit type."), Browsable(false)]
		public List<string> immediateChildren 
		{ 
			get 
			{ 
				return m_ImmediateChildren; 
			}
			set
			{
				if (value != null)
				{
					m_ImmediateChildren = new List<string>(value);
				}
				else
				{
					m_ImmediateChildren.Clear();
				}
			}
		}


		[DisplayName("Full Hierarchy"), Category("CHILDREN"), Description("The full hierarchy of children of this unit type."), Browsable(false)]
		public List<string> fullHierarchyOfChildren 
		{ 
			get 
			{ 
				return m_AllChildren; 
			}
			set
			{
				if (value != null)
				{
					m_AllChildren = new List<string>(value);
				}
				else
				{
					m_AllChildren.Clear();
				}
			}
			
		}

		[DisplayName("Bit Lookup Array"), Category("DATA"), Description("the bit look up to see if this is a type of unittype."), Browsable(false)]
		public List<byte> bitLookupArray
		{
			get 
			{ 
				return m_BitLookUp; 
			}
			set
			{
				if (value != null)
				{
					m_BitLookUp = new List<byte>(value);
				}
				else
				{
					m_BitLookUp.Clear();
				}
			}

		}

		//whenever anything changes on the unit type it recalculates this. It's an easy way to understand when things have changed.
		
		public int getHashCodeOfUnitType() { return m_iHashCodeOfUnitType; } 

		private void _calculateHashCodeForUnitType()
		{
			m_iHashCodeOfUnitType = (unitTypeName + "," + string.Join(",", m_AllChildren)).GetHashCode();
		}

		public bool getFullHierarchyConfigured() { return m_bFullHierarchyConfigured;  }

	
		public void setFullHierarchy(List<string> mAllChildren)
		{
			fullHierarchyOfChildren = mAllChildren;
			_calculateBitArray();
			m_bFullHierarchyConfigured = true;			
			_calculateHashCodeForUnitType();
			 
		}
		public void resetFullHierarchy()
		{
			m_bFullHierarchyConfigured = false;
			m_AllChildren.Clear();
		}
		//cleans a string for code
		static public string convertToCodeName(string strStringToClean)
		{
			if(strStringToClean == null )
			{
				return "";
			}
			bool bValidName = false;
			StringBuilder sb = new StringBuilder();
			foreach (char c in strStringToClean)
			{
				if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
				{
					bValidName = true;
					sb.Append(c);
				}
				else
				{
					sb.Append("_");
				}
			}
			if(bValidName == false )
			{
				return "";
			}
			return sb.ToString().ToUpper();
		}

		public void addChildByUnitType(UnitType mUnitType) { addChildByCodeName(mUnitType.unitTypeName); }
		public void addChildByCodeName(string strUnitTypeCodeName)
		{
			foreach( string strChildName in immediateChildren)
			{
				if( strChildName == strUnitTypeCodeName)
				{
					return;	//already have it
				}
			}
			UnitType mUnitType = m_UnitTypeCategory.getUnitTypeByCodeName(strUnitTypeCodeName);
			if (mUnitType != null &&
				mUnitType != this)
			{
				m_ImmediateChildren.Add(strUnitTypeCodeName);				
				m_UnitTypeCategory.childAddedToUnitType(this);				
			}
		}

		public void removeChildByUnitType(UnitType mUnitType) { removeChildByName(mUnitType.unitTypeName); }
		public void removeChildByName(string strUnitTypeName)
		{
			while (m_ImmediateChildren.Remove(strUnitTypeName));
			while (immediateChildren.Remove(strUnitTypeName)) ;
			m_UnitTypeCategory.childRemovedFromUnitType(this);
		}

		public void _notifyOfChildNameChange(string strOldName, string strNewName)
		{
			for (int iIndex = 0; iIndex < m_ImmediateChildren.Count; iIndex++)
			{
				string strChildName = m_ImmediateChildren[iIndex];
				if (strChildName == strOldName)
				{
					m_ImmediateChildren[iIndex] = strNewName;
					break;
				}
			}
			for (int iIndex = 0; iIndex < m_AllChildren.Count; iIndex++)
			{
				string strChildName = m_AllChildren[iIndex];
				if (strChildName == strOldName)
				{
					m_AllChildren[iIndex] = strNewName;
					break;
				}
			}
		}

		public bool isa(string strUnitTypeName)
		{
			if( unitTypeName == strUnitTypeName)
			{
				return true;
			}
			return m_AllChildren.Contains(strUnitTypeName);
		}


		private int _calculateTheNumberOfBytesNeeded()
		{
			int iCountOf = m_UnitTypeCategory.getUnitTypes().Count;
			int iRemainder = iCountOf % 8;
			int iFractionOf = iCountOf / 8;
			int iTotalVariablesNeeded = Math.Max(1, iFractionOf + ((iRemainder > 0) ? 1 : 0));
			return iTotalVariablesNeeded;
		}
		public void _calculateBitArray()
		{
			bitLookupArray.Clear();
			if(m_UnitTypeCategory == null )
			{
				return;
			}
			int iNumberOfBytes = _calculateTheNumberOfBytesNeeded();
			for(int iCount = 0; iCount < iNumberOfBytes; iCount++)
			{
				bitLookupArray.Add(0);
			}
			for(int iUnitTypeIndex = 0; iUnitTypeIndex < m_UnitTypeCategory.unitTypes.Count; iUnitTypeIndex++ )
			{
				UnitType mUnitType = m_UnitTypeCategory.unitTypes[iUnitTypeIndex];
				if ( isa(mUnitType.unitTypeName))
				{
					int iBitOffsetIntoShort = iUnitTypeIndex % 8;
					int iIndexOfShort = iUnitTypeIndex / 8;
					//weird C# won't let me bit shift a byte - even after converting it to byte
					int mValueOf = (int)bitLookupArray[iIndexOfShort];
					int mValueToMod = 1 << iBitOffsetIntoShort;
					bitLookupArray[iIndexOfShort] = Convert.ToByte( mValueOf | mValueToMod );
				}
			}
		}


	}
}
