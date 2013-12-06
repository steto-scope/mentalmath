using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mentalmath
{
    /// <summary>
    /// Class to produce Expressions. See Generate()
    /// </summary>
    class ExprFactory
    {
        private Configs cnf;

        /// <summary>
        /// The Configuration for Expression-Generation
        /// </summary>
        public Configs Config
        {
            get { return cnf; }
            set { cnf = value; }
        }

        /// <summary>
        /// Random Generator
        /// </summary>
        private Random r = new Random();

        public ExprFactory(Configs c)
        {
            Config = c;
        }

        /// <summary>
        /// Creates a new Expression based on the Configuration (Config-Property)
        /// </summary>
        /// <returns></returns>
        public Expr Generate()
        {
            if (Config == null)
                return null;

            return Generate(r.Next(Config.MaxOperands+1 - Config.MinOperands) + Config.MinOperands);
        }

        /// <summary>
        /// Generates recursively the Expression
        /// </summary>
        /// <param name="layer">The number of Operands to be in this Expression and Subexpressions</param>
        /// <returns></returns>
        private Expr Generate(int layer)
        {
            Expr e =  new Expr();
            
            //if there are 4 or more Operands to create, both A and B are Subexpressions 
            if (layer >= 4)
            {
                int sublayer = Math.Abs(layer / 2); //split the number of operands for balanced Expression tree
                e.A = Generate(sublayer);
                e.B = Generate(layer - sublayer);
                e.Operator = Operator.Plus; //use Plus, Multiplay would cause extremly high numbers, for Division the B-Part has to be divider of A
            }
            else 
            {
                e.Operator = RandomOperator();
                if (layer > 2) //if there are more than 2 Operands (3) one of A or B have to be a subexpression
                {
                    if (r.Next(1) == 1)
                        e.B = Generate(--layer);
                    else
                        e.A = Generate(--layer);
                }
            }

            //now fill the leafs of the expression-tree with numbers
            if (e.A == null)
            {
                switch(e.Operator)
                {
                    case Operator.Multiply: //not higher than the square-root of the MaxValue
                        e.A = new ExprValue(r.Next((int)Config.MaxValue/4) + 1);
                        break;
                    case Operator.Divide: //no prime
                        decimal v=0;
                        do
                        {
                            v = (int)(r.Next((int)((double)Config.MaxValue - Math.Sqrt((double)Config.MaxValue))) + (decimal)Math.Sqrt((double)Config.MaxValue));
                        } 
                        while (IsPrime((int)v));
                        e.A = new ExprValue(v);
                        break;
                    default:
                        e.A = new ExprValue(r.Next((int)Config.MaxValue-1)+1);
                        break;
                }
                    
            }
            if (e.B == null)
            {
                switch(e.Operator)
                {
                    case Operator.Multiply: //not higher than squre-root of A
                        e.B = new ExprValue(r.Next((int)e.A.Solve()/4) + 1);
                        break;
                    case Operator.Minus: //not below 0
                        e.B = new ExprValue(r.Next((int)e.A.Solve() - 1) + 1);
                        break;
                    case Operator.Divide: //use a divider of A to avoid decimal places or even periodic/irrational results
                        var factors = CalculateFactors((int)e.A.Solve());
                        decimal v=1;
                        if (factors.Count >= 1)
                            v = factors[r.Next(factors.Count)];
                        e.B = new ExprValue(v);
                        break;
                    default:
                        e.B = new ExprValue(r.Next((int)Config.MaxValue-1)+1);
                        break;
                }                    
            }

            //add a brace-information for the output. If a subexpression is a line-operation and the expression is a point-operation the line-operation need to be braced
            if(e.Operator == Operator.Multiply || e.Operator == Operator.Divide)
            {
                if (e.A.Operator == Operator.Plus || e.A.Operator == Operator.Minus)
                    e.BraceA = true;
                if (e.B.Operator == Operator.Plus || e.B.Operator == Operator.Minus)
                    e.BraceB = true;
            }
            
            return e;
        }

        /// <summary>
        /// Generates a random operator. Depends on the Operator-Flags (Plus, Minus, Multiply, etc) in Config
        /// </summary>
        /// <returns></returns>
        private Operator RandomOperator()
        {
            List<int> ops = new List<int>();
            if (Config.Plus)
                ops.Add(0);
            if (Config.Minus)
                ops.Add(1);
            if (Config.Multiply)
                ops.Add(2);
            if (Config.Divide)
                ops.Add(3);

            int n = ops[r.Next(ops.Count)];
            switch(n)
            {
                case 1:
                    return Operator.Minus;
                case 2:
                    return Operator.Multiply;
                case 3:
                    return Operator.Divide;
                case 0:
                default:
                    return Operator.Plus;
            }
        }

        /// <summary>
        /// Checks if a number is a prime number
        /// </summary>
        /// <param name="candidate">number to be checked</param>
        /// <returns></returns>
        private static bool IsPrime(int candidate)
        {
            if ((candidate & 1) == 0)
            {
                if (candidate == 2)
                    return true;
                else
                    return false;
            }
            for (int i = 3; (i * i) <= candidate; i += 2)
            {
                if ((candidate % i) == 0)
                    return false;
            }
            return candidate != 1;
        }

        /// <summary>
        /// Calculates all Dividers of a number n. Multiples of Dividers too (for example, 24 -> 2,3,4,6,8,12
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static List<int> CalculateFactors(int n)
        {
            List<int> results = new List<int>();

            for(int i=2; i<=n/2; i++)
                if (n % i == 0)
                    results.Add(i);

            return results;
        }
    }
}
