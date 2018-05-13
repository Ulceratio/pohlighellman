using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoligHellman
{
    class lib
    {
        #region Structs
        /// <summary>
        /// Структура для разложения на множители
        /// </summary>
        public struct Factor
        {
            public long q { get; set; }
            public long alpha { get; set; }

            public long ToCompositeNumber()
            {
                return (long)Math.Pow(q, alpha);
            }
        }

        public struct Remainder
        {
            public Int64 a { get; set; }
            public Int64 m { get; set; }
        }

        #endregion

        #region Pow and Multiply
        /// <summary>
        /// (alpha ^ beta) mod n
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public decimal ModPow(decimal alpha, decimal beta, decimal n)  // (alpha ^ beta) mod n
        {
            decimal res = 1;
            while (beta != 0)
            {
                if ((beta % 2) != 0)//beta%2!=0
                {
                    res = (res * alpha) % n;
                    beta -= 1;
                }
                alpha = (alpha * alpha) % n;
                beta /= 2; // beta/=2;
            }
            return res;
        }

        public int BinPow(int alpha, int beta)  // (alpha ^ beta)
        {
            int res = 1;
            while (beta != 0)
            {
                if ((beta & 1) != 0)//beta%2!=0
                {
                    res = (res * alpha);
                    beta -= 1;
                }
                alpha = (alpha * alpha);
                beta = beta >> 1; // beta/=2;
            }
            return res;
        }

        /// <summary>
        /// Усножение по модулю n
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public decimal ModMultiple(decimal a, decimal b, decimal n)
        {
            return (a * b) % n;
        }
        #endregion

        #region Discrete logarithms

        public int logA(int a, int b, int p)
        {
            int x = 0;
            while (true)
            {
                if (ModPow(a, x, p) == b)
                {
                    return x;
                }
                else
                {
                    x++;
                }
            }
        }

        /// <summary>
        /// Шаги младенца , шаги гиганта alpha ^ x = beta (mod n)
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public Int64 logB(Int64 alpha, Int64 beta, Int64 n) // alpha ^ x = beta (mod n)
        {
            Int64 x = 0;
            Int64 k = Convert.ToInt64(Math.Sqrt(n));
            Int64[] litleSteps = new Int64[k];
            Int64[] bigSteps = new Int64[k];
            for (Int64 i = 0, j = 1; i < k; i++, j++)
            {
                bigSteps[i] = Convert.ToInt64(ModPow(alpha, j * k, n).ToString());
                litleSteps[i] = (Convert.ToInt64(ModPow(alpha, j, n).ToString()) * beta) % n;
            }
            Int64 sameEl = 0;
            try
            {
                sameEl = Convert.ToInt32(string.Join("\n", bigSteps.Intersect(litleSteps)));
            }
            catch
            {
                for (Int64 i = 0; i < bigSteps.Length; i++)
                {
                    for (Int64 j = 0; j < litleSteps.Length; j++)
                    {
                        if (bigSteps[i] == litleSteps[j])
                        {
                            sameEl = bigSteps[i];
                            break;
                        }
                    }
                }
            }
            x = (Array.IndexOf(bigSteps, sameEl) + 1) * k - (Array.IndexOf(litleSteps, sameEl) + 1);
            return x;
        }
        #endregion

        #region Inverse Element
        /// <summary>
        /// Обратный элемент через GCD
        /// </summary>
        /// <param name="a"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        public decimal inverseElement(decimal a, decimal mod)
        {
            decimal result = ((gcdex((Int64)a, (Int64)mod).Item2 % mod) + mod) % mod;
            return result;
        }

        /// <summary>
        /// Обратный элемент через возведение в степень
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public Int64 invEl(Int64 a, Int64 n)
        {
            Int64 t = 1;
            return (long)ModPow(a, ((int)t << (int)n) - 2, n);
        }

        public Tuple<long, long, long> gcdex(long m, long n)
        {
            long u = 0;
            long v = 0;
            long NOD = 0;
            long a = m;
            long b = n;
            long u1 = 1, v1 = 0, u2 = 0, v2 = 1;
            long q, r;
            long temp;
            while (b != 0)
            {
                q = a / b;
                r = a % b;
                a = b;
                b = r;
                temp = u2; u2 = u1 - q * u2; u1 = temp;
                temp = v2; v2 = v1 - q * v2; v1 = temp;
                a = u1 * m + v1 * n;
                b = u2 * m + v2 * n;
            }
            NOD = a; u = u1; v = v1;
            return Tuple.Create(NOD, u, v);
        }
        #endregion

        public Int64 ChineseReminderTheorem(List<Remainder> equation)
        {
            Int64 result = 0;
            Int64 M = 1;

            foreach (var item in equation)
            {
                M *= item.m;
            }

            for (int i = 0; i < equation.Count; i++)
            {
                result += (Int64)(equation[i].a * (M / equation[i].m) * inverseElement((M / equation[i].m), equation[i].m));
            }

            return result % M;
        }

        public List<Factor> FactorizationOfTheNumber(long p)
        {
            List<Factor> result = new List<Factor>();
            foreach (var item in GetSimpleNumbersEratosthenes(p))
            {
                result.Add(new Factor { q = item, alpha = 0 });
            }
            Parallel.For(1, result.Count, (i, state) =>
            {
                Factor temp = new Factor();
                while ((p % result[i].q) == 0)
                {
                    temp.q = result[i].q;
                    temp.alpha = result[i].alpha + 1;
                    result[i] = temp;
                    p /= result[i].q;
                }
            }
            );
            result = result.Where(x => x.alpha != 0).ToList();
            return result;
        }

        public List<long> GetSimpleNumbersEratosthenes(long thisTaskValue)
        {
            bool[] result = new bool[thisTaskValue + 1];
            for (long i = 0; i < thisTaskValue + 1; i++)
            {
                result[i] = true;
            }
            long p = 2;
            while (p * p < thisTaskValue)
            {
                for (long i = 2; p * i <= thisTaskValue; i++)
                {
                    result[p * i] = false;
                }

                for (long i = p; i <= thisTaskValue; i++)
                {
                    if (result[i] == true && i > p)
                    {
                        p = i;
                        break;
                    }
                }
            }

            List<long> resultNums = new List<long>();

            for (long i = 1; i < result.Length; i++)
            {
                if (result[i])
                {
                    resultNums.Add(i);
                }
            }
            return resultNums;
        }

        public decimal? GCD(decimal? a, decimal? b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }
    }
}
