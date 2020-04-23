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
        public static string createTypeConverters(Dictionary<string,string> mTypeConverterLookup, List<ClassCreatorContentFolders> mContentFolders, string strDatabaseDirectory, string strListDirectory, ProjectWrapper mProjectWrapper)
        {
            string strReturnString = _createListConverters(mTypeConverterLookup, strListDirectory) + Environment.NewLine;
            strReturnString = strReturnString + _createDBConverters(mTypeConverterLookup, strDatabaseDirectory) + Environment.NewLine;
            strReturnString = strReturnString + _createDynamicConverters(mTypeConverterLookup, mContentFolders, mProjectWrapper) + Environment.NewLine;            
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
        private static string _createDynamicConverters(Dictionary<string, string> mTypeConverterLookup, List<ClassCreatorContentFolders> mContentFolders, ProjectWrapper mProjectWrapper)
        {
            string strReturnString = "";
            foreach (ClassStructure mClass in mProjectWrapper.classStructures.Values)
            {
                if (mClass.isSerialized)
                {
                    foreach (ClassVariable mVariable in mClass.variables)
                    {
                        string strUniqueConverter = "";
                        if(_createUnitTypeConverters(mVariable, mTypeConverterLookup, ref strUniqueConverter))
                        {
                            strReturnString = strReturnString + strUniqueConverter;
                            continue;
                        }
                        if(_createPathSelectConverter(mVariable, mTypeConverterLookup, mContentFolders, ref strUniqueConverter))
                        {
                            strReturnString = strReturnString + strUniqueConverter;
                            continue;
                        }
                        if(_createFileFindConverter(mVariable, mTypeConverterLookup, mContentFolders, ref strUniqueConverter))
                        {
                            strReturnString = strReturnString + strUniqueConverter;
                            continue;
                        }

                    }
                }
            }
            return strReturnString;
        }

        private static bool _getPath(ref string strPathSpecified, List<ClassCreatorContentFolders> mContentFolders)
        {
            strPathSpecified = strPathSpecified.Trim().Replace("/", "\\");
            string[] mFolders = strPathSpecified.Split('\\');
            if (mFolders == null ||
                mFolders.Count() == 0)
            {
                return false;
            }
            foreach (ClassCreatorContentFolders mContentFolder in mContentFolders)
            {
                if (mContentFolder.codeName.ToUpper() == mFolders[0].ToUpper())
                {
                    mFolders[0] = mContentFolder.path;
                    break;
                }
            }
            strPathSpecified = "";
            foreach (string strFolder in mFolders)
            {
                strPathSpecified = strPathSpecified + strFolder + "\\";
            }
            strPathSpecified = strPathSpecified.Replace("\\\\", "\\");
            return true;
        }

        private static bool _createPathSelectConverter(ClassVariable mVariable, Dictionary<string, string> mTypeConverterLookup, List<ClassCreatorContentFolders> mContentFolders, ref string strReturnString)
        {
            strReturnString = "";
            string strFolderPath = "";
            if (mVariable.variableProperties.ContainsKey("FOLDERPATH"))
            {
                strFolderPath = mVariable.variableProperties["FOLDERPATH"];
                if(_getPath(ref strFolderPath, mContentFolders) )
                {
                    string strLookupName = "";
                    strReturnString = strReturnString + _createPathConverter(mTypeConverterLookup, strFolderPath, ref strLookupName) + Environment.NewLine;
                    mVariable.variableProperties["LIST"] = strLookupName;    //this makes it so the ClassCreatorManager will link it to a type converter.
                    return true;
                }                
            }
            return false;
        }
        private static bool _createFileFindConverter(ClassVariable mVariable, Dictionary<string, string> mTypeConverterLookup, List<ClassCreatorContentFolders> mContentFolders, ref string strReturnString)
        {
            strReturnString = "";
            string strFilePath = "";
            if (mVariable.variableProperties.ContainsKey("FILEPATH"))
            {
                strFilePath = mVariable.variableProperties["FILEPATH"];
                if (_getPath(ref strFilePath, mContentFolders))
                {
                    string strFileSearch = "All Files(*.*)| *.*";
                    if( mVariable.variableProperties.ContainsKey("FILEFILTER"))
                    {
                        strFileSearch = "Custom File|" + mVariable.variableProperties["FILEFILTER"];
                    }
                    string strLookupName = "";
                    strReturnString = strReturnString + _createFileSelectConverter(mTypeConverterLookup, strFilePath, strFileSearch, ref strLookupName) + Environment.NewLine;
                    mVariable.variableProperties["LIST"] = strLookupName;    //this makes it so the ClassCreatorManager will link it to a type converter.
                    return true;
                }
            }
            return false;
        }

        private static bool _createUnitTypeConverters(ClassVariable mVariable, Dictionary<string, string> mTypeConverterLookup, ref string strReturnString)
        {
            strReturnString = "";
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
                return true;
            }
            return false;
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
            mTypeConverterLookup[strListName.ToUpper()] = "[TypeConverter(typeof(" + strTypeConverterName + "))]";
            return strReturnString;
        }

        private static string _createPathConverter(Dictionary<string, string> mTypeConverterLookup, string strPath, ref string strLookupName)
        {
            string strReturnString = "";
            string strListName = strPath.ToUpper();
            strListName = strListName.Replace('\\', '_').Replace('.','_');
            
            string strTypeConverterName = "PathSelectConverter_" + strListName;
            strLookupName = strTypeConverterName;

            //now lets write the drop down code for it.
            //http://web.archive.org/web/20090218231316/http://www.winterdom.com/weblog/2006/08/23/ACustomUITypeEditorForActivityProperties.aspx
            strReturnString = strReturnString + Environment.NewLine + "    public class " + strTypeConverterName + " : UITypeEditor" + Environment.NewLine;
            strReturnString = strReturnString + "    {" + Environment.NewLine;
            //strReturnString = strReturnString + "       StandardValuesCollection m_ReturnStandardCollection = null;" + Environment.NewLine;
            strReturnString = strReturnString + "       public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context){ if ( context == null || context.Instance == null ){return base.GetEditStyle(context);} return UITypeEditorEditStyle.Modal;}" + Environment.NewLine;            
            strReturnString = strReturnString + "       public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)" + Environment.NewLine;
            strReturnString = strReturnString + @"
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
                   folderBrowserDialog1.Description = " + "\"Select the directory that you want to use as the default.\";" + @"                    
                   folderBrowserDialog1.ShowNewFolderButton = true;
                   string strPath = (string)value;
                   string strInitialPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), " + "\"" + strPath.Replace("\\", "\\\\") + "\"));" + @"
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
";
            strReturnString = strReturnString + "    }" + Environment.NewLine;
            mTypeConverterLookup[strTypeConverterName.ToUpper()] = "[Editor(typeof(" + strTypeConverterName + "),typeof(UITypeEditor))]";
            
            return strReturnString;
        }

        private static string _createFileSelectConverter(Dictionary<string, string> mTypeConverterLookup, string strPath, string strFileSearch, ref string strLookupName)
        {
            string strReturnString = "";
            string strListName = strPath.ToUpper();
            strListName = strListName.Replace('\\', '_').Replace('.', '_');

            string strTypeConverterName = "FileSelectConverter_" + strListName;
            strLookupName = strTypeConverterName;
            //                     System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            //                     folderBrowserDialog1.Description = "Select the directory that you want to use as the default.";                    
            //                     folderBrowserDialog1.ShowNewFolderButton = true;
            //                     folderBrowserDialog1.SelectedPath = Directory.GetCurrentDirectory() + ".\\Content\\";
            //                     folderBrowserDialog1.ShowDialog();
            //now lets write the drop down code for it.
            //http://web.archive.org/web/20090218231316/http://www.winterdom.com/weblog/2006/08/23/ACustomUITypeEditorForActivityProperties.aspx
            strReturnString = strReturnString + Environment.NewLine + "    public class " + strTypeConverterName + " : UITypeEditor" + Environment.NewLine;
            strReturnString = strReturnString + "    {" + Environment.NewLine;
            //strReturnString = strReturnString + "       StandardValuesCollection m_ReturnStandardCollection = null;" + Environment.NewLine;
            strReturnString = strReturnString + "       public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context){ if ( context == null || context.Instance == null ){return base.GetEditStyle(context);} return UITypeEditorEditStyle.Modal;}" + Environment.NewLine;
            strReturnString = strReturnString + "       public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)" + Environment.NewLine;
            strReturnString = strReturnString + @"
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
                   dlg.Filter = " + "\"" + strFileSearch + "\";" + @"
                   dlg.CheckFileExists = true;
                   string strFullPath = (string)value;
                   if (!File.Exists(strFullPath))
                   {
                       dlg.FileName = null;
                       dlg.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory()," + "\"" + strPath.Replace("\\", "\\\\") + "\");" + @"
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
";
            strReturnString = strReturnString + "    }" + Environment.NewLine;
            mTypeConverterLookup[strTypeConverterName.ToUpper()] = "[Editor(typeof(" + strTypeConverterName + "),typeof(UITypeEditor))]";

            return strReturnString;
        }


    } //end class
}//end namespace
