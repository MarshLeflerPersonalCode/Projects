using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Library.IO;
using Newtonsoft.Json;

namespace Library.ClassCreator
{

	public enum EVARIABLE_CSHARP_TYPES
	{
		NOT_DEFINED,
		BYTE,
		SBYTE,
		SHORT,
		USHORT,
		INT,
		UINT,
		LONG,
		ULONG,
		STRING,
		FLOAT,
		DOUBLE,
		CLASS,
		ENUM,
		COUNT
	};
	public static class EVARIABLE_CSHARP_TYPES_NAMES
	{
		public static string[] g_Names = new string[] {
			"",			//NOT_DEFINED
			"byte",
			"sbyte",
			"short",
			"ushort",
			"int",
			"uint",
			"long",		
			"ulong",	
			"string",	
			"float",	
			"double",	
			"",			//CLASS
			"",			//ENUM
			""			//COUNT
		};

	}
	public class VariableDefinition
	{
		public VariableDefinition()
		{
			eCSharpVariable = EVARIABLE_CSHARP_TYPES.NOT_DEFINED;
		}
		//this is something like int32 or uint64
		public string variableName { get; set; }

		public EVARIABLE_CSHARP_TYPES eCSharpVariable { get; set; }

		//these are if the variable is a core type of variable. such as an int or a float. Usually things get defined as these
		public bool isPrimitiveType { get; set; }

		//when using property grids you might want to use a type converter. 
		public string typeConverter { get; set; }

	}

	public class VariableDefinitionHandler
	{
		public List<VariableDefinition> m_VariableDefinitions = new List<VariableDefinition>();
		public VariableDefinitionHandler(string strDefinitionsFile)
		{
			if (load(strDefinitionsFile) == false)
			{
				defineInitialVariables();
				save(strDefinitionsFile);
			}
		}

		public void defineInitialVariables()
		{
			addVariable("int32", EVARIABLE_CSHARP_TYPES.INT, true);
			addVariable("uint32", EVARIABLE_CSHARP_TYPES.UINT, true);
			addVariable("int8", EVARIABLE_CSHARP_TYPES.SBYTE, true);
			addVariable("uint8", EVARIABLE_CSHARP_TYPES.BYTE, true);
			addVariable("char", EVARIABLE_CSHARP_TYPES.BYTE, true);
			addVariable("int16", EVARIABLE_CSHARP_TYPES.SHORT, true);
			addVariable("uint16", EVARIABLE_CSHARP_TYPES.USHORT, true);
			addVariable("int64", EVARIABLE_CSHARP_TYPES.LONG, true);
			addVariable("uint64", EVARIABLE_CSHARP_TYPES.ULONG, true);
			addVariable("float", EVARIABLE_CSHARP_TYPES.FLOAT, true);
			addVariable("float", EVARIABLE_CSHARP_TYPES.DOUBLE, true);
			addVariable("string", EVARIABLE_CSHARP_TYPES.STRING, true);
			addVariable("fname", EVARIABLE_CSHARP_TYPES.STRING, true);
			addVariable("fstring", EVARIABLE_CSHARP_TYPES.STRING, true);
			addVariable("KCName", EVARIABLE_CSHARP_TYPES.STRING, true);
			addVariable("KCString", EVARIABLE_CSHARP_TYPES.STRING, true);
		}

		public LogFile logFile { get; set; }
		public void log(string strMessage)
		{
			if (logFile != null)
			{
				log(strMessage);
			}
		}

		public bool load(string strPathAndFile)
		{
			try
			{
				if (File.Exists(strPathAndFile))
				{
					object mRawObject = JsonConvert.DeserializeObject(File.ReadAllText(strPathAndFile), m_VariableDefinitions.GetType());
					if (mRawObject != null)
					{
						m_VariableDefinitions = (List<VariableDefinition>)mRawObject;
						return true;
					}
				}
			}
			catch (Exception e)
			{
				log("Error - attempting to load Variable definitions and it failed. File location: " + strPathAndFile + Environment.NewLine + "Error message was: " + e.Message);
			}
			return false;
		}

		public bool save(string strPathAndFile)
		{
			try
			{
				string strValue = JsonConvert.SerializeObject(m_VariableDefinitions, Newtonsoft.Json.Formatting.Indented);
				if (strValue == null ||
					strValue == "")
				{
					log("ERROR - Unable to serialize variable definitions.");
					return false;
				}
				File.WriteAllText(strPathAndFile, strValue);
				if (File.Exists(strPathAndFile) == false)
				{
					log("ERROR - unable to save variable definitions to " + strPathAndFile);
					return false;
				}
				return true;
			}
			catch (Exception e)
			{
				log("Error - attempting to save Variable definitions and it failed. File location: " + strPathAndFile + Environment.NewLine + "Error message was: " + e.Message);
			}
			return false;
		}

		public List<VariableDefinition> getVariableDefinitions() { return m_VariableDefinitions; }

		public VariableDefinition addVariable(string strVariableName, EVARIABLE_CSHARP_TYPES eVariableType, bool bIsPrimitive)
		{
			if (eVariableType == EVARIABLE_CSHARP_TYPES.CLASS)
			{
				return addVariableAsClass(strVariableName);
			}
			else if (eVariableType == EVARIABLE_CSHARP_TYPES.ENUM)
			{
				return addVariableAsEnum(strVariableName);
			}
			else
			{
				VariableDefinition mVariable = new VariableDefinition();
				mVariable.variableName = strVariableName;
				mVariable.eCSharpVariable = eVariableType;
				mVariable.isPrimitiveType = bIsPrimitive;
				return addVariable(mVariable);
			}
		}
		public VariableDefinition addVariableAsClass(string strVariableName)
		{
			VariableDefinition mVariable = new VariableDefinition();
			mVariable.variableName = strVariableName;
			mVariable.eCSharpVariable = EVARIABLE_CSHARP_TYPES.CLASS;
			return addVariable(mVariable);
		}
		public VariableDefinition addVariableAsEnum(string strVariableName)
		{
			VariableDefinition mVariable = new VariableDefinition();
			mVariable.variableName = strVariableName;
			mVariable.eCSharpVariable = EVARIABLE_CSHARP_TYPES.CLASS;
			return addVariable(mVariable);
		}
	
		public VariableDefinition addVariable(VariableDefinition mVariable)
		{
			for( int i = 0; i < m_VariableDefinitions.Count; i++)
			{
				if(m_VariableDefinitions[i].variableName == mVariable.variableName)
				{
					log("Error - adding dupicate variable with name " + mVariable.variableName);
					return null;
				}
			}
			m_VariableDefinitions.Add(mVariable);
			return mVariable;
		}

	} //end of class
}//end of namespace
