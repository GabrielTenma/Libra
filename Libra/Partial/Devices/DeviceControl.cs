using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Libra
{
    partial class Devices
    {
        public abstract class DeviceControl
        {
            private string _IpAddress;
            private short _Port;
            private int _TimeOut;
            private bool _Connected = false;
            private int _ConnectingAttempt = 0;
            private DateTime _LoggingStartDate = new DateTime();

            private Dictionary<string, Job> dictJob = new Dictionary<string, Job>();

            public string IpAddress { get { return _IpAddress; } }
            public short Port { get { return _Port; } }
            public int TimeOut { get { return _TimeOut; } }
            public int MinInterval { get { return FindMinimumIntervalInJobs(); } }
            public int MaxInterval { get { return FindMaximumIntervalInJobs(); } }
            public bool Connected { get { return _Connected; ; } }
            public int ConnectingAttemp { get { return _ConnectingAttempt; } }
            public DateTime LoggingStartDate { get { return _LoggingStartDate; } }
            public TimeSpan LoggingTimeSpan { get { return DateTime.Now - _LoggingStartDate; } }


            private bool LogCycleEnable = false;
            private bool FirstLogging = true;

            // Base Constructor
            public DeviceControl(string ipaddress, short port, int timeout)
            {
                this._IpAddress = ipaddress;
                this._Port = port;
                this._TimeOut = timeout;
                this._LoggingStartDate = DateTime.Now;
            }

            // Base Destructor
            ~DeviceControl()
            {
                Disconnect();
            }

            // Adding Job Methode
            public void AddJob(string job, string da_type, string da_start, string da_stop, int interval)
            {
                // Job job
                dictJob.Add(job, new Job(
                    command: job,
                    datatype: da_type,
                    addressstart: da_start,
                    addressstop: da_stop,
                    interval: interval
                    ));
            }

            // Clear a Job
            public void ClearJob()
            {
                dictJob.Clear();
            }

            // Contain Job, return true if exist
            public bool ContainJob(string job)
            {
                return dictJob.ContainsKey(job);
            }

            public string GetJobResult(string command)
            {
                return dictJob[command].Result;
            }

            // Start Logging Process
            public void StartLogging()
            {
                if (!LogCycleEnable)
                {
                    LogCycleEnable = true;

                    Task TaskLogging = new Task(() => DoLogging());
                    TaskLogging.Start();
                }
            }

            // Stop Logging Process
            public void StopLogging()
            {
                LogCycleEnable = false;
            }

            // Do Logging, Called by StartLogging
            private void DoLogging()
            {
                Stopwatch myStopWatch = new Stopwatch();

                int MinimumInterval = FindMinimumIntervalInJobs();
                int MaximumInterval = FindMaximumIntervalInJobs();

                int CalculatedSleep;

                bool EnableJob;

                this._Connected = false;
                this._ConnectingAttempt = 0;
                this.FirstLogging = true;

                while (LogCycleEnable)
                {
                    if (this._Connected)
                    {
                        myStopWatch.Reset();
                        myStopWatch.Start();

                        this._ConnectingAttempt = 0;

                        // ProcessJob for each value
                        foreach (Job job in dictJob.Values)
                        {
                            EnableJob = FirstLogging || job.HasJob();

                            if (EnableJob)
                            {
                                job.AddResult(ProcessJob(job.Command));
                                job.ResetCurrentTimer();                    // Reset counter
                            }
                        }

                        // First Logging only once per-Connect
                        if (FirstLogging)
                        {
                            FirstLogging = false;
                            this._LoggingStartDate = DateTime.Now;
                        }


                        myStopWatch.Stop();

                        // Optimize Code, so just once methode MinInterval is Called rather than calling ( x times, MinInterval Methode )
                        MinimumInterval = FindMinimumIntervalInJobs();

                        // Increment CurrentTimer in job dictionary
                        foreach (Job job in dictJob.Values)
                        {
                            if (myStopWatch.ElapsedMilliseconds < MinimumInterval)                      // If Elapsed is smaller than add MinimumInterval
                                job.AddCurrentTimer(MinimumInterval);
                            else
                                job.AddCurrentTimer((int)myStopWatch.ElapsedMilliseconds);
                        }

                        // Calculate timer, and sleep..
                        CalculatedSleep = MinimumInterval - (int)myStopWatch.ElapsedMilliseconds;

                        if (CalculatedSleep > 0)
                        {
                            Thread.Sleep(CalculatedSleep);
                        }
                    }
                    else
                    {
                        myStopWatch.Reset();
                        myStopWatch.Start();

                        // Connect the Devices
                        this._Connected = Connect();
                        this._ConnectingAttempt += 1;

                        FirstLogging = true;

                        myStopWatch.Stop();

                        CalculatedSleep = this._TimeOut - (int)myStopWatch.ElapsedMilliseconds;

                        if (CalculatedSleep > 0)
                        {
                            Thread.Sleep(CalculatedSleep);
                        }
                    }
                }
            }

            private int FindMinimumIntervalInJobs()
            {
                int min = int.MaxValue;

                foreach (Job job in dictJob.Values)
                {
                    if (job.Interval > 0)
                        min = job.Interval < min ? job.Interval : min;
                }
                return min;
            }

            private int FindMaximumIntervalInJobs()
            {
                int max = int.MinValue;

                foreach (Job job in dictJob.Values)
                {
                    max = job.Interval > max ? job.Interval : max;
                }

                return max;
            }

            public abstract bool Connect();
            public abstract void Disconnect();
            public abstract string ProcessJob(string command);
        }

        public class Job
        {
            private int _CurrentTimer = 0;
            private string _Result = string.Empty;
            private int _DataQuantityPerSecond;

            public string Command { get; }
            public string DataType { get; }
            public string AddressStart { get; }
            public string AddressStop { get; }
            public string Result { get { return _Result; } }
            public int Interval { get; }
            public int CurrentTimer { get { return _CurrentTimer; } }
            public char CharacterDelimiter { get; set; } = ',';

            public Job(string command, string datatype, string addressstart, string addressstop, int interval)
            {
                this.Command = command;
                this.DataType = datatype;
                this.AddressStart = addressstart;
                this.AddressStop = addressstop;
                this.Interval = interval;                                                   // 50
                this._DataQuantityPerSecond = interval == 0 ? 0 : 1000 / interval;          // 20    if interval change it should be re-calculated

            }

            public void ResetCurrentTimer()
            {
                this._CurrentTimer = 0;
            }

            public void AddCurrentTimer(int counter)
            {
                _CurrentTimer += counter;
            }

            public bool HasJob()
            {
                if (Interval > 0)                                                       // jika interval =0, berarti pengambilan data hanya satu kali yaitu saat first logging
                    return this._CurrentTimer >= Interval ? true : false;
                else
                    return false;
            }

            public void ClearResult()
            {
                this._Result = string.Empty;
            }

            public void AddResult(string result)
            {
                int DelimiterCount;

                if (string.IsNullOrEmpty(_Result))
                {
                    _Result = result;
                    return;
                }
                else
                    _Result = _Result + CharacterDelimiter + result;

                DelimiterCount = _Result.Where(c => c == CharacterDelimiter).Count();

                if (DelimiterCount >= _DataQuantityPerSecond)
                {
                    //_Result = _Result.Remove(0, GetCharacterIndex(_Result, CharacterDelimiter, 1)+1);
                    _Result = _Result.Remove(0, _Result.IndexOf(CharacterDelimiter) + 1);

                }
            }

            public int GetCharacterIndex(string s, char t, int n)
            {
                int count = 0;
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == t)
                    {
                        count++;
                        if (count == n)
                            return i;
                    }
                }
                return -1;
            }

        }
    }
}
