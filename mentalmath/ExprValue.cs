using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mentalmath
{
    class ExprValue : Expr
    {
        public override bool IsValue
        {
            get
            {
                return true;
            }
        }

        private decimal val;

        public decimal Value
        {
            get { return val; }
            set { val = value; }
        }

        public ExprValue(decimal v)
        {
            Value = v;
        }

        public override decimal Solve()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }


        

    }
}
