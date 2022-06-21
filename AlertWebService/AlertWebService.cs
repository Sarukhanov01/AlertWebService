using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace AlertWebService
{
    public partial class AlertWebService : ServiceBase
    {
        static Thread listenerThread;
        //static HttpListener listener;
        public AlertWebService()
        {
            InitializeComponent();
            this.CanStop = true;
        }

        protected override void OnStart(string[] args)
        {

            listenerThread = new Thread(new ThreadStart(Listener.Listen));
            listenerThread.Start();
            
        }

        protected override void OnStop()
        {
            Listener.isStopped = true;
            listenerThread.Join();
        }
        
    }

}
