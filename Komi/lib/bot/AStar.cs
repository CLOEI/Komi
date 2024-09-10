using Komi.lib.itemdatabase;
using Komi.lib.world;

namespace Komi.lib.bot;

public class AStar(ItemDatabase itemDatabase)
{
    private uint Width { get; set; }
    private uint Height { get; set; }
    private List<Node> Grid { get; set; } = [];
    private ItemDatabase ItemDatabase { get; set; } = itemDatabase;

    public void Reset()
    {
        Width = 0;
        Height = 0;
        Grid.Clear();
    }

    public void Update(World? world)
    {
        Reset();
        if (world == null) return;
        Width = world.Width;
        Height = world.Height;

        for (int i = 0; i < world.Tiles.Count; i++)
        {
            var node = new Node();
            node.X = (uint)(i % world.Width);
            node.Y = (uint)(i / world.Width);
            var item = ItemDatabase.GetItem(world.Tiles[i].ForegroundItemId);
            node.CollisionType = item.CollisionType;
            Grid.Add(node);
        }
    }

    public List<Node> FindPath(uint fromX, uint fromY, uint toX, uint toY)
    {
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        Dictionary<(uint, uint), (uint, uint)> cameFrom = new Dictionary<(uint, uint), (uint, uint)>();

        int startIndex = (int)(fromY * Width + fromX);
        Node startNode = Grid[startIndex];
        startNode.G = 0;
        startNode.H = CalculateH(fromX, fromY, toX, toY);
        startNode.F = startNode.G + startNode.H;
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            var currentIndex = openList.Select((node, index) => new { node.F, index }).OrderBy(n => n.F).First().index;
            var currentNode = openList[currentIndex];
            openList.RemoveAt(currentIndex);

            if (currentNode.X == toX && currentNode.Y == toY)
            {
                return ReconstructPath(cameFrom, (toX, toY), (fromX, fromY));
            }

            List<Node> children = GetNeighbors(currentNode);

            foreach (Node child in children)
            {
                if (closedList.Any(n => n.X == child.X && n.Y == child.Y))
                {
                    continue;
                }

                child.G = currentNode.G + 1;
                child.H = CalculateH(child.X, child.Y, toX, toY);
                child.F = child.G + child.H;

                var openNode = openList.FirstOrDefault(n => n.X == child.X && n.Y == child.Y);
                if (openNode != null && child.G > openNode.G)
                {
                    continue;
                }

                cameFrom[(child.X, child.Y)] = (currentNode.X, currentNode.Y);
                openList.Add(child);
            }

            closedList.Add(currentNode);
        }

        return null;
    }

    private uint CalculateH(uint fromX, uint fromY, uint toX, uint toY)
    {
        int dx = Math.Abs((int)fromX - (int)toX);
        int dy = Math.Abs((int)fromY - (int)toY);
        return (uint)(dx == dy ? 14 * dx : 10 * (dx + dy));
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        (int, int)[] directions = new (int, int)[] {
            (-1, 0), (1, 0), (0, -1), (0, 1),
            (-1, -1), (-1, 1), (1, -1), (1, 1)
        };

        foreach (var (dx, dy) in directions)
        {
            int newX = (int)node.X + dx;
            int newY = (int)node.Y + dy;

            if (newX >= 0 && newX < Width && newY >= 0 && newY < Height)
            {
                int index = newY * (int)Width + newX;
                Node neighbor = Grid[index];

                if (neighbor.CollisionType != 1)
                {
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    private List<Node> ReconstructPath(Dictionary<(uint, uint), (uint, uint)> cameFrom, (uint, uint) current, (uint, uint) start)
    {
        List<Node> path = new List<Node>();
        (uint, uint) currentPos = current;

        while (currentPos != start)
        {
            Node node = Grid.FirstOrDefault(n => n.X == currentPos.Item1 && n.Y == currentPos.Item2);
            if (node != null)
            {
                path.Add(node);
            }

            currentPos = cameFrom[currentPos];
        }

        Node startNode = Grid.FirstOrDefault(n => n.X == start.Item1 && n.Y == start.Item2);
        if (startNode != null)
        {
            path.Add(startNode);
        }

        path.Reverse();
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