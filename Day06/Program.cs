Console.WriteLine("Part1 " + FindMarker(4));
Console.WriteLine("Part2 " + FindMarker(14));

int FindMarker(int lenght)
{
    var input = File.ReadAllText("input.txt");
    for (int i = 0; i < input.Length; i++)
    {
        if (input.Substring(i, lenght).Distinct().Count() == lenght)
            return i + lenght;
    }
    return 0;
}