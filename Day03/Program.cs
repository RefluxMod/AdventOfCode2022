Console.WriteLine("Part1: " + GetItemSum(true));
Console.WriteLine("Part2: " + GetItemSum(false));

int GetItemSum(bool isPart1) => (isPart1 
    ? File.ReadLines("input.txt").Select(GetParts)
    : File.ReadLines("input.txt").Chunk(3))
    .Select(GetCommonType)
    .Select(x => x < 'a' ? x - 38 : x - 96)
    .Sum();

IEnumerable<string> GetParts(string s) => s.Chunk(s.Length / 2).Select(x => new string(x));

char GetCommonType(IEnumerable<string> g) => g.First().First(c => g.All(x => x.Contains(c)));