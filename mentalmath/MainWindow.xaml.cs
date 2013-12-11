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
                ((MainViewModel)DataContext).SolutionEntered += MainWindow_SolutionEntered;
                input.Focus();
                Keyboard.Focus(input);
            }
        }

        /// <summary>
        /// Shows Effect, if a solution has been sent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_SolutionEntered(object sender, SolutionEnteredEventArgs e)
        {
            Storyboard s;
            if (e.Correct)
                s = (Storyboard)TryFindResource("sok");
            else
                s = (Storyboard)TryFindResource("snok");
            s.Begin();
        }

        private void input_KeyDown(object sender, KeyEventArgs e)
        {
            //exit
            if (e.Key.Equals(Key.Escape))
                Close();

            //start or stop countdown
            if (e.Key.Equals(Key.Multiply) && DataContext != null)
            {
                ((MainViewModel)DataContext).StartStopCountdownCommand.Execute(null);
                e.Handled = true;

            }

            //executes the validation and shows the result
            if (e.Key.Equals(Key.Enter) && DataContext != null && sender is TextBox)
            {
               ((MainViewModel)DataContext).EnterSolutionCommand.Execute(null);
            }
        }

        private void ComboBox_Selected(object sender, RoutedEventArgs e)
        {
            if(DataContext!=null && DataContext is MainViewModel && sender is ComboBox)
            {
                ComboBox box = (ComboBox)sender;
                ((MainViewModel)DataContext).ChangeProfileCommand.Execute(box.SelectedItem);
            }
        }
        

    }
}
