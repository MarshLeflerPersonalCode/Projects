
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



    public class UnitTypeConverter_ITEMS_NEW4 : StringConverter
    {
       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }
       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }
       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
       {
           return new StandardValuesCollection(UnitTypeManager.getUnitTypeManager().getUnitTypeNames("ITEMS","NEW4"));
       }
    }



////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////EUNITTYPES_ITEMS//////////////////////////////////
    public enum EUNITTYPES_ITEMS
    {
         ANY,
         NEW,
         NEW1,
         NEW2,
         NEW3,
         NEW4,
         NEW5,
         NEW6,
         NEW7,
         NEW8,
    };

    public class _TypeConverter_EUNITTYPES_ITEMS : StringConverter
    {
       StandardValuesCollection m_ReturnStandardCollection = null;
       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }
       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }
       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
       {
           if(m_ReturnStandardCollection == null )
           {
                List<String> mEnumList = new List<String>();
                mEnumList.Add("ANY");
                mEnumList.Add("NEW");
                mEnumList.Add("NEW1");
                mEnumList.Add("NEW2");
                mEnumList.Add("NEW3");
                mEnumList.Add("NEW4");
                mEnumList.Add("NEW5");
                mEnumList.Add("NEW6");
                mEnumList.Add("NEW7");
                mEnumList.Add("NEW8");
                m_ReturnStandardCollection = new StandardValuesCollection(mEnumList);
           }
           return m_ReturnStandardCollection;
       }
    }

//////////////////////////////////////////////////EUNITTYPES_ITEMS//////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////

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
//////////////////////////////////////////////////FKCStatDefinition/////////////////////////////////

    public class FKCStatDefinition: ClassInstance
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
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\Private\KCStatDefinition.h
        //Class Name:FKCStatDefinition
        //Variable Name:m_eStatType
        //Variable Type:ESTAT_PRIMITIVE_TYPES
        //Variable Value:ESTAT_PRIMITIVE_TYPES::INT32
        //Variable Line Number:17
        //Variable Properties: CATEGORY = GENERAL, DISPLAYNAME = Type
        private ESTAT_PRIMITIVE_TYPES _m_eStatType = ESTAT_PRIMITIVE_TYPES.INT32;
        [DisplayName("Type"), Category("GENERAL"), Description("The stat type dictates how it will be used in game.")]
        public ESTAT_PRIMITIVE_TYPES m_eStatType
        {
            get{ return _m_eStatType; }
            set{ _m_eStatType = value; _notifyOfPropertyChanged("m_eStatType");}
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\KCCore\Systems\Stats\Private\KCStatDefinition.h
        //Class Name:FKCStatDefinition
        //Variable Name:m_strDefaultValue
        //Variable Type:KCString
        //Variable Value:"0"
        //Variable Line Number:20
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
        //Variable Line Number:23
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
        //Variable Line Number:26
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
        //Variable Line Number:29
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
        //Variable Line Number:32
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
        //Variable Line Number:35
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
        //Variable Line Number:38
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
        //Variable Name:m_strUnitType
        //Variable Type:KCString
        //Variable Value:""
        //Variable Line Number:41
        //Variable Properties: CATEGORY = GRAPH, DISPLAYNAME = Unit Type Test, UNITTYPECATEGORY = Items, UNITTYPEFILTER = New4, LIST = ITEMS_NEW4
        private string _m_strUnitType = "";
        [DisplayName("Unit Type Test"), Category("GRAPH"), Description("The stat which will be used in the graph. Most times it's the rank.")]
        [TypeConverter(typeof(UnitTypeConverter_ITEMS_NEW4))]
        public string m_strUnitType
        {
            get{ return _m_strUnitType; }
            set{ _m_strUnitType = value; _notifyOfPropertyChanged("m_strUnitType");}
        }
        
    } //end of FKCStatDefinition

//////////////////////////////////////////////////FKCStatDefinition/////////////////////////////////
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
        //Variable Line Number:14
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

    public class KCIncludeTest: ClassInstance
    {
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Class File:..\CoreClasses\TestCases\SerializeTest\KCIncludeTest.h
        //Class Name:KCIncludeTest
        //Variable Name:m_fX
        //Variable Type:float
        //Variable Value:0
        //Variable Line Number:69
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
        //Variable Line Number:71
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
        //Variable Line Number:73
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
        //Variable Line Number:75
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
        //Variable Line Number:77
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
        //Variable Line Number:79
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
        //Variable Line Number:81
        //Variable Properties: DISPLAYNAME = Child Serialized Objects
        private List<KCSerializeChild> _m_Array = new List<KCSerializeChild>();
        [DisplayName("Child Serialized Objects")]
        public List<KCSerializeChild> m_Array
        {
            get{ return _m_Array; }
            set{ _m_Array = value; _notifyOfPropertyChanged("m_Array"); }
        }
        
    } //end of KCIncludeTest

//////////////////////////////////////////////////KCIncludeTest/////////////////////////////////////
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

} //end of namespace

