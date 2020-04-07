using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.ClassParser
{
	public class EnumItem
	{
		public EnumItem()
		{
			name = "";
			value = -1;
			properties = new Dictionary<string, string>();
		}
		public string name { get; set; }
		public int value { get; set; }
		public Dictionary<string,string> properties { get; set; }
		public string comment { get; set; }
	};
	public class EnumList
	{


		public EnumList()
		{
			enumName = "";
			comment = "";
			enumItems = new List<EnumItem>();
			type = "";
		}

		public string enumName { get; set; }
		public string type { get; set; }
		public string comment { get; set; }
		public List<EnumItem> enumItems { get; set; }		

	}
}
