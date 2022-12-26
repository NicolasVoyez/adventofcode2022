using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2022.Solvers
{
    class SolverDay25 : ISolver
    {
        private List<string> _snafus = new List<string>();
        private List<BigInteger> _translatedSnafus = new List<BigInteger>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var currentLine in splitContent)
            {
                _snafus.Add(currentLine);
            }
        }

        public string SolveFirstProblem()
        {
            BigInteger sum = 0;
            foreach(var snafu in _snafus)
            {
                BigInteger t = SnafuToInt(snafu);
                _translatedSnafus.Add(t);
                sum += t;
            }
            return sum + " or in SNAFU : " + IntToSnafu(sum);
        }

        private BigInteger SnafuToInt(string snafu)
        {
            BigInteger pow = 1;
            BigInteger res = 0;
            for (int i = snafu.Length-1; i >= 0; i--)
            {
                switch (snafu[i])
                {
                    case '2':
                        res += 2 * pow;
                        break;
                    case '1':
                        res += pow;
                        break;
                    case '-':
                        res -= pow;
                        break;
                    case '=':
                        res -= 2 * pow;
                        break;
                }
                pow *= 5;
            }
            return res;
        }
        private string IntToSnafu(BigInteger i)
        {
            BigInteger maxPow = 1;
            while (2*maxPow < i)
            {
                maxPow *= 5;
            }


            var remaining = i;
            string res = "";
            while (maxPow != 1)
            {
                if (remaining <= -2 * maxPow / 5 || remaining >= 2 * maxPow / 5)
                {
                    if (remaining > 0)
                    {
                        var d = BigInteger.Abs(2 * maxPow - remaining);
                        var s = BigInteger.Abs(maxPow - remaining);
                        if (d > maxPow && s > maxPow)
                            throw new Exception("WTF !");
                        else if (d < s)
                        {
                            res = res + "2";
                            remaining -= 2 * maxPow;
                        }
                        else
                        {
                            res = res + "1";
                            remaining -= maxPow;
                        }
                    }
                    else if (remaining < 0)
                    {
                        var d = BigInteger.Abs(2 * maxPow + remaining);
                        var s = BigInteger.Abs(maxPow + remaining);
                        if (d  > maxPow && s > maxPow)
                            throw new Exception("WTF !");
                        else if (d < s)
                        {
                            res = res + "=";
                            remaining += 2 * maxPow;
                        }
                        else
                        {
                            res = res + "-";
                            remaining += maxPow;
                        }
                    }
                    else
                    {
                        res = res + "0";
                    }
                }
                else
                {
                    res = res + "0";
                }
                maxPow /= 5;

            }

            switch ((int)remaining)
            {
                case -2:
                    return res + "=";
                case -1:
                    return res + "-";
                case 0:
                    return res + "0";
                case 1:
                    return res + "1";
                case 2:
                    return res + "2";
            }
            throw new Exception("WTF");
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            throw new NotImplementedException();
        }

        public bool Question2CodeIsDone { get; } = false;
        public bool TestOnly { get; } = false;
    }
}
