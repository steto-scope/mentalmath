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

            return Generate(r.Next(Config.MaxOperators - Config.MinOperators) + Config.MinOperators,(int)Config.MaxResult,1);
        }

        /// <summary>
        /// Generates recursively the Expression
        /// </summary>
        /// <param name="layer">The number of Operands to be in this Expression and Subexpressions</param>
        /// <returns></returns>
        private Expr Generate(int layer, int maximum, decimal factorof)
        {
            Expr e =  new Expr();
            int submax = r.Next(maximum);

           Expr a = null;
            Expr b = null;
            Expr temp = null;

           do
            {              
            //if there are 4 or more Operands to create, both A and B are Subexpressions 
            if (layer >= 3)
            {
               
                layer--;
                int sublayer = Math.Abs(layer / 2); //split the number of operands for balanced Expression tree
                e.Operator = RandomOperator();
                e.A = Generate(sublayer,submax,1);
                e.B = Generate(layer - sublayer,maximum-submax, e.Operator == Operator.Divide ? e.A.Solve() : 1);
                //e.Operator = Operator.Plus; //use Plus, Multiplay would cause extremly high numbers, for Division the B-Part has to be divider of A
            }
            else 
            {
                e.Operator = RandomOperator();
                if (layer > 1) //if there are more than 2 Operands (3) one of A or B have to be a subexpression
                {
                    //if (r.Next(1) == 1)
                    //    e.B = Generate(--layer,submax,e.Operator== Operator.Divide ? );
                    //else
                        e.A = Generate(--layer,submax,1);
                    
                }
            }

           

           
            //now fill the leafs of the expression-tree with numbers
            if (e.A == null)
            {
                switch(e.Operator)
                {
                    case Operator.Multiply: //not higher than the square-root of the MaxValue
                        a = new ExprValue(r.Next(maximum/2) + 1);
                        break;
                    case Operator.Divide: //no prime
                        decimal v=0;
                        do
                        {
                            v = (int)(r.Next((int)((double)maximum- Math.Sqrt((double)maximum))) + (decimal)Math.Sqrt((double)maximum));
                        } 
                        while (IsPrime((int)v));
                        a = new ExprValue(v);
                        break;
                    default:
                        a = new ExprValue(r.Next((int)maximum-1)+1);
                        break;
                }
                    
            }
            if (e.B == null)
            {
                switch(e.Operator)
                {
                    case Operator.Multiply: //not higher than squre-root of A
                        b = new ExprValue(r.Next((maximum-submax)/2) + 1);
                        break;
                    case Operator.Minus: //not below 0
                        b = new ExprValue(r.Next((int)(a??e.A).Solve() - 1) + 1);
                        break;
                    case Operator.Divide: //use a divider of A to avoid decimal places or even periodic/irrational results
                        var factors = CalculateFactors((int)(a??e.A).Solve());
                        decimal v=1;
                        if (factors.Count >= 1)
                            v = factors[r.Next(factors.Count)];
                        b = new ExprValue(v);
                        break;
                    default:
                        b = new ExprValue(r.Next(maximum-submax-1)+1);
                        break;
                }                    
            }
            temp = new Expr();
            temp.A = a ?? e.A;
            temp.B = b ?? e.B;
            temp.Operator = e.Operator;
            Console.WriteLine(temp+" = "+temp.Solve());
            }
           while( temp.Solve() % factorof != 0 );

            e.A = a ?? e.A;
            e.B = b ?? e.B;

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
