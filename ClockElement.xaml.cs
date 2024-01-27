using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class ClockElement : UserControl
    {
        public event RoutedEventHandler? Edit;
        public event RoutedEventHandler? Exit;
        public event RoutedEventHandler? Add;
        public event RoutedEventHandler? Delete;
        public event RoutedEventHandler? Set_Horizontal;
        public event RoutedEventHandler? Set_Vertical;

        public ClockElement()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Exit?.Invoke(DataContext, null);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Edit?.Invoke(DataContext, null);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Delete?.Invoke(DataContext, null);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Add?.Invoke(DataContext, null);
        }

        private void Direction_Horizontal_Click(object sender, RoutedEventArgs e)
        {
            Set_Horizontal?.Invoke(DataContext, null);
        }

        private void Direction_Vertical_Click(object sender, RoutedEventArgs e)
        {
            Set_Vertical?.Invoke(DataContext, null);
        }
    }
}
