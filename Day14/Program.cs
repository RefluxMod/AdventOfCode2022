var rockPaths = File.ReadAllLines("input.txt")
    .Select(x => x.Split(" -> ").Select(ParsePoint).ToArray());

var caveGrid = CreateCaveGrid(rockPaths);

foreach (var row in caveGrid)
    Console.WriteLine(row);

Console.WriteLine("Part1: " + caveGrid.Sum(x => x.Count(s => s == 'O')));

caveGrid = CreateCaveGrid(rockPaths.Append(new[] { (300, 160), (700, 160) })); // Lägg till ett golv längst ner
Console.WriteLine("Part2: " + caveGrid.Sum(x => x.Count(s => s == 'O')));


char[][] CreateCaveGrid(IEnumerable<(int X, int Y)[]> paths)
{
    int maxX = paths.Max(x => x.Max(z => z.X)); // 573
    int maxY = paths.Max(x => x.Max(z => z.Y)); // 158
    int minX = paths.Min(x => x.Min(z => z.X)); // 489

    char[][] grid = Enumerable.Range(0, maxY + 1)
        .Select(y => Enumerable.Range(0, maxX - minX + 1).Select(x => '.').ToArray())
        .ToArray();

    foreach (var path in paths) // Markera alla hinder med '#'
    {
        for (int i = 0; i < path.Length - 1; i++)
        {
            (int x1, int y1) = path[i];
            (int x2, int y2) = path[i + 1];
            MarkRockPath(grid, x1 - minX, y1, x2 - minX, y2);
        }
    }

    while (true) // Fyll på med sand 'O'
    {
        var (x, y) = MoveSandUnit(grid, 500 - minX, 0);
        if (x < 0) break; // Sand faller utanför 
        grid[y][x] = 'O';
        if (y == 0 && x == 500 - minX) break; // Sand har nått toppen
    }

    return grid;
}

(int X, int Y) MoveSandUnit(char[][] grid, int x, int y)
{
    if (!IsValidXY(grid, x, y) || !IsValidXY(grid, x, y + 1) || !IsValidXY(grid, x - 1, y + 1) || !IsValidXY(grid, x + 1, y + 1))
        return (-1, -1);

    if (grid[y + 1][x] == '.') // Testa ner
        return MoveSandUnit(grid, x, y + 1);
    if (grid[y + 1][x - 1] == '.') // Testa vänster ner
        return MoveSandUnit(grid, x - 1, y + 1);
    if (grid[y + 1][x + 1] == '.') // Testa höger ner
        return MoveSandUnit(grid, x + 1, y + 1);

    return (x, y);
}

bool IsValidXY(char[][] grid, int x, int y) => y > -1 && x > -1 && y < grid.Length && x < grid[y].Length;

void MarkRockPath(char[][] grid, int x1, int y1, int x2, int y2) // Använder Bresenham's line algorithm
{
    int dx = Math.Abs(x2 - x1);
    int dy = Math.Abs(y2 - y1);
    int sx = x1 < x2 ? 1 : -1;
    int sy = y1 < y2 ? 1 : -1;
    int err = dx - dy;

    while (true)
    {
        grid[y1][x1] = '#';
        if (x1 == x2 && y1 == y2)
            break;
        int e2 = 2 * err;
        if (e2 > -dy)
        {
            err -= dy;
            x1 += sx;
        }
        if (e2 < dx)
        {
            err += dx;
            y1 += sy;
        }
    }
}

(int X, int Y) ParsePoint(string s)
{
    var split = s.Split(',');
    return new(int.Parse(split[0]), int.Parse(split[1]));
}