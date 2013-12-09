using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mentalmath
{
    public class Configs : INotifyPropertyChanged
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


        public Configs()
        {
            AllowedDecimalPlaces = 0;
            MaxResult = 999;
            MaxValue = 500;
            Root = false;
            Plus = true;
            Minus = true;
            Multiply = true;
            Divide = true;
            Countdown = 10;
            MinOperators = 1;
            MaxOperators = 1;
        }


        public void Save()
        {
            try
            {
                XmlSerializer s = new XmlSerializer(this.GetType());
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/mentalmath/"))
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/mentalmath/");
                FileStream fs = File.OpenWrite(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/mentalmath/config.xml");
                s.Serialize(fs, this);
                fs.Close();
            }
            catch //catch possible write-protection. in this case do not save configs
            {
            }
        }

        public static Configs Load()
        {
            try
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/mentalmath/config.xml"))
                {
                    FileStream fs = File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/mentalmath/config.xml");
                    XmlSerializer s = new XmlSerializer(typeof(Configs));
                    Configs conf = (Configs)s.Deserialize(fs);
                    fs.Close();
                    return conf;
                }
            }
            catch
            {
            }
            return new Configs(); //no or invalid savefile? return defaults
        }


    }
}
