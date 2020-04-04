using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.CommandLine
{
	public class CommandLineArguments
	{
		private List<CommandLineOption> m_CommandLineOptions = new List<CommandLineOption>();
		

		public CommandLineArguments()
		{

		}
		//adds a command line option
		public void addCommandLineOption( CommandLineOption mOption )
		{
			m_CommandLineOptions.Add(mOption);
		}
		//lists all the commands
		public string getCommandsAsAstring()
		{
			string strOutput = "Commands: " + Environment.NewLine;
			int iCount = 0;
			foreach(CommandLineOption mCommand in m_CommandLineOptions)
			{
				iCount++;
				strOutput = strOutput + iCount.ToString() + ") " +  mCommand.getCommandAsHelpString() + Environment.NewLine;
			}
			return strOutput;
		}

		public bool setCommand(string strCommand, string strValue )
		{
			CommandLineOption mCommand = _getCommand(strCommand);
			if( mCommand != null)
			{
				mCommand.setValue(strValue);
				return true;
			}
			return false;
		}

		//puts all the commands into a string list to print out
		public string getCommandsAsDisplayString()
		{
			string strOutput = "Command line arguments:" + Environment.NewLine;
			int iCount = 0;
			foreach (CommandLineOption mCommand in m_CommandLineOptions)
			{
				if (mCommand.getHasValidValue())
				{
					iCount++;
					strOutput = strOutput + iCount.ToString() + ") " + mCommand.getCommandsAsString() + " : " + mCommand.getValueAsString() + Environment.NewLine;
				}
			}
			if( iCount == 0 )
			{
				strOutput = "No arguments passed in." + Environment.NewLine;
			}
			return strOutput;
		}

		public void parseArguments(string[] args)
		{
			//todo
			for( int iIndexOfArg = 0; iIndexOfArg < args.Length; iIndexOfArg++)
			{
				string strValue = args[iIndexOfArg];
				CommandLineOption mCommand = _getCommand(strValue);
				if( mCommand != null)
				{
					if( mCommand.getWantsNextArgument() &&
						iIndexOfArg != args.Length - 1)
					{
						mCommand.setValue(args[iIndexOfArg + 1]);
					}
					else
					{
						mCommand.setValue("");
					}
				}
			}

			
		}

		private CommandLineOption _getCommand(string strPossibleCommand)
		{
			
			foreach( CommandLineOption mCommand in m_CommandLineOptions)
			{
				if( mCommand.getIsCommand(strPossibleCommand))
				{
					return mCommand;
				}
			}
			return null;
		}

		public int getCommandValueAsInt(string strCommand)
		{
			CommandLineOption mOption = _getCommand(strCommand);
			if (mOption != null)
			{
				return mOption.getValueAsInt();
			}
			return -1;
		}
		public string getCommandValueAsString(string strCommand)
		{

			CommandLineOption mOption = _getCommand(strCommand);
			if (mOption != null)
			{
				return mOption.getValueAsString();
			}
			return "";
		}

		public bool getCommandValueAsBool(string strCommand)
		{
			CommandLineOption mOption = _getCommand(strCommand);
			if (mOption != null)
			{
				return mOption.getValueAsBool();
			}
			return false;
		}

	}
}
