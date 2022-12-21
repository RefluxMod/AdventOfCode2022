Console.WriteLine("Part1: " + SumCalories(1));
Console.WriteLine("Part2: " + SumCalories(3));

int SumCalories(int numberOfElves) => 
    File.ReadAllText("input.txt")
    .Split("\r\n\r")
    .Select(x => x.Split("\r\n"))
    .Select(x => x.Sum(int.Parse))
    .OrderByDescending(x => x)
    .Take(numberOfElves)
    .Sum();


