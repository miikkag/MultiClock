using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiClock
{
    public class ClockData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string _timestring;
        public string TimeString { get { return _timestring; } private set { if (_timestring != value) { _timestring = value; OnPropertyChanged(nameof(TimeString)); } } }

        private string _datestring;
        public string DateString { get { return _datestring; } private set { if (_datestring != value) { _datestring = value; OnPropertyChanged(nameof(DateString)); } } }


        public readonly int ID;

        private Thread thr;

        private bool run;

        public GlobalSettings Settings { get; private set; }

        private ClockParameters _params;
        public ClockParameters Params { get { return _params; } set { _params = value; OnPropertyChanged(nameof(Params)); } }

        public ClockData(int id, GlobalSettings settings, ClockParameters p)
        {
            _timestring = "";
            _datestring = "";

            ID = id;
            Settings = settings;
            _params = p;

            run = false;

            Settings = settings;

            thr = new Thread(RunThread);
        }

        public void Start()
        {
            run = true;

            thr.Start();
        }

        public void Stop()
        {
            run = false;
        }


        private void RunThread()
        {
            DateTime prev = DateTime.UtcNow;

            while (run)
            {
                DateTime utc = DateTime.UtcNow;

                if (prev.Second != utc.Second)
                {
                    DateTime usetime = TimeZoneInfo.ConvertTime(utc, Params.Timezone);

                    TimeString = $"{usetime.Hour:D2}:{usetime.Minute:D2}:{usetime.Second:D2}";
                    DateString = $"{usetime.Day}.{usetime.Month}.{usetime.Year}";
                    prev = utc;

                    Thread.Sleep(800);  // Sleep longer after update -- second is unlikely to change anytime soon
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }
    }
}
