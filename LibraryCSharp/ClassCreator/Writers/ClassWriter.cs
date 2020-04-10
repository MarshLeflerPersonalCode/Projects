using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.ClassParser;
namespace Library.ClassCreator.Writers
{
	public class ClassWriter
	{

		public static string writeClass(ClassStructure mClass, ProjectWrapper mProjectWrapper)
		{
			if (mClass == null)
			{
				return "";
			}

			string strClass = Environment.NewLine;
			strClass = strClass + "    public class " + mClass.name + ": ClassInstance" + Environment.NewLine;


			strClass = strClass + "    {" + Environment.NewLine;
			//todo add properties in.
			foreach(ClassVariable mVariable in mClass.variables)
			{
				strClass = strClass + _getVariable(mProjectWrapper, mClass, mVariable);
			}
			strClass = strClass + "    } //end of " + mClass.name + Environment.NewLine;

			return strClass;
		}

		private static string _getVariable(ProjectWrapper mProjectWrapper, ClassStructure mClass, ClassVariable mVariable)
		{
			return "";
		}

	}

}
