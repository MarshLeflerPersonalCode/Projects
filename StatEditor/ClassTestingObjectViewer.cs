using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace StatEditor
{
	public class simpleObject
	{
		public int intPropertyInSimpleObject { get; set; }
	}

	public enum EEnumTest
	{
		first,
		second,
		third,
		fourth
	}

	public class ClassTestingObjectViewer
	{

		public ClassTestingObjectViewer()
		{
			objectTest = new simpleObject();
			arrayListOfStrings = new List<string>();
			arrayListOfObjects = new List<simpleObject>();
			arrayListOfEnums = new List<EEnumTest>();
		}

		public bool boolProperty { get; set; }
		public EEnumTest enumTest { get; set; }

		public simpleObject objectTest { get; set; }
		[DisplayName("Array List Of String")]
		public List<string> arrayListOfStrings { get; set; }
		[DisplayName("Array List Of Objects")]
		public List<simpleObject> arrayListOfObjects { get; set; }
		[DisplayName("Array List Of Enums")]
		public List<EEnumTest> arrayListOfEnums { get; set; }
	}
}
