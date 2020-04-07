using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.CommandLine
{
	public class CommandLineVariable
	{

		public enum EVARIABLE_TYPE
		{
			UNKNOWN,
			STRING,
			BOOL,
			INTEGER
		};
		public EVARIABLE_TYPE m_eType = EVARIABLE_TYPE.UNKNOWN;
		public string m_strValue = "";
		public bool m_bValue = false;
		public int m_iValue = -1;
		public CommandLineVariable(string strDefaultString)
		{
			m_eType = EVARIABLE_TYPE.STRING;
			m_strValue = strDefaultString;
		}
		public CommandLineVariable(bool bDefaultValue)
		{
			m_eType = EVARIABLE_TYPE.BOOL;
			m_bValue = bDefaultValue;
		}
		public CommandLineVariable(int iDefaultValue)
		{
			m_eType = EVARIABLE_TYPE.INTEGER;
			m_iValue = iDefaultValue;
		}
		public bool setValueByString(string strValue)
		{
			try
			{
				switch (m_eType)
				{
					case EVARIABLE_TYPE.BOOL:
						{
							m_bValue = true;
						}
						return true;
					case EVARIABLE_TYPE.INTEGER:
						return int.TryParse(strValue, out m_iValue);
					default:
						{
							m_strValue = parseString(strValue);
						}
						return true;

				}
			}
			catch
			{


			}
			return false;
		}

		public string getValueAsString()
		{
			switch (m_eType)
			{
				case EVARIABLE_TYPE.BOOL:
					return m_bValue.ToString();
				case EVARIABLE_TYPE.INTEGER:
					return m_iValue.ToString();
				default:
					return m_strValue;
			}
		}

		private string parseString(string strString)
		{
			strString = strString.Trim();
			if(strString.StartsWith("'") || strString.StartsWith("\""))
			{
				strString = strString.Substring(1);
			}
			if(strString.EndsWith("'") || strString.EndsWith("\""))
			{
				strString = strString.Substring(0, strString.Length - 1);
			}
			return strString;
		}

		public bool getHasValidValue()
		{
			switch (m_eType)
			{
				case EVARIABLE_TYPE.BOOL:
					return m_bValue;
				case EVARIABLE_TYPE.INTEGER:
					return (m_iValue != -1) ? true : false;
				default:
					return (m_strValue != null && m_strValue != "")?true:false;
			}
		}


	}
}
