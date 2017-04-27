using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace PatchLib
{
    public class PatchLog
    {
        static PatchLog s_instance;

        public static void Initialize(string logFileName)
        {
            s_instance = new PatchLog(logFileName);
        }

        public static void Shutdown()
        {
            if(s_instance != null )
                s_instance.Kill();
            s_instance = null;
        }

        public static void Print(string format, params object[] args)
        {
            s_instance.PrintThreadSafe(format, args);
        }


        string _logFileName;
        List<string> _outputStrings;
        Mutex _outputStringLock;

        Thread _logThread;
        ManualResetEvent _threadKill;

        private PatchLog(string logFileName)
        {
            _logFileName = logFileName;
            _outputStrings = new List<string>();
            _outputStringLock = new Mutex();

            _threadKill = new ManualResetEvent(false);
            _logThread = new Thread(new ThreadStart(LogThread));
            _logThread.Name = "Log Thread";
            _logThread.Start();
        }

        void Kill()
        {
            if (_logThread != null && _logThread.IsAlive)
            {
                _threadKill.Set();
                _logThread.Join();
            }
            _logThread = null;
        }

        void PrintThreadSafe(string format, object[] args)
        {
            string formatted = args.Length > 0 ? string.Format(format, args) : format;
            _outputStringLock.WaitOne();
            _outputStrings.Add(formatted);
            _outputStringLock.ReleaseMutex();
        }

        void LogThread()
        {
            while (true)
            {
                if (_outputStrings.Count > 0)
                {
                    // get log entries
                    _outputStringLock.WaitOne();
                    string[] output = _outputStrings.ToArray();
                    _outputStrings.Clear();
                    _outputStringLock.ReleaseMutex();

                    // Open output file
                    FileStream logFile = null;
                    try
                    {
                        if (_logFileName != null)
                        {
                            logFile = File.Open(_logFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                            if( logFile.CanSeek )
                                logFile.Seek(0, SeekOrigin.End);
                        }
                    }
                    catch (Exception) { }
                    StreamWriter logWriter = logFile == null ? null : new StreamWriter(logFile);

                    // Process entries
                    foreach (string line in output)
                    {
                        Debug.WriteLine(line);
                        Console.WriteLine(line);
                        logWriter.WriteLine(line);
                    }
                    
                    // Close output file
                    if( logWriter != null )
                        logWriter.Close();
                }


                if (_threadKill.WaitOne(1))
                    return;

                Thread.Sleep(50);
            }
        }
    }
}
