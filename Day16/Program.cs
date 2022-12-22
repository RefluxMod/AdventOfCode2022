
List<RouteStep> EmptyRouteList = new();

var inputValves = GetValves("input.txt");
var startValve = inputValves["AA"];

var part1 = CreateRouteSteps(startValve, 30);
Console.WriteLine("Part1: " + part1.Score);

var part2 = CreateDualRouteSteps(startValve, startValve, 26, 26);
Console.WriteLine("Part2: " + (part2.Score1 + part2.Score2));
PrintRoutes(26, inputValves, ("Tomten", part2.Steps1), ("Elefanten", part2.Steps2));


//Del1 av uppgiften. Iterativ metod som testar alla rimliga rutter för att hitta den snabbaste
(int Score, List<RouteStep> Steps) CreateRouteSteps(Valve current, int stepsLeft, string openValves = "")
{
    if (stepsLeft < 2)
        return (0, EmptyRouteList);

    int maxScore = 0;
    var bestRoute = EmptyRouteList;

    foreach (var p in current.FastestPaths.Values) // Loopa igenom alla möjliga vägar där destinationen har flow rate > 0
    {
        if (p.Count > stepsLeft - 1)
            continue;

        var destination = p.Last(); // Sista valve i pathen är alltid den som ska öppnas (om den inte redan är öppen)

        if (!openValves.Contains(destination.Name))
        {
            int newStepsLeft = stepsLeft - p.Count - 1;
            var (newScore, newSteps) = CreateRouteSteps(destination, newStepsLeft, openValves + ";" + destination.Name);
            
            var steps = new List<RouteStep>(p.Count + newSteps.Count + 1);
            p.ForEach(x => steps.Add(new(true, x)));
            steps.Add(new(false, destination));
            steps.AddRange(newSteps);

            int score = newStepsLeft * destination.Rate + newScore;
            if (score > maxScore)
            {
                maxScore = score;
                bestRoute = steps;
            }
        }
    }
    return (maxScore, bestRoute);
}

// Del2 av uppgiften. I princip samma som Del1 men nu skapar vi två rutter samtidigt
(int Score1, int Score2, List<RouteStep> Steps1, List<RouteStep> Steps2) CreateDualRouteSteps(Valve current1, Valve current2, int stepsLeft1, int stepsLeft2, string openValves = "")
{
    if (stepsLeft1 < 2 && stepsLeft2 < 2)
        return (0, 0, EmptyRouteList, EmptyRouteList);

    int combinedMaxScore = 0;
    int maxScore1 = 0;
    int maxScore2 = 0;
    var bestRoute1 = EmptyRouteList;
    var bestRoute2 = EmptyRouteList;
    var route1Found = false;

    foreach (var p1 in current1.FastestPaths.Values) // För route 1, loopa igenom alla möjliga vägar där destinationen har flow rate > 0
    {
        if (p1.Count > stepsLeft1 - 1)
            continue;
        
        var dest1 = p1.Last();
        if (openValves.Contains(dest1.Name))
            continue;

        int newStepsLeft1 = stepsLeft1 - p1.Count - 1;
        route1Found = true;

        foreach (var p2 in current2.FastestPaths.Values) // För route 2, loopa igenom alla möjliga vägar där destinationen har flow rate > 0
        {
            if (p2.Count > stepsLeft2 - 1)
                continue;

            var dest2 = p2.Last();
            if (dest1 == dest2 || openValves.Contains(dest2.Name))
                continue;

            int newStepsLeft2 = stepsLeft2 - p2.Count - 1;

            var (newScore1, newScore2, newSteps1, newSteps2) = CreateDualRouteSteps(dest1, dest2, newStepsLeft1, newStepsLeft2, openValves + ";" + dest1.Name + ";" + dest2.Name);

            var steps1 = new List<RouteStep>(p1.Count + newSteps1.Count + 1);
            p1.ForEach(x => steps1.Add(new(true, x)));
            steps1.Add(new(false, dest1));
            steps1.AddRange(newSteps1);

            var steps2 = new List<RouteStep>(p2.Count + newSteps2.Count + 1);
            p2.ForEach(x => steps2.Add(new(true, x)));
            steps2.Add(new(false, dest2));
            steps2.AddRange(newSteps2);

            int score = (newStepsLeft1 * dest1.Rate + newScore1) + (newStepsLeft2 * dest2.Rate + newScore2);

            if (score > combinedMaxScore)
            {
                combinedMaxScore = score;
                maxScore1 = newStepsLeft1 * dest1.Rate + newScore1;
                maxScore2 = newStepsLeft2 * dest2.Rate + newScore2;
                bestRoute1 = steps1;
                bestRoute2 = steps2;
            }
        }
    }

    if(!route1Found) // Om route1 fått slut på steg så måste vi ändå testa om route2 kan gå vidare
        (maxScore2, bestRoute2) = CreateRouteSteps(current2, stepsLeft2, openValves);

    return (maxScore1, maxScore2, bestRoute1, bestRoute2);
}

Dictionary<string, Valve> GetValves(string fileName)
{
    var inputValves = File.ReadAllLines(fileName).Select(Valve.Parse).ToDictionary(k => k.Name);
    foreach (var v in inputValves.Values)
    {
        var s = v.ValvesString.Split(", ");
        v.AdjacentValves = inputValves.Values.Where(x => s.Contains(x.Name)).ToList();
    }

    // Hitta snabbaste vägen från start till end. Alla Valves har en lista "FastestPaths" med de snabbaste vägarna till alla andra valves som har flow rate > 0
    // Vi förbereder dessa i förväg så att det ska gå snabbare att räkna fram hela rutten senare
    foreach (var start in inputValves.Values)
    {
        foreach (var end in inputValves.Values)
        {
            if (end.Rate > 0 && end != start)
            {
                var route = FindFastestRoute(start, end);
                start.FastestPaths[end.Name] = route;
            }
        }
    }
    return inputValves;
}

// Sökalgorithm Breadth-first, hitta snabbaste vägen mellan start och end
List<Valve> FindFastestRoute(Valve start, Valve end)
{
    var queue = new Queue<Valve>();
    var previousPoints = new Dictionary<Valve, Valve>();
    var visited = new HashSet<Valve>();
    queue.Enqueue(start);
    visited.Add(start);

    while (queue.Count > 0 && !visited.Contains(end))
    {
        var current = queue.Dequeue();
        foreach (var adjacent in current.AdjacentValves)
        {
            if (!visited.Contains(adjacent))
            {
                queue.Enqueue(adjacent);
                visited.Add(adjacent);
                previousPoints[adjacent] = current;
            }
        }
    }

    var fastestRoute = new List<Valve>();
    if (visited.Contains(end))
    {
        var current = end;
        while (current != start)
        {
            fastestRoute.Insert(0, current);
            current = previousPoints[current];
        }
    }
    return fastestRoute;
}

// Printa alla stegen i rutterna
void PrintRoutes(int minutesToRun, Dictionary<string, Valve> valves, params (string Name, List<RouteStep> Steps)[] routes)
{
    int releasedPressure = 0;
    var currentValves = routes.ToDictionary(k => k, v => v.Steps.First().Valve);
    var openedValves = new HashSet<Valve>();

    for (int i = 0; i < minutesToRun; i++)
    {
        foreach (var v in valves.Values)
        {
            if (openedValves.Contains(v))
                releasedPressure += v.Rate;
        }
        foreach (var r in routes)
        {
            if (i >= r.Steps.Count)
                continue;

            var step = r.Steps[i];
            var valve = step.Valve;
            if (step.IsMove)
            {
                Console.WriteLine($"Minute {i + 1} {r.Name} goes to {valve.Name} ({valve.Rate})");
                currentValves[r] = valve;
            }
            else
            {
                Console.WriteLine($"Minute {i + 1} {r.Name} opens valve {valve.Name} ({valve.Rate})");
                openedValves.Add(valve);
            }
        }
        Console.WriteLine($"Minute {i + 1} releasing pressure {openedValves.Sum(x => x.Rate)} total {releasedPressure}");
    }
}

// RouteStep är ett steg i en rutt. Kan antigen vara en förflyttning (IsMove = true) eller en öppning av valve (IsMove = false)
record RouteStep(bool IsMove, Valve Valve);

class Valve
{
    public string Name = string.Empty;
    public int Rate;
    public Dictionary<string, List<Valve>> FastestPaths = new();
    public List<Valve> AdjacentValves = new();
    public string ValvesString = string.Empty;

    public static Valve Parse(string s) => new() { Name = s[6..8], Rate = int.Parse(s[23..s.IndexOf(';')]), ValvesString = s[(s.LastIndexOf("valve") + 6)..].Trim() };

    public override string ToString() => Name + " (" + Rate + ")";
}