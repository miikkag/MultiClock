using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiClock
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public GlobalSettings Settings { get; private set; }

        public ObservableCollection<ClockData> Clocks { get; private set; }

        public MainWindow()
        {
            Settings = new GlobalSettings();

            Clocks = new ObservableCollection<ClockData>();

            int set_id = 1;

            foreach (string c in Settings.Clocks)
            {
                ClockParameters p = new ClockParameters(c);

                Clocks.Add(new ClockData(set_id, Settings, p));

                set_id++;
            }

            if (Clocks.Count == 0)
            {
                Clocks.Add(new ClockData(set_id, Settings, new ClockParameters()));
            }


            foreach (ClockData c in Clocks)
            {
                c.Start();
            }

            DataContext = new { Clocks, Settings };

            // Validate coordinates
            if ((Settings.X >= SystemParameters.VirtualScreenLeft) && (Settings.X < (SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth)) &&
                (Settings.Y >= SystemParameters.VirtualScreenTop) && (Settings.Y < (SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight)))
            {
                Left = Settings.X;
                Top = Settings.Y;
            }
            else
            {
                Left = 0;
                Top = 0;
            }

            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (ClockData c in Clocks)
            {
                c.Stop();
            }

            Save_Settings();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (sender is Window w)
                {
                    Settings.X = (int)w.Left;
                    Settings.Y = (int)w.Top;
                }
            }
        }


        private void ClockElement_Exit(object sender, RoutedEventArgs e)
        {
            foreach (ClockData c in Clocks)
            {
                c.Stop();
            }

            Application.Current.Shutdown();
        }

        private void ClockElement_Edit(object sender, RoutedEventArgs e)
        {
            int id = ((ClockData)sender).ID;

            foreach (ClockData c in Clocks)
            {
                if (c.ID == id)
                {
                    ClockParameters edit_params = c.Params.DeepCopy();

                    ClockSettingsWindow window_settings = new ClockSettingsWindow(edit_params);

                    bool? diag_result = window_settings.ShowDialog();

                    if (diag_result == true)
                    {
                        c.Params = edit_params;

                        Save_Settings();
                    }

                    break;
                }
            }
        }

        private void ClockElement_Add(object sender, RoutedEventArgs e)
        {
            int new_id = -1;

            foreach (ClockData c in Clocks)
            {
                new_id = Math.Max(new_id, c.ID);
            }

            ClockParameters edit_params = new ClockParameters();

            ClockSettingsWindow window_settings = new ClockSettingsWindow(edit_params);

            bool? diag_result = window_settings.ShowDialog();

            if (diag_result == true)
            {
                ClockData new_clock = new ClockData(new_id + 1, Settings, edit_params);

                new_clock.Start();

                Clocks.Add(new_clock);

                Save_Settings();
            }
        }

        private void ClockElement_Set_Horizontal(object sender, RoutedEventArgs e)
        {
            Settings.DirectionVertical = false;
            Settings.DirectionHorizontal = true;

            Save_Settings();
        }

        private void ClockElement_Set_Vertical(object sender, RoutedEventArgs e)
        {
            Settings.DirectionVertical = true;
            Settings.DirectionHorizontal = false;

            Save_Settings();
        }


        private void ClockElement_Delete(object sender, RoutedEventArgs e)
        {
            int id = ((ClockData)sender).ID;

            foreach (ClockData c in Clocks)
            {
                if (c.ID == id)
                {
                    MessageBoxResult confirm = MessageBox.Show("Are you sure to delete this clock:" + Environment.NewLine + c.Params.Caption,
                        "Confirm delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (confirm == MessageBoxResult.Yes)
                    {
                        c.Stop();

                        Clocks.Remove(c);
                    }

                    break;
                }
            }

            // Add default clock in case last clock was deleted
            if (Clocks.Count == 0)
            {
                ClockData cd = new ClockData(1, Settings, new ClockParameters());

                cd.Start();

                Clocks.Add(cd);
            }

            Save_Settings();
        }


        private void Save_Settings()
        {
            List<string> clock_strings = new List<string>();

            foreach (ClockData c in Clocks)
            {
                clock_strings.Add(c.Params.Make_Confstring());
            }

            Settings.Clocks = clock_strings;

            Settings.Write_Config();
        }
    }
}
