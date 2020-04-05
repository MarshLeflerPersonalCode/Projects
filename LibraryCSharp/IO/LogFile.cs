using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Library.IO
{
	public class LogFile
	{
		private int m_iThreadsCreated = 0;
		private Thread m_Thread = null;
		private bool m_bPrintToConsole = false;
		private Mutex m_MutexListOfLogEntries = new Mutex();
		private Mutex m_MutexCopyOfListOfLogEntries = new Mutex();
		private System.Timers.Timer m_Timer = new System.Timers.Timer();
		private string m_strLogFile = "";
		private bool m_bIsValidLogFile = false;
		private List<string> m_ListOfLogEntries = new List<string>();
		private List<string> m_CopyOfListOfLogEntries = new List<string>();
		public LogFile(string strPathToLogFile)
		{
			_configure(strPathToLogFile, 100, true);
		}
		public LogFile(string strPathToLogFile, int iUpdateTick)
		{
			_configure(strPathToLogFile, iUpdateTick, true);
		}
		public LogFile(string strPathToLogFile, int iUpdateTick, bool bShowInConsole)
		{
			_configure(strPathToLogFile, iUpdateTick, bShowInConsole);
		}

		private void _configure(string strPathToLogFile, int iUpdateTick, bool bShowInConsole)
		{
			m_bPrintToConsole = bShowInConsole;
			_attemptToCreateLog(strPathToLogFile);
			if (getValidLogFile())
			{
				AppDomain.CurrentDomain.ProcessExit += new EventHandler(currentDomainProcessExit);

				m_Timer.Elapsed += new ElapsedEventHandler(onTimedEvent);
				m_Timer.Interval = iUpdateTick;
				m_Timer.Enabled = true;
			}
		}
		private void onTimedEvent(object source, ElapsedEventArgs e)
		{
			if(m_Thread == null &&
				(m_ListOfLogEntries.Count > 0 || m_CopyOfListOfLogEntries.Count > 0 ))
			{
				m_MutexListOfLogEntries.WaitOne();
				m_MutexCopyOfListOfLogEntries.WaitOne();
				m_CopyOfListOfLogEntries.AddRange(m_ListOfLogEntries);
				m_ListOfLogEntries.Clear();
				m_MutexCopyOfListOfLogEntries.ReleaseMutex();
				m_MutexListOfLogEntries.ReleaseMutex();
				try
				{
					m_Thread = new Thread(new ThreadStart(_threadCopyToLog));
					m_Thread.Name = String.Format("Thread{0}", m_iThreadsCreated);
					m_iThreadsCreated++;
					m_Thread.Start();
				}
				catch
				{
					m_Thread = null;
				}
			}
			m_Timer.Enabled = m_bIsValidLogFile;
		}

		private void _threadCopyToLog()
		{
			m_MutexCopyOfListOfLogEntries.WaitOne();
			_writeToLogNotThreadSafe();
			m_MutexCopyOfListOfLogEntries.ReleaseMutex();
			m_Thread = null;
		}

		void currentDomainProcessExit(object sender, EventArgs e)
		{
			flushLog();
			
		}

		public bool getValidLogFile() { return m_bIsValidLogFile; }

		private void _attemptToCreateLog(string strPathToLogFile)
		{
			if (m_bIsValidLogFile == false)
			{
				m_strLogFile = strPathToLogFile;
				FileStream mStream = File.Open(strPathToLogFile, FileMode.Create);
				if (mStream != null)
				{
					mStream.Close();
					m_bIsValidLogFile = true;
				}
				
			}			
		}

		private void _writeToLogNotThreadSafe()
		{
			StreamWriter mStream = new StreamWriter( File.Open(m_strLogFile, FileMode.Append) );
			if (mStream != null)
			{
				
				foreach (string strLine in m_CopyOfListOfLogEntries)
				{
					mStream.WriteLine(strLine);					
				}
				m_CopyOfListOfLogEntries.Clear();
				mStream.Close();
				m_bIsValidLogFile = true;
			}
			else
			{
				m_bIsValidLogFile = false;
			}

		}

		public void log(string strLineToLog)
		{
			if (m_bPrintToConsole)
			{
				bool bChangeColor = strLineToLog.ToLower().StartsWith("error");
				if( bChangeColor )
				{
					Console.ForegroundColor = ConsoleColor.Red;
				}
				Console.WriteLine(strLineToLog);
				if( bChangeColor )
				{
					Console.ResetColor();
				}
			}
			if (getValidLogFile())
			{
				m_MutexListOfLogEntries.WaitOne();
				m_ListOfLogEntries.Add(strLineToLog);
				m_MutexListOfLogEntries.ReleaseMutex();
			}
		}

		public void flushLog()
		{
			m_MutexListOfLogEntries.WaitOne();
			m_MutexCopyOfListOfLogEntries.WaitOne();
			m_CopyOfListOfLogEntries = new List<string>(m_ListOfLogEntries);
			m_ListOfLogEntries.Clear();
			_writeToLogNotThreadSafe();
			m_MutexCopyOfListOfLogEntries.ReleaseMutex();
			m_MutexListOfLogEntries.ReleaseMutex();


		}

	}
}
