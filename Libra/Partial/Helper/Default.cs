using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace Libra
{
    public partial class Helper
    {
        /*
         *  Partial Class Helper
         *  Most Used Functions
         */


        /// <summary>
        /// Function For Relaunch Application
        /// </summary>
        public static void ApplicationRelaunch(string Location)
        {
            Process.Start(Location);
            Environment.Exit(0);
        }


        /// <summary>
        /// Function For Ask & Relaunch Application
        /// </summary>
        public static void ApplicationAskRelaunch(string Location, string messageStackTrace)
        {
            Console.WriteLine($"ERROR : {messageStackTrace}");
            ApplicationRelaunch(Location);
        }

        public static string GetApplicationLocation()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
        }


        /// <summary>
        /// Function For Save String data to File
        /// </summary>
        public static void IOStringToFile(string filepath, string text)
        {
            File.WriteAllText(filepath, text);
        }

        /// <summary>
        /// Function For Check Directory if Exist
        /// </summary>
        public static bool IOCheckDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return false;
            }
            else
            {
                Directory.CreateDirectory(path);
                return true;
            }
        }


        /// <summary>
        /// Function For Remove File Older Than //Date
        /// </summary>
        public static void IORemoveOlderFile(string path, int days = -7)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.LastAccessTime < DateTime.Now.AddDays(days))
                {
                    fi.Delete();
                }
            }
        }


        /// <summary>
        /// Function For Kill While Already Running
        /// </summary>
        public static void ApplicationKillWhileRunning()
        {
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }


        /// <summary>
        /// Function For Execute Command Line (cmd)
        /// </summary>
        /// <param name="CmdParam">Command Line parameter</param>
        /// <param name="HideWindow">Hide Commandline Window</param>
        public static void ExecuteCommandline(string CmdParam, bool HideWindow = true)
        {
            if (HideWindow)
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = CmdParam;
                process.StartInfo = startInfo;
                process.Start();
            }
            else
            {
                System.Diagnostics.Process.Start("CMD.exe", CmdParam);
            }
        }



        /// <summary>
        /// Function For Check Database Before Application Running
        /// </summary>
        public static void DBCheckDBisAlive(Connection.Database DBConfig, string ApplicationLocation)
        {
            var check = DBConfig.IsConnected(out string Error);
            if (!check)
            {
                Console.WriteLine($"Can't Connect to Database!, Is Server Running?\n Error");
                ApplicationRelaunch(ApplicationLocation);
            }
        }



        /// <summary>
        /// Function For Check Database Connection State
        /// </summary>
        /// <param name="DBConfig">Database Class Configuration</param>
        /// <returns></returns>
        public static bool DBCheckDBIsAliveBool(Connection.Database DBConfig)
        {
            return DBConfig.IsConnected(out string error);
        }


        /// <summary>
        /// Function For Ping Request Alive OR Not
        /// </summary>
        public static string NetPingRequest(string IP)
        {
            string result = "ERROR";
            try
            {
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingReply reply = ping.Send(IP);
                bool req = reply.Status == System.Net.NetworkInformation.IPStatus.Success;
                result = string.Format("Response: {0}, Estimate: {1} ms", req, reply.RoundtripTime);
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
                Debug.WriteLine(x.StackTrace);
            }
            return result;
        }


        /// <summary>
        /// Function For GET Client IP Address
        /// </summary>
        public static string NetGetLocalIPAddress()
        {
            string result = "No Network Adapters Detected!.";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    result = ip.ToString();
                }
            }
            return result;
        }


        /// <summary>
        /// Function For GET Client IP Gateway
        /// </summary>
        /// <returns></returns>
        public static IPAddress NetGetDefaultGateway()
        {
            try
            {
                return NetworkInterface
               .GetAllNetworkInterfaces()
               .Where(n => n.OperationalStatus == OperationalStatus.Up)
               .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
               .SelectMany(n => n.GetIPProperties()?.GatewayAddresses)
               .Select(g => g?.Address)
               .Where(a => a != null)
               // .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
               // .Where(a => Array.FindIndex(a.GetAddressBytes(), b => b != 0) >= 0)
               .FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Function For Rounded Corner Form
        /// INPUT THIS CODE TO FORM : 
        ///  Region = System.Drawing.Region.FromHrgn(Actions.CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        /// </summary>
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );


        /// <summary>
        /// Function For Database Check Connection (For Application Startup Only)
        /// </summary>
        /// <param name="DBConfig">Database Configuration</param>
        public static void DBDatabaseCheck(Connection.Database DBConfig, int MaxReconnect)
        {
            int counter = 0;
            bool result;

            while (true)
            {
                result = DBConfig.IsConnected(out string ErrorResult);
                if (!result)
                {
                    if (counter >= MaxReconnect)
                    {
                        Console.WriteLine($"Can't Connect to Database!\nErrorResult");
                        Environment.Exit(0);
                    }
                    counter++;
                    System.Threading.Thread.Sleep(500);
                }
                else
                {
                    break;
                }
            }
        }


        /// <summary>
        /// Function For Getdata from Datarow Fix NullException
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="Columnname"></param>
        /// <returns></returns>
        public static string GetDataFromRow(System.Data.DataRow dr, string Columnname)
        {
            return (dr[Columnname].ToString() != null) ? dr[Columnname].ToString() : "0";
        }


        /// <summary>   
        /// Function For Split String
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static List<object> SplitString(string Value)
        {
            object[] data = Value.Split(' ');
            List<object> listvalue = new List<object>();
            for (int i = 0; i < data.Length; i++)
            {
                listvalue.Add(data[i]);
            }
            return listvalue;
        }


        /// <summary>
        /// Function For Copy Column & Rows to new Datatable. (Data Cannot be NULL!)
        /// </summary>
        /// <param name="dtPrimary">Primary Datatable with Data</param>
        /// <param name="dtSecondary">Secondary Datatable without Data</param>
        /// <returns></returns>
        public static bool DatatableValueCopy(DataTable dtPrimary, DataTable dtSecondary)
        {
            if (dtPrimary != null)
            {
                if (dtSecondary.Columns.Count is 0)
                {
                    dtSecondary.Columns.Clear();

                    // add column if count zero
                    string[] ColumnPrimary = (from dc in dtPrimary.Columns.Cast<DataColumn>()
                                              select dc.ColumnName).ToArray();
                    foreach (string col in ColumnPrimary)
                    {
                        Type type = dtPrimary.Columns[col].DataType;
                        dtSecondary.Columns.Add(col,type);
                    }

                    // add rows
                    foreach (DataRow dr in dtPrimary.Rows)
                    {
                        dtSecondary.Rows.Add(dr.ItemArray);
                    }

                    return true;
                }
                else
                {
                    // check column same value
                    if (DatatableColumnNameSameCheck(dtPrimary, dtSecondary))
                    {
                        // rows count check
                        if (dtPrimary.Rows.Count == dtSecondary.Rows.Count)
                        {
                            int index = 0;
                            foreach (DataRow dr in dtSecondary.Rows)
                            {
                                dr.ItemArray = dtPrimary.Rows[index].ItemArray;
                                index++;
                            }
                        }

                        // if isnot same
                        else
                        {
                            dtSecondary.Rows.Clear(); //selected cell will be reset
                            foreach (DataRow dr in dtPrimary.Rows)
                            {
                                dtSecondary.Rows.Add(dr.ItemArray);
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Function For Cross Check Same Column in Datatable
        /// </summary>
        /// <param name="dtPrimary">Primary Datatable</param>
        /// <param name="dtSecondary">Secondary Datatable</param>
        /// <returns>Bool Is Same ?</returns>
        public static bool DatatableColumnNameSameCheck(DataTable dtPrimary, DataTable dtSecondary)
        {
            if (dtPrimary != null && dtSecondary != null)
            {
                if (dtPrimary.Columns.Count > 0 && dtSecondary.Columns.Count > 0)
                {
                    int columncount = dtPrimary.Columns.Count;

                    for (int i = 0; i < columncount; i++)
                    {
                        bool check = dtPrimary.Columns[i].ColumnName == dtSecondary.Columns[i].ColumnName;
                        if (!check)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Function For Move Object by X Y
        /// </summary>
        /// <param name="objTarget">Object Target</param>
        /// <param name="XLocation">X Coordinate</param>
        /// <param name="YLocation">Y Coordinate</param>
        public static void FormRelocateObject(object objTarget, int XLocation, int YLocation)
        {
            Type objecttype = objTarget.GetType();
        }
    }
}
