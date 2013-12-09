using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mentalmath
{
    /// <summary>
    /// Class the represents an mathematical expression
    /// </summary>
    class Expr
    {
        private Expr a;
        /// <summary>
        /// First Operand
        /// </summary>
        public Expr A
        {
            get { return a; }
            set { a = value; }
        }

        private Expr b;
        /// <summary>
        /// Second Operand
        /// </summary>
        public Expr B
        {
            get { return b; }
            set { b = value; }
        }

        private Operator op;
        /// <summary>
        /// The Operator of this Expression
        /// </summary>
        public Operator Operator
        {
            get { return op; }
            set { op = value; }
        }

        /// <summary>
        /// Determines if this Object is an actual number
        /// </summary>
        public virtual bool IsValue
        {
            get
            {
                return false;
            }
        }


        /// <summary>
        /// Solves the equation
        /// </summary>
        /// <returns></returns>
        public virtual decimal Solve()
        {
            decimal a = A!=null ? A.Solve() : 0;
            decimal b = B!=null ? B.Solve() : 1;

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

        /// <summary>
        /// Builts the visual representation of the expression
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (A == null || B == null)
                return "";

            StringBuilder sb = new StringBuilder();
            if ( ((Operator == mentalmath.Operator.Divide) ||
                 (A.Operator == mentalmath.Operator.Plus && Operator != mentalmath.Operator.Plus)||
                 (A.Operator == mentalmath.Operator.Minus && Operator != mentalmath.Operator.Plus) ||
                 (A.Operator == mentalmath.Operator.Divide && Operator == mentalmath.Operator.Multiply))&&
                (A.GetType() == typeof(Expr)))
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

                if ( ( (Operator == mentalmath.Operator.Divide) ||
                       (Operator == mentalmath.Operator.Multiply && B.Operator!= mentalmath.Operator.Multiply) ||
                        (B.Operator == mentalmath.Operator.Minus) ||
                        (Operator == mentalmath.Operator.Minus && B.Operator == mentalmath.Operator.Plus)) &&
                    (B.GetType() == typeof(Expr)) )
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

        /// <summary>
        /// Gets the visual representation for an Operator
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static char OperatorToChar(Operator o)
        {
            switch(o)
            {
                case Operator.Minus:
                    return '-';
                case Operator.Multiply:
                    return 'x';
                case Operator.Divide:
                    return '÷';
                case Operator.Plus:
                default:
                    return '+';
            }
        }
    }
}
