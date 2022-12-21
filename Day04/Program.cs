Console.WriteLine("Part1: " + CountAssignments(x => (x[0] <= x[2] && x[1] >= x[3]) || (x[2] <= x[0] && x[3] >= x[1])));
Console.WriteLine("Part2: " + CountAssignments(x => x[0] <= x[3] && x[2] <= x[1]));

int CountAssignments(Func<int[], bool> countFunc) =>
    File.ReadLines("input.txt")
    .Select(x => x.Split(',', '-').Select(int.Parse).ToArray())
    .Count(countFunc);