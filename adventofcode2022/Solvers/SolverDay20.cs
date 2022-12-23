using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2022.Solvers
{
    class SolverDay20 : ISolver
    {
        private Dictionary<int,int> _numbers = new Dictionary<int, int>();
        private Dictionary<int, BigInteger> _bignumbers = new Dictionary<int, BigInteger>();
        public void InitInput(string content)
        {
            var key = new BigInteger(811589153L);
            var split = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            for (int i = 0; i < split.Count; i++)
            {
                _numbers[i] = split[i];
                _bignumbers[i] = key* split[i];
            }
        }

        public string SolveFirstProblem()
        {
            var finalList = new LinkedList<KeyValuePair<int,int>>(_numbers);
            for (int i = 0; i < _numbers.Count; i++)
            {
                var curr = _numbers.ElementAt(i);
                var curNode = finalList.Find(curr);

                LinkedListNode<KeyValuePair<int,int>> nextNode = GetNNext(finalList, curNode, curr.Value);
                if (nextNode != curNode)
                {
                    finalList.Remove(curNode);
                    finalList.AddAfter(nextNode, curr);
                }

                //if (newPos != curPos)
                //{
                //    finalState.RemoveAt(curPos);
                //    finalState.Insert(newPos, curr);
                //}
            }
            var zero = finalList.Find(_numbers.FirstOrDefault(kvp => kvp.Value == 0));
            var thousandth = ValueAtNThAfter(finalList, zero, 1000).Value;
            var twothousandth = ValueAtNThAfter(finalList, zero, 2000).Value;
            var threethousandth = ValueAtNThAfter(finalList, zero, 3000).Value;

            return (thousandth + twothousandth + threethousandth).ToString();
        }

        private static T ValueAtNThAfter<T>(LinkedList<T> list, LinkedListNode<T> node, int moves)
        {
            var limitedMoves = moves % (list.Count);
            LinkedListNode<T> nextNode = node;
            for (int m = 0; m < limitedMoves; m++)
                nextNode = nextNode.Next ?? list.First;
            return nextNode.Value;
        }
        private static LinkedListNode<T> GetNNext<T>(LinkedList<T> list, LinkedListNode<T> node, BigInteger moves)
        {
            var limitedMoves = moves % (list.Count - 1);
            LinkedListNode<T> nextNode = node;
            if (limitedMoves > 0)
            {
                for (BigInteger m = 0; m < limitedMoves; m++)
                    nextNode = nextNode.Next ?? list.First;
            }
            else if (limitedMoves < 0)
            {
                for (BigInteger m = limitedMoves; m <= 0; m++)
                    nextNode = nextNode.Previous ?? list.Last;
            }
            return nextNode;
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            var finalList = new LinkedList<KeyValuePair<int, BigInteger>>(_bignumbers);
            for (int run = 0; run < 10; run++)
            for (int i = 0; i < _bignumbers.Count; i++)
            {
                var curr = _bignumbers.ElementAt(i);
                var curNode = finalList.Find(curr);

                LinkedListNode<KeyValuePair<int, BigInteger>> nextNode = GetNNext(finalList, curNode, curr.Value);
                if (nextNode != curNode)
                {
                    finalList.Remove(curNode);
                    finalList.AddAfter(nextNode, curr);
                }
            }
            var zero = finalList.Find(_bignumbers.FirstOrDefault(kvp => kvp.Value == 0));
            var thousandth = ValueAtNThAfter(finalList, zero, 1000).Value;
            var twothousandth = ValueAtNThAfter(finalList, zero, 2000).Value;
            var threethousandth = ValueAtNThAfter(finalList, zero, 3000).Value;

            return (thousandth + twothousandth + threethousandth).ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
