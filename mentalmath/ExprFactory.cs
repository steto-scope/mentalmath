using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mentalmath
{
    class ExprFactory
    {
        private Configs cnf;

        public Configs Config
        {
            get { return cnf; }
            private set { cnf = value; }
        }

        private Random r = new Random();

        public ExprFactory(Configs c)
        {
            Config = c;
        }

        public Expr Generate()
        {
            return Generate(r.Next(Config.MaxOperands+1 - Config.MinOperands) + Config.MinOperands);
        }

        public Expr Generate(int layer)
        {
            Expr e =  new Expr();
            e.Operator = RandomOperator(layer <= 2);
            if(layer > 2)
            {
                if(r.Next(1)==1)
                    e.B = Generate(--layer);
                else 
                    e.A = Generate(--layer);
            }
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
                        } while (IsPrime((int)v));
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
                    case Operator.Divide:
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

            
            return e;
        }

        public Operator RandomOperator(bool canmult)
        {
            List<int> ops = new List<int>();
            if (Config.Plus)
                ops.Add(0);
            if (Config.Minus)
                ops.Add(1);
            bool nos = ops.Count == 0;
            if (Config.Multiply && (canmult||nos))
                ops.Add(2);
            if (Config.Divide && (canmult || nos))
                ops.Add(3);

            int n = ops[r.Next(ops.Count)];
           /* if (!canmult && n > 1)
                n -= 2;*/
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

        public static bool IsPrime(int candidate)
        {
            // Test whether the parameter is a prime number.
            if ((candidate & 1) == 0)
            {
                if (candidate == 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // Note:
            // ... This version was changed to test the square.
            // ... Original version tested against the square root.
            // ... Also we exclude 1 at the end.
            for (int i = 3; (i * i) <= candidate; i += 2)
            {
                if ((candidate % i) == 0)
                {
                    return false;
                }
            }
            return candidate != 1;
        }

        public static List<int> CalculateFactors(int n)
        {
            List<int> results = new List<int>();

            for(int i=2; i<Math.Sqrt(n); i++)
            {
                if (n % i == 0)
                    results.Add(i);
            }
            return results;
        }
    }
}
