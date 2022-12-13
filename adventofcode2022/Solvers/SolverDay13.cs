using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.Solvers
{
    class SolverDay13 : ISolver
    {
        class IntOrList
        {
            private string _str;
            public IntOrList(string toCreate)
            {
                _str = toCreate;
                if (int.TryParse(toCreate, out int intval))
                {
                    IsInt = true;
                    IntValue = intval;
                }
                else
                {
                    IsInt = false;
                    ListValue = new List<IntOrList>();
                    foreach (var element in SplitLowerLevel(toCreate.Substring(1, toCreate.Length - 2)))
                        ListValue.Add(new IntOrList(element));
                }
            }

            private IList<string> SplitLowerLevel(string s)
            {
                List<string> results = new List<string>();
                int level = 0;
                string current = "";
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == ',' && level == 0)
                    {
                        results.Add(current);
                        current = "";
                    }
                    else
                    {
                        current += s[i];
                        if (s[i] == '[')
                            level++;
                        else if (s[i] == ']')
                            level--;
                    }
                }
                if (!string.IsNullOrEmpty(current))
                    results.Add(current);
                return results;
            }

            public bool IsInt { get; }
            public int IntValue { get; }
            public List<IntOrList> ListValue { get; }

            public override string ToString()
            {
                return _str;
            }

            public bool? IsInRightOrder(IntOrList second)
            {
                if (IsInt && second.IsInt)
                {
                    if (IntValue < second.IntValue)
                        return true;
                    else if (IntValue > second.IntValue)
                        return false;
                    return null;
                }
                if (!IsInt && second.IsInt)
                    return IsInRightOrder(new IntOrList("[" + second.IntValue + "]"));
                if (IsInt && !second.IsInt)
                    return new IntOrList("[" + IntValue + "]").IsInRightOrder(second);
                if (!IsInt && !second.IsInt)
                {
                    for (int i = 0; i < Math.Min(ListValue.Count, second.ListValue.Count); i++)
                    {
                        var res = ListValue[i].IsInRightOrder(second.ListValue[i]);
                        if (res.HasValue)
                            return res.Value;
                    }
                    if (ListValue.Count < second.ListValue.Count)
                        return true;
                    if (ListValue.Count > second.ListValue.Count)
                        return false;
                    return null;
                }

                throw new NotImplementedException("Should never get there");
            }
        }

        class IntOrListPair: IComparer<IntOrList>
        {
            public IntOrListPair(string element1, string element2)
            {
                Element1 = new IntOrList(element1);
                Element2 = new IntOrList(element2);
            }
            public IntOrList Element1 { get; set; }
            public IntOrList Element2 { get; set; }

            public override string ToString()
            {
                return Element1.ToString() + " & " + Element2.ToString();
            }

            public bool? IsInRightOrder()
            {
                return Element1.IsInRightOrder(Element2);
            }

            public int Compare(IntOrList x, IntOrList y)
            {
                var ro = x.IsInRightOrder(y);
                if (ro == true)
                    return 1;
                else if (ro == false)
                    return -1;
                else return 0;
            }
        }

        private List<IntOrListPair> _pairs = new List<IntOrListPair>();
        private List<IntOrList> _all = new List<IntOrList>();
        private IntOrList _two = new IntOrList("[[2]]");
        private IntOrList _six = new IntOrList("[[6]]");
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            _all.Add(_two);
            _all.Add(_six);

            for (int i = 0; i < splitContent.Length; i += 3)
            {
                _pairs.Add(new IntOrListPair(splitContent[i], splitContent[i + 1]));
                _all.Add(new IntOrList(splitContent[i]));
                _all.Add(new IntOrList(splitContent[i + 1]));
            }
        }

        public string SolveFirstProblem()
        {
            int res = 0;
            for (int i = 0; i < _pairs.Count; i++)
            {
                var cur = _pairs[i];
                if (cur.IsInRightOrder() == true)
                    res += i + 1;
            }
            return res.ToString();
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            var comparer = new IntOrListPair("1", "2");
            var ordered = _all.OrderByDescending(x => x, comparer).ToList();

            Console.WriteLine();
            foreach (var el in ordered)
            {
                if (el == _two || el == _six)
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(el);
                if (el == _two || el == _six)
                    Console.BackgroundColor = ConsoleColor.Black;
            }
            Console.WriteLine();

            return ((ordered.IndexOf(_two) +1)* (ordered.IndexOf(_six) +1)).ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
