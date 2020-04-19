using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Library.IO;
namespace Library.ClassParser.Private
{
	public class ClassParser
	{
		private string m_strLastLineParsing = "";
		private int m_iLastLineIndex = 0;
		private List<ClassStructure> m_StuctsOrClassesProcessing = new List<ClassStructure>();
		public ClassParser(string strFile)
		{
			inComment = false;
			fileParsing = strFile;
			classStructures = new List<ClassStructure>();
			enumLists = new List<EnumList>();
			defines = new Dictionary<string, string>();
		}



		public LogFile logFile { set; get; }

		public string fileParsing { get; set; }

		public List<ClassStructure> classStructures { get; set; }

		public List<EnumList> enumLists { get; set; }
		public Dictionary<string, string> defines { get; set; }

		private ClassStructure pushClassOn()
		{
			m_StuctsOrClassesProcessing.Add(new ClassStructure());
			
			return getCurrentStructure();
		}

		private void popClassOff()
		{
			if (m_StuctsOrClassesProcessing.Count == 0)
			{
				return;
			}
			ClassStructure mClassStructure = m_StuctsOrClassesProcessing.Last();
			mClassStructure.file = fileParsing;
			m_StuctsOrClassesProcessing.RemoveAt(m_StuctsOrClassesProcessing.Count - 1);
			classStructures.Add(mClassStructure);
		}

		private ClassStructure getCurrentStructure()
		{
			if (m_StuctsOrClassesProcessing.Count == 0)
			{
				return null;
			}
			return m_StuctsOrClassesProcessing[m_StuctsOrClassesProcessing.Count - 1];

		}

		private void log(string strMessage)
		{
			if (logFile != null)
			{
				logFile.log(strMessage);
			}
		}

		public bool parse(List<string> mErrors)
		{
			try
			{



				string strFileContents = File.ReadAllText(fileParsing);
				if (strFileContents == null ||
					strFileContents == "")
				{
					log("Empty File:" + fileParsing);
					return false;
				}
				if( fileParsing.Contains("KCIncludeTest"))
				{
					log("found it");
				}

				StringReader mStringReader = new StringReader(strFileContents);

				_parse(mStringReader);


				string strStructsAndClassesFound = "";
				foreach (ClassStructure mStruct in classStructures)
				{
					strStructsAndClassesFound = strStructsAndClassesFound + mStruct.name + ",";
				}
				if (strStructsAndClassesFound != "")
				{
					strStructsAndClassesFound = strStructsAndClassesFound.Substring(0, strStructsAndClassesFound.Length - 1);
				}
				log("Classes/Structures found in header file: " + Path.GetFileName(fileParsing) + " " + classStructures.Count.ToString() + "(" + strStructsAndClassesFound + ")");				
				return (classStructures.Count > 0 || enumLists.Count > 0 || defines.Count > 0 )?true:false;
			}
			catch (Exception e)
			{
				log("ERROR - parsing error in file: " + fileParsing + Environment.NewLine + "Last line read was: ln:" +m_iLastLineIndex.ToString() + m_strLastLineParsing + Environment.NewLine + "Error was - " + e.Message + ((e.InnerException != null) ? e.InnerException.Message : ""));
			}
			return false;
		}
		private bool inComment { get; set; }

		private string unrealTag { get; set; }

		private bool isParsingClassOrStructure { get { return m_StuctsOrClassesProcessing.Count > 0; } }

		private bool isParsingPrivateVariables { get; set; }

		private string currentComment { get; set; }

		private int bracketCount { get; set; }

		private bool inNameSpace { get; set; }

		private bool inFunction 
		{
			get
			{
				int iBracketTestCase = ((inNameSpace) ? 1 : 0) + m_StuctsOrClassesProcessing.Count;
				return (bracketCount > iBracketTestCase);
			}
		}

		private void _parse(StringReader mStringReader)
		{
			unrealTag = "";
			bracketCount = 0;
			m_iLastLineIndex = -1;
			m_strLastLineParsing = "";

			while (true)
			{
				m_iLastLineIndex++;
				string strLine = mStringReader.ReadLine();
				if( strLine == null )
				{
					m_strLastLineParsing = "";					
					return; //done
				}
				m_strLastLineParsing = strLine;
				_cleanLine(ref strLine);
				if (_easyToHandle(ref strLine))
				{
					currentComment = "";
					continue;
				}
				if( _handleNameSpace(ref strLine))
				{
					continue;
				}
				if (_handleCommentLine(ref strLine))
				{
					continue;
				}

				if(_handleBracket(ref strLine))
				{
					continue;
				}
				if (_handleDefinesAndStatics(ref strLine))
				{
					continue;
				}
				if (_parseEnum(ref strLine, ref mStringReader))
				{
					continue;
				}
				if(_handleFunctions(ref strLine, ref mStringReader))
				{
					continue;
				}

				if (_attemptToFindClassOrStructure(ref strLine))
				{
					continue;
				}
				if(isParsingClassOrStructure == false )
				{
					continue;
				}
				if(_checkSerialized(ref strLine))
				{
					continue;
				}
				if(_handlePublicProtectedOrPrivate(ref strLine))
				{
					continue;
				}
				if (_handleUE4Property(ref strLine))
				{
					continue;
				}

				if (_handleMacro(ref strLine))
				{
					continue;
				}

				if(_handleVariableParsing(ref strLine))
				{
					continue;
				}

			}
		}

		private bool _handleFunctions(ref string strLine, ref StringReader mStringReader)
		{
			int iIndexOfBracket = strLine.IndexOf("(");
			if (iIndexOfBracket < 0 )
			{
				return false;
			}
			
			bool bSpaceFound = false;
			bool bIsFunction = false;
			for( int iCountBackwards = iIndexOfBracket; iCountBackwards >= 0; iCountBackwards--)
			{
				if (bSpaceFound == false)
				{
					if (strLine[iCountBackwards] == ' ')
					{
						bSpaceFound = true;
					}
				}
				else if( strLine[iCountBackwards] != ' ')
				{
					bIsFunction = true;
					break;
				}
			}
			if( bIsFunction == false )
			{
				return false;
			}
			strLine = strLine.Trim();
			if ( strLine.EndsWith("}"))
			{
				return true;	//something like bool isValid(){return true;}
			}
			if (strLine.EndsWith(")"))
			{
				return true;    //something like bool isValid()
			}
			if (strLine.EndsWith(",") ||
				strLine.EndsWith("(") )
			{
				while (true)
				{
					strLine = mStringReader.ReadLine();

					if( strLine == null)
					{
						log("Error - unable to determin function. End of file hit in parsing file: " + fileParsing);
						return true;
					}
					_cleanLine(ref strLine);
					_handleCommentLine(ref strLine);

					if (strLine.Trim().Contains(")"))
					{
						return true;	//we are done.
					}

				};
				
			}

			return false;
		}

		private bool _handlePublicProtectedOrPrivate(ref string strLine)
		{
			if(strLine.StartsWith("public:") ||
			   strLine.StartsWith("protected:"))
			{
				isParsingPrivateVariables = false;
				return true;
			}
			else if(strLine.StartsWith("private:"))
			{
				isParsingPrivateVariables = true;
				return true;
			}
			else if(strLine.EndsWith(":"))
			{
				//usually a label
				if(getCurrentStructure() != null)
				{
					getCurrentStructure().labels.Add(strLine.Substring(0, strLine.Length-1));
				}
				return true;
			}
			return false;
		}


		private bool _handleNameSpace(ref string strLine)
		{
			if(strLine.StartsWith("namespace"))
			{
				inNameSpace = true;
				return true;
			}
			return false;
		}
		private bool _handleBracket(ref string strLine)
		{
			int iBracketTestCase = ((inNameSpace)?1:0) + m_StuctsOrClassesProcessing.Count;
			for (int iIndex = 0; iIndex < strLine.Length; iIndex++)
			{
				if (strLine[iIndex] == '{')
				{
					bracketCount++;
				}
				else if (strLine[iIndex] == '}')
				{
					bracketCount--;
					if (bracketCount < iBracketTestCase)
					{
						if (getCurrentStructure() != null)
						{
							popClassOff();
						}
						else if (inNameSpace)
						{
							inNameSpace = false;
						}						
					}					
				}
			}
			if( strLine.Length == 1 &&
				(strLine[0] == '{' ||
				strLine[0] == '}') )
			{
				return true;
			}
			if (strLine.Length > 1 &&
				strLine[strLine.Length - 1] == ';' &&
				strLine[strLine.Length - 2] == '}')
			{
				return true;
			}
			
			
			return false;
		}

		private void _cleanLine(ref string strLine)
		{
			strLine = strLine.Replace("\t", " ");
			strLine = strLine.Replace("\r", "");
			strLine = strLine.Replace("\n", "");
			strLine = strLine.Trim();
		}

		private bool _attemptToFindClassOrStructure(ref string strLine)
		{
			if( inFunction)
			{
				return false;
			}
			if (strLine.StartsWith("class") ||
				strLine.StartsWith("struct"))
			{
				if(strLine.EndsWith(";"))
				{
					return true;	//it's a class definition. for instance: class TestClass;
				}



				ClassStructure mStruct = pushClassOn();
				mStruct.comment = currentComment;
				
				if (strLine.StartsWith("class"))
				{
					isParsingPrivateVariables = true;
					mStruct.isClass = true;
					strLine = strLine.Substring(6, strLine.Length - 6); //include space
				}
				else
				{
					strLine = strLine.Substring(7, strLine.Length - 7); //include space
				}


				//we now have something like KCTest : public KCParent
				strLine = strLine.Replace(":", "");
				strLine = strLine.Replace("public", "");
				strLine = strLine.Replace("private", "");
				while(strLine.Contains("  "))
				{
					strLine = strLine.Replace("  ", " ");
				}
				_cleanLine(ref strLine);
				string[] classes = strLine.Split(' ');
				if( classes == null ||
					classes.Count() == 0)
				{
					log("ERROR - in parsing line: " + m_strLastLineParsing + " in file: " + fileParsing);
					mStruct.name = m_strLastLineParsing;
					return true; 
				}
				if (strLine.EndsWith(","))  //error classes have to be on the same line
				{
					log("ERROR - classes or structs extending must be on the same line. parsing line: " + m_strLastLineParsing + " in file: " + fileParsing);
					mStruct.name = m_strLastLineParsing;
					return true;
				}
				mStruct.name = classes[0];
				for( int iIndex = 1; iIndex < classes.Count(); iIndex++)
				{
					mStruct.classStructuresInheritingFrom.Add(classes[iIndex]);
				}
				currentComment = "";
				return true;
			}

			return false;
		}

	

		private bool _easyToHandle(ref string strLine)
		{
			if (strLine.Length == 0)
			{
				currentComment = "";
				return true;
			}

			if (strLine.StartsWith("~") ||				
				strLine.StartsWith("TypeDef") )
			{
				return true;
			}
			
			
			return false;
		}

		private bool _handleCommentLine(ref string strLine)
		{

			if (inComment)
			{
				int iIndexOf = strLine.IndexOf("*/");
				if (iIndexOf > 0)
				{
					inComment = false;
					iIndexOf += 2;					
					currentComment = currentComment + strLine.Substring(0, iIndexOf) + Environment.NewLine;
					strLine = strLine.Substring(iIndexOf, strLine.Length - iIndexOf).Trim();
					if (strLine == "")
					{
						return true;
					}
					return false;
				}
				currentComment = currentComment + strLine + Environment.NewLine;
				return true;
			}
			int iIndexOfCommentStart = strLine.IndexOf("/*");
			int iIndexOfCommentEnd = -1;
			if (iIndexOfCommentStart >= 0)
			{
				iIndexOfCommentEnd = strLine.IndexOf("*/");
				if (iIndexOfCommentEnd < 0)
				{
					inComment = true;
				}
			}
			else
			{
				iIndexOfCommentStart = strLine.IndexOf("//");
			}

			if (iIndexOfCommentStart < 0)
			{
				return false;
			}
			if (iIndexOfCommentEnd < 0 ||
				iIndexOfCommentEnd > strLine.Length)
			{
				iIndexOfCommentEnd = strLine.Length;
			}
			if (currentComment == "")
			{
				currentComment = currentComment + strLine.Substring(iIndexOfCommentStart, iIndexOfCommentEnd - iIndexOfCommentStart);
			}
			else
			{
				currentComment = currentComment + strLine.Substring(iIndexOfCommentStart, iIndexOfCommentEnd - iIndexOfCommentStart) + Environment.NewLine;
			}
			strLine = strLine.Remove(iIndexOfCommentStart, iIndexOfCommentEnd - iIndexOfCommentStart);
			if (strLine.Length == 0)
			{
				return true;
			}
			return false;
		}

		private bool _handleMacro(ref string strLine)
		{
			bool bIsUProperty = strLine.Contains("UPROPERTY") || strLine.Contains("KCPROPERTY");
			if(inFunction)
			{
				return false;
			}
			if (getCurrentStructure() == null)
			{
				return false;
			}
			if(strLine.StartsWith(getCurrentStructure().name))
			{
				return true;
			}
			for( int iIndex = 0; iIndex < strLine.Length; iIndex++)
			{
				if( strLine[iIndex] == ' ' ||
					strLine[iIndex] == '"' ||
					strLine[iIndex] == '\'')
				{
					return false;
				}
				if(strLine[iIndex] == '(')
				{
					if (getCurrentStructure() != null)
					{
						getCurrentStructure().unknownMacros.Add(strLine);
					}
					return true;    //this is basically something like THIS_IS_A_MACRO( "df");
				}
			}
			//if we made it here there are no spaces, no nothin - so probably a macro
			if (getCurrentStructure() != null)
			{
				getCurrentStructure().unknownMacros.Add(strLine);
			}
			return true;    //this is basically something like THIS_IS_A_MACRO( "df");

		}



		
		private bool _handleUE4Property(ref string strLine)
		{
			if (strLine.StartsWith("UPROPERTY") == false &&
				strLine.StartsWith("KCPROPERTY") == false)
			{
				return false;
			}
			if (strLine.Contains("TRANSIENT") ||
				strLine.Contains("EDITOR_IGNORE"))
			{
				return true; //we don't do anything with this uproperty
			}
			//make my life a little easier, lets remove some of the properties
			unrealTag = strLine;
			return true;

		}
		
		private bool _handleVariableParsing(ref string strLine)
		{
			return _handleVariableParsing(ref strLine, null);
		}
		private bool _handleVariableParsing(ref string strLine, ClassVariable mVariable)
		{
			if (getCurrentStructure() == null ||
				inFunction)
			{
				return false;
			}
			if (strLine.Contains("(") || //function
				strLine[strLine.Length - 1] != ';') //it's not a variable
			{
				return false;
			}

			string strVariableLine = strLine.Substring(0, strLine.Length - 1); //removes the ';' at the end
			string strValueSet = "";
			int iIndexOfEqual = strVariableLine.IndexOf("=");
			if (iIndexOfEqual > 0)
			{
				strValueSet = strVariableLine.Substring(iIndexOfEqual + 1, strVariableLine.Length - iIndexOfEqual - 1).Trim(); 
				strVariableLine = strVariableLine.Substring(0, iIndexOfEqual).Trim();
			}

			bool bIsConst = false;
			int iConstIndex = strVariableLine.IndexOf("const ");
			if(iConstIndex >= 0 &&
				( iConstIndex == 0 || strVariableLine[iConstIndex - 1] == ' ') )
			{
				bIsConst = true;
				strVariableLine = strVariableLine.Replace("const ", "");
			}
			bool bIsStatic = false;
			int iStaticIndex = strVariableLine.IndexOf("static ");
			if (iStaticIndex >= 0 &&
				(iStaticIndex == 0 || strVariableLine[iStaticIndex - 1] == ' '))
			{
				bIsStatic = true;
				strVariableLine = strVariableLine.Replace("static ", "");
			}
			bool bIsPointer = false;
			int iIndexOfPointer = strVariableLine.LastIndexOf("*");
			if(iIndexOfPointer > 0)
			{
				int iIndexOfLessThan = strVariableLine.IndexOf("<");
				int iIndexOfGreaterThan = strVariableLine.LastIndexOf(">");
				if (iIndexOfPointer > iIndexOfLessThan &&
					iIndexOfPointer < iIndexOfGreaterThan)
				{
				}
				else
				{
					bIsPointer = true;
					strVariableLine = strVariableLine.Replace("*", "");
				}
			}
			while ( strVariableLine.Contains("  "))
			{
				strVariableLine = strVariableLine.Replace("  ", " ");
			};
			strVariableLine = strVariableLine.Trim();
			//we should now be down to "type variableName"
			int iLastIndexOf = strVariableLine.LastIndexOf(' ');
			if(iLastIndexOf < 0 )
			{
				return false;
			}
			string strVariableType = strVariableLine.Substring(0, iLastIndexOf).Trim();
			string strVariableName = strVariableLine.Substring(iLastIndexOf + 1, strVariableLine.Length - iLastIndexOf - 1).Trim();
			if(strVariableType == "" ||
				strVariableName == "" )
			{
				return false;
			}

			if(mVariable == null)
			{
				mVariable = new ClassVariable();
			}
			mVariable.isConst = bIsConst;
			mVariable.isStatic = bIsStatic;
			mVariable.isPointer = bIsPointer;
			mVariable.isPrivateVariable = isParsingPrivateVariables;
			mVariable.variableType = strVariableType;
			mVariable.variableName = strVariableName;
			mVariable.variableValue = strValueSet;
			mVariable.variableComment = currentComment;
			mVariable.lineNumber = m_iLastLineIndex - 1;
			if ( unrealTag != null &&
				unrealTag != "")
			{
				_parseUE4UpropertyLine(unrealTag, mVariable);
				unrealTag = "";
				mVariable.isSerialized = true;
			}
			mVariable.category = (mVariable.variableProperties.ContainsKey("CATEGORY")) ? mVariable.variableProperties["CATEGORY"] : "";
			mVariable.displayName = (mVariable.variableProperties.ContainsKey("DISPLAYNAME")) ? mVariable.variableProperties["DISPLAYNAME"] : mVariable.variableName;
			getCurrentStructure().variables.Add(mVariable);
			currentComment = "";

			return true;
		}

		private void _parseProperties( string strLine, Dictionary<string, string> mProperties)
		{
			strLine = strLine.Replace("UPROPERTY", "");
			strLine = strLine.Replace("KCPROPERTY", "");
			strLine = strLine.Replace("UMETA", "");
			strLine = strLine.Replace("(", "");
			strLine = strLine.Replace(")", "");
			strLine = strLine.Replace("meta=", "");
			strLine = strLine.Replace("meta =", "");
			bool bInDoubleQuote = false;
			char[] mCharArray = new char[strLine.Length];
			Array.Clear(mCharArray, 0, strLine.Length);
			int iCharArrayIndex = 0;
			int iLastIndexOfLine = strLine.Length - 1;
			//the idea here is that we parse 
			//Category = AbilitySystem, VisibleAnywhere, BlueprintReadOnly, AllowPrivateAccess = "true"
			//into
			//Category=AbilitySystem VisibleAnywhere BlueprintReadOnly AllowPrivateAccess="true"
			//where spaces not in double quotes are \n
			for (int iIndexOfChar = 0; iIndexOfChar < strLine.Length; iIndexOfChar++)
			{

				if (strLine[iIndexOfChar] == '"')
				{
					bInDoubleQuote = !bInDoubleQuote;
				}
				if (bInDoubleQuote)
				{
					mCharArray[iCharArrayIndex++] = strLine[iIndexOfChar];
					continue;
				}
				if (strLine[iIndexOfChar] == ',')
				{
					continue;
				}
				if (strLine[iIndexOfChar] == ' ')
				{
					if (iIndexOfChar != 0)
					{
						if (strLine[iIndexOfChar - 1] == '=') //previous char was a equal. Lets just skip this space.
						{
							continue;
						}
						if (strLine[iIndexOfChar - 1] == ' ')
						{
							continue;
						}
					}
					if (iIndexOfChar != iLastIndexOfLine)
					{
						if (strLine[iIndexOfChar + 1] == '=') //previous char was a equal. Lets just skip this space.
						{
							continue;
						}
						if (strLine[iIndexOfChar + 1] == ' ')
						{
							continue;
						}
					}
					mCharArray[iCharArrayIndex++] = '\n';
					continue;
				}

				mCharArray[iCharArrayIndex++] = strLine[iIndexOfChar];
			}
			string strNewString = new string(mCharArray).Substring(0, iCharArrayIndex).Trim();
			if (strNewString == "")
			{
				return;
			}
			//now lets split on \n			
			string[] strProperties = strNewString.Split('\n');
			//and lets make pairs here of property = value
			foreach (string strProperty in strProperties)
			{
				if(strProperty == "")
				{
					continue;
				}
				string strPropertyName = "";
				string strPropertyValue = "";
				int iIndexOfDoubleQuote = strProperty.IndexOf('"');
				int iIndexOfEqual = strProperty.IndexOf('=');
				if (iIndexOfDoubleQuote >= 0)
				{
					strPropertyValue = strProperty.Substring(iIndexOfDoubleQuote, strProperty.Length - iIndexOfDoubleQuote);
					strPropertyValue = strPropertyValue.Replace("\"", "");

					strPropertyName = strProperty.Substring(0, iIndexOfDoubleQuote);
					strPropertyName = strPropertyName.Replace("=", "");
				}
				else if (iIndexOfEqual >= 0)
				{
					strPropertyName = strProperty.Substring(0, iIndexOfEqual);
					strPropertyValue = strProperty.Substring(iIndexOfEqual, strProperty.Length - iIndexOfEqual);
					strPropertyValue = strPropertyValue.Replace("=", "");
					strPropertyValue = strPropertyValue.Replace("\"", "");
				}
				else
				{
					strPropertyName = strProperty;
				}
				mProperties[strPropertyName.Trim().ToUpper()] = strPropertyValue;
				
			}

		}

		private void _parseUE4UpropertyLine( string strLine, ClassVariable mVariable)
		{
			//the following lines make 
			//UPROPERTY(Category = AbilitySystem, VisibleAnywhere, BlueprintReadOnly, meta = (AllowPrivateAccess = "true"))
			//into
			//Category = AbilitySystem, VisibleAnywhere, BlueprintReadOnly, AllowPrivateAccess = "true"
			mVariable.isUE4Variable = true;
			strLine = strLine.Replace("UPROPERTY", "");
			strLine = strLine.Replace("KCPROPERTY", "");
			strLine = strLine.Replace("(", "");
			strLine = strLine.Replace(")", "");
			strLine = strLine.Replace("meta=", "");
			strLine = strLine.Replace("meta =", "");
			_cleanLine(ref strLine);
			if( strLine.Length == 0 ||
				mVariable == null)
			{
				return;	//just simply a UPROPERTY()
			}
			
			_parseProperties(strLine, mVariable.variableProperties);
			//mVariable.variableProperties[strPropertyName.Trim().ToUpper()] = strPropertyValue;

		}

		private bool _parseEnum(ref string strLine, ref StringReader mStringReader)
		{
			if (strLine.StartsWith("enum ") == false)
			{
				return false;
			}
			
			int iIndexOfEndBracket = strLine.IndexOf('}');
			if( iIndexOfEndBracket > 0 )
			{
				log("Error - Unable to parse enum. Needs to be multiple lined Enum: " + m_strLastLineParsing + "   In File: " + fileParsing);
				return true;
			}
			int iIndexOfBracket = strLine.IndexOf('{');
			if ( iIndexOfBracket > 0)
			{
				log("Error - Unable to parse enum. Needs to be multiple lined Enum: " + m_strLastLineParsing + "   In File: " + fileParsing);
				return true;
			}
			string strType = "";
			int iTypeDefined = strLine.IndexOf(':');
			if( iTypeDefined > 0 &&
				iTypeDefined != strLine.Length - 1)
			{
				strType = strLine.Substring(iTypeDefined + 1, strLine.Length - iTypeDefined - 1);
				strLine = strLine.Substring(0, iTypeDefined);
				strLine = strLine.Trim();
			}
			strLine = strLine.Replace("enum ", "");
			strLine = strLine.Replace("class ", "");
			strLine = strLine.Trim();
			EnumList mEnum = new EnumList();
			mEnum.file = fileParsing;
			enumLists.Add(mEnum);
			mEnum.enumName = strLine;
			mEnum.comment = currentComment;
			currentComment = "";
			while(true)
			{
				strLine = mStringReader.ReadLine();
				if(strLine == null )
				{
					log("Error - parsing enum. " + mEnum.enumName + " in file " + fileParsing);
					return true;
				}
				_cleanLine(ref strLine);
				_handleCommentLine(ref strLine);
				strLine = strLine.Trim();
				if ( strLine == "" ||
					strLine == "{" )
				{
					continue;
				}
				
				if( strLine.EndsWith(";"))
				{					
					return true;
				}

				int iEnumItemNameIndex = strLine.Length;
				for(int iIndex = 0; iIndex < strLine.Length; iIndex++)
				{
					if(strLine[iIndex] == ' ' ||
						strLine[iIndex] == ',' ||
						strLine[iIndex] == '=')
					{
						iEnumItemNameIndex = iIndex;
						break;
					}
				}
				if(iEnumItemNameIndex == -1)
				{
					continue;
				}
				
				string strNameOfEnumItem = strLine.Substring(0, iEnumItemNameIndex).Trim();
				if (strNameOfEnumItem != "")
				{
					int iEnumIndex = mEnum.enumItems.Count;
					string strRemaining = strLine.Substring(iEnumItemNameIndex, strLine.Length - iEnumItemNameIndex - ((strLine.EndsWith(",")) ? 1 : 0));
					int iFirstCharacterFound = -1;
					int iSecondCharacterFound = -1;
					bool bEqualFound = false;
					for (int iIndex = 0; iIndex < strRemaining.Length; iIndex++)
					{
						if (bEqualFound == false)
						{
							if (strRemaining[iIndex] == '=')
							{
								bEqualFound = true;
							}
							else if(strRemaining[iIndex] != ' ')
							{
								break;
							}
						}
						else
						{
							if (iFirstCharacterFound < 0)
							{
								if (strRemaining[iIndex] != ' ')
								{
									iFirstCharacterFound = iIndex;
								}
							}
							else if(iSecondCharacterFound < 0)
							{
								if (strRemaining[iIndex] == ' ')
								{
									iSecondCharacterFound = iIndex;
									break;
								}
							}
						}
					}
					if( iFirstCharacterFound > 0 &&
						iSecondCharacterFound > 0 )
					{
						string strValue = strRemaining.Substring(iFirstCharacterFound, iSecondCharacterFound - iFirstCharacterFound);
						strRemaining = strRemaining.Substring(iSecondCharacterFound, strRemaining.Length - iSecondCharacterFound);
						if (strValue != "")
						{
							if( Int32.TryParse(strValue.Trim(), out int iResult) )
							{
								iEnumIndex = iResult;
							}

						}
					}
					
					EnumItem mItem = new EnumItem();
					mEnum.enumItems.Add(mItem);
					mItem.name = strNameOfEnumItem;
					mItem.comment = currentComment;
					mItem.value = iEnumIndex;
					currentComment = "";
					iEnumItemNameIndex++;
					if (iEnumItemNameIndex < strLine.Length - 2)
					{

						_parseProperties(strRemaining, mItem.properties); //removes comma
					}

				}


			}
			

			
		}

		private bool _checkSerialized(ref string strLine)
		{
			if(getCurrentStructure() != null&&
				strLine.Contains("SERIALIZE_CODE"))
			{
				getCurrentStructure().isSerialized = true;
			}
			return false;
		}

		private bool _handleDefinesAndStatics(ref string strLine)
		{
			if(getCurrentStructure() != null)
			{
				return false;
			}
			if(strLine.StartsWith("#"))
			{
				if( strLine.StartsWith("#define") == false)
				{
					return true;//it's not a define but it's also not a valid line
				}

				if (strLine.Contains('('))
				{
					return false;	//this is a macro
				}
				strLine = strLine.Substring(7, strLine.Length - 7).Trim();  //removes #define
				int iFirstSpace = strLine.IndexOf(' ');
				if( iFirstSpace < 0)
				{
					if (defines.ContainsKey(strLine.ToUpper()) == false)
					{
						defines[strLine.ToUpper()] = "";
					}
				}
				else
				{
					string strName = strLine.Substring(0, iFirstSpace).Trim();
					iFirstSpace++;
					string strValue = strLine.Substring(iFirstSpace, strLine.Length - iFirstSpace).Trim();
					defines[strName.ToUpper()] = strValue; 
				}
				return true;
			}
			if(strLine.Contains("static") )
			{
				if (strLine.EndsWith(";") == false ||
					strLine.Contains('{') ||
					strLine.Contains('['))	//it's an array
				{
					return false;   //static defines must end with ; and can't be a function {}
				}
				string strValue = strLine.Substring(0, strLine.Length - 1 ); //removes ;
				
				while (true)
				{
					if (strValue.StartsWith("static")) { strValue = strValue.Substring(6, strValue.Length - 6).Trim(); continue; }
					if (strValue.StartsWith("const")) { strValue = strValue.Substring(5, strValue.Length - 5).Trim(); continue; }
					break;
				};
				//so now we should have Type, variable and value KCString EMPTY_STRING = ""
				//lets just drop the type
				int iFirstSpace = strValue.IndexOf(' ');
				if( iFirstSpace < 0)
				{
					//don't even know what to think.
					log("ERROR - attempting to parse line " + strLine + " for a static value but failed.");
					return false;
				}
				iFirstSpace++;
				strValue = strValue.Substring(iFirstSpace, strValue.Length - iFirstSpace).Trim();
				int iEqualIndex = strValue.IndexOf('=');
				if( iEqualIndex < 0)
				{
					int iParenIndex = strValue.IndexOf('(');
					if (iParenIndex < 0)
					{
						//this is something like static int32 ANY;						
						return false;
					}
					iEqualIndex = iParenIndex;
					strValue = strValue.Substring(0, strValue.Length - 1);	//removes ) at the end
				}
				if ( iEqualIndex > 0)
				{
					string strName = strValue.Substring(0, iEqualIndex).Trim();
					iEqualIndex++;
					strValue = strValue.Substring(iEqualIndex, strValue.Length - iEqualIndex).Trim();
					if(strValue.Contains("\"") == false &&
						strValue.IndexOf(' ') >= 0 )
					{
						return false;   //this is static KCString &  getDataTypeName(EDATATYPES eType);
					}
					defines[strName.ToUpper()] = strValue;
					return true;
				}
				log("ERROR - attempting to parse line " + strLine + " for a static value but failed.");
				return false;
			}
			return false;
		}


	}//end of class
}//end of namespace
