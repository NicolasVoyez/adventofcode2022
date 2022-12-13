using AdventOfCode2022.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.Solvers
{
    class SolverDay7 : ISolver
    {
        interface IFileSystemElement
        {
            string Name { get; }
            int Size { get; }
            Directory Parent { get; }
        }

        struct File : IFileSystemElement
        {
            public File(string commandLineResult, Directory parent)
            {
                var split = commandLineResult.SplitREE();
                Name = split[1];
                Size = int.Parse(split[0]);
                Parent = parent;
            }
            public string Name { get; }
            public int Size { get; }
            public Directory Parent { get; }

        }
        class Directory : IFileSystemElement
        {
            public Directory(string commandLineResult, Directory parent)
            {
                // remove "dir "
                Name = commandLineResult.Substring(4);
                SubElements = new Dictionary<string,IFileSystemElement>();
                Parent = parent;
            }

            public string Name { get;  }
            private int _size = -1;
            public int Size
            {
                get
                {
                    if (_size == -1)
                        _size = SubElements.Values.Sum(s => s.Size);
                    return _size;
                }
            }
            public IDictionary<string,IFileSystemElement> SubElements {get; }
            public Directory Parent { get; }
        }

        private Directory _root = new Directory("dir /", null);
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            Directory current = _root;
            foreach (var currentLine in splitContent)
            {
                if (currentLine == "$ ls")
                    continue;
                if (currentLine == "$ cd /")
                    current = _root;
                else if (currentLine.StartsWith("$ cd "))
                {
                    // Moving
                    var direction = currentLine.Replace("$ cd ", "");
                    if (direction == "..")
                    {
                        current = current.Parent;
                    }
                    else
                    {
                        if (!current.SubElements.TryGetValue(direction, out var sub))
                        {
                            sub = new Directory(direction, current);
                            current.SubElements.Add(direction, sub);
                        }
                        if (!(sub is Directory subdir))
                            throw new DirectoryNotFoundException("Was a file");
                        current = subdir;
                    }
                }
                else
                {
                    IFileSystemElement e;
                    if (currentLine.StartsWith("dir"))
                        e = new Directory(currentLine, current);
                    else
                        e = new File(currentLine, current);
                    if (!current.SubElements.ContainsKey(e.Name))
                        current.SubElements.Add(e.Name, e);
                }
            }
        }

        public string SolveFirstProblem()
        {
            List<Directory> directories = new List<Directory>() { };
            RecursiveGetDirectories(_root, directories);

            return directories.Where(d => d.Size <= 100000).Sum(d => d.Size).ToString();
            
        }
        private void RecursiveGetDirectories (Directory current, List<Directory> result)
        {
            result.Add(current);
            foreach (IFileSystemElement el in current.SubElements.Values)
            {
                if (el is Directory dir)
                    RecursiveGetDirectories(dir, result);
            }
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            List<Directory> directories = new List<Directory>() { };
            RecursiveGetDirectories(_root, directories);

            var toFind = 30000000 - (70000000 - _root.Size);

            foreach(var dir in directories.OrderBy(d => d.Size))
            {
                if (dir.Size > toFind)
                    return dir.Size.ToString();
            }
            return "Not Found";
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
