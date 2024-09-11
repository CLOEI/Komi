using Komi.lib.itemdatabase;
using Komi.lib.world;

namespace Komi.lib.bot;

public class AStar(ItemDatabase itemDatabase)
{
    private uint Width { get; set; }
    private uint Height { get; set; }
    private List<Node> Grid { get; set; } = [];

    private ItemDatabase ItemDatabase { get; set; } = itemDatabase;

    private readonly object _gridLock = new();

    public void Reset()
    {
        lock (_gridLock)
        {
            Width = 0;
            Height = 0;
            Grid.Clear();
        }
    }

    public void Update(World? world)
    {
        Reset();

        if (world == null) return;

        lock (_gridLock)
        {
            Width = world.Width;
            Height = world.Height;

            for (var i = 0; i < world.Tiles.Count; i++)
            {
                var node = new Node
                {
                    X = (uint)(i % world.Width),
                    Y = (uint)(i / world.Width),
                    CollisionType = ItemDatabase.GetItem(world.Tiles[i].ForegroundItemId).CollisionType
                };
                Grid.Add(node);
            }
        }
    }

    public List<Node> FindPath(uint fromX, uint fromY, uint toX, uint toY)
    {
        lock (_gridLock)
        {
            if (Grid.Count == 0) return null;

            var startIndex = (int)(fromY * Width + fromX);
            var goalIndex = (int)(toY * Width + toX);

            if (!IsValidNode(startIndex) || !IsValidNode(goalIndex))
            {
                return null;
            }

            var openStart = new List<Node>();
            var openGoal = new List<Node>();
            var closedStart = new HashSet<(uint, uint)>();
            var closedGoal = new HashSet<(uint, uint)>();

            var cameFromStart = new Dictionary<(uint, uint), (uint, uint)>();
            var cameFromGoal = new Dictionary<(uint, uint), (uint, uint)>();

            var startNode = Grid[startIndex];
            startNode.G = 0;
            startNode.H = CalculateH(fromX, fromY, toX, toY);
            startNode.F = startNode.G + startNode.H;
            openStart.Add(startNode);

            var goalNode = Grid[goalIndex];
            goalNode.G = 0;
            goalNode.H = CalculateH(toX, toY, fromX, fromY);
            goalNode.F = goalNode.G + goalNode.H;
            openGoal.Add(goalNode);

            while (openStart.Count > 0 && openGoal.Count > 0)
            {
                if (Step(openStart, closedStart, cameFromStart, closedGoal, (fromX, fromY), (toX, toY), true, out var meetingNode))
                {
                    return ReconstructPathBidirectional(cameFromStart, cameFromGoal, meetingNode, (fromX, fromY), (toX, toY));
                }

                if (Step(openGoal, closedGoal, cameFromGoal, closedStart, (toX, toY), (fromX, fromY), false, out meetingNode))
                {
                    return ReconstructPathBidirectional(cameFromStart, cameFromGoal, meetingNode, (fromX, fromY), (toX, toY));
                }
            }

            return null;
        }
    }

    private bool Step(List<Node> openList, HashSet<(uint, uint)> closedList,
                      Dictionary<(uint, uint), (uint, uint)> cameFrom,
                      HashSet<(uint, uint)> otherClosedList,
                      (uint, uint) start, (uint, uint) goal,
                      bool isStart, out Node meetingNode)
    {
        meetingNode = null;

        if (openList.Count == 0) return false;

        var currentNode = openList.OrderBy(n => n.F).First();
        openList.Remove(currentNode);

        closedList.Add((currentNode.X, currentNode.Y));

        if (otherClosedList.Contains((currentNode.X, currentNode.Y)))
        {
            meetingNode = currentNode;
            return true;
        }

        foreach (var neighbor in GetNeighbors(currentNode))
        {
            if (closedList.Contains((neighbor.X, neighbor.Y))) continue;

            var tentativeG = currentNode.G + 1;

            if (openList.Any(n => n.X == neighbor.X && n.Y == neighbor.Y && tentativeG >= n.G))
            {
                continue;
            }

            cameFrom[(neighbor.X, neighbor.Y)] = (currentNode.X, currentNode.Y);
            neighbor.G = tentativeG;
            neighbor.H = CalculateH(neighbor.X, neighbor.Y, goal.Item1, goal.Item2);
            neighbor.F = neighbor.G + neighbor.H;

            openList.Add(neighbor);
        }

        return false;
    }

    private uint CalculateH(uint fromX, uint fromY, uint toX, uint toY)
    {
        return (uint)(Math.Abs((int)fromX - (int)toX) + Math.Abs((int)fromY - (int)toY));
    }

    private List<Node> GetNeighbors(Node node)
    {
        var neighbors = new List<Node>();
        var directions = new[] {
            (-1, 0), (1, 0), (0, -1), (0, 1), (-1, -1), (-1, 1), (1, -1), (1, 1)
        };

        foreach (var (dx, dy) in directions)
        {
            var newX = (int)node.X + dx;
            var newY = (int)node.Y + dy;

            if (newX >= 0 && newX < Width && newY >= 0 && newY < Height)
            {
                var index = newY * (int)Width + newX;
                if (index >= 0 && index < Grid.Count && IsValidNode(index))
                {
                    var neighbor = Grid[index];
                    if (neighbor.CollisionType != 1)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }

        return neighbors;
    }

    private bool IsValidNode(int index)
    {
        lock (_gridLock)
        {
            return index >= 0 && index < Grid.Count && Grid[index].CollisionType != 1;
        }
    }

    private List<Node> ReconstructPathBidirectional(Dictionary<(uint, uint), (uint, uint)> cameFromStart,
                                                    Dictionary<(uint, uint), (uint, uint)> cameFromGoal,
                                                    Node meetingNode, (uint, uint) start, (uint, uint) goal)
    {
        var path = new List<Node>();

        (uint, uint) currentPos = (meetingNode.X, meetingNode.Y);
        while (currentPos != start)
        {
            var node = Grid.FirstOrDefault(n => n.X == currentPos.Item1 && n.Y == currentPos.Item2);
            if (node != null)
            {
                path.Add(node);
            }
            currentPos = cameFromStart[currentPos];
        }

        path.Reverse();

        currentPos = (meetingNode.X, meetingNode.Y);
        while (currentPos != goal)
        {
            currentPos = cameFromGoal[currentPos];
            var node = Grid.FirstOrDefault(n => n.X == currentPos.Item1 && n.Y == currentPos.Item2);
            if (node != null)
            {
                path.Add(node);
            }
        }

        return path;
    }
}

public class Node
{
    public uint G { get; set; }
    public uint H { get; set; }
    public uint F { get; set; }
    public uint X { get; set; }
    public uint Y { get; set; }
    public byte CollisionType { get; set; }
}
