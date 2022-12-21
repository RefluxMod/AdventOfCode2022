var (start, end, points) = GetPoints();
var route = FindFastestRoute(start, end);

Console.WriteLine("Part1: " + route.Count);

var routes = new List<List<Point>>();
foreach (var s in points.Where(x => x.Elevation == 'a'))
{
    points.ForEach(x => x.IsVisited = false);
    routes.Add(FindFastestRoute(s, end));
}

Console.WriteLine("Part2: " + routes.Where(x => x.Count > 0).Min(x => x.Count));


(Point start, Point end, List<Point> points) GetPoints()
{
    var points = new List<Point>();
    var grid = File.ReadAllLines("input.txt").Select(x => x.ToCharArray()).ToArray();

    for (int y = 0; y < grid.Length; y++)
        for (int x = 0; x < grid[y].Length; x++)
            points.Add(new Point(x, y, grid[y][x]));

    var start = points.First(x => x.Elevation == 'S');
    start.Elevation = 'a';
    var end = points.First(x => x.Elevation == 'E');
    end.Elevation = 'z';

    foreach (var s in points)
    {
        s.AdjacentPoints = points
            .Where(a => a != s && (a.Elevation - s.Elevation < 2)
            && ((a.X == s.X && Math.Abs(a.Y - s.Y) < 2) || (a.Y == s.Y && Math.Abs(a.X - s.X) < 2))).ToList();
    }

    return (start, end, points);
}

// Uses the Breadth-first search algorithm to find the fastest route from start to end.
List<Point> FindFastestRoute(Point start, Point end)
{
    var queue = new Queue<Point>(); // A queue to store the points that need to be visited
    var previousPoints = new Dictionary<Point, Point>(); // A dictionary to store the previous point for each point visited
    queue.Enqueue(start);
    start.IsVisited = true;

    while (queue.Count > 0 && !end.IsVisited)
    {
        var current = queue.Dequeue();
        foreach (var adjacent in current.AdjacentPoints)
        {
            if (!adjacent.IsVisited)
            {
                queue.Enqueue(adjacent);
                adjacent.IsVisited = true;
                previousPoints[adjacent] = current;
            }
        }
    }

    // If the end point was reached, construct the fastest route by starting at the end and use previous points to trace back to the starting point
    var fastestRoute = new List<Point>();
    if (end.IsVisited)
    {
        var current = end;
        while (current != start)
        {
            fastestRoute.Insert(0, current);
            current = previousPoints[current];
        }
    }
    return fastestRoute;
}

class Point
{
    public int X;
    public int Y;
    public char Elevation;
    public bool IsVisited;
    public List<Point> AdjacentPoints = new();
    public Point(int x, int y, char elevation)
    {
        X = x;
        Y = y;
        Elevation = elevation;
    }
}