var sensors = File.ReadAllLines("input.txt").Select(ParseSensors).ToList();

int minX = sensors.Min(x => x.SensorX - x.Reach);
int minY = sensors.Min(x => x.SensorY - x.Reach);
int maxX = sensors.Max(x => x.SensorX + x.Reach);
int maxY = sensors.Max(x => x.SensorY + x.Reach);

Console.WriteLine("Part1: " + CountBeaconlessPositions(2000000)); // 4793062

int maxSearchX = 4000000;
int maxSearchY = 4000000;
int minSearchX = 0;
int minSearchY = 0;

for (int y = minSearchY; y <= maxSearchY; y++)
{
    int x = FindGap(GetSpans(y));
    if (x != -1)
    {
        var tuningFrequency = ((long)x * 4000000) + y;
        Console.WriteLine("Part2: " + tuningFrequency); // 10826395253551
        break;
    }
}

// Räkna ut räckvidden för varje sensor från angivet y
List<(int From, int To)> GetSpans(int y)
{
    var list = new List<(int From, int To)>();
    foreach(var s in sensors)
    {
        int reachMinY = s.SensorY - s.Reach;
        int reachMaxY = s.SensorY + s.Reach;
        if (y >= reachMinY && y <= reachMaxY)
        {
            int reachMinX = s.SensorX - s.Reach + Math.Abs(s.SensorY - y);
            int reachMaxX = s.SensorX + s.Reach - Math.Abs(s.SensorY - y);
            list.Add((reachMinX, reachMaxX));
        }
    }
    return list;
}

// Sök efter nummer som inte finns med i någon av räckvidderna i listan
int FindGap(List<(int From, int To)> spans)
{
    spans.Sort((x, y) => x.From.CompareTo(y.From));
    int previousTo = minSearchX - 1;
    foreach (var span in spans)
    {
        if (span.From > previousTo + 1)
        {
            int found = previousTo + 1;
            if (found <= maxSearchX)
                return found;
        }
        previousTo = Math.Max(previousTo, span.To);
    }
    return -1;
}

// Räkna hur många positioner på en rad som inte kan innehålla okända beacon
int CountBeaconlessPositions(int y)
{
    int count = 0;
    for (int i = minX; i <= maxX; i++)
    {
        if (sensors.Any(s => GetDistance(i, y, s.SensorX, s.SensorY) <= s.Reach))
            count++;
    }
    int sensorCount = sensors.Count(s => s.SensorY == y);
    int beaconCount = sensors.Where(s => s.BeaconY == y).DistinctBy(s => s.BeaconX).Count();
    return count - (sensorCount + beaconCount);
}

int GetDistance(int x1, int y1, int x2, int y2) => Math.Abs(x1 - x2) + Math.Abs(y1 - y2);

(int SensorX, int SensorY, int BeaconX, int BeaconY, int Reach) ParseSensors(string line)
{
    var split = line.Split('=');
    int x1 = int.Parse(split[1][..^3]);
    int y1 = int.Parse(split[2][..^24]);
    int x2 = int.Parse(split[3][..^3]);
    int y2 = int.Parse(split[4]);
    int reach = GetDistance(x1, y1, x2, y2);
    return (x1, y1, x2, y2, reach);
}

void PrintTestSensors()
{
    var testSensors = File.ReadAllLines("test.txt").Select(ParseSensors).ToList();
    int testMinX = testSensors.Min(x => x.SensorX - x.Reach);
    int testMinY = testSensors.Min(x => x.SensorY - x.Reach);
    int testMaxX = testSensors.Max(x => x.SensorX + x.Reach);
    int testMaxY = testSensors.Max(x => x.SensorY + x.Reach);

    for (int y = testMinY; y <= testMaxY; y++)
    {
        string row = ("" + y).PadRight(3);
        for (int x = testMinX; x <= testMaxX; x++)
        {
            string point = ".";
            foreach (var s in testSensors)
            {
                if (GetDistance(x, y, s.SensorX, s.SensorY) <= s.Reach)
                {
                    point = "#";
                    break;
                }
            }
            if (testSensors.Any(s => s.SensorX == x && s.SensorY == y))
                point = "S";
            if (testSensors.Any(s => s.BeaconX == x && s.BeaconY == y))
                point = "B";
            row += point;
        }
        Console.WriteLine(row + " " + row.Count(s => s == '#'));
    }
}