using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CommandLineSerializer
{
	class Program
	{
		
		static void Main(string[] args)
		{
			SerializerController m_Controller = null;

			try
			{
				m_Controller = new SerializerController(args);
			}
			catch(Exception e)
			{
				Console.Write(e.Message);
			}
			// The code provided will print ‘Hello World’ to the console.
			// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.						
			//lets check to see if pause in the args
			bool bPauseAtEnd = false;
			foreach( string strArgLine in args)
			{
				string strArgLineUpper = strArgLine.ToUpper();
				if(strArgLineUpper == "-P"  ||
					strArgLineUpper == "-PAUSE" )
				{
					bPauseAtEnd = true;
					break;
				}
			}
			if (bPauseAtEnd)
			{
				Console.Write("Press any key..");
				Console.ReadKey();
			}
			
			// Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
		}
	}
}
