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
			SerializerController m_Controller = new SerializerController(args);
			// The code provided will print ‘Hello World’ to the console.
			// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.						
			Console.ReadKey();

			// Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
		}
	}
}
