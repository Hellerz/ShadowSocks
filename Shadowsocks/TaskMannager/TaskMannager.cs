using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shadowsocks.Controller;
using Shadowsocks.View;

namespace Shadowsocks.TaskMannager
{
    public class TaskMannager
    {
        public static RunTask AutoTask;
        public static RunTask IntervalTask;

        static TaskMannager()
        {
            AutoTask=new RunTask();
            AutoTask.RunTaskHandler += (sender, args) =>
            {
                if (!AuthorityMannager.CheckConnection())
                {
                    AuthorityMannager.Update();
                }
            };

            IntervalTask=new RunTask();
            IntervalTask.RunTaskHandler += (sender, args) =>
            {
                AuthorityMannager.Update();
            };
        }

    }
}
