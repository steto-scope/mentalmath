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
        /// <summary>
        /// Plus Operator is allowed
        /// </summary>
        public bool Plus
        {
            get { return plus; }
            set
            {
                if (Divide || Multiply || Minus || value)
                {
                    plus = value;
                    Raise("Plus");
                }
            }
        }

        private bool minus;
        /// <summary>
        /// Minus-Operator is allowed
        /// </summary>
        public bool Minus
        {
            get { return minus; }
            set
            {
                if (Plus || Multiply || Divide || value)
                { 
                    minus = value;
                    Raise("Minus"); 
                }
            }
        }

        private bool multiply;
        /// <summary>
        /// Multiply-Operator is allowed
        /// </summary>
        public bool Multiply
        {
            get { return multiply; }
            set 
            {
                if (Plus || Divide || Minus || value)
                { 
                    multiply = value;
                    Raise("Multiply");
                }
            }
        }

        private bool divide;
        /// <summary>
        /// Division-Operator is allowed
        /// </summary>
        public bool Divide
        {
            get { return divide; }
            set
            {
                if (Plus || Multiply || Minus || value)
                {
                    divide = value; 
                    Raise("Divide"); 
                }
            }
        }


        private bool root;
        /// <summary>
        /// Root-Operator is allowed. Not used yet.
        /// </summary>
        public bool Root
        {
            get { return root; }
            set { root = value; Raise("Root"); }
        }

        private decimal maxvalue;
        /// <summary>
        /// Maximum Operand Value
        /// </summary>
        public decimal MaxValue
        {
            get { return maxvalue; }
            set { maxvalue = value; Raise("MaxValue"); }
        }

        private int allowedDecimalPlaces;
        /// <summary>
        /// Number of decimal places in Operands or Results. Not used yet.
        /// </summary>
        public int AllowedDecimalPlaces
        {
            get { return  allowedDecimalPlaces; }
            set { allowedDecimalPlaces = value; Raise("AllowedDecimalPlaces"); }
        }

        private int minOperands;
        /// <summary>
        /// Minimum number of Operands. 
        /// </summary>
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
        /// <summary>
        /// Maximim number of Operands. 
        /// </summary>
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
        /// <summary>
        /// Maximum depth of Equations. Not used yet.
        /// </summary>
        public int MaxLayers
        {
            get { return maxLayers; }
            set { maxLayers = value; Raise("MaxLayers"); }
        }

        private decimal maxresult;
        /// <summary>
        /// Maximum of the Result. Not used yet.
        /// </summary>
        public decimal MaxResult
        {
            get { return maxresult; }
            set { maxresult = value; Raise("MaxResult"); }
        }

        private int countdown;
        /// <summary>
        /// Number of seconds for the countdown
        /// </summary>
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
