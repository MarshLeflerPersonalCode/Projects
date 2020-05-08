using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.IO;
using Library.ClassParser;

namespace CommandLineSerializer.codeWriters
{
	public class ClassOrStructWriter : CodeWriter
	{


		public override bool attemptByteWriterCode(HeaderFile mHeaderFile, ClassVariable mVariable) 
		{
			if (_isPrimaryVariableType(mHeaderFile, mVariable))
			{
				return false;
			}
			ClassStructure mVariableClassStruct = getClassStructure(mHeaderFile, mVariable);
			if (mVariableClassStruct == null)
			{
				return false;
			}
            mHeaderFile.addClassRef(mVariableClassStruct);
			if( mVariable.isPointer)
			{
				mHeaderFile.addLine("if(" + mVariable.variableName + " != nullptr)");
				mHeaderFile.addLine("{");
                mHeaderFile.addLine("mByteWriter << " + mVariable.variableName + "->getClassName();");
                mHeaderFile.addLine(mVariable.variableName + "->serialize(mByteWriter);");
				mHeaderFile.addLine("}");
                mHeaderFile.addLine("else");
                mHeaderFile.addLine("{");
                mHeaderFile.addLine("mByteWriter << EMPTY_KCSTRING;");
                mHeaderFile.addLine("}");
            }
			else
			{                
                mHeaderFile.addLine(mVariable.variableName + ".serialize(mByteWriter);");
			}
            
			return true; 
		}
		public override bool attemptByteReaderCode(HeaderFile mHeaderFile, ClassVariable mVariable)
		{
			if (_isPrimaryVariableType(mHeaderFile, mVariable))
			{
				return false;
			}
			ClassStructure mVariableClassStruct = getClassStructure(mHeaderFile, mVariable);
			if (mVariableClassStruct == null)
			{
				return false;
			}
            
            
            
            

            if (mVariable.isPointer)
            {
                mHeaderFile.addLine("{"); //start scope
                mHeaderFile.addLine("KCString strClassName;");
                mHeaderFile.addLine("mByteReader << strClassName;");
                mHeaderFile.addLine("DELETE_SAFELY(" + mVariable.variableName + ");");
                mHeaderFile.addLine("if(strClassName != EMPTY_KCSTRING)");
                mHeaderFile.addLine("{");
                mHeaderFile.addLine(mVariable.variableName + " = (" + mVariableClassStruct.name + " *)_SERIALIZE_CLASS_CREATION_::createObject(strClassName, \"" + mVariableClassStruct.name + "\");");
                mHeaderFile.addLine("if(" + mVariable.variableName + " != nullptr)");
                mHeaderFile.addLine("{");
                mHeaderFile.addLine(mVariable.variableName + "->deserialize(mByteReader);");
                mHeaderFile.addLine("}");
                mHeaderFile.addLine("else");
                mHeaderFile.addLine("{");
                mHeaderFile.addLine("std::cout << \"ERROR IN CREATING CLASS \" << strClassName << std::endl;");
                mHeaderFile.addLine("}");
                mHeaderFile.addLine("}"); //end string test
                mHeaderFile.addLine("}"); //end scope

            }
			else
			{
                mHeaderFile.addLine(mVariable.variableName + " = " + mVariable.variableType + "();");
                mHeaderFile.addLine(mVariable.variableName + ".deserialize(mByteReader);");
			}
            
			return true;
		}
		public override bool attemptDataGroupWriteCode(HeaderFile mHeaderFile, ClassVariable mVariable, string strDataGroupName)
		{
			if (_isPrimaryVariableType(mHeaderFile, mVariable))
			{
				return false;
			}
			ClassStructure mVariableClassStruct = getClassStructure(mHeaderFile, mVariable);
			if (mVariableClassStruct == null)
			{
				return false;
			}
            string strDataGroupOverride = (mVariable.dataataGroupOverride != "") ? mVariable.dataataGroupOverride : "\"mVariable.variableName\"";
            mHeaderFile.addLine("{"); //start object scope
            if(mVariable.isPointer)
            {
                mHeaderFile.addLine("if(" + mVariable.variableName + " != nullptr)");
                mHeaderFile.addLine("{"); //doing pointer check
            }
            mHeaderFile.addLine("KCDataGroup mObjectDataGroup = " + strDataGroupName + ".getOrCreateChildGroup(" + strDataGroupOverride + ");");
            if (mVariable.isPointer)
			{                
                mHeaderFile.addLine("mObjectDataGroup.setProperty(\"_SERIALIZE_AS_\", " + mVariable.variableName + "->getClassName());");                
                mHeaderFile.addLine(mVariable.variableName + "->serialize(mObjectDataGroup, " + strDataGroupOverride + ");");				
			}
			else
			{
				mHeaderFile.addLine(mVariable.variableName + ".serialize(mObjectDataGroup, " + strDataGroupOverride + ");");
			}
            mHeaderFile.addLine("if(mObjectDataGroup.isEmpty()){ " + strDataGroupName + ".removeChildGroup(mObjectDataGroup);}");
            if (mVariable.isPointer)
            {                
                mHeaderFile.addLine("}"); //end pointer check
            }
            mHeaderFile.addLine("}");//end object scope
            return true;
		}
		public override bool attemptDataGroupReadCode(HeaderFile mHeaderFile, ClassVariable mVariable, string strDataGroupName)
		{
			if (_isPrimaryVariableType(mHeaderFile, mVariable))
			{
				return false;
			}
			ClassStructure mVariableClassStruct = getClassStructure(mHeaderFile, mVariable);
			if (mVariableClassStruct == null)
			{
				return false;
			}
            string strDataGroupOverride = (mVariable.dataataGroupOverride != "") ? mVariable.dataataGroupOverride : "\"mVariable.variableName\"";
            mHeaderFile.addLine("{"); //start object scope
            if (mVariable.isPointer)
            {
                mHeaderFile.addLine("DELETE_SAFELY(" + mVariable.variableName + ");");                
            }
            mHeaderFile.addLine("const KCDataGroup *pObjectDataGroup = " + strDataGroupName + ".getChildGroupWithInhertance(" + strDataGroupOverride + ");");
            mHeaderFile.addLine("if(pObjectDataGroup != nullptr)");
            mHeaderFile.addLine("{");
           // mHeaderFile.addLine("std::cout << \"Data group was FOUND! \" << std::endl;");
            if (mVariable.isPointer)
			{
                mHeaderFile.addLine("KCString strClass = pObjectDataGroup->getProperty(\"_SERIALIZE_AS_\", \"" + mVariableClassStruct.name + "\");");
                mHeaderFile.addLine(mVariable.variableName + " = (" + mVariableClassStruct.name + " *)_SERIALIZE_CLASS_CREATION_::createObject(strClass, \"" + mVariableClassStruct.name + "\");");
                mHeaderFile.addLine("if(" + mVariable.variableName + " != nullptr)");
                mHeaderFile.addLine("{");
                mHeaderFile.addLine(mVariable.variableName + "->deserialize(*pObjectDataGroup);");
                mHeaderFile.addLine("}");
                mHeaderFile.addLine("else");
                mHeaderFile.addLine("{");
                mHeaderFile.addLine("std::cout << \"ERROR IN CREATING CLASS \" << strClass.c_str() << std::endl;");                
                mHeaderFile.addLine("}");

            }
			else
			{
				mHeaderFile.addLine(mVariable.variableName + ".deserialize(*pObjectDataGroup);");
			}
            mHeaderFile.addLine("}");//else{std::cout << \"Data group was null \" << std::endl;}");//end object data group test.
            
            mHeaderFile.addLine("}");//end object scope
            return true;
		}

		private ClassStructure getClassStructure(HeaderFile mHeaderFile, ClassVariable mVariable)
		{
			string strVariableType = mVariable.variableType;
			if (strVariableType.Contains("<"))
			{
				strVariableType = strVariableType.Substring(0, strVariableType.IndexOf("<"));
			}
			return mHeaderFile.getClassStructure(strVariableType);
		}

		private bool _isPrimaryVariableType(HeaderFile mHeaderFile, ClassVariable mVariable)
		{
			return mHeaderFile.getTypeDefs().ContainsKey(mVariable.variableType.ToLower());
		}
	}
}
