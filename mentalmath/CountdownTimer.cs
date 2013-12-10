using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace mentalmath
{
    class CountdownTimer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Raise(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        
        public decimal RemainingMilliseconds
        {
            get { return (decimal)elapsed; }
        }

        private TimeSpan interval;

        public TimeSpan Interval
        {
            get { return interval; }
            set 
            {
                if (IsRunning)
                    throw new Exception("Interval can not be changed while Timer is running. Execute Stop() before assign a value.");
                interval = value;
                Raise("Countdown");
            }
        }

        private bool isrunning;

        public bool IsRunning
        {
            get { return isrunning; }
            private set { isrunning = value; Raise("IsRunning"); }
        }

        private bool repeat;

        public bool RepeatForever
        {
            get { return repeat; }
            set 
            {
                if (IsRunning)
                    throw new Exception("RepeatForever can not be changed while Timer is running. Execute Stop() before assign a value.");
                repeat = value;
            }
        }


        private ReportInterval repint;

        public ReportInterval ReportInterval
        {
            get { return repint; }
            set 
            {
                if (IsRunning)
                    throw new Exception("ReportInterval can not be changed while Timer is running. Execute Stop() before assign a value.");
                repint = value; 
            }
        }

        private DispatcherTimer timer;

        private int elapsed = 0;

        public void Start()
        {
            if(!IsRunning)
            {
                elapsed = 0;
                IsRunning = true;
                timer.Start();
            }
        }

        public void Stop()
        {
            if(IsRunning)
            {
                timer.Stop();
                elapsed = 0;
                IsRunning = false;
            }
        }

        public CountdownTimer(TimeSpan interval, ReportInterval ri)
        {
            Interval = interval;
            ReportInterval = ri;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, (int)ri);
            timer.Tick += timer_Tick;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            elapsed += (int)ReportInterval*2;
            if(elapsed>=Interval.TotalMilliseconds)
            {
                if (CountdownAccomplished != null)
                    CountdownAccomplished(this, null);
                elapsed = 0;

                if (!RepeatForever)
                    Stop();
            }
            else
            {
                Raise("RemainingMilliseconds");
                if (ReportProgress != null)
                    ReportProgress(this, null);
            }
        }

        public event EventHandler<EventArgs> ReportProgress;

        public event EventHandler<EventArgs> CountdownAccomplished;
    }

    public enum ReportInterval
    {
        Millisecond=1, HundredthSecond=10, TenthSecond=100, Second=1000, TenSecond=10000, Minute=60000
    }
}
