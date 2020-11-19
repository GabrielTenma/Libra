using BeetleX;
using BeetleX.Buffers;
using BeetleX.Clients;
using System;
using System.Threading;

namespace Libra
{
    public partial class Connection
    {
        public class SocketClientSingle
        {
            public string Hostname { get; set; } = "127.0.0.1";
            public int Port { get; set; } = 9090;
            public bool ConnectionState { get { return _State; } }

            private TcpClient client;
            private bool _State = false;

            public SocketClientSingle(string strHost, int intPort)
            {
                Hostname = strHost;
                Port = intPort;

                try
                {
                    client = SocketFactory.CreateClient<TcpClient, StringPacketClient>(Hostname, Port);
                }
                
                catch (Exception x)
                {
                    Console.WriteLine(x.Message);
                    Console.WriteLine(x.StackTrace);
                }
                
                ConnectionChecker();
            }

            public object Query(string Message)
            {
                object result = "";

                if (_State)
                {
                    try
                    {
                        client.SendMessage(Message);
                        result = client.ReceiveMessage<object>();
                    }
                    catch (Exception x)
                    {
                        Console.WriteLine(x.StackTrace);
                        Console.WriteLine(x.Message);
                    }
                }

                return result;
            }

            public bool Connect()
            {
                bool result = false;

                try
                {
                    client.Connect(out result);
                }
                catch
                {
                    result = false;
                }
                
                return result;
            }

            private void ConnectionChecker()
            {
                Thread thread = new Thread(() =>
                {
                    for (; ; )
                    {
                        Console.WriteLine("BeetleX Client Check");
                        try
                        {
                            if (client.IsConnected)
                            {
                                _State = true;
                            }
                            else
                            {
                                Console.WriteLine("BeetleX Client Trying Connect");
                                client.Connect(out bool result);
                                _State = result;
                            }
                        }
                        catch (Exception x)
                        {
                            Console.WriteLine(x.StackTrace);
                            Console.WriteLine(x.Message);
                        }

                        Thread.Sleep(500);
                    }
                });
                thread.Start();
            }
        }

        protected class StringPacketClient : BeetleX.Packets.FixeHeaderClientPacket
        {
            public override IClientPacket Clone()
            {
                return new StringPacketClient();
            }

            protected override object OnRead(IClient client, PipeStream stream)
            {
                return stream.ReadString(CurrentSize);
            }

            protected override void OnWrite(object data, IClient client, PipeStream stream)
            {
                stream.Write((string)data);
            }
        }
    }
}
