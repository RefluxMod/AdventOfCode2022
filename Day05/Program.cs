var moves1 = File.ReadLines("input.txt").SelectMany(GetMovesPart1);
var stakcs1 = GetStacks();
foreach (var m in moves1)
    stakcs1[m.To].Push(stakcs1[m.From].Pop());
Console.WriteLine("Part1: " + GetStacksTop(stakcs1));

var moves2 = File.ReadLines("input.txt").Select(GetMovesPart2);
var stakcs2 = GetStacks();
foreach (var m in moves2)
{
    var tmpStack = new Stack<char>();
    for (int i = 0; i < m.Count; i++)
        tmpStack.Push(stakcs2[m.From].Pop());
    for (int i = 0; i < m.Count; i++)
        stakcs2[m.To].Push(tmpStack.Pop());
}
Console.WriteLine("Part2: " + GetStacksTop(stakcs2));


Stack<char>[] GetStacks() => new Stack<char>[]
{
    new (new[] { 'H', 'T', 'Z', 'D' }),
    new (new[] { 'Q', 'R', 'W', 'T', 'G', 'C', 'S' }),
    new (new[] { 'P', 'B', 'F', 'Q', 'N', 'R', 'C', 'H' }),
    new (new[] { 'L', 'C', 'N', 'F', 'H', 'Z' }),
    new (new[] { 'G', 'L', 'F', 'Q', 'S' }),
    new (new[] { 'V', 'P', 'W', 'Z', 'B', 'R', 'C', 'S' }),
    new (new[] { 'Z', 'F', 'J' }),
    new (new[] { 'D', 'L', 'V', 'Z', 'R', 'H', 'Q' }),
    new (new[] { 'B', 'H', 'G', 'N', 'F', 'Z', 'L', 'D' })
};

IEnumerable<(int From, int To)> GetMovesPart1(string row)
{
    var s = row.Split(' ');
    var count = int.Parse(s[1]);
    var from = int.Parse(s[3]);
    var to = int.Parse(s[5]);
    for (int i = 0; i < count; i++)
        yield return (from - 1, to - 1);
}

(int Count, int From, int To) GetMovesPart2(string row)
{
    var s = row.Split(' ');
    var count = int.Parse(s[1]);
    var from = int.Parse(s[3]);
    var to = int.Parse(s[5]);
    return (count, from - 1, to - 1);
}

string GetStacksTop(Stack<char>[] stacks) => new(stacks.Select(x => x.Pop()).ToArray());



