using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.Solvers
{
    class SolverDay19 : ISolver
    {
        class BluePrint
        {
            public BluePrint(string costString)
            {
                var split = costString.Split(new string[]
                {
                    "Blueprint ",": Each ore robot costs "," ore. Each clay robot costs "," ore. Each obsidian robot costs "," ore and "," clay. Each geode robot costs "," ore and "," obsidian."
                }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                Identifier = split[0];
                OreRobotCostOre = split[1];
                ClayRobotCostOre = split[2];
                ObsidianRobotCostOre = split[3];
                ObsidianRobotCostClay = split[4];
                GeodeRobotCostOre = split[5];
                GeodeRobotCostObsidian = split[6];
            }
            public int Identifier { get; }
            public int OreRobotCostOre { get; }
            public int ClayRobotCostOre { get; }
            public int ObsidianRobotCostOre { get; }
            public int ObsidianRobotCostClay { get; }
            public int GeodeRobotCostOre { get; }
            public int GeodeRobotCostObsidian { get; }
        }

        class RunState
        {
            public RunState(BluePrint b)
            {
                Blueprint = b;
                MaxOreProd = Math.Max(Math.Max(b.OreRobotCostOre, b.ClayRobotCostOre), Math.Max(b.ObsidianRobotCostOre, b.GeodeRobotCostOre));
                MaxClayProd = b.ObsidianRobotCostClay;
                MaxObsidianProd = b.GeodeRobotCostObsidian;
                OreProd = 1;
                ClayProd = 0;
                ObsidianProd = 0;
                GeodeProd = 0;
                Ore = 0;
                Clay = 0;
                Obsidian = 0;
                Geode = 0;
            }
            public RunState(RunState old)
            {
                Blueprint = old.Blueprint;
                MaxOreProd = old.MaxOreProd;
                MaxClayProd = old.MaxClayProd;
                MaxObsidianProd = old.MaxObsidianProd;
                OreProd = old.OreProd;
                ClayProd = old.ClayProd;
                ObsidianProd = old.ObsidianProd ;
                GeodeProd = old.GeodeProd;
                Ore = old.Ore;
                Clay = old.Clay;
                Obsidian = old.Obsidian;
                Geode = old.Geode;
            }

            public BluePrint Blueprint { get; }
            public int MaxOreProd { get; }
            public int MaxClayProd { get; }
            public int MaxObsidianProd { get; }
            public int OreProd { get; private set; }
            public int ClayProd { get; private set; }
            public int ObsidianProd { get; private set; }
            public int GeodeProd { get; private set; }
            public int Ore { get; private set; }
            public int Clay { get; private set; }
            public int Obsidian { get; private set; }
            public int Geode { get; private set; }

            internal RunState GetBestRun(int minutes)
            {
                if (minutes == 1)
                {
                    Grow();
                    if (Geode == 9)
                        return this;
                    return this;
                }

                RunState bestState = this;
                if (Ore >= Blueprint.GeodeRobotCostOre && Obsidian >= Blueprint.GeodeRobotCostObsidian)
                {
                    RunState next = GetBestBuyingGeodeBot(minutes - 1);
                    if (next.Geode > bestState.Geode)
                        bestState = next;
                }
                else
                {
                    if (OreProd < MaxOreProd && Ore >= Blueprint.OreRobotCostOre)
                    {
                        RunState next = GetBestBuyingOreBot(minutes - 1);
                        if (next.Geode > bestState.Geode)
                            bestState = next;
                    }
                    if (ClayProd < MaxClayProd && Ore >= Blueprint.ClayRobotCostOre)
                    {
                        RunState next = GetBestBuyingClayBot(minutes - 1);
                        if (next.Geode > bestState.Geode)
                            bestState = next;
                    }
                    if (ObsidianProd < MaxObsidianProd && Ore >= Blueprint.ObsidianRobotCostOre && Clay >= Blueprint.ObsidianRobotCostClay)
                    {
                        RunState next = GetBestBuyingObsidianBot(minutes - 1);
                        if (next.Geode > bestState.Geode)
                            bestState = next;
                    }
                    if (Ore < MaxOreProd)// ||
                        //(Clay < MaxClayProd && ClayProd > 0) )//||
                        //(Obsidian < MaxObsidianProd && ObsidianProd > 0))
                    {
                        RunState wait = new RunState(this);
                        wait.Grow();
                        wait = wait.GetBestRun(minutes - 1);
                        if (wait.Geode > bestState.Geode)
                            bestState = wait;
                    }
                }

                return bestState;
            }

            private void Grow()
            {
                Ore += OreProd;
                Clay += ClayProd;
                Obsidian += ObsidianProd;
                Geode += GeodeProd;
            }

            private RunState GetBestBuyingGeodeBot(int minutes)
            {
                RunState newRun = new RunState(this);
                newRun.Grow();
                newRun.Ore -= Blueprint.GeodeRobotCostOre;
                newRun.Obsidian -= Blueprint.GeodeRobotCostObsidian;
                newRun.GeodeProd++;
                return newRun.GetBestRun(minutes);
            }

            private RunState GetBestBuyingObsidianBot(int minutes)
            {
                RunState newRun = new RunState(this);
                newRun.Grow();
                newRun.Ore -= Blueprint.ObsidianRobotCostOre;
                newRun.Clay -= Blueprint.ObsidianRobotCostClay;
                newRun.ObsidianProd++;
                return newRun.GetBestRun(minutes);
            }

            private RunState GetBestBuyingClayBot(int minutes)
            {
                RunState newRun = new RunState(this);
                newRun.Grow();
                newRun.Ore -= Blueprint.ClayRobotCostOre;
                newRun.ClayProd++;
                return newRun.GetBestRun(minutes);
            }

            private RunState GetBestBuyingOreBot(int minutes)
            {
                RunState newRun = new RunState(this);
                newRun.Grow();
                newRun.Ore -= Blueprint.OreRobotCostOre;
                newRun.OreProd++;
                return newRun.GetBestRun(minutes);
            }
        }

        private List<BluePrint> bluePrints = new List<BluePrint>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var currentLine in splitContent)
            {
                bluePrints.Add(new BluePrint(currentLine));
            }
        }

        public string SolveFirstProblem()
        {
            int score = 0;
            foreach (var bluePrint in bluePrints)
            {
                var initState = new RunState(bluePrint);
                RunState run = initState.GetBestRun(24);
                score += run.Blueprint.Identifier * run.Geode;
            }

            return (score).ToString();
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            int score = 1;
            for (int i = 0; i < Math.Min(bluePrints.Count, 3); i++)
            {
                var initState = new RunState(bluePrints[i]);
                RunState run = initState.GetBestRun(32);
                score *= run.Geode;
            }
            return (score).ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
