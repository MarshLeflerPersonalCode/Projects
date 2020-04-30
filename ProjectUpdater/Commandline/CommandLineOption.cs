using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLine
{
	public class CommandLineOption
	{
		List<string> m_Arguments;
		string m_strDescription;
		CommandLineVariable m_Variable;

		public CommandLineOption(string[] args, string strDescription)
		{
			m_Arguments = new List<string>(args);
			m_strDescription = strDescription;
			m_Variable = new CommandLineVariable(false);
		}
		public CommandLineOption(string[] args, string strDescription, string strDefaultValue)
		{
			m_Arguments = new List<string>(args);
			m_strDescription = strDescription;
			m_Variable = new CommandLineVariable(strDefaultValue);
		}

		public CommandLineOption(string[] args, string strDescription, int iDefaultValue)
		{
			m_Arguments = new List<string>(args);
			m_strDescription = strDescription;
			m_Variable = new CommandLineVariable(iDefaultValue);
		}
		public string getCommandsAsString()
		{
			string strOutString = "{ ";
			foreach (string strCommand in m_Arguments)
			{
				if (strCommand != m_Arguments[m_Arguments.Count - 1])
				{
					strOutString = strOutString + strCommand + ", ";
				}
				else
				{
					strOutString = strOutString + strCommand + " ";
				}
			}
			strOutString = strOutString + "}";
			return strOutString;
		}
		public string getCommandAsHelpString()
		{
			string strOutString = getCommandsAsString();			
			while (strOutString.Length < 30)
			{
				strOutString = strOutString + " ";
			}
			strOutString = strOutString + m_strDescription;
			return strOutString;
		}

		public bool getHasValidValue()
		{
			return m_Variable.getHasValidValue();
		}

		public bool getIsCommand(string strCommand)
		{
			for (int iIndex = 0; iIndex < m_Arguments.Count; iIndex++ )
			{
				if(m_Arguments[iIndex].ToLower() == strCommand.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		public bool getWantsNextArgument()
		{
			if( m_Variable.m_eType == CommandLineVariable.EVARIABLE_TYPE.BOOL)
			{
				return false;
			}
			return true;
		}
		

		public void setValue(string strValue)
		{
			m_Variable.setValueByString(strValue);
		}

		public int getValueAsInt()
		{
			return m_Variable.m_iValue;
		}
		public string getValueAsString()
		{

			return m_Variable.getValueAsString();
		}

		public bool getValueAsBool()
		{
			return m_Variable.m_bValue;
		}

	}
}
