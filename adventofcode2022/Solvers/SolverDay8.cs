using AdventOfCode2022.Helpers;

namespace AdventOfCode2022.Solvers
{
    class SolverDay8 : ISolver
    {
        private class Tree
        {
            public int Height { get; }
            public bool IsVisible { get; set; }

            public Tree(int height)
            {
                Height = height;
                IsVisible = false;
            }

            public override string ToString()
            {
                return Height + (IsVisible ? " : Visible" : " : Invisible");
            }
        }
        private Grid<Tree> _forest;

        public void InitInput(string content)
        {
            _forest = new Grid<Tree>(content, (c) => new Tree(int.Parse(c.ToString())));
        }

        public string SolveFirstProblem()
        {
            int maxHeight = 0;

            void computeVisibility(Grid<Tree>.Cell<Tree> cell)
            {
                if (cell.X == 0 || cell.Y == 0 || cell.X == _forest.XMax - 1 || cell.Y == _forest.XMax - 1)
                {
                    cell.Value.IsVisible = true;
                    maxHeight = cell.Value.Height;
                }
                else if (cell.Value.Height > maxHeight)
                {
                    maxHeight = cell.Value.Height;
                    cell.Value.IsVisible = true;
                }
            }
            _forest.ForEachPerRow(computeVisibility);
            _forest.ForEachPerRow(computeVisibility, true);
            _forest.ForEachPerColumn(computeVisibility);
            _forest.ForEachPerColumn(computeVisibility, true);

            //PrintForest();

            return _forest.Count(c => c.IsVisible).ToString();
        }

        private void PrintForest()
        {
            _forest.Print(c =>
            {
                if (c.IsVisible)
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(c.Height);
                if (c.IsVisible)
                    Console.ForegroundColor = ConsoleColor.White;
            });
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            int maxScenicValue = 0;
            foreach (var tree in _forest.All())
            {
                if (tree.X == 0 || tree.Y == 0 || tree.X == _forest.XMax - 1 || tree.Y == _forest.XMax - 1)
                    continue;

                var dirs = _forest.GetFirstInEachDirection(
                tree.Y,
                tree.X,
                t => t.Height >= tree.Value.Height,
                (nfX, nfY) =>
                {
                    if (nfY < 0)
                        nfY = 0;
                    else if (nfX < 0)
                        nfX = 0;
                    else if (nfY > _forest.YMax - 1)
                        nfY = _forest.YMax - 1;
                    else if (nfX > _forest.XMax - 1)
                        nfX = _forest.XMax - 1;

                    return new Grid<Tree>.Cell<Tree>(nfY, nfX, default);
                }, true);
                var curScenic = 1;
                foreach (var dir in dirs)
                {
                    curScenic *= (Math.Abs(dir.X - tree.X) + Math.Abs(dir.Y - tree.Y));
                }
                if (curScenic > maxScenicValue)
                    maxScenicValue = curScenic;
            }
            return maxScenicValue.ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
