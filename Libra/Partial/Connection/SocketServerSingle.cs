using BeetleX;
using BeetleX.Buffers;
using System.Collections.Generic;
using System.Diagnostics;

namespace Libra
{
    public partial class Connection
    {
        public class SocketServerSingle : ServerHandlerBase
        {
            public static string Hostname { get; set; } = "127.0.0.1";
            public static int Port { get; set; } = 9090;
            public static Dictionary<string, SocketServerCommand> DictCommandList = new Dictionary<string, SocketServerCommand>();

            private static IServer Server;

            public static void StartServer()
            {
                Server = SocketFactory.CreateTcpServer<SocketServerSingle, StringPacketServer>();
                Server.Options.DefaultListen.Host = Hostname;
                Server.Options.DefaultListen.Port = Port;
                Server.Open();

                Debug.WriteLine($"Server BeetleX Core Started Host: {Hostname}, Port: {Port}");
            }

            protected override void OnReceiveMessage(IServer server, ISession session, object message)
            {
               object result;
                
                if(DictCommandList.TryGetValue(message.ToString(), out SocketServerCommand Val))
                {
                    result = Val.Value();
                }
                else
                {
                    result = "NULL";
                }
                
                server.Send(result, session);
            }
        }

        protected class StringPacketServer : BeetleX.Packets.FixedHeaderPacket
        {
            public override IPacket Clone()
            {
                return new StringPacketServer();
            }

            protected override object OnRead(ISession session, PipeStream stream)
            {
                return stream.ReadString(CurrentSize);
            }
            protected override void OnWrite(ISession session, object data, PipeStream stream)
            {
                stream.Write((string)data);
            }
        }

        public abstract class SocketServerCommand
        {
            public abstract object Value();
        }
    }
}
