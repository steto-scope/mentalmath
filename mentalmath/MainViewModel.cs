﻿using System;
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

       
        public MainViewModel()
        {
            Reset();
            
            Countdown = new CountdownTimer(new TimeSpan(0,0,(int)Config.GeneratorConfig.Countdown), ReportInterval.HundredthSecond);
            Countdown.RepeatForever = true;
            Countdown.CountdownAccomplished += Countdown_CountdownAccomplished;
          
        }

        private string profileName;

        public string ProfileName
        {
            get { return profileName; }
            set { profileName = value; Raise("ProfileName"); }
        }


        /// <summary>
        /// Triggers the EnterSolution-Comnmand if the countdown reaches zero.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Countdown_CountdownAccomplished(object sender, EventArgs e)
        {
            EnterSolutionCommand.Execute(null);
        }

        private ExprFactory factory;
        /// <summary>
        /// Resets the Program
        /// </summary>
        public void Reset()
        {
            Config = ApplicationConfiguration.Load();
            Config.GeneratorConfig.PropertyChanged += Config_PropertyChanged;
            factory = new ExprFactory(Config.GeneratorConfig);
            CurrentExpression = factory.Generate();
            NumCorrect = 0;
            NumIncorrect = 0;
            exprgen = new BackgroundWorker();
            exprgen.DoWork += exprgen_DoWork;
            exprgen.RunWorkerCompleted += exprgen_RunWorkerCompleted;
            exprgen.RunWorkerAsync();
        }

        /// <summary>
        /// Calculates new NextExpression if user changes Config (expression generation related stuff)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Config_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "Plus":
                case "Minus":   
                case "Multiply":
                case "Divide":
                case "MaxResult":
                case "MinOperators":
                case "MaxOperators":
                case "Countdown":
                    if (!exprgen.IsBusy)
                        exprgen.RunWorkerAsync();
                    Config.Save();
                    break;
            }
        }


        private ApplicationConfiguration conf;
        /// <summary>
        /// Configuration of the Application
        /// </summary>
        public ApplicationConfiguration Config
        {
            get { return conf; }
            set { conf = value; }
        }


        /// <summary>
        /// The Backgroundworker that generates the next expression
        /// </summary>
        private BackgroundWorker exprgen;


        /// <summary>
        /// Updates NextExpression
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void exprgen_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            NextExpression = (Expr)e.Result;
        }

        /// <summary>
        /// Implementation for Worker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void exprgen_DoWork(object sender, DoWorkEventArgs e)
        {
            bool failed;
            Expr exp=null;
            do
            {
                failed = false;
                try
                {
                    exp = factory.Generate();
                    if (exp.Solve() < 0)
                        throw new Exception();
                    e.Result = exp;
                }
                catch
                {
                    failed = true;
                }
                //Console.WriteLine(e.Result);
            }
            while (failed || e.Result==null || ((Expr)e.Result).Solve() > Config.GeneratorConfig.MaxResult);
        }

        #region Commands

        private ICommand enterSolution;
       
        public RelayCommand EnterSolutionCommand
        {
            get
            {
                if (enterSolution == null)
                    enterSolution = new RelayCommand(p=> EnterSolution(p));
                return (RelayCommand)enterSolution;
            }
        }
                
        /// <summary>
        /// The command executed by pressing enter 
        /// </summary>
        public void EnterSolution(object param)
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

            if (SolutionEntered != null)
                SolutionEntered(this, new SolutionEnteredEventArgs() { Correct = result });
        }



        private ICommand startstopcountdown;

        public RelayCommand StartStopCountdownCommand
        {
            get
            {
                if (startstopcountdown == null)
                    startstopcountdown = new RelayCommand(p=>StartStopCountdown(p));
                return (RelayCommand)startstopcountdown;
            }
        }

        public void StartStopCountdown(object param)
        {
            if (Countdown.IsRunning)
            {
                Countdown.Stop();
            }
            else
            {
                Countdown.Interval = new TimeSpan(0, 0, Config.GeneratorConfig.Countdown);
                Countdown.Start();
                Raise("CountdownTotal");
            }
        }


        private ICommand changeProfileCommand;

        public RelayCommand ChangeProfileCommand
        {
            get
            {
                if (changeProfileCommand == null)
                    changeProfileCommand = new RelayCommand(p=>ChangeProfile(p), p=> CanChangeProfile(p));
                return (RelayCommand)changeProfileCommand;
            }
        }

        public void ChangeProfile(object param)
        {
            if (param != null && param is GeneratorConfiguration)
            {
                Config.GeneratorConfig.Apply((GeneratorConfiguration)param);
                ProfileName = ((GeneratorConfiguration)param).Name;
            }
        }

        public bool CanChangeProfile(object param)
        {
            return !Countdown.IsRunning;
        }




        private ICommand saveProfileCommand;

        public RelayCommand SaveProfileCommand
        {
            get
            {
                if (saveProfileCommand == null)
                    saveProfileCommand = new RelayCommand(p=>SaveProfile(p));
                return (RelayCommand)saveProfileCommand;
            }
        }

        public void SaveProfile(object param)
        {
            string pname = ProfileName;
            
            if (!string.IsNullOrWhiteSpace(pname))
            {
                Config.GeneratorProfiles.Remove(p => p.Name == ProfileName);
                GeneratorConfiguration conf = Config.GeneratorConfig.Clone();
                conf.Name = pname;
                Config.GeneratorProfiles.Add(conf);
                Config.Raise("GeneratorProfiles");
            }
        }

        private ICommand removeProfileCommand;

        public RelayCommand RemoveProfileCommand
        {
            get
            {
                if (removeProfileCommand == null)
                    removeProfileCommand = new RelayCommand(p=>RemoveProfile(p), p=>CanRemoveProfile(p));
                return (RelayCommand)removeProfileCommand;
            }
        }

        public void RemoveProfile(object param)
        {
            Config.GeneratorProfiles.Remove(p => p.Name == ProfileName);
        }

        public bool CanRemoveProfile(object param)
        {
            return true;
        }


        #endregion

        /// <summary>
        /// Wrapper for Countdown.Interval.TotalMilleseconds to enable Notifications
        /// </summary>
        public int CountdownTotal
        {
            get
            {
                return (int)Countdown.Interval.TotalMilliseconds;
            }
        }

        public event EventHandler<SolutionEnteredEventArgs> SolutionEntered;

        /// <summary>
        /// NumCorrect to GridLength
        /// </summary>
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

        /// <summary>
        /// NumIncorrect to GridLength
        /// </summary>
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
            if(Countdown.IsRunning)
            {
                Countdown.Stop();
                Countdown.Start();
            }
            UserInput = "";
            CurrentExpression = NextExpression;
            if(!exprgen.IsBusy)
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
            set { numcorrect = value; Raise("NumCorrect"); Raise("HasAnswers"); Raise("NumCorrectPercent"); Raise("NumIncorrect"); Raise("NumIncorrectPercent"); Raise("IncorrectLength"); Raise("CorrectLength"); }
        }

        /// <summary>
        /// Checks if user has made answers
        /// </summary>
        public bool HasAnswers
        {
            get
            {
                return (NumCorrect + NumIncorrect) > 0;
            }
        }

        private int numincorrect;
        /// <summary>
        /// Number of incorrectly answered equations
        /// </summary>
        public int NumIncorrect
        {
            get { return numincorrect; }
            set { numincorrect = value; Raise("NumCorrect"); Raise("HasAnswers"); Raise("NumCorrectPercent"); Raise("NumIncorrect"); Raise("NumIncorrectPercent"); Raise("IncorrectLength"); Raise("CorrectLength"); }
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

        private CountdownTimer countdown;
        /// <summary>
        /// The Stopwatch
        /// </summary>
        public CountdownTimer Countdown
        {
            get { return countdown; }
            private set { countdown = value; }
        }



    }

    public class SolutionEnteredEventArgs : EventArgs
    {
        private bool correct;

        public bool Correct
        {
            get { return correct; }
            set { correct = value; }
        }

    }
}
