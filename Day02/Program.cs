Console.WriteLine("Part1: " + GetTotalScore(true));
Console.WriteLine("Part2: " + GetTotalScore(false));

int GetTotalScore(bool isPart1) =>
    File.ReadLines("input.txt")
    .Select(x => GetPlayers(x, isPart1))
    .Select(GetRoundScore)
    .Sum();

int GetRoundScore((char p1, char p2) r)
{
    int score = r is ('A', 'B') or ('B', 'C') or ('C', 'A') ? 6 : r.p1 == r.p2 ? 3 : 0;
    return score + (r.p2 == 'A' ? 1 : r.p2 == 'B' ? 2 : 3);
}

(char p1, char p2) GetPlayers(string r, bool isPart1) => isPart1
    ? (r[0], r[2] == 'X' ? 'A' : r[2] == 'Y' ? 'B' : 'C')
    : (r[0], r[2] == 'Y' ? r[0] : r[2] == 'X' ? GetLoss(r[0]) : GetWin(r[0]));

char GetLoss(char c) => c == 'A' ? 'C' : c == 'B' ? 'A' : 'B';

char GetWin(char c) => c == 'A' ? 'B' : c == 'B' ? 'C' : 'A';