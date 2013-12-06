using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace mentalmath
{
    /// <summary>
    /// ViewModel for the Main Window
    /// </summary>
    class MainViewModel : ViewModelBase
    {
        private Expr current;

        /// <summary>
        /// The current Expression (visible on screen)
        /// </summary>
        public Expr CurrentExpression
        {
            get { return current; }
            set { current = value; Raise("CurrentExpression"); }
        }

        private Expr nextExpression;
        /// <summary>
        /// The following expression (after CurrentExpression has been answered)
        /// </summary>
        public Expr NextExpression
        {
            get { return nextExpression; }
            set { nextExpression = value; }
        }

        /// <summary>
        /// The Backgroundworker that generates the next expression
        /// </summary>
        private BackgroundWorker exprgen;

        public MainViewModel()
        {
            exprgen = new BackgroundWorker();
            exprgen.DoWork += exprgen_DoWork;
            exprgen.RunWorkerCompleted += exprgen_RunWorkerCompleted;
        }

        private ExprFactory factory;
        /// <summary>
        /// Resets the Program
        /// </summary>
        public void Reset()
        {
            Config = new Configs();
            factory = new ExprFactory(Config);
            CurrentExpression = factory.Generate();
            NumCorrect = 0;
            NumIncorrect = 0;
            exprgen.RunWorkerAsync();
        }

        private Configs config;
        /// <summary>
        /// The Configuration used for Expression Generation
        /// </summary>
        public Configs Config
        {
            get { return config; }
            set { config = value; }
        }

        /// <summary>
        /// Updates NextExpression
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void exprgen_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            NextExpression = (Expr)e.Result;
        }

        void exprgen_DoWork(object sender, DoWorkEventArgs e)
        {
            bool failed;
            do
            {
                failed = false;
                try
                {
                    e.Result = factory.Generate();
                }
                catch
                {
                    failed = true;
                }
            }
            while (failed);
        }

        private ICommand enterSolution;
       
        public RelayCommand EnterSolutionCommand
        {
            get
            {
                if (enterSolution == null)
                    enterSolution = new RelayCommand(new Func<bool>(EnterSolution));
                return (RelayCommand)enterSolution;
            }
        }

        /// <summary>
        /// The command executed by pressing enter 
        /// </summary>
        public bool EnterSolution()
        {
            decimal d = 0;
            bool erg = decimal.TryParse(UserInput, out d);
            bool result = false;
            if (erg && d == CurrentExpression.Solve())
            {
                NumCorrect++;
                result = true;
            }
            else
                NumIncorrect++;

            ShowNext();
            return result;
        }

        public GridLength CorrectLength
        {
            get 
            {
                if (NumIncorrect + NumCorrect != 0)
                {
                    double d = (double)NumCorrect / (double)(NumCorrect + NumIncorrect);
                    return new GridLength(d * 100, GridUnitType.Star);
                }
                return new GridLength(1,GridUnitType.Star);
            }
        }
        public GridLength IncorrectLength
        {
            get
            {
                if (NumIncorrect + NumCorrect != 0)
                {
                    double d = (double)NumIncorrect / (double)(NumCorrect + NumIncorrect);
                    return new GridLength(d * 100, GridUnitType.Star);
                }
                return new GridLength(0);
            }
        }
        /// <summary>
        /// Displays next Expression
        /// </summary>
        private void ShowNext()
        {
            UserInput = "";
            CurrentExpression = NextExpression;
            exprgen.RunWorkerAsync();
        }

        private string userInput;
        /// <summary>
        /// Input of the User
        /// </summary>
        public string UserInput
        {
            get { return userInput; }
            set { userInput = value; Raise("UserInput"); }
        }

        private int numcorrect;

        /// <summary>
        /// Number of correctly answered equations
        /// </summary>
        public int NumCorrect
        {
            get { return numcorrect; }
            set { numcorrect = value; Raise("NumCorrect"); Raise("NumCorrectPercent"); Raise("NumIncorrect"); Raise("NumIncorrectPercent"); Raise("IncorrectLength"); Raise("CorrectLength"); }
        }

        private int numincorrect;
        /// <summary>
        /// Number of incorrectly answered equations
        /// </summary>
        public int NumIncorrect
        {
            get { return numincorrect; }
            set { numincorrect = value; Raise("NumCorrect"); Raise("NumCorrectPercent"); Raise("NumIncorrect"); Raise("NumIncorrectPercent"); Raise("IncorrectLength"); Raise("CorrectLength"); }
        }
        /// <summary>
        /// NumIncorrect in percentage
        /// </summary>
        public string NumIncorrectPercent
        {
            get
            {
                return Math.Round((double)NumIncorrect / (double)(NumCorrect + NumIncorrect),2)*100 + "%";
            }
        }
        /// <summary>
        /// NumCorrect in percentage
        /// </summary>
        public string NumCorrectPercent
        {
            get
            {
                return Math.Round((double)NumCorrect /   (double)(NumCorrect + NumIncorrect),2)*100  + "%";
            }
        }

        private Stopwatch watch;
        /// <summary>
        /// The Stopwatch
        /// </summary>
        public Stopwatch Watch
        {
            get { return watch; }
            set { watch = value; }
        }



    }
}
