using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FluentScheduler;
using FluentScheduler.Model;

namespace Shadowsocks.Common
{
    public delegate void LimitTaskEventHandler(LimitTask sender, LimitTaskEventArgs e);

    public class LimitTaskEventArgs
    {
        public DateTime NextRunTime { get; set; }
        public DateTime CurrentRunTime{ get; set; }
        public object Data { get; set; }
    }

    

    public class LimitTask : Registry
    {
        public DateTime StopDateTime { get; private set; }

        public event LimitTaskEventHandler LimitTaskHandler;

        public string Name { get; private set; }

        

        private Schedule Schedule
        {
            get
            {
                return TaskManager.GetSchedule(Name);
            }
        }
        private TaskStatus _status;
        public LimitTask(string name, DateTime stopDateTime, LimitTaskEventHandler action,
            Action<Schedule> scheduelAction)
        {
            StopDateTime = stopDateTime;
            Name = name;
            this.LimitTaskHandler += action;
            SetStatus(TaskStatus.Created);
            scheduelAction(Schedule(() =>
            {
                lock (this)
                {
                    Debug.WriteLine("StopDateTime:{0:O},NextRunTime:{1:O}", StopDateTime, this.Schedule.NextRunTime);
                    SetStatus(TaskStatus.Running);
                    if (StopDateTime < this.Schedule.NextRunTime)
                    {
                        if (DateTimeChanged != null)
                        {
                            DateTimeChanged(this, new DateTime());
                        }
                        Stop();
                    }
                    else
                    {
                        if (DateTimeChanged != null)
                        {
                            DateTimeChanged(this, this.Schedule.NextRunTime);
                        }
                        if (this.LimitTaskHandler != null)
                        {
                            this.LimitTaskHandler(this, new LimitTaskEventArgs
                            {
                                CurrentRunTime = DateTime.Now,
                                NextRunTime = this.Schedule.NextRunTime
                            });
                        }
                        SetStatus(TaskStatus.WaitingToRun);
                    }
                    
                }
            }).WithName(Name));
        }

        public void Stop()
        {
            this.Schedule.Disable();
            SetStatus(TaskStatus.RanToCompletion);
        }

        public void Start()
        {
            if (_status != TaskStatus.Created) return;
            TaskManager.Initialize(this);
            SetStatus(TaskStatus.WaitingToRun);
        }
        public void ReStart()
        {
            Stop();
            if (_status != TaskStatus.Created) return;
            TaskManager.Initialize(this);
            SetStatus(TaskStatus.WaitingToRun);
        }
        

        public void StartAt(DateTime startDateTime)
        {
            if (_status != TaskStatus.Created) return;
            TaskManager.AddTask(() =>
                {
                    TaskManager.Initialize(this);
                    SetStatus(TaskStatus.WaitingToRun);
                },
                startSchedule => startSchedule.ToRunOnceAt(startDateTime)
            );
            SetStatus(TaskStatus.WaitingForActivation);
            
        }

        public event Action<LimitTask, TaskStatus> StatusChanged;

        public event Action<LimitTask, DateTime> DateTimeChanged;

        private void SetStatus(TaskStatus status)
        {
            if (_status != TaskStatus.RanToCompletion)
            {
                _status = status;
                if (StatusChanged != null) StatusChanged(this, _status);
            }

        }

        public LimitTask(LimitTaskEventHandler action, Action<Schedule> scheduelAction)
            : this("Schedule" + Guid.NewGuid(),  DateTime.MaxValue, action, scheduelAction)
        {
        }

        public LimitTask(string name, LimitTaskEventHandler action, Action<Schedule> scheduelAction)
            : this(name, DateTime.MaxValue, action, scheduelAction)
        {
        }

        public LimitTask(DateTime stopDateTime, LimitTaskEventHandler action, Action<Schedule> scheduelAction)
            : this("Schedule" + Guid.NewGuid(), stopDateTime, action, scheduelAction)
        {
        }
    }
}
