var jobs = new Dictionary<string, (char Op, string Left, string Right, Func<long?> Result)>();
var reverse = new Dictionary<string, Func<long?>>();

foreach (var line in File.ReadAllLines("input.txt")) // Antingen har jobbet ett konstant värde eller så har det ett uträknat värde
    jobs[line[..4]] = long.TryParse(line[6..], out long num) ? (' ', "", "", () => num) : GetJob(line[11], line[6..10], line[13..]);

var root = jobs["root"];
Console.WriteLine("Part1: " + root.Result()); // Genom att anropa root func så traverseras hela funktionskedjan

foreach(var (k, j) in jobs.Where(x => x.Value.Op != ' ')) // Skapa omvända funktioner så att vi kan räkna baklänges
    (reverse[j.Left], reverse[j.Right]) = GetReverseFunc(k, j.Op, jobs[j.Left].Result(), jobs[j.Right].Result());

jobs["humn"] = (' ', "", "", () => null); // Nollställ humn så att hela funktionskedjan kan räknas om. Kedjan innehåller då endast värden som går att räkna fram utifrån konstanterna
var value = jobs[root.Left].Result() ?? jobs[root.Right].Result(); // Vi vet att root vill ha två lika värden. Hämta det värdet som gick att räkna fram.
reverse[root.Left] = () => value;
reverse[root.Right] = () => value;

Console.WriteLine("Part2: " + reverse["humn"]()); // Räkna baklänges för att få fram värdet för humn

(char, string, string, Func<long?>) GetJob(char op, string left, string right) => op switch
{
    '+' => (op, left, right, () => jobs[left].Result() + jobs[right].Result()),
    '-' => (op, left, right, () => jobs[left].Result() - jobs[right].Result()),
    '*' => (op, left, right, () => jobs[left].Result() * jobs[right].Result()),
    '/' => (op, left, right, () => jobs[left].Result() / jobs[right].Result()),
    _ => throw new ArgumentException()
};


(Func<long?> Left, Func<long?> Right) GetReverseFunc(string key, char op, long? left, long? right) => op switch
{
    '+' => (() => reverse[key]() - right, () => reverse[key]() - left),
    '-' => (() => reverse[key]() + right, () => left - reverse[key]()),
    '*' => (() => reverse[key]() / right, () => reverse[key]() / left),
    '/' => (() => reverse[key]() * right, () => left / reverse[key]()),
    _ => throw new ArgumentException()
};