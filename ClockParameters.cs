using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MultiClock
{
    public class ClockParameters : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string _caption;
        public string Caption { get { return _caption; } set { if (_caption != value) { _caption = value; OnPropertyChanged(nameof(Caption)); } } }

        private TimeZoneInfo _timezone;
        public TimeZoneInfo Timezone { get { return _timezone; } set { if (_timezone != value) { _timezone = value; OnPropertyChanged(nameof(Timezone)); } } }

        private bool _local;
        public bool Local { get { return _local; } set {
                if (_local != value)
                {
                    _local = value;

                    if (_local)
                    {
                        _timezone = TimeZoneInfo.Local;
                        OnPropertyChanged(nameof(Timezone));
                    }

                    OnPropertyChanged(nameof(Local));
                }
            }
        }


        public SolidColorBrush DisplayColorSolid { get; private set; }
        public SolidColorBrush DisplayColorAlpha { get; private set; }

        private byte _colorr;
        public byte ColorR { get { return _colorr; } set { if (_colorr != value) { _colorr = value; UpdateDisplayColor(); OnPropertyChanged(nameof(ColorR)); } } }

        private byte _colorg;
        public byte ColorG { get { return _colorg; } set { if (_colorg != value) { _colorg = value; UpdateDisplayColor(); OnPropertyChanged(nameof(ColorG)); } } }

        private byte _colorb;
        public byte ColorB { get { return _colorb; } set { if (_colorb != value) { _colorb = value; UpdateDisplayColor(); OnPropertyChanged(nameof(ColorB)); } } }

        private byte _colora;
        public byte ColorA { get { return _colora; } set { if (_colora != value) { _colora = value; UpdateDisplayColor(); OnPropertyChanged(nameof(ColorA)); } } }



        public ClockParameters()
        {
            Init_Empty();

            UpdateDisplayColor();
        }

        public ClockParameters(string confstr)
        {
            string[] parts = confstr.Split(';');

            if (parts.Length == 7)
            {
                try
                {
                    _caption = parts[0];
                    _timezone = TimeZoneInfo.FindSystemTimeZoneById(parts[1]);
                    _local = bool.Parse(parts[2]);
                    _colorr = byte.Parse(parts[3]);
                    _colorg = byte.Parse(parts[4]);
                    _colorb = byte.Parse(parts[5]);
                    _colora = byte.Parse(parts[6]);

                    DisplayColorSolid = new SolidColorBrush();
                    DisplayColorAlpha = new SolidColorBrush();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception handling configration string: " + Environment.NewLine + confstr + Environment.NewLine + ex.Message);

                    Init_Empty();
                }
            }
            else
            {
                MessageBox.Show("Invalid conf string: " + Environment.NewLine + confstr);

                Init_Empty();
            }

            UpdateDisplayColor();
        }

        [MemberNotNull(nameof(_caption), nameof(_timezone), nameof(DisplayColorSolid), nameof(DisplayColorAlpha))]
        private void Init_Empty()
        {
            _caption = "";
            _timezone = TimeZoneInfo.Local;
            _local = true;

            _colorr = 0;
            _colorg = 0;
            _colorb = 0;
            _colora = 144;

            DisplayColorSolid = new SolidColorBrush();
            DisplayColorAlpha = new SolidColorBrush();
        }

        public string Make_Confstring()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(_caption.Replace(";", " "));
            sb.Append(';');

            sb.Append(Timezone.Id);
            sb.Append(';');

            sb.Append(_local);
            sb.Append(';');

            sb.Append(_colorr);
            sb.Append(';');

            sb.Append(_colorg);
            sb.Append(';');

            sb.Append(_colorb);
            sb.Append(';');

            sb.Append(_colora);

            return sb.ToString();
        }

        public ClockParameters DeepCopy()
        {
            ClockParameters result = new ClockParameters();

            result.Caption = _caption;
            result.Timezone = _timezone;
            result.Local = _local;

            result.ColorR = _colorr;
            result.ColorG = _colorg;
            result.ColorB = _colorb;
            result.ColorA = _colora;

            return result;
        }

        private void UpdateDisplayColor()
        {
            DisplayColorSolid.Color = Color.FromRgb(ColorR, ColorG, ColorB);
            DisplayColorAlpha.Color = Color.FromArgb(ColorA, ColorR, ColorG, ColorB);

            OnPropertyChanged(nameof(DisplayColorSolid));
            OnPropertyChanged(nameof(DisplayColorAlpha));
        }
    }
}
