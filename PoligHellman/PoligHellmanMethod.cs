using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoligHellman
{
    class PoligHellmanMethod
    {
        #region Fields
        private List<lib.Factor> factors;

        private List<lib.Remainder> remainders = new List<lib.Remainder>();

        private Int64 H { get; set; }

        private Int64 G { get; set; }

        private Int64 N { get { return MainN - 1; } }

        private Int64 MainN { get; set; }

        private lib solution;

        #endregion

        #region Constructors
        public PoligHellmanMethod(Int64 H, Int64 G, Int64 N)
        {
            this.H = H;
            this.G = G;
            MainN = N;
            solution = new lib();
            DivideIntoFactors();
        }
        #endregion

        #region Functions

        private void DivideIntoFactors()
        {
            factors = solution.FactorizationOfTheNumber(N);
        }

        public Int64 GetX()
        {
            Parallel.ForEach(factors, factor =>
            {
                lock (remainders)
                {
                    remainders.Add(new lib.Remainder()
                    {
                        a = CalculateXi(factor),
                        m = factor.ToCompositeNumber()
                    });
                }
            });
            return solution.ChineseReminderTheorem(remainders);
        }

        public Int64 CalculateXi(lib.Factor factor)
        {
            if (factor.alpha != 1)
            {
                List<long> C = new List<long>();

                long PrevH = H;

                for (int i = 0; i < factor.alpha; i++)
                {
                    if (C.Count == 0)
                    {
                        var t1 = solution.logB((long)solution.ModPow(G, N / factor.q, MainN), (long)solution.ModPow(PrevH, N / factor.q, MainN), MainN) % factor.q;
                        C.Add(t1);
                    }
                    else
                    {
                        var invEl = solution.inverseElement(
                            solution.ModPow(G, (long)Math.Pow(factor.q, i - 1) * C[i - 1], MainN), 
                            MainN
                            );
                        var HI = solution.ModMultiple(PrevH, invEl, MainN);
                        var HII = (long)solution.ModPow(HI, N / (long)Math.Pow(factor.q, i + 1), MainN);
                        var GI = (long)solution.ModPow(G, N / factor.q, MainN);
                        PrevH = (long)HI;
                        C.Add(solution.logB(GI, HII, MainN) % factor.q);
                    }

                }
                long Xi = (new Func<long>(() =>
                {
                    long sumVar = 0;
                    Parallel.For(0, (int)factor.alpha, (i) =>
                    {
                        sumVar += C[i] * (long)Math.Pow(factor.q, i);
                    });
                    return sumVar;
                }))();
                return Xi;
            }
            else
            {
                Int64 HI = (long)solution.ModPow(H, N / factor.ToCompositeNumber(), MainN);
                Int64 GI = (long)solution.ModPow(G, N / factor.ToCompositeNumber(), MainN);
                Int64 X1 = solution.logB(GI, HI, MainN) % factor.q;
                return X1;
            }
        }

        #endregion
    }
}
