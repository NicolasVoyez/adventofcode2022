using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.Solvers
{
    class SolverDay2 : ISolver
    {
        private enum RPS
        {
            Rock = 1,
            Paper = 2,
            Cisors = 3
        }

        private enum Result
        {
            Loose = 'X',
            Draw = 'Y',
            Win = 'Z'
        }


        private class Round
        {
            public Round( char opponent, char you)
            {
                You = RpsFromText(you);
                #region Part 1
                Opponent = RpsFromText(opponent);
                RoundScore = 0;
                if (You == Opponent)
                    RoundScore = 3;
                else if ((You == RPS.Rock && Opponent == RPS.Cisors) ||
                    (You == RPS.Cisors && Opponent == RPS.Paper) ||
                    (You == RPS.Paper && Opponent == RPS.Rock))
                    RoundScore = 6;
                _text = $"{you} {opponent} : {You}-{Opponent} : {RoundScore}+{(int) You}={RoundScore + (int)You}";

                RoundScore += (int)You;
                #endregion
                #region Part 2 
                var result = (Result)you;
                switch (result)
                {
                    case Result.Loose: 
                        Round2Score = (int)Opponent - 1 == 0 ? 3 : (int)Opponent - 1;
                        break;
                    case Result.Draw:
                        Round2Score = (int)Opponent + 3;
                        break;
                    case Result.Win:
                        Round2Score = 6 + ((int)Opponent + 1 == 4 ? 1 : (int)Opponent + 1);
                        break;
                }

                #endregion
            }

            private string _text;
            public override string ToString() => _text;

            public RPS You { get; private set; }
            public RPS Opponent { get; private set; }
            public int RoundScore { get; private set; }
            public int Round2Score { get; private set; }

            private RPS RpsFromText(char text)
            {
                switch (text)
                {
                    case 'A': return RPS.Rock;
                    case 'B': return RPS.Paper;
                    case 'C': return RPS.Cisors;
                    case 'X': return RPS.Rock;
                    case 'Y': return RPS.Paper;
                    case 'Z': return RPS.Cisors;
                }
                throw new NotSupportedException();
            }
        }

        private List<Round> _rounds = new List<Round>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var currentLine in splitContent)
            {
                _rounds.Add(new Round(currentLine[0], currentLine[2]));
            }
        }

        public string SolveFirstProblem()
        {
            return _rounds.Sum(r => r.RoundScore).ToString();
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            return _rounds.Sum(r => r.Round2Score).ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
