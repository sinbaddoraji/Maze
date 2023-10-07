namespace Maze
{
    public class MazeGenerator
    {
        private const char Wall = '#';
        private const char Path = ' ';
        private const char Player = 'P';
        private const char ExitDoor = 'E';
        private char[,] _maze;
        private int _size;
        private Random _rand = new Random();
        private List<Tuple<int, int>> _solutionPath = null!;

        public MazeGenerator(int size)
        {
            if (size < 5) // A minimum size to ensure solvability
            {
                throw new ArgumentException("Size should be at least 5 for solvability.");
            }

            this._size = size + 2;  // Adding 2 for the borders
            _maze = new char[_size, _size];

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    _maze[i, j] = Wall;
                }
            }
        }

        public void GenerateSolvableMaze()
        {
            Tuple<int, int>? start = new Tuple<int, int>(1, 1);
            Tuple<int, int> end = new Tuple<int, int>(_size - 2, _size - 2);

            _maze[start.Item1, start.Item2] = Path;
            List<Tuple<int, int>?> walls = new List<Tuple<int, int>?>();
            walls.AddRange(GetWallNeighbors(start));

            while (walls.Count > 0)
            {
                var randomWall = walls[_rand.Next(walls.Count)];
                Tuple<int, int> oppositeCell = GetOppositeCell(randomWall);

                if (oppositeCell != null && _maze[oppositeCell.Item1, oppositeCell.Item2] == Wall)
                {
                    _maze[randomWall.Item1, randomWall.Item2] = Path;
                    _maze[oppositeCell.Item1, oppositeCell.Item2] = Path;
                    walls.AddRange(GetWallNeighbors(oppositeCell));
                }
                walls.Remove(randomWall);
            }

            _maze[start.Item1, start.Item2] = Player;
            _maze[end.Item1, end.Item2] = ExitDoor;
        }

        private List<Tuple<int, int>> GetWallNeighbors(Tuple<int, int>? position)
        {
            List<Tuple<int, int>> walls = new List<Tuple<int, int>>();
            List<Tuple<int, int>> neighbors = GetNeighbors(position);
            foreach (var neighbor in neighbors)
            {
                if (_maze[neighbor.Item1, neighbor.Item2] == Wall)
                {
                    walls.Add(neighbor);
                }
            }
            return walls;
        }

        private Tuple<int, int>? GetOppositeCell(Tuple<int, int>? wall)
        {
            List<Tuple<int, int>> neighbors = GetNeighbors(wall);
            foreach (var neighbor in neighbors)
            {
                if (_maze[neighbor.Item1, neighbor.Item2] == Path)
                {
                    int dx = wall.Item1 - neighbor.Item1;
                    int dy = wall.Item2 - neighbor.Item2;

                    int oppositeX = wall.Item1 + dx;
                    int oppositeY = wall.Item2 + dy;

                    if (oppositeX >= 0 && oppositeX < _size && oppositeY >= 0 && oppositeY < _size)
                    {
                        return new Tuple<int, int>(oppositeX, oppositeY);
                    }
                }
            }
            return null;
        }

        private List<Tuple<int, int>> GetNeighbors(Tuple<int, int>? position)
        {
            List<Tuple<int, int>> neighbors = new List<Tuple<int, int>>();
            if (position.Item1 > 0) neighbors.Add(new Tuple<int, int>(position.Item1 - 1, position.Item2));
            if (position.Item1 < _size - 1) neighbors.Add(new Tuple<int, int>(position.Item1 + 1, position.Item2));
            if (position.Item2 > 0) neighbors.Add(new Tuple<int, int>(position.Item1, position.Item2 - 1));
            if (position.Item2 < _size - 1) neighbors.Add(new Tuple<int, int>(position.Item1, position.Item2 + 1));
            return neighbors;
        }

        public void SolveMaze()
        {
            Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
            Dictionary<Tuple<int, int>, Tuple<int, int>> predecessors = new Dictionary<Tuple<int, int>, Tuple<int, int>>();
            HashSet<Tuple<int, int>> visited = new HashSet<Tuple<int, int>>();

            Tuple<int, int> start = new Tuple<int, int>(1, 1);
            Tuple<int, int> end = new Tuple<int, int>(_size - 2, _size - 2);

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current.Equals(end))
                {
                    _solutionPath = new List<Tuple<int, int>>();
                    while (current != null)
                    {
                        _solutionPath.Add(current);
                        predecessors.TryGetValue(current, out current);
                    }
                    _solutionPath.Reverse();
                    break;
                }

                List<Tuple<int, int>> neighbors = GetNeighbors(current);
                foreach (var neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor) && _maze[neighbor.Item1, neighbor.Item2] != Wall)
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                        predecessors[neighbor] = current;
                    }
                }
            }
        }

        public void MovePlayer(Tuple<int, int> oldPosition, Tuple<int, int> newPosition)
        {
            _maze[oldPosition.Item1, oldPosition.Item2] = Path;
            _maze[newPosition.Item1, newPosition.Item2] = Player;
        }

        public void DisplayPlayerJourney(int playSpeed)
        {
            Tuple<int, int> currentPlayerPosition = _solutionPath[0];

            foreach (var position in _solutionPath)
            {
                MovePlayer(currentPlayerPosition, position);
                currentPlayerPosition = position;
                DisplayMaze();
                Thread.Sleep(playSpeed);
            }
        }

        public void DisplayMaze()
        {
            Console.Clear();

            for (int i = 0; i <= _size; i++)
            {
                if (i == _size)
                {
                    Console.WriteLine(string.Join('\0', Enumerable.Repeat("#", _size + 1)));
                    continue;
                }
                for (int j = 0; j <= _size; j++)
                {
                    if (j == _size)
                    {
                        Console.Write("#");
                        continue;
                    }
                    Console.Write(_maze[i, j]);
                }
                Console.WriteLine();
            }
        }
    }
}
