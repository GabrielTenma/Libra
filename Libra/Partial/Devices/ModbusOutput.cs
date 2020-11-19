using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libra
{
    partial class Devices
    {
        public class ModbusOutput
        {

            private string _IpAddress;
            private int _Port;
            private byte _UID;
            private bool _Connected = false;
            private int _ConnectingAttempt = 0;
            private int _Values = 0b1111_1111_1111_1111;

            public string IpAddress { get { return _IpAddress; } }
            public int Port { get { return _Port; } }
            public byte UID { get { return _UID; } }
            public int Values { get { return _Values; } }
            public bool Connected { get { return _Connected; } }

            public ModbusOutput(string IPAddress,int Portaddress,byte UIDentifier )
            {
                _IpAddress = IPAddress;
                _Port = Portaddress;
                _UID = UIDentifier;
            }

        }
    }
}
