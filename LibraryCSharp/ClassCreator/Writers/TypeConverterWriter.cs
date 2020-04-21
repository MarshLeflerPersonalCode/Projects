using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Library.ClassParser;

namespace Library.ClassCreator.Writers
{
    public class TypeConverterWriter
    {
        public static string createTypeConverters(Dictionary<string,string> mTypeConverterLookup, string strDatabaseDirectory, string strListDirectory, ProjectWrapper mProjectWrapper)
        {
            string strReturnString = _createListConverters(mTypeConverterLookup, strListDirectory) + Environment.NewLine;
            strReturnString = strReturnString + _createDBConverters(mTypeConverterLookup, strDatabaseDirectory) + Environment.NewLine;
            strReturnString = strReturnString + _createUnitTypeConverters(mTypeConverterLookup, mProjectWrapper) + Environment.NewLine;
            return strReturnString;
        }


        //this looks a folder with txt files that are single ordered list of items.
        private static string _createListConverters(Dictionary<string, string> mTypeConverterLookup, string strListDirectory)
        {
            string strReturnString = "";
            string[] mFiles = Directory.GetFiles(strListDirectory, "*.txt");
            foreach(string strFile in mFiles)
            {
                string[] mLines = File.ReadAllLines(strFile);
                if( mLines == null) { continue; }
                string strListName = Path.GetFileNameWithoutExtension(strFile).Trim().ToUpper();
                string strTypeConverterName = "ListTypeConverter_" + strListName;
                //now lets write the drop down code for it.
                //https://www.codeproject.com/Articles/9517/PropertyGrid-and-Drop-Down-properties
                strReturnString = strReturnString + Environment.NewLine + "    public class " + strTypeConverterName + " : StringConverter" + Environment.NewLine;
                strReturnString = strReturnString + "    {" + Environment.NewLine;
                strReturnString = strReturnString + "       StandardValuesCollection m_ReturnStandardCollection = null;" + Environment.NewLine;
                strReturnString = strReturnString + "       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }" + Environment.NewLine;
                strReturnString = strReturnString + "       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }" + Environment.NewLine;
                strReturnString = strReturnString + "       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)" + Environment.NewLine;
                strReturnString = strReturnString + "       {" + Environment.NewLine;
                strReturnString = strReturnString + "           if(m_ReturnStandardCollection == null )" + Environment.NewLine;
                strReturnString = strReturnString + "           {" + Environment.NewLine;
                strReturnString = strReturnString + "                List<String> mEnumList = new List<String>();" + Environment.NewLine;
                foreach (string strLineItem in mLines)
                {
                    strReturnString = strReturnString + "                mEnumList.Add(\"" + strLineItem + "\");" + Environment.NewLine;
                }
                strReturnString = strReturnString + "                m_ReturnStandardCollection = new StandardValuesCollection(mEnumList);" + Environment.NewLine;
                strReturnString = strReturnString + "           }" + Environment.NewLine;
                strReturnString = strReturnString + "           return m_ReturnStandardCollection;" + Environment.NewLine;
                strReturnString = strReturnString + "       }" + Environment.NewLine;
                strReturnString = strReturnString + "    }" + Environment.NewLine;
                mTypeConverterLookup[strListName] = "[TypeConverter(typeof(" + strTypeConverterName + "))]";
            }

            return strReturnString + Environment.NewLine;
        }

        private static string _createDBConverters(Dictionary<string, string> mTypeConverterLookup, string strDatabaseDirectory)
        {
            string strReturnString = "";
            string[] mDirectories = Directory.GetDirectories(strDatabaseDirectory);
            foreach (string strDatabaseName in mDirectories)
            {
                string[] strFolders = strDatabaseName.Split('\\');
                string strListNameNormal = strFolders[strFolders.Length - 1];
                if (strListNameNormal.StartsWith("."))
                {
                    continue;
                }

                string strListName = strListNameNormal.ToUpper();
                string strTypeConverterName = "ListTypeConverter_" + strListName;
                //now lets write the drop down code for it.
                //https://www.codeproject.com/Articles/9517/PropertyGrid-and-Drop-Down-properties
                strReturnString = strReturnString + Environment.NewLine + "    public class " + strTypeConverterName + " : StringConverter" + Environment.NewLine;
                strReturnString = strReturnString + "    {" + Environment.NewLine;
                //strReturnString = strReturnString + "       StandardValuesCollection m_ReturnStandardCollection = null;" + Environment.NewLine;
                strReturnString = strReturnString + "       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }" + Environment.NewLine;
                strReturnString = strReturnString + "       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }" + Environment.NewLine;
                strReturnString = strReturnString + "       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)" + Environment.NewLine;
                strReturnString = strReturnString + "       {" + Environment.NewLine;
                strReturnString = strReturnString + "           return new StandardValuesCollection(DatabaseManager.getDatabaseManager().getDatabase(\"" + strListNameNormal + "\").getListOfNames());" + Environment.NewLine;
                strReturnString = strReturnString + "       }" + Environment.NewLine;
                strReturnString = strReturnString + "    }" + Environment.NewLine;
                mTypeConverterLookup[strListName] = "[TypeConverter(typeof(" + strTypeConverterName + "))]";
            }

            return strReturnString + Environment.NewLine;

        }
        
        private static string _createUnitTypeConverters(Dictionary<string, string> mTypeConverterLookup, ProjectWrapper mProjectWrapper)
        {
            string strReturnString = "";
            foreach (ClassStructure mClass in mProjectWrapper.classStructures.Values)
            {
                if (mClass.isSerialized)
                {
                    foreach (ClassVariable mVariable in mClass.variables)
                    {
                        string strCategory = "";
                        if (mVariable.variableProperties.ContainsKey("UNITTYPE_CATEGORY"))
                        {
                            strCategory = mVariable.variableProperties["UNITTYPE_CATEGORY"];
                        }
                        else if(mVariable.variableProperties.ContainsKey("UNITTYPECATEGORY"))
                        {
                            strCategory = mVariable.variableProperties["UNITTYPECATEGORY"];
                        }
                        if( strCategory != "")
                        {
                            string strUnitType = "";
                            if(mVariable.variableProperties.ContainsKey("UNITTYPEFILTER"))
                            {
                                strUnitType = mVariable.variableProperties["UNITTYPEFILTER"];
                            }
                            string strLookupName = "";
                            strReturnString = strReturnString + _createUnitTypeConverter(mTypeConverterLookup, strCategory, strUnitType, ref strLookupName) + Environment.NewLine;
                            mVariable.variableProperties["LIST"] = strLookupName;    //this makes it so the ClassCreatorManager will link it to a type converter.
                        }
                    }
                }
            }
            return strReturnString;
        }

        private static string _createUnitTypeConverter(Dictionary<string, string> mTypeConverterLookup, string strCategory, string strUnittype, ref string strLookupName)
        {
            string strReturnString = "";
            string strListName = strCategory.ToUpper();
            if(strUnittype != null )
            {
                strListName = strListName + "_" + strUnittype.ToUpper();
            }
            strLookupName = strListName;
            string strTypeConverterName = "UnitTypeConverter_" + strListName;
            if( mTypeConverterLookup.ContainsKey(strListName))
            {
                
                return "";
            }
            //now lets write the drop down code for it.
            //https://www.codeproject.com/Articles/9517/PropertyGrid-and-Drop-Down-properties
            strReturnString = strReturnString + Environment.NewLine + "    public class " + strTypeConverterName + " : StringConverter" + Environment.NewLine;
            strReturnString = strReturnString + "    {" + Environment.NewLine;
            //strReturnString = strReturnString + "       StandardValuesCollection m_ReturnStandardCollection = null;" + Environment.NewLine;
            strReturnString = strReturnString + "       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }" + Environment.NewLine;
            strReturnString = strReturnString + "       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }" + Environment.NewLine;
            strReturnString = strReturnString + "       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)" + Environment.NewLine;
            strReturnString = strReturnString + "       {" + Environment.NewLine;
            if(strUnittype == null ||
                strUnittype == "" )
            {
                strReturnString = strReturnString + "           return new StandardValuesCollection(UnitTypeManager.getUnitTypeManager().getUnitTypeNames(\"" + strCategory.ToUpper() + "\", \"\"));" + Environment.NewLine;
            }
            else
            {
                strReturnString = strReturnString + "           return new StandardValuesCollection(UnitTypeManager.getUnitTypeManager().getUnitTypeNames(\"" + strCategory.ToUpper() + "\",\"" + strUnittype.ToUpper() + "\"));" + Environment.NewLine;
            }
            
            strReturnString = strReturnString + "       }" + Environment.NewLine;
            strReturnString = strReturnString + "    }" + Environment.NewLine;
            mTypeConverterLookup[strListName] = "[TypeConverter(typeof(" + strTypeConverterName + "))]";
            return strReturnString;
        }



    } //end class
}//end namespace
