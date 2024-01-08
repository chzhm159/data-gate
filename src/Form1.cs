using Akka.Actor;
using plc.monitor.socket;

namespace plc.monitor
{
    public partial class Form1 : Form
    {
        public Form1() {
            InitializeComponent();
        }
        bool start = false;
        List<IActorRef> clientArray = new List<IActorRef>();
        private void button1_Click(object sender, EventArgs e) {
            if (!start) {
                var system = ActorSystem.Create("MySystem");
                Random random = new Random();
                for (int i = 0; i < 10; i++) {
                    IActorRef client = system.ActorOf<AkkaTcpClientDemo>("client"+DateTime.Now.Millisecond+ random.NextDouble());
                    clientArray.Add(client);
                    client.Tell(clientArray[i]);
                }
                start=true;
            } else {
                clientArray[0].Tell("close");
                // clientArray.ForEach(client => {
                //    client.Close();
                //    client.Tell("close");
                // });
                start = false;
            }
                
        }
    }
}