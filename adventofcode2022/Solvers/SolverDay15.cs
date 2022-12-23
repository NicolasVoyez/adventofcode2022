using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2022.Solvers
{
    class SolverDay15 : ISolver
    {
        class SensorAndBeacon
        {
            public SensorAndBeacon(string def)
            {
                //Sensor at x=3937279, y=2452476: closest beacon is at x=3597034, y=2313095
                var split = def.Replace("Sensor at x=", "").Replace(" closest beacon is at x=", "").Split(new[] { ", y=", ":" }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                Sensor = new Point(split[1], split[0]);
                Beacon = new Point(split[3], split[2]);
                _distance = Sensor.ManhattanDistance(Beacon);
            }
            private readonly int _distance;
            public Point Sensor { get; }
            public Point Beacon { get; }

            public (int, int)? NoGoZoneAt(int y)
            {
                var dy = Math.Abs(Sensor.Y - y);
                if (dy > _distance)
                    return null;

                var yRange = _distance - dy;
                return (Sensor.X - yRange, Sensor.X + yRange);
            }
        }

        private List<SensorAndBeacon> _sensorsAndBeacons = new List<SensorAndBeacon>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            foreach (var currentLine in splitContent)
            {
                _sensorsAndBeacons.Add(new SensorAndBeacon(currentLine));
            }
        }

        public string SolveFirstProblem()
        {
            return "At 10:" + GetNoGoCountAt(10) + ", at 2000000:" + GetNoGoCountAt(2000000);
        }

        private int GetNoGoCountAt(int targetY)
        {
            var noGoZones = GetNoGoZonesAt(targetY);

            return noGoZones.Sum(z => z.Item2 - z.Item1);
        }

        private List<(int, int)> GetNoGoZonesAt(int targetY)
        {
            var noGoZones = _sensorsAndBeacons.Select(sb => sb.NoGoZoneAt(targetY)).Where(z => z.HasValue).Select(z => z.Value).ToList();
            var oldCount = noGoZones.Count + 1;
            while (oldCount != noGoZones.Count)
            {
                oldCount = noGoZones.Count;
                for (
                    int i = 0; i < noGoZones.Count; i++)
                {
                    var z1 = noGoZones[i];
                    for (int j = i + 1; j < noGoZones.Count; j++)
                    {
                        var z2 = noGoZones[j];
                        if ((z2.Item1 >= z1.Item1 && z2.Item1 <= (z1.Item2 + 1)) || // z2 intersects z1 on the right
                            (z2.Item2 >= (z1.Item1 - 1) && z2.Item2 <= z1.Item2) ||  // z2 intersects z1 on the left
                            (z1.Item1 >= z2.Item1 && z1.Item1 <= z2.Item2)) // z1 is in z2
                        {
                            z1 = (Math.Min(z1.Item1, z2.Item1), Math.Max(z1.Item2, z2.Item2));
                            noGoZones[i] = z1;
                            noGoZones.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
            return noGoZones;
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            var fpbig = GetFreepos(0, 4000000);
            var fp20 = GetFreepos(0, 20);
            var res20 = new BigInteger(fp20.X) * 4000000 + fp20.Y;
            var resbig = new BigInteger(fpbig.X) * 4000000 + fpbig.Y;
            return "At 0,20:" + fp20 + "(result " + res20 + "), at 0,4000000:" + fpbig + "(result " + resbig + ")";
        }
        public Point GetFreepos(int min, int max)
        {
            Point notFound = new Point(-1, -1);
            Point found = notFound;
            Parallel.For(min, max, (y) =>
            {
                if (!found.Equals(notFound))
                    return;
                foreach (var range in GetNoGoZonesAt(y))
                {
                    if (range.Item1 > min && range.Item1 < max)
                    {
                        found = new Point(y, range.Item1 - 1);
                        break;
                    }
                    else if (range.Item2 > min && range.Item2 < max)
                    {
                        found = new Point(y, range.Item2 + 1);
                        break;
                    }
                }
            });

            return found;
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
