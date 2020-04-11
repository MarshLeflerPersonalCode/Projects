using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.ClassParser;
namespace Library.ClassCreator.Writers
{
	public class EnumWriter
	{
		public static string writeEnum(EnumList mList, ProjectWrapper mProjectWrapper)
		{
			   string strEnum = "    public enum " + mList.enumName + Environment.NewLine;
			strEnum = strEnum + "    {" + Environment.NewLine;
			foreach (EnumItem mEnumItem in mList.enumItems)
			{
				strEnum = strEnum + "         " + mEnumItem.name + "," + Environment.NewLine;
			}
			strEnum = strEnum + "    };" + Environment.NewLine;

			//now lets write the drop down code for it.
			//https://www.codeproject.com/Articles/9517/PropertyGrid-and-Drop-Down-properties
			strEnum = strEnum + Environment.NewLine + "    public class _TypeConverter_" + mList.enumName + " : StringConverter" + Environment.NewLine;
			strEnum = strEnum + "    {" + Environment.NewLine;
			strEnum = strEnum + "       StandardValuesCollection m_ReturnStandardCollection = null;" + Environment.NewLine;
			strEnum = strEnum + "       public override bool GetStandardValuesSupported(ITypeDescriptorContext context){ return true; }" + Environment.NewLine;
			strEnum = strEnum + "       public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){ return true; }" + Environment.NewLine;
			strEnum = strEnum + "       public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)" + Environment.NewLine;
			strEnum = strEnum + "       {" + Environment.NewLine;
			strEnum = strEnum + "           if(m_ReturnStandardCollection == null )" + Environment.NewLine;
			strEnum = strEnum + "           {" + Environment.NewLine;
			strEnum = strEnum + "                List<String> mEnumList = new List<String>();" + Environment.NewLine;			
			foreach (EnumItem mEnumItem in mList.enumItems)
			{
				strEnum = strEnum + "                mEnumList.Add(\"" + mEnumItem.name + "\");" + Environment.NewLine;
			}
			strEnum = strEnum + "                m_ReturnStandardCollection = new StandardValuesCollection(mEnumList);" + Environment.NewLine;
			strEnum = strEnum + "           }" + Environment.NewLine;
			strEnum = strEnum + "           return m_ReturnStandardCollection;" + Environment.NewLine;
			strEnum = strEnum + "       }" + Environment.NewLine;
			strEnum = strEnum + "    }" + Environment.NewLine;

			return strEnum;
		}
	}
}
