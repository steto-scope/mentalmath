using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace mentalmath
{
    public class RelayCommand : ICommand
    {

        #region private fields

        private readonly Func<bool> execute;

        private readonly Func<bool> canExecute;

        #endregion



        public event EventHandler CanExecuteChanged
        {

            // wire the CanExecutedChanged event only if the canExecute func

            // is defined (that improves perf when canExecute is not used)

            add
            {

                if (this.canExecute != null)

                    CommandManager.RequerySuggested += value;

            }

            remove
            {

                if (this.canExecute != null)

                    CommandManager.RequerySuggested -= value;

            }

        }



        /// <summary>

        /// Initializes a new instance of the RelayCommand class

        /// </summary>

        /// <param name="execute">The execution logic.</param>

        public RelayCommand(Func<bool> execute)

            : this(execute, null)
        {

        }



        /// <summary>

        /// Initializes a new instance of the RelayCommand class

        /// </summary>

        /// <param name="execute">The execution logic.</param>

        /// <param name="canExecute">The execution status logic.</param>

        public RelayCommand(Func<bool> execute, Func<bool> canExecute)
        {

            if (execute == null)

                throw new ArgumentNullException("execute");



            this.execute = execute;

            this.canExecute = canExecute;

        }



        public bool Execute(object parameter)
        {

            return this.execute();

        }



        public bool CanExecute(object parameter)
        {

            return this.canExecute == null ? true : this.canExecute();

        }



        void ICommand.Execute(object parameter)
        {
            this.execute();
        }
    }
}
