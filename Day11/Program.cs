var monkeys = GetMonkeys();
for(int i = 0; i < 20; i++)
    monkeys.ForEach(x => x.DoTurnPart1(monkeys));

Console.WriteLine("Part1: " + GetMonkeyBusiness(monkeys));

int lcm = Monkey.LeastCommonMultiple(monkeys);
monkeys = GetMonkeys();
for (int i = 0; i < 10000; i++)
    monkeys.ForEach(x => x.DoTurnPart2(monkeys, lcm));

Console.WriteLine("Part2: " + GetMonkeyBusiness(monkeys));


long GetMonkeyBusiness(List<Monkey> monkeys) => 
    monkeys.Select(x => x.Inspections).Order().TakeLast(2).Aggregate((x, y) => x * y);

List<Monkey> GetMonkeys() => 
    File.ReadAllText("input.txt").Split("Monkey").Skip(1).Select(Monkey.Parse).ToList();

class Monkey
{
    public List<long> Items;
    public Func<long, long> Operation;
    public Func<long, int> Test;
    public int Divisor;
    public long Inspections;

    public Monkey(List<long> items, Func<long, long> operation, Func<long, int> test, int divisor)
    {
        Items = items;
        Operation = operation;
        Test = test;
        Divisor = divisor;
    }

    public void DoTurnPart1(List<Monkey> monkeys) => InspectAndThrow(x => Operation(x) / 3, monkeys);

    public void DoTurnPart2(List<Monkey> monkeys, int lcm) => InspectAndThrow(x => Operation(x) % lcm, monkeys);

    private void InspectAndThrow(Func<long, long> func, List<Monkey> monkeys)
    {
        Items.Select(func).ToList().ForEach(x => monkeys[Test(x)].Items.Add(x));
        Inspections += Items.Count;
        Items.Clear();
    }

    public static Monkey Parse(string input)
    {
        var properties = input.TrimEnd().Split("\r\n").Select(x => x.Split(':')[1]).ToArray();
        var items = properties[1].Split(", ").Select(long.Parse).ToList();

        var op = properties[2][11..];
        Func<long, long> opFunc = (op[0], op[2..]) switch
        {
            ('*', "old") => x => x * x,
            ('*', _) => x => x * long.Parse(op[2..]),
            (_, "old") => x => (x + x),
            _ => x => (x + long.Parse(op[2..]))
        };
        
        int div = int.Parse(properties[3][14..]);
        var ifTrue = int.Parse(properties[4][17..]);
        var ifFalse = int.Parse(properties[5][17..]);
        Func<long, int> testFunc = x => x % div == 0 ? ifTrue : ifFalse;
        return new Monkey(items, opFunc, testFunc, div);
    }

    public static int LeastCommonMultiple(IEnumerable<Monkey> monkeys) => monkeys.Select(x => x.Divisor).Aggregate(LeastCommonMultiple);

    static int LeastCommonMultiple(int a, int b) => (a / GreatestCommonFactor(a, b)) * b;

    static int GreatestCommonFactor(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}