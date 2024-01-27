using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MultiClock
{
    public class GlobalSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private const string CONFIGFILE_NAME = "settings.conf";

        private const string PREFIX_CLOCK = "CLOCK";
        private const string PREFIX_COORDINATES = "COORDS";
        private const string PREFIX_DIRECTION = "DIRECTION";

        private const string DIRECTION_HORIZONTAL = "horizontal";
        private const string DIRECTION_VERTICAL = "vertical";

        public List<string> Clocks;

        public int X { get; set; }
        public int Y { get; set; }

        private bool _directionhorizontal;
        public bool DirectionHorizontal { get { return _directionhorizontal; } set { if (_directionhorizontal != value) { _directionhorizontal = value; OnPropertyChanged(nameof(DirectionHorizontal)); } } }

        private bool _directionvertical;
        public bool DirectionVertical { get { return _directionvertical; } set { if (_directionvertical != value) { _directionvertical = value; OnPropertyChanged(nameof(DirectionVertical)); } } }


        public GlobalSettings()
        {
            Clocks = new List<string>();

            _directionhorizontal = false;
            _directionvertical = true;

            X = 0;
            Y = 0;

            // Read config file
            try
            {
                string[] lines = File.ReadAllLines(CONFIGFILE_NAME);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(':', 2);

                    if (parts.Length == 2)
                    {
                        if (parts[0] == PREFIX_CLOCK)
                        {
                            Clocks.Add(parts[1]);
                        }
                        else if (parts[0] == PREFIX_COORDINATES)
                        {
                            string[] coord_parts = parts[1].Split(',');

                            if (coord_parts.Length == 2)
                            {
                                X = int.Parse(coord_parts[0]);
                                Y = int.Parse(coord_parts[1]);
                            }
                        }
                        else if (parts[0] == PREFIX_DIRECTION)
                        {
                            if (parts[1] == DIRECTION_VERTICAL)
                            {
                                DirectionVertical = true;
                                DirectionHorizontal = false;
                            }
                            else if (parts[1] == DIRECTION_HORIZONTAL)
                            {
                                DirectionVertical = false;
                                DirectionHorizontal = true;
                            }
                            else
                            {
                                // Use default
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignore errors -> use defaults
            }
        }

        public void Write_Config()
        {
            System.Diagnostics.Debug.WriteLine("Write config");

            List<string> lines = new List<string>();

            foreach (string cl in Clocks)
            {
                lines.Add($"{PREFIX_CLOCK}:{cl}");
            }

            lines.Add($"{PREFIX_COORDINATES}:{X},{Y}");

            if (DirectionVertical)
            {
                lines.Add($"{PREFIX_DIRECTION}:{DIRECTION_VERTICAL}");
            }
            else
            {
                lines.Add($"{PREFIX_DIRECTION}:{DIRECTION_HORIZONTAL}");
            }

            File.WriteAllLines(CONFIGFILE_NAME, lines);
        }

    }
}
