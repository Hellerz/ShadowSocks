using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shadowsocks.Common;

namespace Shadowsocks.TaskMannager
{
    public class RunTask
    {
        public LimitTask Task { get; private set; }

        public event LimitTaskEventHandler RunTaskHandler;

        private void OnRunTaskHandler(LimitTask sender, LimitTaskEventArgs e)
        {
            if (RunTaskHandler != null)
            {
                RunTaskHandler(sender, e);
            }
        }

        public void Start(int intervel, int startIntervel = 0)
        {
            Task = new LimitTask("Update Server", OnRunTaskHandler, sc =>
            {
                sc.ToRunNow().AndEvery(intervel).Seconds();
            });
            Task.StartAt(DateTime.Now.AddSeconds(startIntervel));
        }

        public void Stop()
        {
            if (Task != null)
            {
                Task.Stop();
            }
        }

        public void ReStart(int intervel, int startIntervel = 0)
        {
            Stop();
            Start(intervel, startIntervel);
        }
    }
}
