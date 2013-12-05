using MahApps.Metro.Controls;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace mentalmath
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext != null)
            {
                ((MainViewModel)DataContext).Reset();
                input.Focus();
                Keyboard.Focus(input);
                
            }
        }

        private void input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Escape))
                Close();

            // if user didn't press Enter, do nothing
            if (!e.Key.Equals(Key.Enter)) return;

            // execute the command, if it exists
            if (DataContext != null)
            {
                bool res = ((MainViewModel)DataContext).EnterSolutionCommand.Execute(null);
                Storyboard s;
                if(res)
                    s = (Storyboard)TryFindResource("sok");
                else
                    s = (Storyboard)TryFindResource("snok");
                s.Begin();
            }
        }


    }
}
