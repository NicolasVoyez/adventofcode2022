using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022.Helpers
{
    public class DijkstraNode
    {
        public DijkstraNode(int cost) : this(null , cost)
        {
        }

        protected DijkstraNode(IEnumerable<DijkstraNode> linked, int cost) 
        {
            PathDone = double.MaxValue;
            Previous = null;
            if (linked == null)
                Linked = new List<DijkstraNode>();
            else
                Linked = new List<DijkstraNode>(linked);
            Cost = cost;
        }

        public double PathDone { get; set; }
        public DijkstraNode Previous { get; set; }
        public List<DijkstraNode> Linked { get; set; }
        public int Cost { get; }
    }

    public class Dijkstra
    {
        public List<DijkstraNode> Nodes;

        public Dijkstra(List<DijkstraNode> nodes)
        {
            Nodes = nodes;
        }

        public IList<DijkstraNode> GetShortestPath(DijkstraNode from, DijkstraNode to)
        {
            var unSeen = new List<DijkstraNode>(Nodes);

            from.PathDone = 0;

            while (unSeen.Count != 0)
            {
                var n1 = unSeen.OrderBy(x => x.PathDone).First();
                unSeen.Remove(n1);
                foreach (var n2 in n1.Linked)
                {
                    var dist = n1.PathDone + n2.Cost;

                    if (!(n2.PathDone > dist)) continue;

                    n2.PathDone = dist;
                    n2.Previous = n1;
                }
            }
            var finalWay = new List<DijkstraNode>();
            var currNode = to;
            while (currNode != from && currNode != null)
            {
                finalWay.Add(currNode);
                currNode = currNode.Previous as DijkstraNode;
            }
            if (currNode == from)
            {
                finalWay.Add(currNode);
                finalWay.Reverse();
                return finalWay;
            }
            return new List<DijkstraNode>();
        }
    }
}
