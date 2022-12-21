int[][] lines = File.ReadLines("input.txt").Select(x => x.Select(t => t - 48).ToArray()).ToArray();

int count = 0;
for (int r = 0; r < lines.Length; r++)
{
    for (int c = 0; c < lines[r].Length; c++)
        if (IsVisibleFromAnyDirection(r, c))
            count++;
}
Console.WriteLine("Part1: " + count);

var scores = new List<int>();
for (int r = 0; r < lines.Length; r++)
{
    for (int c = 0; c < lines[r].Length; c++)
        scores.Add(VisibleTrees(r, c, Direction.Top) * VisibleTrees(r, c, Direction.Bottom) * VisibleTrees(r, c, Direction.Left) * VisibleTrees(r, c, Direction.Right));
}
Console.WriteLine("Part2: " + scores.Max());


bool IsVisible(int row, int col, Direction direction)
{
    var (start, end, increment) = direction switch
    {
        Direction.Top => (0, row, 1),
        Direction.Bottom => (lines.Length - 1, row, -1),
        Direction.Left => (0, col, 1),
        Direction.Right => (lines[row].Length - 1, col, -1),
    };

    for (int i = start; i != end; i += increment)
    {
        int t = direction is Direction.Top or Direction.Bottom ? lines[i][col] : lines[row][i];
        if (t >= lines[row][col])
            return false;
    }
    return true;
}

int VisibleTrees(int row, int col, Direction direction)
{
    var (rowStep, colStep) = direction switch
    {
        Direction.Top => (-1, 0),
        Direction.Bottom => (1, 0),
        Direction.Left => (0, -1),
        Direction.Right => (0, 1),
    };

    int num = 0;
    int tree = lines[row][col];
    int i = row + rowStep;
    int j = col + colStep;
    while (i >= 0 && i < lines.Length && j >= 0 && j < lines[row].Length)
    {
        num++;
        if (lines[i][j] >= tree)
            break;
        i += rowStep;
        j += colStep;
    }
    return num;
}

bool IsVisibleFromAnyDirection(int r, int c) => 
    IsVisible(r, c, Direction.Top) || IsVisible(r, c, Direction.Bottom) || IsVisible(r, c, Direction.Left) || IsVisible(r, c, Direction.Right);

enum Direction { Top, Bottom, Left, Right }