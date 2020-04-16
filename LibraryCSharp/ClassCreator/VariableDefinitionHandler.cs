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
        LIST,
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
            "",         //LIST
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

        //This is the class that the variable name will turn into
        public string variableClassName { get; set; }
        //This is enum that the variable name will turn into
        public string variableEnumName { get; set; }
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
			addVariablePrimitive("int32", EVARIABLE_CSHARP_TYPES.INT);
            addVariablePrimitive("uint32", EVARIABLE_CSHARP_TYPES.UINT);
            addVariablePrimitive("int8", EVARIABLE_CSHARP_TYPES.SBYTE);
            addVariablePrimitive("uint8", EVARIABLE_CSHARP_TYPES.BYTE);
            addVariablePrimitive("char", EVARIABLE_CSHARP_TYPES.BYTE);
            addVariablePrimitive("int16", EVARIABLE_CSHARP_TYPES.SHORT);
            addVariablePrimitive("uint16", EVARIABLE_CSHARP_TYPES.USHORT);
            addVariablePrimitive("int64", EVARIABLE_CSHARP_TYPES.LONG);
            addVariablePrimitive("uint64", EVARIABLE_CSHARP_TYPES.ULONG);
            addVariablePrimitive("float", EVARIABLE_CSHARP_TYPES.FLOAT);
            addVariablePrimitive("float", EVARIABLE_CSHARP_TYPES.DOUBLE);
            addVariablePrimitive("string", EVARIABLE_CSHARP_TYPES.STRING);
            addVariablePrimitive("fname", EVARIABLE_CSHARP_TYPES.STRING);
            addVariablePrimitive("fstring", EVARIABLE_CSHARP_TYPES.STRING);
            addVariablePrimitive("KCName", EVARIABLE_CSHARP_TYPES.STRING);
            addVariablePrimitive("KCString", EVARIABLE_CSHARP_TYPES.STRING);
            addVariablePrimitive("KCDatabaseGuid", EVARIABLE_CSHARP_TYPES.INT);
            addVariableList("KCTArray");
            
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

		public VariableDefinition addVariablePrimitive(string strVariableName, EVARIABLE_CSHARP_TYPES eVariableType)
		{
			if (EVARIABLE_CSHARP_TYPES_NAMES.g_Names[(int)eVariableType] == "")
            {
                return null;
			}
			else
			{
				VariableDefinition mVariable = new VariableDefinition();
				mVariable.variableName = strVariableName;
				mVariable.eCSharpVariable = eVariableType;
				mVariable.isPrimitiveType = true;
				return addVariable(mVariable);
			}
		}
		public VariableDefinition addVariableAsClass(string strVariableName, string strClassName)
		{
			VariableDefinition mVariable = new VariableDefinition();
			mVariable.variableName = strVariableName;
            mVariable.variableClassName = strClassName;
			mVariable.eCSharpVariable = EVARIABLE_CSHARP_TYPES.CLASS;
			return addVariable(mVariable);
		}
		public VariableDefinition addVariableAsEnum(string strVariableName, string strEnumName)
		{
			VariableDefinition mVariable = new VariableDefinition();

            mVariable.variableName = strVariableName;
            mVariable.variableEnumName = strEnumName;
            mVariable.eCSharpVariable = EVARIABLE_CSHARP_TYPES.ENUM;
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

        public VariableDefinition addVariableList(string strListName)
        {
            VariableDefinition mVariable = new VariableDefinition();
            mVariable.variableName = strListName;            
            mVariable.eCSharpVariable = EVARIABLE_CSHARP_TYPES.LIST;
            return addVariable(mVariable);
        }

	} //end of class
}//end of namespace
