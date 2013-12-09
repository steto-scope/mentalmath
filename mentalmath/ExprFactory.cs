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

            List<Operator> operators = new List<Operator>();
            int numOperators = r.Next(Config.MaxOperators - Config.MinOperators) + Config.MinOperators;
            for(int i=0; i<numOperators ; i++)
            {
                operators.Add(RandomOperator());
            }

            return Generate(operators, false, (int)Config.MaxResult, null, null);
        }

        /// <summary>
        /// Generates recursively the Expression
        /// </summary>
        /// <param name="layer">The number of Operands to be in this Expression and Subexpressions</param>
        /// <returns></returns>
        private Expr Generate(List<Operator> operators, bool notprime,  int maxresult, int? minresult, decimal? factorof)
        {
            
            Expr e = new Expr();
            e.Operator = operators.First();
            operators.RemoveAt(0);

            switch(operators.Count)
            {
                case 0: //leaf reached, built expression
                    switch(e.Operator)
                    {
                        case Operator.Divide:
                            if (factorof == null)
                                e.A = new ExprValue(GenerateNumber(true, 1, maxresult)); //use a random number if it does not have to match with a factor
                            else
                                e.A = new ExprValue(GenerateNumber((int)factorof)); //use a random divider of factorof
                            e.B = new ExprValue(GenerateNumber((int)e.A.Solve()));
                            break;
                       case Operator.Minus:
                            e.A = new ExprValue(GenerateNumber(false,minresult??0,maxresult));
                            if (factorof == null)
                            {
                                e.B = new ExprValue(GenerateNumber(false, 0, (int)e.A.Solve()));
                            }
                            else
                            {
                                int w = (int)e.A.Solve();
                                var m = GenerateNumber(false, 1, w); //pick a random number between 0 and maxresult
                                int closest = CalculateFactors((int)factorof).ClosestTo((int)m); //get the biggest divider of maxresult that is smaller than the generated number
                                e.B = new ExprValue(closest==0 ? 0 : w-closest); //and use the difference (if the final result is not 0)
                            }
                            break;
                        case Operator.Multiply:
                            if (factorof == null)
                            {
                                e.A = new ExprValue(GenerateNumber(false, 1, (int)Math.Sqrt(maxresult) + 1));
                                e.B = new ExprValue(GenerateNumber(false, 1, (int)Math.Sqrt(maxresult) + 1));
                            }
                            else
                            {
                                e.A = new ExprValue(GenerateNumber((int)factorof)); //pick a random divider of factorof
                                e.B = new ExprValue(factorof.Value / e.A.Solve()); //use the division, so that factorof is dividable by A*B 
                            }
                            break;
                        default:
                            if (factorof == null)
                            {
                                e.A = new ExprValue(GenerateNumber(false, minresult ?? 0, maxresult / 2));
                                e.B = new ExprValue(GenerateNumber(false, minresult ?? 0, maxresult / 2));
                            }
                            else
                            {
                                List<int> factors = CalculateFactors((int)factorof);
                                int num = factors[r.Next(factors.Count)];
                                e.A = new ExprValue(GenerateNumber(false,0,num));
                                e.B = new ExprValue(factorof.Value - e.A.Solve());
                            }
                            break;
                    }
                    break;

                case 1: 
                    switch(e.Operator)
                    {
                        case Operator.Divide:
                            e.A = Generate(operators, true, maxresult, 1, factorof);
                            e.B = new ExprValue(GenerateNumber((int)e.A.Solve()));
                            break;
                        case Operator.Multiply:
                            e.A = Generate(operators, false, (int)Math.Sqrt(maxresult)+1, null, factorof);
                            e.B = new ExprValue(GenerateNumber(false,0,(int)Math.Sqrt(maxresult)+1));
                            break;
                        case Operator.Minus:
                            e.A = Generate(operators, false, maxresult, null, factorof);
                            e.B = new ExprValue(GenerateNumber(false,0,(int)e.A.Solve()));
                            break;
                        default:
                            e.A = Generate(operators, false, maxresult/2, null, factorof);
                            e.B = new ExprValue(GenerateNumber(false,0,maxresult/2));
                            break;
                    }

                    break;

                case 2:
                default:
                    List<Operator> a = operators.Take((int)operators.Count / 2).ToList();
                    List<Operator> b = operators.Skip(a.Count).ToList();
                    Console.WriteLine(a.Count + ":" + ", " + b.Count + ":" );
                    switch(e.Operator)
                    {
                        case Operator.Divide:
                            e.A = Generate(a, true,  maxresult, minresult, factorof); //should not be a prime, this is boring
                            e.B = Generate(b, false, (int)e.A.Solve(), minresult, e.A.Solve()); //has to be a factor of A
                            break;
                        case Operator.Minus:
                            e.A = Generate(a, false, maxresult, minresult, factorof);
                            e.B = Generate(b, false, (int)e.A.Solve(), null, factorof);
                            break;
                        case Operator.Multiply:
                            e.A = Generate(a, true, (int)Math.Sqrt(maxresult) + 1, minresult, factorof);
                            e.B = Generate(b, true, (int)Math.Sqrt(maxresult) + 1, minresult, factorof);
                            break;
                        case Operator.Plus:
                            e.A = Generate(a, true, maxresult/2, minresult, factorof);
                            e.B = Generate(b, true, maxresult/2, minresult, factorof);
                            break;
                    }

                    

                    break;
            }

            return e;
        }

        /// <summary>
        /// Generates a random number by specified requirements
        /// </summary>
        /// <param name="notprime">true, if the number should not be a prime</param>
        /// <param name="positive">true, if only positive numbers allowed (not used yet)</param>
        /// <param name="min">Minimum range [0,max]. Absolute value</param>
        /// <param name="max">Maximum range [min,). Absolute value</param>
        /// <returns></returns>
        public decimal GenerateNumber(bool notprime,  int min, int max)
        {
            if (min < 0 || max < 0)
                throw new Exception("min and max have to be positive");
            if (min > max)
                throw new Exception("min has to be smaller than max");

            if (min - max == 0) //special case
                return 0;

            decimal d;
            do
            {
                d = r.Next(max-min)+min;
            }
            while (notprime && IsPrime((int)d));
            return d;
        }

        /// <summary>
        /// Generates a random number that is a factor of a number
        /// </summary>
        /// <param name="factorof">[1,)</param>
        /// <returns></returns>
        public decimal GenerateNumber(int factorof)
        {
            if (factorof < 1)
                throw new Exception("factorof has to be greater than 0");

            List<int> factors = CalculateFactors(factorof);
            if (factors.Count > 0)
                return factors[r.Next(factors.Count)];
            return 1;
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
        /// Calculates all Dividers of a number n. Multiples of Dividers too (for example, 24 -> 1,2,3,4,6,8,12,24
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static List<int> CalculateFactors(int n)
        {
            List<int> results = new List<int>();

            for(int i=1; i<=n/2; i++)
                if (n % i == 0)
                    results.Add(i);
            results.Add(n);

            return results;
        }
    }
}
