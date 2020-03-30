using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace UnitTypeCore.LoadAndSave
{
	public class UnitTypeManagerConfig
	{

		public UnitTypeManagerConfig()
		{
			unittypeConfigFile = "";
			enumHeaderLayout = @"[ENUM]
[FUNCTIONS]";
			defineInt8 = "int8";
			defineUint8 = "uint8";
			defineInt32 = "int32";
			defineString = "KCString";
		}


		[DisplayName("Config File"), 
				Category("FILES"), 
				Description("The file containing all the unittypes.")]		
		public string unittypeConfigFile { get; set; }


		[DisplayName("Enum Header Layout"),
				Category("C++"),
				Description("this is what will be printed out when saving enum headers."),
				Browsable(false)]
		public string enumHeaderLayout { get; set; }

		[DisplayName("int8/signed char"),
				Category("C++ Defines"),
				Description("This is a signed char. Used when exporting the header for the enums.")]
		public string defineInt8 { get; set; }

		[DisplayName("uint8/unsigned char"),
				Category("C++ Defines"),
				Description("This is a unsigned char. Used when exporting the header for the enums.")]
		public string defineUint8 { get; set; }

		[DisplayName("int32/int"),
				 Category("C++ Defines"),
				 Description("This is an integer 32bit. Used when exporting the header for the enums.")]
		public string defineInt32 { get; set; }

		[DisplayName("String"),
				 Category("C++ Defines"),
				 Description("This is the string to use when exporting the header for the enums.")]
		public string defineString { get; set; }


		[NonSerialized] private string m_strBinaryFolder = "";
		[DisplayName("Folder For Binary File"),
		 Category("DATA"),
		 Description("The folder which the binary file will save.")]
		[EditorAttribute(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string binaryDirectory
		{
			get { return m_strBinaryFolder; }
			set { m_strBinaryFolder = UnitTypeFile.getRelativePath(value);  }
		}

		[NonSerialized] private string m_strBinaryFile = "unittypes.bin";
		[DisplayName("Unit Type Binary File"),
		 Category("DATA"),
		 Description("This is the file that c++ will load.")]
		public string binaryFile
		{
			get { return m_strBinaryFile; }
			set { m_strBinaryFile = UnitTypeFile.getRelativePath(value);  }
		}



		public static UnitTypeManagerConfig createFromConfigFile()
		{
			return createFromConfigFile("config.json");
		}

		public static UnitTypeManagerConfig createFromConfigFile(string strConfig)
		{
			try
			{
				if (File.Exists(strConfig) == false)
				{
					strConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strConfig);
					if (File.Exists(strConfig) == false)
					{
						return new UnitTypeManagerConfig();
					}
				}			
				UnitTypeManagerConfig mConfig = JsonConvert.DeserializeObject<UnitTypeManagerConfig>(File.ReadAllText(strConfig));
				return mConfig;
			}
			catch (Exception e)
			{
				// Let the user know what went wrong.
				Console.WriteLine("Unable to write file " + strConfig + ":");
				Console.WriteLine(e.Message);
			}
			
			return new UnitTypeManagerConfig();
		}
		public bool save()
		{
			return save("config.json");
		}

		public bool save(string strConfig)
		{
			try
			{
				strConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strConfig);
				using (StreamWriter file = File.CreateText(strConfig))
				{
					JsonSerializer serializer = new JsonSerializer();
					serializer.Serialize(file, this);
				}
			}
			catch (Exception e)
			{
				// Let the user know what went wrong.
				Console.WriteLine("Unable to write file " + strConfig + ":");
				Console.WriteLine(e.Message);
				return false;
			}
			return true;
		}

	}
}