using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mentalmath
{
    class Expr
    {
        private Expr a;

        public Expr A
        {
            get { return a; }
            set { a = value; }
        }

        private Expr b;

        public Expr B
        {
            get { return b; }
            set { b = value; }
        }

        private Operator op;

        public Operator Operator
        {
            get { return op; }
            set { op = value; }
        }

        public virtual bool IsValue
        {
            get
            {
                return false;
            }
        }

        private bool bracea;

        public bool BraceA
        {
            get { return bracea && !a.IsValue; }
            set { bracea = value; }
        }

        private bool braceb;

        public bool BraceB
        {
            get { return braceb && !b.IsValue; }
            set { braceb = value; }
        }

        public virtual decimal Solve()
        {
            decimal a = A.Solve();
            decimal b = B.Solve();

            decimal result = 0;

            switch(Operator)
            {
                
                case mentalmath.Operator.Minus:
                    result = a - b;
                    break;
                case mentalmath.Operator.Multiply:
                    result = a * b;
                    break;
                case mentalmath.Operator.Divide:
                    result = a / b;
                    break;
                case mentalmath.Operator.Plus:
                default:
                    result = a + b;
                    break;
            }
            return result;
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (BraceA)
            {
                sb.Append("(");
                sb.Append(A.ToString());
                sb.Append(")");
            }
            else
                sb.Append(A.ToString());

            if(!IsValue)
            {
                sb.Append(" ");
                sb.Append(OperatorToChar(Operator));
                sb.Append(" ");

                if (BraceB)
                {
                    sb.Append("(");
                    sb.Append(B.ToString());
                    sb.Append(")");
                }
                else
                    sb.Append(B.ToString());
            }

            return sb.ToString();
        }

        public static char OperatorToChar(Operator o)
        {
            switch(o)
            {
                case Operator.Minus:
                    return '-';
                case Operator.Multiply:
                    return 'x';
                case Operator.Divide:
                    return ':';
                case Operator.Plus:
                default:
                    return '+';
            }
        }
    }
}
