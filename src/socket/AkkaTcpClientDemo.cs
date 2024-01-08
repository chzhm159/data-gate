using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.IO;
using log4net;

namespace plc.monitor.socket
{
    internal class AkkaTcpClientDemo : UntypedActor
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AkkaTcpClientDemo));
        private  IActorRef _connection;
        public AkkaTcpClientDemo() {
            DnsEndPoint endpoint = new DnsEndPoint("127.0.0.1", 8501);
            Tcp.Connect connc  = new Tcp.Connect(endpoint);
            Context.System.Tcp().Tell(connc);
        }
        protected override void OnReceive(object message) {
            log.DebugFormat("收到消息:",message.ToString());
            if (message is Tcp.Connected connected) {
                log.DebugFormat("成功连接至: {0}", connected.RemoteAddress);

                // Register self as connection handler
                Sender.Tell(new Tcp.Register(Self));
                
                Become(Connected(Sender));
            } else if (message is Tcp.CommandFailed) {
                log.DebugFormat("连接断开!");
            } else if (message is string && string.Equals("close",message.ToString())) {
                Context.Stop(Self);
            } else Unhandled(message);
        }
        private UntypedReceive Connected(IActorRef connection) {
            return message => {
                if (message is Tcp.Received received)  // data received from network
                {
                    if (_connection == null) {
                        _connection = connection;
                    }
                    log.DebugFormat("接收服务器消息:{0}",Encoding.ASCII.GetString(received.Data.ToArray()));
                    connection.Tell(Tcp.Write.Create(ByteString.FromString("ping")));
                    
                } else if (message is string s)   // data received from console
                  {
                    // connection.Tell(Tcp.Write.Create(ByteString.FromString("ping")));
                    
                } else if (message is Tcp.PeerClosed) {
                    log.DebugFormat("Connection closed");
                } else Unhandled(message);
            };
        }
        public override void AroundPostRestart(Exception cause, object message) {
            base.AroundPostRestart(cause, message);
        }

        public override void AroundPostStop() {
            base.AroundPostStop();
        }

        public override void AroundPreRestart(Exception cause, object message) {
            base.AroundPreRestart(cause, message);
        }

        public override void AroundPreStart() {
            base.AroundPreStart();
        }

        protected override bool AroundReceive(Receive receive, object message) {
            return base.AroundReceive(receive, message);
        }

        

        protected override void PostRestart(Exception reason) {
            base.PostRestart(reason);
        }

        protected override void PostStop() {
            base.PostStop();
        }

        protected override void PreRestart(Exception reason, object message) {
            base.PreRestart(reason, message);
        }

        protected override void PreStart() {
            base.PreStart();
        }

        protected override SupervisorStrategy SupervisorStrategy() {
            return base.SupervisorStrategy();
        }

        protected override void Unhandled(object message) {
            base.Unhandled(message);
        }

        internal void Close() {
            Context.Stop(Self);
        }
    }
}
