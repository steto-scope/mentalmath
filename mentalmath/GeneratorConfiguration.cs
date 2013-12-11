using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mentalmath
{
    [Serializable]
    public class GeneratorConfiguration : ConfigBase
    {
       
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

        private int minoperators;
        /// <summary>
        /// Number of minimum operators
        /// </summary>
        public int MinOperators
        {
            get { return minoperators; }
            set
            {
                minoperators = value;
                if (maxoperators < value)
                    maxoperators = value;

                Raise("MinOperators"); Raise("MaxOperators"); 
            }
        }

        private int maxoperators;
        /// <summary>
        /// Number of maximum operators
        /// </summary>
        public int MaxOperators
        {
            get { return maxoperators; }
            set
            {
                maxoperators = value;
                if (minoperators > value)
                    minoperators = value;

                Raise("MinOperators"); Raise("MaxOperators");
            }
        }


        public GeneratorConfiguration()
        {
            AllowedDecimalPlaces = 0;
            MaxResult = 999;
            MaxValue = 500;
            Plus = true;
            Minus = true;
            Multiply = true;
            Divide = true;
            Countdown = 10;
            MinOperators = 1;
            MaxOperators = 1;
        }
               
        /// <summary>
        /// Clones the Object (only Properties)
        /// </summary>
        /// <returns></returns>
        public GeneratorConfiguration Clone()
        {
            GeneratorConfiguration c = new GeneratorConfiguration();
            foreach (PropertyInfo pi in GetType().GetProperties())
                pi.SetValue(c, pi.GetValue(this));
            return c;
        }

        /// <summary>
        /// Applies all Properties of the given object
        /// </summary>
        /// <param name="c"></param>
        public void Apply(GeneratorConfiguration c)
        {
            foreach(PropertyInfo pi in GetType().GetProperties())
                pi.SetValue(this, pi.GetValue(c));
        }
    }
}
