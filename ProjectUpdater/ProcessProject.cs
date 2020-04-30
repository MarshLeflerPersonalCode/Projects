using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using CommandLine;
namespace ProjectUpdater
{
    public class ProcessProject
    {
        List<string> m_FilesFound = new List<string>();
        public ProcessProject(string[] args)
        {

            if (_createCommandLineArguments(args) &&
                _createBackup() &&
                _findFiles() &&
                _writeProjectFile() &&
                _writeFilterFile())
            {
                Console.WriteLine("Project updated correctly. " + _rootFolder() + _projectFile());
                if (_pause())
                {
                    Console.ReadKey();
                }
                return;
            }
            if (_pause())
            {
                Console.ReadKey();
            }
        }

        //finds the files
        private bool _findFiles()
        {
            m_FilesFound.Clear();
            string[] mHeaderFiles = Directory.GetFiles(_rootFolder(), "*.h", SearchOption.AllDirectories);
            string[] mCodeFiles = Directory.GetFiles(_rootFolder(), "*.cpp", SearchOption.AllDirectories);
            foreach(string strFile in mHeaderFiles)
            {
                m_FilesFound.Add(strFile);
            }
            foreach (string strFile in mCodeFiles)
            {
                m_FilesFound.Add(strFile);
            }
            if ( m_FilesFound.Count == 0 )
            {
                Console.WriteLine("No source or header files found in root folder. " + _rootFolder() );

                return false;
            }

            return true;
        }

        //returns the command line arguments
        public CommandLineArguments commandLineArguments { get; set; }
        private bool _createCommandLineArguments(string[] args)
        {
            commandLineArguments = new CommandLineArguments();
            commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-Help", "-?" }, "Prints out all the commands"));
            commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-Pause", "-P" }, "Pauses at the end waiting for a key stroke"));
            commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-ProjectFile", "-PF" }, "The project file that contains the list of all source and header files.", ""));
            commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-FilterFile", "-FF" }, "The filter file override. If you don't specify it uses the Project File and appends .filters to it.", ""));
            commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-ProjectTag", "-PT" }, "The project file that contains the list of all source and header files.", "ItemGroup"));
            commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-FilterTag", "-FT" }, "The project file that contains the list of all source and header files.", "ItemGroup"));
            commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-FilterHeader", "-FH" }, "The project file that contains the list of all source and header files.", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Project ToolsVersion=\"4.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\" >\n<code>\n</Project>"));
            
            string strRootFolder = System.Reflection.Assembly.GetExecutingAssembly().Location;
            strRootFolder = strRootFolder.Substring(0, strRootFolder.Length - Path.GetFileName(strRootFolder).Length);            
            commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-RootFolder", "-RF" }, "This is where the project and filter files are located. If you leave it blank it uses the current executable folder.", strRootFolder));

            commandLineArguments.parseArguments(args);

            if (commandLineArguments.getCommandValueAsBool("-?"))
            {
                Console.WriteLine(commandLineArguments.getCommandsAsAstring());
            }
            strRootFolder = commandLineArguments.getCommandValueAsString("-RootFolder");
            if (strRootFolder == "" ||
                Directory.Exists(strRootFolder) == false)
            {
                Console.WriteLine("Invalid root folder. " + commandLineArguments.getCommandValueAsString("-RootFolder"));
                return false;
            }
            
            if (strRootFolder.EndsWith("\\") == false )
            {
                strRootFolder = strRootFolder + "\\";
            }
            commandLineArguments.setCommand("-RootFolder", strRootFolder);
            string strProjectFile = _projectFile();
            if( strProjectFile == "" )
            {
                string[] mFiles =  Directory.GetFiles(_rootFolder(), "*.vcxproj", SearchOption.TopDirectoryOnly);
                if( mFiles.Length == 0 )
                {
                    Console.WriteLine("no project files found in root folder: " + _rootFolder());
                    return false;
                }
                if( mFiles.Length > 1 )
                {
                    Console.WriteLine("To many project files to parse in folder: " + _rootFolder());
                    return false;
                }
                commandLineArguments.setCommand("-ProjectFile", Path.GetFileName(mFiles[0]) );
                strProjectFile = _projectFile(); ;
            }
            if (strProjectFile == "" ||
                File.Exists(strRootFolder + strProjectFile) == false )
            {
                Console.WriteLine("Invalid project file. " + strProjectFile);
                return false;
            }
            if (_FilterFile() == "")
            {
                commandLineArguments.setCommand("-FilterFile", strProjectFile + ".filters");
            }
            return true;
        }
        private bool _pause()
        {
            return commandLineArguments.getCommandValueAsBool("-Pause");
        }
        private string _rootFolder()
        {
            return commandLineArguments.getCommandValueAsString("-RootFolder");
        }
        private string _projectFile()
        {
            return commandLineArguments.getCommandValueAsString("-ProjectFile");
        }
        private string _FilterFile()
        {
            return commandLineArguments.getCommandValueAsString("-FilterFile");
        }
        private bool _createBackup()
        {
            File.Delete(_rootFolder() + _projectFile() + ".backup");
            File.Delete(_rootFolder() + _FilterFile() + ".backup");
            if(File.Exists(_rootFolder() + _projectFile()) == false )
            {
                Console.WriteLine("Invalid project file. " + _rootFolder() + _projectFile());
                return false;
            }
            File.Copy(_rootFolder() + _projectFile(), _rootFolder() + _projectFile() + ".backup");
            File.Copy(_rootFolder() + _FilterFile(), _rootFolder() + _FilterFile() + ".backup");
            return true;
        }

        private bool _writeProjectFile()
        {
            string strOriginalFile = File.ReadAllText(_rootFolder() + _projectFile());
            string[] mProjectFileLines = File.ReadAllLines(_rootFolder() + _projectFile());
            StringWriter mStringWriter = new StringWriter();
            if(mProjectFileLines == null ||
                mProjectFileLines.Length == 0)
            {
                Console.WriteLine("Project File is empty. " + _rootFolder() + _projectFile());
                return false;
            }
            string strProjectTag = "<" + commandLineArguments.getCommandValueAsString("-ProjectTag") + ">";
            string strProjectEndTag = "</" + commandLineArguments.getCommandValueAsString("-ProjectTag") + ">";
            bool bWrittenTags = false;            
            for(int iIndex = 0; iIndex < mProjectFileLines.Count(); iIndex++)
            {
            
                string strLineClean = mProjectFileLines[iIndex].Trim();
                if(bWrittenTags == false &&
                    strLineClean == strProjectTag)
                {
                    bWrittenTags = true;
                    _writeProjectTags(mStringWriter);
                    int iEndTagsFound = 0;
                    for( ; iIndex < mProjectFileLines.Count(); iIndex++)
                    {
                        strLineClean = mProjectFileLines[iIndex].Trim();
                        if(strLineClean == strProjectEndTag)
                        {
                            iEndTagsFound++;
                            if(iEndTagsFound ==2)
                            {
                                break;
                            }
                        }
                    }
                    if( iEndTagsFound != 2)
                    {
                        Console.WriteLine("Unable to parse project file. " + _rootFolder() + _projectFile());
                        return false;
                    }
                    continue;   //the tags for the cpp and header files are in the sequential groups. So we just look for two end tags.
                }
                mStringWriter.WriteLine(mProjectFileLines[iIndex]);

            }

            string strNewFile = mStringWriter.ToString();
            if (strNewFile != strOriginalFile)
            {
                File.WriteAllText(_rootFolder() + _projectFile(), strNewFile);
            }
            return true;

        }

        private bool _writeProjectTags(StringWriter mStringWriter)
        {
            string strProjectTag = "<" + commandLineArguments.getCommandValueAsString("-ProjectTag") + ">";
            string strProjectEndTag = "</" + commandLineArguments.getCommandValueAsString("-ProjectTag") + ">";
            mStringWriter.WriteLine(strProjectTag);
            foreach (string strFile in m_FilesFound)
            {
                if (strFile.EndsWith(".h") == false)
                {
                    
                    mStringWriter.WriteLine("    <ClCompile Include=\"" + strFile.Substring(_rootFolder().Length, strFile.Length - _rootFolder().Length) + "\" />");
                }
            }
            mStringWriter.WriteLine(strProjectEndTag);
            mStringWriter.WriteLine(strProjectTag);
            foreach (string strFile in m_FilesFound)
            {
                if (strFile.EndsWith(".h"))
                {
                    
                    mStringWriter.WriteLine("    <ClInclude Include=\"" + strFile.Substring(_rootFolder().Length, strFile.Length - _rootFolder().Length) + "\" />");
                }
            }
            mStringWriter.WriteLine(strProjectEndTag);
            return true;
        }

        private bool _writeFilterFile()
        {
            string strOriginalFile = File.ReadAllText(_rootFolder() + _FilterFile());
            string[] mFilterLines = File.ReadAllLines(_rootFolder() + _FilterFile());
            StringWriter mStringWriter = new StringWriter();
            if (mFilterLines == null ||
                mFilterLines.Length == 0)
            {
                Console.WriteLine("Filter File is empty. " + _rootFolder() + _projectFile());
                return false;
            }
            string strFilterFile = commandLineArguments.getCommandValueAsString("-FilterHeader");
            string strFilterTag = "<" + commandLineArguments.getCommandValueAsString("-FilterTag") + ">";
            string strFilterEndTag = "</" + commandLineArguments.getCommandValueAsString("-FilterTag") + ">";
            string strFilters = _buildFilters();
            string strTags = _buildTags();
            string strNewFile = strFilterFile.Replace("<code>", strFilters + strTags);            
            if (strNewFile != strOriginalFile)
            {
                File.WriteAllText(_rootFolder() + _FilterFile(), strNewFile);
            }
            return true;
        }

        public string _buildFilters()
        {
            string strReturnString = "<" + commandLineArguments.getCommandValueAsString("-FilterTag") + ">" + Environment.NewLine;
            List<string> mFilters = new List<string>();
            foreach (string strFile in m_FilesFound)
            {
                string strFilter = strFile.Substring(_rootFolder().Length, strFile.Length - _rootFolder().Length);
                strFilter = strFilter.Substring(0, strFilter.Length - Path.GetFileName(strFile).Length);
                if (strFilter.EndsWith("\\"))
                {
                    strFilter = strFilter.Substring(0, strFilter.Length - 1);
                }
                string[] strFiltersPerDirectory = strFilter.Split('\\');
                strFilter = "";
                foreach (string strFilterPerFolder in strFiltersPerDirectory)
                {
                    if (strFilterPerFolder == "")
                    {
                        continue;
                    }
                    strFilter = strFilter + strFilterPerFolder;
                    
                    if (mFilters.Contains(strFilter) == false)
                    {

                        strReturnString = strReturnString + "    <Filter Include=\"" + strFilter + "\" />" + Environment.NewLine;
                        //strReturnString = strReturnString + "      <Extensions>cpp;c;cc;cxx;def;odl;idl;hpj;bat;asm;asmx</Extensions>" + Environment.NewLine;
                        //strReturnString = strReturnString + "    </Filter>" + Environment.NewLine;
                        mFilters.Add(strFilter);
                    }
                    strFilter = strFilter + "\\";
                }
            }
            strReturnString = strReturnString + "</" + commandLineArguments.getCommandValueAsString("-FilterTag") + ">" + Environment.NewLine;
            return strReturnString;
        }

        public string _buildTags()
        {
            string strReturnStringHeaders = "<" + commandLineArguments.getCommandValueAsString("-FilterTag") + ">" + Environment.NewLine;
            string strReturnStringCode = "<" + commandLineArguments.getCommandValueAsString("-FilterTag") + ">" + Environment.NewLine;
            foreach (string strFullFile in m_FilesFound)
            {
                string strFile = strFullFile.Substring(_rootFolder().Length, strFullFile.Length - _rootFolder().Length);
                string strFilter = strFile;
                strFilter = strFilter.Substring(0, strFilter.Length - Path.GetFileName(strFilter).Length);
                if (strFilter.EndsWith("\\"))
                {
                    strFilter = strFilter.Substring(0, strFilter.Length - 1);
                }
                if (strFile.EndsWith(".h"))
                {
                    strReturnStringHeaders = strReturnStringHeaders + "    <ClInclude Include=\"" + strFile + "\">" + Environment.NewLine;
                    strReturnStringHeaders = strReturnStringHeaders + "      <Filter>" + strFilter + "</Filter>" + Environment.NewLine;
                    strReturnStringHeaders = strReturnStringHeaders + "    </ClInclude>" + Environment.NewLine;
                }
                else
                {
                    strReturnStringCode = strReturnStringCode + "    <ClCompile Include=\"" + strFile + "\">" + Environment.NewLine;
                    strReturnStringCode = strReturnStringCode + "      <Filter>" + strFilter + "</Filter>" + Environment.NewLine;
                    strReturnStringCode = strReturnStringCode + "    </ClCompile>" + Environment.NewLine;

                }


            }
            strReturnStringCode = strReturnStringCode + "</" + commandLineArguments.getCommandValueAsString("-FilterTag") + ">" + Environment.NewLine;
            strReturnStringHeaders = strReturnStringHeaders + "</" + commandLineArguments.getCommandValueAsString("-FilterTag") + ">" + Environment.NewLine;
            return strReturnStringCode + strReturnStringHeaders;
        }

    }//end class
}//end namespace
