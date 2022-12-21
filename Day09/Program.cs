var moves = File.ReadLines("input.txt").SelectMany(x => Enumerable.Repeat(x[0], int.Parse(x[2..])));
Console.WriteLine("Part1: " + MoveRope(2).Count); //5981
Console.WriteLine("Part2: " + MoveRope(10).Count); //2352

HashSet<Position> MoveRope(int ropeLength)
{
    var rope = Enumerable.Range(0, ropeLength).Select(x => new Position(0, 0)).ToList();
    var visited = new HashSet<Position>();
    foreach (var m in moves)
    {
        rope[0] = MoveHeadOneStep(rope[0], m);
        for (int i = 0; i < rope.Count - 1; i++)
            rope[i + 1] = CatchUp(rope[i], rope[i + 1]);
        visited.Add(rope.Last());
    }
    return visited;
}

Position MoveHeadOneStep(Position p, char direction) => direction switch
{
    'R' => p with { X = p.X + 1 },
    'L' => p with { X = p.X - 1 },
    'U' => p with { Y = p.Y + 1 },
    'D' => p with { Y = p.Y - 1 },
    _ => throw new ArgumentException()
};

Position CatchUp(Position head, Position tail) => IsAdjacent(head, tail) ? tail : new(
    head.X == tail.X ? tail.X : head.X > tail.X ? tail.X + 1 : tail.X - 1,
    head.Y == tail.Y ? tail.Y : head.Y > tail.Y ? tail.Y + 1 : tail.Y - 1);

bool IsAdjacent(Position p1, Position p2) => Math.Abs(p1.X - p2.X) < 2 && Math.Abs(p1.Y - p2.Y) < 2;

record Position(int X, int Y);