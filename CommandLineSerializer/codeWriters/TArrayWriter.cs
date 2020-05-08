using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.IO;
using Library.ClassParser;
namespace CommandLineSerializer.codeWriters
{
	public class TArrayWriter : CodeWriter
	{

		public override bool attemptByteWriterCode(HeaderFile mHeaderFile, ClassVariable mVariable)
		{
			string strType = "";
			if(_parseType(mVariable, ref strType) == false ) { return false; }

			
			
			

			string strCountVar = "iCount_" + mVariable.variableName;
			string strLoopVar = "iLoop_" + mVariable.variableName;
			mHeaderFile.addLine("uint16 " + strCountVar + " = (uint16)" + mVariable.variableName + ".Num();");
			mHeaderFile.addLine("mByteWriter << " + strCountVar + ";");
			if (_isPrimitive(mHeaderFile, strType))
			{
				mHeaderFile.addLine("for(uint16 " + strLoopVar + " = 0; " + strLoopVar + " < " + strCountVar + "; " + strLoopVar + "++)");
				mHeaderFile.addLine("{");
				mHeaderFile.addLine("mByteWriter << " + mVariable.variableName + "[" + strLoopVar+ "];");
				mHeaderFile.addLine("}");
			}
			else
			{
				string strTmpVar = "_tmp" + mVariable.variableName;
				ClassVariable mNewVariable = _createNewVariable(mHeaderFile, strTmpVar, strType);
				mHeaderFile.addLine("for(uint16 " + strLoopVar + " = 0; " + strLoopVar + " < " + strCountVar + "; " + strLoopVar + "++)");
				mHeaderFile.addLine("{");
				mHeaderFile.addLine(strType + ((mNewVariable.isPointer)?"": " &") + strTmpVar + " = " + mVariable.variableName + "[" + strLoopVar + "];");
				foreach(CodeWriter mWriter in mHeaderFile.getCodeWriters())
				{
					if(mWriter.attemptByteWriterCode(mHeaderFile, mNewVariable))
					{
						break;
					}
				}				
				mHeaderFile.addLine("}");
			}

			return true;
		}
		public override bool attemptByteReaderCode(HeaderFile mHeaderFile, ClassVariable mVariable)
		{
			string strType = "";
			if (_parseType(mVariable, ref strType) == false) { return false; }
			

			string strCountVar = "iCount_" + mVariable.variableName;
			string strLoopVar = "iLoop_" + mVariable.variableName;
			string strTmpVar = "_tmp" + mVariable.variableName;
			mHeaderFile.addLine("uint16 " + strCountVar + "(0);");
			mHeaderFile.addLine("mByteReader << " + strCountVar + ";");
			mHeaderFile.addLine(mVariable.variableName + ".Reset();");
			mHeaderFile.addLine(mVariable.variableName + ".Reserve(" + strCountVar + ");");
			if (_isPrimitive(mHeaderFile, strType))
			{
				mHeaderFile.addLine(_writePrimitiveTypeInitializer(mHeaderFile, strType, strTmpVar));// + " " + strTmpVar + ";");
				mHeaderFile.addLine("for(uint16 " + strLoopVar + " = 0; " + strLoopVar + " < " + strCountVar + "; " + strLoopVar + "++)");
				mHeaderFile.addLine("{");				
				mHeaderFile.addLine("mByteReader << " + strTmpVar + ";");
				mHeaderFile.addLine(mVariable.variableName + ".Add( " + strTmpVar + ");");
				mHeaderFile.addLine("}");
			}
			else
			{				
				ClassVariable mNewVariable = _createNewVariable(mHeaderFile, strTmpVar, strType);
				mHeaderFile.addLine("for(uint16 " + strLoopVar + " = 0; " + strLoopVar + " < " + strCountVar + "; " + strLoopVar + "++)");
				mHeaderFile.addLine("{");
				if (mNewVariable.isPointer == false)
				{
					mHeaderFile.addLine(mVariable.variableName + ".Add(" + mNewVariable.variableType + "() );");
					mHeaderFile.addLine(strType + " &" + strTmpVar + " = " + mVariable.variableName + ".Last();");
				}
				else
				{
					//just just need to define it.
					mHeaderFile.addLine(strType + " " + strTmpVar + " = nullptr;");
				}								
				foreach (CodeWriter mWriter in mHeaderFile.getCodeWriters())
				{
					if (mWriter.attemptByteReaderCode(mHeaderFile, mNewVariable))
					{
						break;
					}
				}
				if (mNewVariable.isPointer)
				{
					//we need to add it back in.
					mHeaderFile.addLine(mVariable.variableName + ".Add(" + strTmpVar + ");");
				}
				mHeaderFile.addLine("}");
			}
			
			return true;
		}
		public override bool attemptDataGroupWriteCode(HeaderFile mHeaderFile, ClassVariable mVariable, string strDataGroupName)
		{
			string strType = "";
			if (_parseType(mVariable, ref strType) == false) { return false; }
			EnumList mEnum = mHeaderFile.getEnumList(strType);
            
			string strTmpVar = "_tmp_" + mVariable.variableName;
            mHeaderFile.addLine("{"); //start the array scope
            mHeaderFile.addLine("KCDataGroup &mArrayDataGroup = " + strDataGroupName + ".getOrCreateChildGroup(\"" + mVariable.variableName + "\");");
            mHeaderFile.addLine("uint16 iElementCount = (uint16)" + mVariable.variableName + ".Num();");
			mHeaderFile.addLine("mArrayDataGroup.setProperty(\"COUNT\", iElementCount);");

            if (_isPrimitive(mHeaderFile, strType))
			{
                mHeaderFile.addLine("for(uint16 iIndex = 0; iIndex < iElementCount; iIndex++)");
                mHeaderFile.addLine("{");                
                mHeaderFile.addLine("mArrayDataGroup.setProperty( DATATYPES_UTILS::getAsString(iIndex), " + mVariable.variableName + "[iIndex]);");
				mHeaderFile.addLine("}");
			}
			else if (mEnum != null)
			{
                mHeaderFile.addLine("for(uint16 iIndex = 0; iIndex < iElementCount; iIndex++)");
                mHeaderFile.addLine("{");
                mHeaderFile.addLine("mArrayDataGroup.setProperty( DATATYPES_UTILS::getAsString(iIndex), (int32)" + mVariable.variableName + "[iIndex]);");
                mHeaderFile.addLine("}");
			}
			else
			{
                ClassVariable mNewVariable = _createNewVariable(mHeaderFile, strTmpVar, strType);
                mNewVariable.dataataGroupOverride = "DATATYPES_UTILS::getAsString(iIndex)";
                mHeaderFile.addLine("for(uint16 iIndex = 0; iIndex < iElementCount; iIndex++)");
                mHeaderFile.addLine("{");
                mHeaderFile.addLine(strType + ((mNewVariable.isPointer) ? "" : " &") + strTmpVar + " = " + mVariable.variableName + "[iElementCount];");
                //mHeaderFile.addLine("KCDataGroup &mChildObject = mArrayDataGroup.getOrCreateChildGroup(DATATYPES_UTILS::getAsString(iIndex));");
                
                foreach (CodeWriter mWriter in mHeaderFile.getCodeWriters())
                {

                    if (mWriter.attemptDataGroupWriteCode(mHeaderFile, mNewVariable, "mArrayDataGroup"))
                    {
                        break;
                    }
                }
                
                mHeaderFile.addLine("}");
               
			}
          
            mHeaderFile.addLine("}");   //end the array scope
            return true;
		}
		public override bool attemptDataGroupReadCode(HeaderFile mHeaderFile, ClassVariable mVariable, string strDataGroupName)
		{
			string strType = "";
			if (_parseType(mVariable, ref strType) == false) { return false; }			
			EnumList mEnum = mHeaderFile.getEnumList(strType);
            mHeaderFile.addLine("{"); //start the array scope
			string strTmpVar = "_tmp_" + mVariable.variableName;
            mHeaderFile.addLine(mVariable.variableName + ".Reset();");
            mHeaderFile.addLine("const KCDataGroup *pArrayGroup = " + strDataGroupName + ".getChildGroup(\"" + mVariable.variableName + "\");");
            mHeaderFile.addLine("if(pArrayGroup != nullptr)");
            mHeaderFile.addLine("{");   //pArrayGroup check
            mHeaderFile.addLine("int32 iCount = pArrayGroup->getProperty(\"COUNT\", 0);");
            mHeaderFile.addLine("if(iCount > 0)");
            mHeaderFile.addLine("{");   //start count check
			mHeaderFile.addLine(mVariable.variableName + ".Reserve((uint32)iCount);");
			if (_isPrimitive(mHeaderFile, strType))
			{

				mHeaderFile.addLine(_writePrimitiveTypeInitializer(mHeaderFile, strType, strTmpVar));
				mHeaderFile.addLine("for(uint16 iIndex = 0; iIndex < iCount; iIndex++)");
				mHeaderFile.addLine("{");                
                mHeaderFile.addLine(strType + " valueOfSomeType = pArrayGroup->getProperty(DATATYPES_UTILS::getAsString(iIndex), " + strTmpVar + ");");
				mHeaderFile.addLine(mVariable.variableName + ".Add(valueOfSomeType);");
				mHeaderFile.addLine("}");
			}	
			else if(mEnum != null )
			{
				string strEnumTempVar = "_enum" + mVariable.variableName;
				mHeaderFile.addLine(_writePrimitiveTypeInitializer(mHeaderFile, "int32", strEnumTempVar));
                mHeaderFile.addLine("for(uint16 iIndex = 0; iIndex < iCount; iIndex++)");
                mHeaderFile.addLine("{");
                mHeaderFile.addLine(strType + " mEnumValue = (" + strType + ")pArrayGroup->getProperty(DATATYPES_UTILS::getAsString(iIndex), " + strEnumTempVar + ");");                
				mHeaderFile.addLine(mVariable.variableName + ".Add(mEnumValue);");
				mHeaderFile.addLine("}");
			}
			else
			{
				ClassVariable mNewVariable = _createNewVariable(mHeaderFile, strTmpVar, strType);
                mNewVariable.dataataGroupOverride = "DATATYPES_UTILS::getAsString(iIndex)";
                mHeaderFile.addLine("std::cout << \"Found array with count \" << iCount << std::endl;");
                mHeaderFile.addLine("for(uint16 iIndex = 0; iIndex < iCount; iIndex++)");
                mHeaderFile.addLine("{");
                if (mNewVariable.isPointer == false)
                {
                    mHeaderFile.addLine(mVariable.variableName + ".Add(" + mNewVariable.variableType + "() );");
                    mHeaderFile.addLine(strType + " &" + strTmpVar + " = " + mVariable.variableName + ".Last();");
                }
                else
                {
                    //just just need to define it. ALSO WEIRD but was easier to code with the type having the *. So type here is actually "SomeClass*"
                    mHeaderFile.addLine(strType + " " + strTmpVar + " = nullptr;");
                    //not this gets filled out in mWriter.attemptDataGroupReadCode
                }

                //mHeaderFile.addLine("const KCDataGroup *pChildAsPointer = pArrayGroup->getChildGroup(DATATYPES_UTILS::getAsString(iIndex));");
                //mHeaderFile.addLine("if(pChildAsPointer != nullptr)");
                //mHeaderFile.addLine("{");
                mHeaderFile.addLine("const KCDataGroup &mArrayGroupRef = *pArrayGroup;");
                foreach (CodeWriter mWriter in mHeaderFile.getCodeWriters())
                {
                    if (mWriter.attemptDataGroupReadCode(mHeaderFile, mNewVariable, "mArrayGroupRef"))
                    {
                        break;
                    }
                }
                if (mNewVariable.isPointer) //pointer gets filled out in mWriter.attemptDataGroupReadCode
                {
                    //we need to add it back in.
                    mHeaderFile.addLine(mVariable.variableName + ".Add(" + strTmpVar + ");");
                }
                //mHeaderFile.addLine("}");//end check for pointer group
//                 if (mNewVariable.isPointer) //if it's a pointer it could be null so we need to add it back in.
//                 {
//                     mHeaderFile.addLine("else");
//                     mHeaderFile.addLine("{");
//                     //we need to add it back in but it's null
//                     mHeaderFile.addLine(mVariable.variableName + ".Add(nullptr);");
//                     mHeaderFile.addLine("}");
//                 }
                mHeaderFile.addLine("}"); //end for loop
			}
            mHeaderFile.addLine("}");   //end count check
            mHeaderFile.addLine("}");   //end pArrayGroup check
            mHeaderFile.addLine("}");   //end the array scope*/
            return true;
		}

		private bool _parseType(ClassVariable mVariable, ref string strType )
		{
			if( mVariable.variableType.Contains("TArray<") == false)
			{
				return false;
			}
			strType = mVariable.variableType;
			int iIndexOf = strType.IndexOf("<");
			strType = strType.Substring(iIndexOf + 1, strType.Length - iIndexOf - 2);
			
			return true;
		}

		private bool _isPrimitive(HeaderFile mHeaderFile, string strType)
		{
			if( mHeaderFile.getTypeDefs().ContainsKey(strType.ToLower()))
			{
				return true;
			}
			return false;
		}
		private string _writePrimitiveTypeInitializer(HeaderFile mHeaderFile, string strType, string varName)
		{
			string strTypeLower = strType.ToLower();
			if (mHeaderFile.getTypeDefs().ContainsKey(strTypeLower))
			{
				switch(mHeaderFile.getTypeDefs()[strTypeLower])
				{
					default:
					case ESERIALIZE_DATA_TYPE.NUMBER:
						return strType + " " + varName + "(0);";
					case ESERIALIZE_DATA_TYPE.STRING:
						return strType + " " + varName + "(\"\");";
						

				}
			}
			return strType + " " + varName + "(0);";
		}
		private ClassVariable _createNewVariable(HeaderFile mHeaderFile, string strVariableName, string strType)
		{
			ClassVariable mVariable = new ClassVariable();
			mVariable.variableName = strVariableName;
			if (strType.EndsWith("*"))
			{
				strType = strType.Substring(0, strType.Length - 1).Trim(); ;
				mVariable.isPointer = true;
			}
			mVariable.variableType = strType;
			return mVariable;
		}

	}
}
