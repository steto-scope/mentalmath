using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mentalmath
{
    class Configs : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Raise(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private bool plus;

        public bool Plus
        {
            get { return plus; }
            set { plus = value; Raise("Plus"); }
        }

        private bool minus;

        public bool Minus
        {
            get { return minus; }
            set { minus = value; Raise("Minus"); }
        }

        private bool multiply;

        public bool Multiply
        {
            get { return multiply; }
            set { multiply = value; Raise("Multiply"); }
        }

        private bool divide;

        public bool Divide
        {
            get { return divide; }
            set { divide = value; Raise("Divide"); }
        }


        private bool root;

        public bool Root
        {
            get { return root; }
            set { root = value; Raise("Root"); }
        }

        private decimal maxvalue;

        public decimal MaxValue
        {
            get { return maxvalue; }
            set { maxvalue = value; Raise("MaxValue"); }
        }

        private int allowedDecimalPlaces;

        public int AllowedDecimalPlaces
        {
            get { return  allowedDecimalPlaces; }
            set { allowedDecimalPlaces = value; Raise("AllowedDecimalPlaces"); }
        }

        private int minOperands;

        public int MinOperands
        {
            get { return minOperands; }
            set 
            { 
                minOperands = value; 
                if (maxOperands < value) 
                    maxOperands = value;

                Raise("MinOperands");
                Raise("MaxOperands"); }
        }

        private int maxOperands;

        public int MaxOperands
        {
            get { return maxOperands; }
            set {
                maxOperands = value;
                if (minOperands > value)
                    minOperands = value;

                Raise("MinOperands"); Raise("MaxOperands"); }
        }

        private int maxLayers;

        public int MaxLayers
        {
            get { return maxLayers; }
            set { maxLayers = value; Raise("MaxLayers"); }
        }

        private decimal maxresult;

        public decimal MaxResult
        {
            get { return maxresult; }
            set { maxresult = value; Raise("MaxResult"); }
        }

        private int countdown;

        public int Countdown
        {
            get { return countdown; }
            set { countdown = value; Raise("Countdown"); }
        }
        

        public Configs()
        {
            MaxLayers = 1;
            MaxOperands = 2;
            MinOperands = 2;
            AllowedDecimalPlaces = 0;
            MaxResult = 999;
            MaxValue = 500;
            Root = false;
            Plus = true;
            Minus = true;
            Multiply = true;
            Divide = true;
            Countdown = 10;
        }


    }
}
