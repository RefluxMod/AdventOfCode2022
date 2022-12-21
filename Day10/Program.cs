var instructions = File.ReadLines("input.txt").Select(x => (x[0..4], x[0..4] == "addx" ? int.Parse(x[5..]) : 0));

int cycle = 0;
int value = 1;
int sum = 0;
var screen = new char[6 * 40]; 

foreach ((string inst, int val) in instructions)
{
    if(inst == "noop")
    {
        OneCycle();
        continue;
    }
    OneCycle();
    OneCycle();
    value += val;
}

Console.WriteLine("Part1: " + sum);
Console.WriteLine("Part2:");
new string(screen).Chunk(40).ToList().ForEach(Console.WriteLine);

void OneCycle()
{
    int screenPos = cycle % (6 * 40);
    int row = screenPos / 40;
    int spritePos =  value + (row * 40);

    screen[screenPos] = Math.Abs(screenPos - spritePos) < 2 ? '#' : '.';

    cycle++;

    if (cycle is 20 or 60 or 100 or 140 or 180 or 220)
        sum += cycle * value;
}