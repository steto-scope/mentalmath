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
    class MainViewModel : ViewModelBase
    {
        private Expr current;

        public Expr CurrentExpression
        {
            get { return current; }
            set { current = value; Raise("CurrentExpression"); }
        }

        private Expr nextExpression;

        public Expr NextExpression
        {
            get { return nextExpression; }
            set { nextExpression = value; }
        }

        private BackgroundWorker exprgen;

        public MainViewModel()
        {
            exprgen = new BackgroundWorker();
            exprgen.DoWork += exprgen_DoWork;
            exprgen.RunWorkerCompleted += exprgen_RunWorkerCompleted;
        }

        private ExprFactory factory;

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

        public Configs Config
        {
            get { return config; }
            set { config = value; }
        }


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

        private void ShowNext()
        {
            UserInput = "";
            CurrentExpression = NextExpression;
            exprgen.RunWorkerAsync();
        }

        private string userInput;

        public string UserInput
        {
            get { return userInput; }
            set { userInput = value; Raise("UserInput"); }
        }

        private int numcorrect;

        public int NumCorrect
        {
            get { return numcorrect; }
            set { numcorrect = value; Raise("NumCorrect"); Raise("NumCorrectPercent"); Raise("NumIncorrect"); Raise("NumIncorrectPercent"); Raise("IncorrectLength"); Raise("CorrectLength"); }
        }

        private int numincorrect;

        public int NumIncorrect
        {
            get { return numincorrect; }
            set { numincorrect = value; Raise("NumCorrect"); Raise("NumCorrectPercent"); Raise("NumIncorrect"); Raise("NumIncorrectPercent"); Raise("IncorrectLength"); Raise("CorrectLength"); }
        }

        public string NumIncorrectPercent
        {
            get
            {
                return Math.Round((double)NumIncorrect / (double)(NumCorrect + NumIncorrect),2)*100 + "%";
            }
        }
        public string NumCorrectPercent
        {
            get
            {
                return Math.Round((double)NumCorrect /   (double)(NumCorrect + NumIncorrect),2)*100  + "%";
            }
        }

        private Stopwatch watch;

        public Stopwatch Watch
        {
            get { return watch; }
            set { watch = value; }
        }



    }
}
