
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Library.ClassParser;
using Library.ClassCreator;
using Library.IO;
using Library.Database;
using Library.UnitType;
using Library;
namespace Dynamic
{

        public class HelperFunctions
        {
            static public string makeRelativePath(string strStartingDirectory, string strAbsolutePath)
		    {
			    try
			    {
				    string strFolderRelativeTo = strStartingDirectory;//AppDomain.CurrentDomain.BaseDirectory;
				    string strFullFilePath = strAbsolutePath;
				    Uri pathUri = new Uri(strFullFilePath);
				    // Folders must end in a slash
				    if (!strFolderRelativeTo.EndsWith(Path.DirectorySeparatorChar.ToString()))
				    {
					    strFolderRelativeTo += Path.DirectorySeparatorChar;
				    }
				    Uri folderUri = new Uri(strFolderRelativeTo);
				    return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
			    }
			    catch
			    {

			    }
			    return strAbsolutePath;
		    }
        } //end helper functions


    public class ListTypeConverter_GRAPHS : StringConverter
    {
       StandardValuesCollection m_ReturnStandardCollection = null;
       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }
       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }
       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
       {
           if(m_ReturnStandardCollection == null )
           {
                List<String> mEnumList = new List<String>();
                mEnumList.Add("Melee");
                mEnumList.Add("Ranged");
                mEnumList.Add("Magic");
                m_ReturnStandardCollection = new StandardValuesCollection(mEnumList);
           }
           return m_ReturnStandardCollection;
       }
    }



    public class ListTypeConverter_STATS : StringConverter
    {
       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }
       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }
       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
       {
           return new StandardValuesCollection(DatabaseManager.getDatabaseManager().getDatabase("Stats").getListOfNames());
       }
    }

    public class ListTypeConverter_TESTING : StringConverter
    {
       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }
       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }
       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
       {
           return new StandardValuesCollection(DatabaseManager.getDatabaseManager().getDatabase("Testing").getListOfNames());
       }
    }



    public class UnitTypeConverter_ITEMS_NEW4 : StringConverter
    {
       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }
       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }
       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
       {
           return new StandardValuesCollection(UnitTypeManager.getUnitTypeManager().getUnitTypeNames("ITEMS","NEW4"));
       }
    }


    public class PathSelectConverter___CONTENT_ : UITypeEditor
    {
       public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context){ if ( context == null || context.Instance == null ){return base.GetEditStyle(context);} return UITypeEditorEditStyle.Modal;}
       public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)

       {
                IWindowsFormsEditorService editorService;

                if ( context == null || context.Instance == null || provider == null )
                {
                   return value;
                }
                
                try
                {
                   // get the editor service, just like in windows forms
                   editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                   System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
                   folderBrowserDialog1.Description = "Select the directory that you want to use as the default.";                    
                   folderBrowserDialog1.ShowNewFolderButton = true;
                   string strPath = (string)value;
                   string strInitialPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), ".\\Content\\"));
                   if (Directory.Exists(strPath) == false)
                   {
                       strPath = strInitialPath;
                   }
                   else
                   {
                       strPath = Path.Combine( Directory.GetCurrentDirectory(), strPath);
                   }
                   
                   folderBrowserDialog1.SelectedPath = Path.GetFullPath(strPath); 
                   using (folderBrowserDialog1)
                   {
                       DialogResult res = folderBrowserDialog1.ShowDialog();
                       if (res == DialogResult.OK)
                       {
                           strPath = folderBrowserDialog1.SelectedPath;
                           return HelperFunctions.makeRelativePath(strInitialPath, strPath);
                       }
                   }

                   return value;

               } finally
               {
                   editorService = null;
               }
       }
    }


    public class FileSelectConverter___CONTENT_ : UITypeEditor
    {
       public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context){ if ( context == null || context.Instance == null ){return base.GetEditStyle(context);} return UITypeEditorEditStyle.Modal;}
       public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)

       {
                IWindowsFormsEditorService editorService;

                if ( context == null || context.Instance == null || provider == null )
                {
                   return value;
                }
                
                try
                {
                   // get the editor service, just like in windows forms
                   editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                   OpenFileDialog dlg = new OpenFileDialog();
                   dlg.RestoreDirectory = true;
                   dlg.Filter = "Custom File|*.dat";
                   dlg.CheckFileExists = true;
                   string strFullPath = (string)value;
                   if (!File.Exists(strFullPath))
                   {
                       dlg.FileName = null;
                       dlg.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(),".\\Content\\");
                   }
                   else
                   {
                       dlg.FileName = Path.GetFileName(strFullPath);
                       dlg.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), strFullPath.Substring(0, strFullPath.Length - dlg.FileName.Length));
                   }
                   string strInitialDirectoryNotCleaned = dlg.InitialDirectory;
                   dlg.InitialDirectory = Path.GetFullPath(dlg.InitialDirectory);
                   

                   using (dlg)
                   {
                       DialogResult res = dlg.ShowDialog();
                       if (res == DialogResult.OK)
                       {
                           return HelperFunctions.makeRelativePath( dlg.InitialDirectory, dlg.FileName);
                       }
                   }
                   return HelperFunctions.makeRelativePath( dlg.InitialDirectory, (string)value);

               } finally
               {
                   editorService = null;
               }
       }
    }



////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////EDATABASE_TABLES//////////////////////////////////
    public enum EDATABASE_TABLES
    {
         UNDEFINED,
         STATS,
         COUNT,
    };

    public class _TypeConverter_EDATABASE_TABLES : StringConverter
    {
       StandardValuesCollection m_ReturnStandardCollection = null;
       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }
       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }
       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
       {
           if(m_ReturnStandardCollection == null )
           {
                List<String> mEnumList = new List<String>();
                mEnumList.Add("UNDEFINED");
                mEnumList.Add("STATS");
                mEnumList.Add("COUNT");
                m_ReturnStandardCollection = new StandardValuesCollection(mEnumList);
           }
           return m_ReturnStandardCollection;
       }
    }

//////////////////////////////////////////////////EDATABASE_TABLES//////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////ESTAT_PRIMITIVE_TYPES/////////////////////////////
    public enum ESTAT_PRIMITIVE_TYPES
    {
         INT32,
         FLOAT,
    };

    public class _TypeConverter_ESTAT_PRIMITIVE_TYPES : StringConverter
    {
       StandardValuesCollection m_ReturnStandardCollection = null;
       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }
       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }
       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
       {
           if(m_ReturnStandardCollection == null )
           {
                List<String> mEnumList = new List<String>();
                mEnumList.Add("INT32");
                mEnumList.Add("FLOAT");
                m_ReturnStandardCollection = new StandardValuesCollection(mEnumList);
           }
           return m_ReturnStandardCollection;
       }
    }

//////////////////////////////////////////////////ESTAT_PRIMITIVE_TYPES/////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////ESTAT_HANDLER_TYPE////////////////////////////////
    public enum ESTAT_HANDLER_TYPE
    {
         CHARACTER,
         ITEM,
    };

    public class _TypeConverter_ESTAT_HANDLER_TYPE : StringConverter
    {
       StandardValuesCollection m_ReturnStandardCollection = null;
       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }
       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }
       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
       {
           if(m_ReturnStandardCollection == null )
           {
                List<String> mEnumList = new List<String>();
                mEnumList.Add("CHARACTER");
                mEnumList.Add("ITEM");
                m_ReturnStandardCollection = new StandardValuesCollection(mEnumList);
           }
           return m_ReturnStandardCollection;
       }
    }

//////////////////////////////////////////////////ESTAT_HANDLER_TYPE////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////ESTAT_AGGREGATE_TYPES/////////////////////////////
    public enum ESTAT_AGGREGATE_TYPES
    {
         ADD,
         SUBTRACT,
         MULTIPLE,
    };

    public class _TypeConverter_ESTAT_AGGREGATE_TYPES : StringConverter
    {
       StandardValuesCollection m_ReturnStandardCollection = null;
       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }
       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }
       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
       {
           if(m_ReturnStandardCollection == null )
           {
                List<String> mEnumList = new List<String>();
                mEnumList.Add("ADD");
                mEnumList.Add("SUBTRACT");
                mEnumList.Add("MULTIPLE");
                m_ReturnStandardCollection = new StandardValuesCollection(mEnumList);
           }
           return m_ReturnStandardCollection;
       }
    }

//////////////////////////////////////////////////ESTAT_AGGREGATE_TYPES/////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////ESTAT_INHERIT_TYPES///////////////////////////////
    public enum ESTAT_INHERIT_TYPES
    {
         [DescriptionAttribute("This won't get any stats from any other children or parents")]
         SELF,
         [DescriptionAttribute("This gets all the stats from the current stat handler and it's children")]
         CHILDREN,
         [DescriptionAttribute("This will get all the stats accumalated by walking up the parent and then getting all the stats including children")]
         FULL_HIERARCHY,
    };

    public class _TypeConverter_ESTAT_INHERIT_TYPES : StringConverter
    {
       StandardValuesCollection m_ReturnStandardCollection = null;
       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }
       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }
       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
       {
           if(m_ReturnStandardCollection == null )
           {
                List<String> mEnumList = new List<String>();
                mEnumList.Add("SELF");
                mEnumList.Add("CHILDREN");
                mEnumList.Add("FULL_HIERARCHY");
                m_ReturnStandardCollection = new StandardValuesCollection(mEnumList);
           }
           return m_ReturnStandardCollection;
       }
    }

//////////////////////////////////////////////////ESTAT_INHERIT_TYPES///////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////ETEST/////////////////////////////////////////////
    public enum ETEST
    {
         ONE,
         TWO,
         THREE,
         FOUR,
         COUNT,
    };

    public class _TypeConverter_ETEST : StringConverter
    {
       StandardValuesCollection m_ReturnStandardCollection = null;
       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }
       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }
       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
       {
           if(m_ReturnStandardCollection == null )
           {
                List<String> mEnumList = new List<String>();
                mEnumList.Add("ONE");
                mEnumList.Add("TWO");
                mEnumList.Add("THREE");
                mEnumList.Add("FOUR");
                mEnumList.Add("COUNT");
                m_ReturnStandardCollection = new StandardValuesCollection(mEnumList);
           }
           return m_ReturnStandardCollection;
       }
    }

//////////////////////////////////////////////////ETEST/////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////EDATATYPES////////////////////////////////////////
    public enum EDATATYPES
    {
         BOOL,
         CHAR,
         INT8,
         UINT8,
         INT16,
         UINT16,
         INT32,
         UINT32,
         INT64,
         UINT64,
         FLOAT,
         STRING,
         COUNT,
    };

    public class _TypeConverter_EDATATYPES : StringConverter
    {
       StandardValuesCollection m_ReturnStandardCollection = null;
       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }
       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }
       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
       {
           if(m_ReturnStandardCollection == null )
           {
                List<String> mEnumList = new List<String>();
                mEnumList.Add("BOOL");
                mEnumList.Add("CHAR");
                mEnumList.Add("INT8");
                mEnumList.Add("UINT8");
                mEnumList.Add("INT16");
                mEnumList.Add("UINT16");
                mEnumList.Add("INT32");
                mEnumList.Add("UINT32");
                mEnumList.Add("INT64");
                mEnumList.Add("UINT64");
                mEnumList.Add("FLOAT");
                mEnumList.Add("STRING");
                mEnumList.Add("COUNT");
                m_ReturnStandardCollection = new StandardValuesCollection(mEnumList);
           }
           return m_ReturnStandardCollection;
       }
    }

//////////////////////////////////////////////////EDATATYPES////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////EDATAGROUP_BINARY_VERSION/////////////////////////
    public enum EDATAGROUP_BINARY_VERSION
    {
         ONE,
         COUNT,
    };

    public class _TypeConverter_EDATAGROUP_BINARY_VERSION : StringConverter
    {
       StandardValuesCollection m_ReturnStandardCollection = null;
       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }
       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }
       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
       {
           if(m_ReturnStandardCollection == null )
           {
                List<String> mEnumList = new List<String>();
                mEnumList.Add("ONE");
                mEnumList.Add("COUNT");
                m_ReturnStandardCollection = new StandardValuesCollection(mEnumList);
           }
           return m_ReturnStandardCollection;
       }
    }

//////////////////////////////////////////////////EDATAGROUP_BINARY_VERSION/////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////ETARRAY_GROW_BY_TYPES/////////////////////////////
    public enum ETARRAY_GROW_BY_TYPES
    {
         DOUBLE,
         PREDEFINED,
    };

    public class _TypeConverter_ETARRAY_GROW_BY_TYPES : StringConverter
    {
       StandardValuesCollection m_ReturnStandardCollection = null;
       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }
       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }
       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
       {
           if(m_ReturnStandardCollection == null )
           {
                List<String> mEnumList = new List<String>();
                mEnumList.Add("DOUBLE");
                mEnumList.Add("PREDEFINED");
                m_ReturnStandardCollection = new StandardValuesCollection(mEnumList);
           }
           return m_ReturnStandardCollection;
       }
    }

//////////////////////////////////////////////////ETARRAY_GROW_BY_TYPES/////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////IKCStatMathFunction///////////////////////////////

    public class IKCStatMathFunction: ClassInstance
    {

    } //end of IKCStatMathFunction

//////////////////////////////////////////////////IKCStatMathFunction///////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////KCSerializeChild//////////////////////////////////

    public class KCSerializeChild: ClassInstance
    {
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\TestCases\SerializeTest\KCIncludeTest.h
        //Class Name:KCSerializeChild
        //Variable Name:m_strTest
        //Variable Type:KCString
        //Variable Value:
        //Variable Line Number:15
        private string _m_strTest = "";
        public string m_strTest
        {
            get{ return _m_strTest; }
            set{ _m_strTest = value; _notifyOfPropertyChanged("m_strTest");}
        }
        
    } //end of KCSerializeChild

//////////////////////////////////////////////////KCSerializeChild//////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////KCIncludeTest/////////////////////////////////////

    public class KCIncludeTest: FKCDBEntry
    {
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\TestCases\SerializeTest\KCIncludeTest.h
        //Class Name:KCIncludeTest
        //Variable Name:m_fX
        //Variable Type:float
        //Variable Value:0
        //Variable Line Number:70
        private float _m_fX = 0;
        public float m_fX
        {
            get{ return _m_fX; }
            set{ _m_fX = value; _notifyOfPropertyChanged("m_fX");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\TestCases\SerializeTest\KCIncludeTest.h
        //Class Name:KCIncludeTest
        //Variable Name:m_fY
        //Variable Type:float
        //Variable Value:0
        //Variable Line Number:72
        private float _m_fY = 0;
        public float m_fY
        {
            get{ return _m_fY; }
            set{ _m_fY = value; _notifyOfPropertyChanged("m_fY");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\TestCases\SerializeTest\KCIncludeTest.h
        //Class Name:KCIncludeTest
        //Variable Name:m_fZ
        //Variable Type:float
        //Variable Value:0
        //Variable Line Number:74
        private float _m_fZ = 0;
        public float m_fZ
        {
            get{ return _m_fZ; }
            set{ _m_fZ = value; _notifyOfPropertyChanged("m_fZ");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\TestCases\SerializeTest\KCIncludeTest.h
        //Class Name:KCIncludeTest
        //Variable Name:m_SerializeChild
        //Variable Type:KCSerializeChild
        //Variable Value:
        //Variable Line Number:76
        private KCSerializeChild _m_SerializeChild = new KCSerializeChild();
        public KCSerializeChild m_SerializeChild
        {
            get{ return _m_SerializeChild; }
            set{ _m_SerializeChild = value; _notifyOfPropertyChanged("m_SerializeChild"); }
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\TestCases\SerializeTest\KCIncludeTest.h
        //Class Name:KCIncludeTest
        //Variable Name:m_pSerializeChildTest
        //Variable Type:KCSerializeChild
        //Variable Value:nullptr
        //Variable Line Number:78
        private KCSerializeChild _m_pSerializeChildTest = new KCSerializeChild();
        public KCSerializeChild m_pSerializeChildTest
        {
            get{ return _m_pSerializeChildTest; }
            set{ _m_pSerializeChildTest = value; _notifyOfPropertyChanged("m_pSerializeChildTest"); }
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\TestCases\SerializeTest\KCIncludeTest.h
        //Class Name:KCIncludeTest
        //Variable Name:m_eEnumTest
        //Variable Type:ETEST
        //Variable Value:ETEST::COUNT
        //Variable Line Number:80
        private ETEST _m_eEnumTest = ETEST.COUNT;
        public ETEST m_eEnumTest
        {
            get{ return _m_eEnumTest; }
            set{ _m_eEnumTest = value; _notifyOfPropertyChanged("m_eEnumTest");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\TestCases\SerializeTest\KCIncludeTest.h
        //Class Name:KCIncludeTest
        //Variable Name:m_Array
        //Variable Type:KCTArray<KCSerializeChild *>
        //Variable Value:
        //Variable Line Number:82
        //Variable Properties: DISPLAYNAME = Child Serialized Objects
        private List<KCSerializeChild> _m_Array = new List<KCSerializeChild>();
        [DisplayName("Child Serialized Objects")]
        public List<KCSerializeChild> m_Array
        {
            get{ return _m_Array; }
            set{ _m_Array = value; _notifyOfPropertyChanged("m_Array"); }
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\TestCases\SerializeTest\KCIncludeTest.h
        //Class Name:KCIncludeTest
        //Variable Name:m_strUnitType
        //Variable Type:KCString
        //Variable Value:""
        //Variable Line Number:85
        //Variable Properties: CATEGORY = TESTING, DISPLAYNAME = Unit Type Test, UNITTYPECATEGORY = Items, UNITTYPEFILTER = New4, LIST = ITEMS_NEW4
        private string _m_strUnitType = "";
        [DisplayName("Unit Type Test"), Category("TESTING"), Description("The stat which will be used in the graph. Most times it's the rank.")]
        [TypeConverter(typeof(UnitTypeConverter_ITEMS_NEW4))]
        public string m_strUnitType
        {
            get{ return _m_strUnitType; }
            set{ _m_strUnitType = value; _notifyOfPropertyChanged("m_strUnitType");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\TestCases\SerializeTest\KCIncludeTest.h
        //Class Name:KCIncludeTest
        //Variable Name:m_strFolderTest
        //Variable Type:KCString
        //Variable Value:""
        //Variable Line Number:87
        //Variable Properties: CATEGORY = TESTING, DISPLAYNAME = Folder Path, FOLDERPATH = Content, LIST = PathSelectConverter___CONTENT_
        private string _m_strFolderTest = "";
        [DisplayName("Folder Path"), Category("TESTING")]
        [Editor(typeof(PathSelectConverter___CONTENT_),typeof(UITypeEditor))]
        public string m_strFolderTest
        {
            get{ return _m_strFolderTest; }
            set{ _m_strFolderTest = value; _notifyOfPropertyChanged("m_strFolderTest");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\TestCases\SerializeTest\KCIncludeTest.h
        //Class Name:KCIncludeTest
        //Variable Name:m_strFileTest
        //Variable Type:KCString
        //Variable Value:""
        //Variable Line Number:89
        //Variable Properties: CATEGORY = TESTING, DISPLAYNAME = File, FILEPATH = Content, FILEFILTER = *.dat, LIST = FileSelectConverter___CONTENT_
        private string _m_strFileTest = "";
        [DisplayName("File"), Category("TESTING")]
        [Editor(typeof(FileSelectConverter___CONTENT_),typeof(UITypeEditor))]
        public string m_strFileTest
        {
            get{ return _m_strFileTest; }
            set{ _m_strFileTest = value; _notifyOfPropertyChanged("m_strFileTest");}
        }
        
    } //end of KCIncludeTest

//////////////////////////////////////////////////KCIncludeTest/////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////KCStatMathFunctionAdd/////////////////////////////

    public class KCStatMathFunctionAdd: IKCStatMathFunction
    {
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\MathFunctions\KCStatMathFunctions.h
        //Class Name:KCStatMathFunctionAdd
        //Variable Name:m_strStat
        //Variable Type:KCName
        //Variable Value:
        //Variable Line Number:29
        //Variable Properties: CATEGORY = GENERAL, DISPLAYNAME = Stat, LIST = Stats
        private string _m_strStat = "";
        [DisplayName("Stat"), Category("GENERAL"), Description("the stat that will be added onto the current stat")]
        [TypeConverter(typeof(ListTypeConverter_STATS))]
        public string m_strStat
        {
            get{ return _m_strStat; }
            set{ _m_strStat = value; _notifyOfPropertyChanged("m_strStat");}
        }
        
    } //end of KCStatMathFunctionAdd

//////////////////////////////////////////////////KCStatMathFunctionAdd/////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////KCStatMathFunctionSubtract////////////////////////

    public class KCStatMathFunctionSubtract: IKCStatMathFunction
    {
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\MathFunctions\KCStatMathFunctions.h
        //Class Name:KCStatMathFunctionSubtract
        //Variable Name:m_strStat
        //Variable Type:KCName
        //Variable Value:
        //Variable Line Number:47
        //Variable Properties: CATEGORY = GENERAL, DISPLAYNAME = Stat, LIST = Stats
        private string _m_strStat = "";
        [DisplayName("Stat"), Category("GENERAL"), Description("the stat that will be added onto the current stat")]
        [TypeConverter(typeof(ListTypeConverter_STATS))]
        public string m_strStat
        {
            get{ return _m_strStat; }
            set{ _m_strStat = value; _notifyOfPropertyChanged("m_strStat");}
        }
        
    } //end of KCStatMathFunctionSubtract

//////////////////////////////////////////////////KCStatMathFunctionSubtract////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////KCStatMathFunctionDivide//////////////////////////

    public class KCStatMathFunctionDivide: IKCStatMathFunction
    {
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\MathFunctions\KCStatMathFunctions.h
        //Class Name:KCStatMathFunctionDivide
        //Variable Name:m_strStat
        //Variable Type:KCName
        //Variable Value:
        //Variable Line Number:65
        //Variable Properties: CATEGORY = GENERAL, DISPLAYNAME = Stat, LIST = Stats
        private string _m_strStat = "";
        [DisplayName("Stat"), Category("GENERAL"), Description("the stat that will be added onto the current stat")]
        [TypeConverter(typeof(ListTypeConverter_STATS))]
        public string m_strStat
        {
            get{ return _m_strStat; }
            set{ _m_strStat = value; _notifyOfPropertyChanged("m_strStat");}
        }
        
    } //end of KCStatMathFunctionDivide

//////////////////////////////////////////////////KCStatMathFunctionDivide//////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////KCStatMathFunctionMultiply////////////////////////

    public class KCStatMathFunctionMultiply: IKCStatMathFunction
    {
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\MathFunctions\KCStatMathFunctions.h
        //Class Name:KCStatMathFunctionMultiply
        //Variable Name:m_strStat
        //Variable Type:KCName
        //Variable Value:
        //Variable Line Number:83
        //Variable Properties: CATEGORY = GENERAL, DISPLAYNAME = Stat, LIST = Stats
        private string _m_strStat = "";
        [DisplayName("Stat"), Category("GENERAL"), Description("the stat that will be added onto the current stat")]
        [TypeConverter(typeof(ListTypeConverter_STATS))]
        public string m_strStat
        {
            get{ return _m_strStat; }
            set{ _m_strStat = value; _notifyOfPropertyChanged("m_strStat");}
        }
        
    } //end of KCStatMathFunctionMultiply

//////////////////////////////////////////////////KCStatMathFunctionMultiply////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////KCStatMathFunctionDivideAndMultiply///////////////

    public class KCStatMathFunctionDivideAndMultiply: IKCStatMathFunction
    {
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\MathFunctions\KCStatMathFunctions.h
        //Class Name:KCStatMathFunctionDivideAndMultiply
        //Variable Name:m_strStat
        //Variable Type:KCName
        //Variable Value:
        //Variable Line Number:102
        //Variable Properties: CATEGORY = GENERAL, DISPLAYNAME = Stat, LIST = Stats
        private string _m_strStat = "";
        [DisplayName("Stat"), Category("GENERAL"), Description("the stat that will be added onto the current stat")]
        [TypeConverter(typeof(ListTypeConverter_STATS))]
        public string m_strStat
        {
            get{ return _m_strStat; }
            set{ _m_strStat = value; _notifyOfPropertyChanged("m_strStat");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\MathFunctions\KCStatMathFunctions.h
        //Class Name:KCStatMathFunctionDivideAndMultiply
        //Variable Name:m_fDenominator
        //Variable Type:float
        //Variable Value:100.0f
        //Variable Line Number:105
        //Variable Properties: CATEGORY = GENERAL, DISPLAYNAME = Denominator
        private float _m_fDenominator = 100.0f;
        [DisplayName("Denominator"), Category("GENERAL"), Description("the denominator")]
        public float m_fDenominator
        {
            get{ return _m_fDenominator; }
            set{ _m_fDenominator = value; _notifyOfPropertyChanged("m_fDenominator");}
        }
        
    } //end of KCStatMathFunctionDivideAndMultiply

//////////////////////////////////////////////////KCStatMathFunctionDivideAndMultiply///////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////KCStatMathFunctionDivideAddAndMultiply////////////

    public class KCStatMathFunctionDivideAddAndMultiply: IKCStatMathFunction
    {
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\MathFunctions\KCStatMathFunctions.h
        //Class Name:KCStatMathFunctionDivideAddAndMultiply
        //Variable Name:m_strStat
        //Variable Type:KCName
        //Variable Value:
        //Variable Line Number:149
        //Variable Properties: CATEGORY = GENERAL, DISPLAYNAME = Stat, LIST = Stats
        private string _m_strStat = "";
        [DisplayName("Stat"), Category("GENERAL"), Description("the stat that will be added onto the current stat")]
        [TypeConverter(typeof(ListTypeConverter_STATS))]
        public string m_strStat
        {
            get{ return _m_strStat; }
            set{ _m_strStat = value; _notifyOfPropertyChanged("m_strStat");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\MathFunctions\KCStatMathFunctions.h
        //Class Name:KCStatMathFunctionDivideAddAndMultiply
        //Variable Name:m_fDenominator
        //Variable Type:float
        //Variable Value:100.0f
        //Variable Line Number:152
        //Variable Properties: CATEGORY = GENERAL, DISPLAYNAME = Denominator
        private float _m_fDenominator = 100.0f;
        [DisplayName("Denominator"), Category("GENERAL"), Description("the denominator")]
        public float m_fDenominator
        {
            get{ return _m_fDenominator; }
            set{ _m_fDenominator = value; _notifyOfPropertyChanged("m_fDenominator");}
        }
        
    } //end of KCStatMathFunctionDivideAddAndMultiply

//////////////////////////////////////////////////KCStatMathFunctionDivideAddAndMultiply////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////FKCDBEntry////////////////////////////////////////

    public class FKCDBEntry: ClassInstance
    {
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Database\KCDBEntry.h
        //Class Name:FKCDBEntry
        //Variable Name:m_DatabaseGuid
        //Variable Type:KCDatabaseGuid
        //Variable Value:UNINITIALIZED_DATABASE_GUID
        //Variable Line Number:39
        //Variable Properties: CATEGORY = DATABASE, DISPLAYNAME = Database Guid, READONLY = 
        private int _m_DatabaseGuid = 0;
        [DisplayName("Database Guid"), Category("DATABASE"), ReadOnly(true), Description("the database guid. Must be unique")]
        public int m_DatabaseGuid
        {
            get{ return _m_DatabaseGuid; }
            set{ _m_DatabaseGuid = value; _notifyOfPropertyChanged("m_DatabaseGuid");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Database\KCDBEntry.h
        //Class Name:FKCDBEntry
        //Variable Name:m_strName
        //Variable Type:KCName
        //Variable Value:
        //Variable Line Number:44
        //Variable Properties: CATEGORY = DATABASE, DISPLAYNAME = Name, READONLY = 
        private string _m_strName = "";
        [DisplayName("Name"), Category("DATABASE"), ReadOnly(true), Description("the name of the entry. Must be unique")]
        public string m_strName
        {
            get{ return _m_strName; }
            set{ _m_strName = value; _notifyOfPropertyChanged("m_strName");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Database\KCDBEntry.h
        //Class Name:FKCDBEntry
        //Variable Name:m_strFileName
        //Variable Type:KCString
        //Variable Value:
        //Variable Line Number:47
        //Variable Properties: CATEGORY = DATABASE, DISPLAYNAME = Name, READONLY = 
        private string _m_strFileName = "";
        [DisplayName("Name"), Category("DATABASE"), ReadOnly(true), Description("The filename of this entry")]
        public string m_strFileName
        {
            get{ return _m_strFileName; }
            set{ _m_strFileName = value; _notifyOfPropertyChanged("m_strFileName");}
        }
        
    } //end of FKCDBEntry

//////////////////////////////////////////////////FKCDBEntry////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////FKCStatDefinition/////////////////////////////////

    public class FKCStatDefinition: FKCDBEntry
    {
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\Private\KCStatDefinition.h
        //Class Name:FKCStatDefinition
        //Variable Name:m_eStatType
        //Variable Type:ESTAT_PRIMITIVE_TYPES
        //Variable Value:ESTAT_PRIMITIVE_TYPES::INT32
        //Variable Line Number:16
        //Variable Properties: CATEGORY = GENERAL, DISPLAYNAME = Variable Type
        private ESTAT_PRIMITIVE_TYPES _m_eStatType = ESTAT_PRIMITIVE_TYPES.INT32;
        [DisplayName("Variable Type"), Category("GENERAL"), Description("The stat type dictates how it will be used in game.")]
        public ESTAT_PRIMITIVE_TYPES m_eStatType
        {
            get{ return _m_eStatType; }
            set{ _m_eStatType = value; _notifyOfPropertyChanged("m_eStatType");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\Private\KCStatDefinition.h
        //Class Name:FKCStatDefinition
        //Variable Name:m_eStateAggregateType
        //Variable Type:ESTAT_AGGREGATE_TYPES
        //Variable Value:ESTAT_AGGREGATE_TYPES::ADD
        //Variable Line Number:19
        //Variable Properties: CATGORY = MISC, DISPLAYNAME = Stat Aggregate Type
        private ESTAT_AGGREGATE_TYPES _m_eStateAggregateType = ESTAT_AGGREGATE_TYPES.ADD;
        [DisplayName("Stat Aggregate Type"), Description("When stat systems get added together they aggregate those values. These are the types allowed")]
        public ESTAT_AGGREGATE_TYPES m_eStateAggregateType
        {
            get{ return _m_eStateAggregateType; }
            set{ _m_eStateAggregateType = value; _notifyOfPropertyChanged("m_eStateAggregateType");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\Private\KCStatDefinition.h
        //Class Name:FKCStatDefinition
        //Variable Name:m_strDefaultValue
        //Variable Type:KCString
        //Variable Value:"0"
        //Variable Line Number:22
        //Variable Properties: CATEGORY = GENERAL, DISPLAYNAME = Default Value
        private string _m_strDefaultValue = "0";
        [DisplayName("Default Value"), Category("GENERAL"), Description("The default value of the stat.")]
        public string m_strDefaultValue
        {
            get{ return _m_strDefaultValue; }
            set{ _m_strDefaultValue = value; _notifyOfPropertyChanged("m_strDefaultValue");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\Private\KCStatDefinition.h
        //Class Name:FKCStatDefinition
        //Variable Name:m_strMinValue
        //Variable Type:KCString
        //Variable Value:"0"
        //Variable Line Number:25
        //Variable Properties: CATEGORY = GENERAL, DISPLAYNAME = Min Value
        private string _m_strMinValue = "0";
        [DisplayName("Min Value"), Category("GENERAL"), Description("The min value of the stat.")]
        public string m_strMinValue
        {
            get{ return _m_strMinValue; }
            set{ _m_strMinValue = value; _notifyOfPropertyChanged("m_strMinValue");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\Private\KCStatDefinition.h
        //Class Name:FKCStatDefinition
        //Variable Name:m_strMaxValue
        //Variable Type:KCString
        //Variable Value:"0"
        //Variable Line Number:28
        //Variable Properties: CATEGORY = GENERAL, DISPLAYNAME = Max Value
        private string _m_strMaxValue = "0";
        [DisplayName("Max Value"), Category("GENERAL"), Description("The max value of the stat.")]
        public string m_strMaxValue
        {
            get{ return _m_strMaxValue; }
            set{ _m_strMaxValue = value; _notifyOfPropertyChanged("m_strMaxValue");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\Private\KCStatDefinition.h
        //Class Name:FKCStatDefinition
        //Variable Name:m_bApplicableToCharacters
        //Variable Type:bool
        //Variable Value:true
        //Variable Line Number:31
        //Variable Properties: CATEGORY = APPLICABLE, DISPLAYNAME = Characters
        private bool _m_bApplicableToCharacters = true;
        [DisplayName("Characters"), Category("APPLICABLE"), Description("Will this stat exist on characters?")]
        public bool m_bApplicableToCharacters
        {
            get{ return _m_bApplicableToCharacters; }
            set{ _m_bApplicableToCharacters = value; _notifyOfPropertyChanged("m_bApplicableToCharacters");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\Private\KCStatDefinition.h
        //Class Name:FKCStatDefinition
        //Variable Name:m_bApplicableToItems
        //Variable Type:bool
        //Variable Value:true
        //Variable Line Number:34
        //Variable Properties: CATEGORY = APPLICABLE, DISPLAYNAME = Items
        private bool _m_bApplicableToItems = true;
        [DisplayName("Items"), Category("APPLICABLE"), Description("Will this stat exist on items?")]
        public bool m_bApplicableToItems
        {
            get{ return _m_bApplicableToItems; }
            set{ _m_bApplicableToItems = value; _notifyOfPropertyChanged("m_bApplicableToItems");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\Private\KCStatDefinition.h
        //Class Name:FKCStatDefinition
        //Variable Name:m_strGraph
        //Variable Type:KCString
        //Variable Value:""
        //Variable Line Number:37
        //Variable Properties: CATEGORY = GRAPH, DISPLAYNAME = Graph Name, LIST = Graphs
        private string _m_strGraph = "";
        [DisplayName("Graph Name"), Category("GRAPH"), Description("the graph that will be used to generate the final value. The graph stat should be a float")]
        [TypeConverter(typeof(ListTypeConverter_GRAPHS))]
        public string m_strGraph
        {
            get{ return _m_strGraph; }
            set{ _m_strGraph = value; _notifyOfPropertyChanged("m_strGraph");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\Private\KCStatDefinition.h
        //Class Name:FKCStatDefinition
        //Variable Name:m_strGraphStat
        //Variable Type:KCString
        //Variable Value:"Rank"
        //Variable Line Number:40
        //Variable Properties: CATEGORY = GRAPH, DISPLAYNAME = Graph Stat, LIST = Stats
        private string _m_strGraphStat = "Rank";
        [DisplayName("Graph Stat"), Category("GRAPH"), Description("The stat which will be used in the graph. Most times it's the rank.")]
        [TypeConverter(typeof(ListTypeConverter_STATS))]
        public string m_strGraphStat
        {
            get{ return _m_strGraphStat; }
            set{ _m_strGraphStat = value; _notifyOfPropertyChanged("m_strGraphStat");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\Private\KCStatDefinition.h
        //Class Name:FKCStatDefinition
        //Variable Name:m_eGraphResultStatType
        //Variable Type:ESTAT_PRIMITIVE_TYPES
        //Variable Value:ESTAT_PRIMITIVE_TYPES::INT32
        //Variable Line Number:43
        //Variable Properties: CATEGORY = GRAPH, DISPLAYNAME = Graph Result Variable Type
        private ESTAT_PRIMITIVE_TYPES _m_eGraphResultStatType = ESTAT_PRIMITIVE_TYPES.INT32;
        [DisplayName("Graph Result Variable Type"), Category("GRAPH"), Description("The stat type that gets returned after the calculations. Sometimes/most times you want the stat type to be float while the graph result type is an int")]
        public ESTAT_PRIMITIVE_TYPES m_eGraphResultStatType
        {
            get{ return _m_eGraphResultStatType; }
            set{ _m_eGraphResultStatType = value; _notifyOfPropertyChanged("m_eGraphResultStatType");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\Private\KCStatDefinition.h
        //Class Name:FKCStatDefinition
        //Variable Name:m_MathFunctions
        //Variable Type:KCTArray<IKCStatMathFunction>
        //Variable Value:
        //Variable Line Number:46
        //Variable Properties: CATEGORY = MISC, DISPLAYNAME = Math Functions
        private List<IKCStatMathFunction> _m_MathFunctions = new List<IKCStatMathFunction>();
        [DisplayName("Math Functions"), Category("MISC"), Description("functions that will do math on the stat. The functions get ran in order 0-to end.")]
        public List<IKCStatMathFunction> m_MathFunctions
        {
            get{ return _m_MathFunctions; }
            set{ _m_MathFunctions = value; _notifyOfPropertyChanged("m_MathFunctions"); }
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\Private\KCStatDefinition.h
        //Class Name:FKCStatDefinition
        //Variable Name:m_StatsReferencing
        //Variable Type:KCTArray<KCName>
        //Variable Value:
        //Variable Line Number:49
        //Variable Properties: CATEGORY = MISC, DISPLAYNAME = Stat Refs, HIDDEN = 
        private List<string> _m_StatsReferencing = new List<string>();
        [DisplayName("Stat Refs"), Category("MISC"), Browsable(false), Description("Stats need to know what other stats are referencing them. This is the array of stat ids")]
        public List<string> m_StatsReferencing
        {
            get{ return _m_StatsReferencing; }
            set{ _m_StatsReferencing = value; _notifyOfPropertyChanged("m_StatsReferencing"); }
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\Private\KCStatDefinition.h
        //Class Name:FKCStatDefinition
        //Variable Name:m_bDirtyAllStats
        //Variable Type:bool
        //Variable Value:false
        //Variable Line Number:52
        //Variable Properties: CATEGORY = MISC, DISPLAYNAME = Dirty All Stats
        private bool _m_bDirtyAllStats = false;
        [DisplayName("Dirty All Stats"), Category("MISC"), Description("When this stat changes all the stats will need to be recalculate")]
        public bool m_bDirtyAllStats
        {
            get{ return _m_bDirtyAllStats; }
            set{ _m_bDirtyAllStats = value; _notifyOfPropertyChanged("m_bDirtyAllStats");}
        }
        
    } //end of FKCStatDefinition

//////////////////////////////////////////////////FKCStatDefinition/////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

} //end of namespace

