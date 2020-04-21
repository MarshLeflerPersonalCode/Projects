using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.ClassParser
{
	
	public class ClassVariable
	{
		public ClassVariable()
		{
			variableProperties = new Dictionary<string, string>();
			variableName = "";
			variableType = "";
			variableValue = "";
			category = "";
			variableComment = "";
            typeConverter = "";
        }
		public bool isPointer { get; set; }
		public bool isUE4Variable { get; set; }
		public string variableName { get; set; }
		public string variableType { get; set; }
		public string variableValue { get; set; }
		public string displayName { get; set; }
		public string category { get; set; }		
		public Dictionary<string,string> variableProperties { get; set; }
		public string variableComment { get; set; }
		public bool isStatic { get; set; }
		public bool isConst { get; set; }		
		public bool isPrivateVariable { get; set; }
		public bool isSerialized { get; set; }
		public int lineNumber { get; set; }
        public string typeConverter { get; set; }
	}
	
	public class ClassStructure
	{
		public ClassStructure()
		{
			labels = new List<string>();
			unknownMacros = new List<string>();
			variables = new List<ClassVariable>();
			enums = new List<EnumList>();
			defines = new Dictionary<string, string>();
			comment = "";
			name = "";
			file = "";
			classStructuresInheritingFrom = new List<string>();
		}
		public List<string> classStructuresInheritingFrom { get; set; }
		public string comment { get; set; }
		public string name { get; set; }
		public bool valid { get; set; }
		//this checks for a macro named SERIALIZE_CODE and if it find it, it flags it a serialized
		public bool isSerialized { get; set; }
		public string file { get; set; }
		public List<ClassVariable> variables { get; set; }
		public bool isClass { get; set; }
		public List<string> unknownMacros { get; set; }
		public List<string> labels { get; set; }
		public List<EnumList> enums { get; set; }
		public Dictionary<string,string> defines { get; set; }
	}
}
