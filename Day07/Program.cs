var lines = File.ReadLines("input.txt");
var allDirs = new List<Dir>();
var root = new Dir("/");
var current = root;

foreach (var l in lines)
{
    if(l.StartsWith("$ cd .."))
    {
        current = current.Parent;
    }
    else if (l.StartsWith("$ cd /"))
    {
        current = root;
    }
    else if (l.StartsWith("$ cd"))
    {
        var dirName = l.Replace("$ cd ", "");
        current = current.Dirs.First(x => x.Name == dirName);
    }
    else if(l.StartsWith("dir"))
    {
        var dirName = l.Replace("dir ", "");
        if (!current.Dirs.Any(x => x.Name == dirName))
        {
            var newDir = new Dir(dirName, current);
            current.Dirs.Add(newDir);
            allDirs.Add(newDir);
        }
    }
    else if(l.StartsWith("$ ls"))
    {
    }
    else
    {
        var s = l.Split(' ');
        if (!current.Files.Any(x => x.Name == s[1]))
            current.Files.Add(new Fil(s[1], int.Parse(s[0]), current));
    }
}

int sum = allDirs.Select(GetDirSize).Where(x => x <= 100000).Sum();
Console.WriteLine("Part1: " + sum); // Part1

int totalSize = GetDirSize(root);
int freeSize = 70000000 - totalSize;
int needSize = 30000000 - freeSize; 
int dirSize = allDirs.Select(GetDirSize).Where(x => x >= needSize).Order().First();
Console.WriteLine("Part2: " + dirSize); // Part2


int GetDirSize(Dir dir) => dir.Files.Sum(x => x.Size) + dir.Dirs.Sum(GetDirSize);

class Dir
{
    public Dir(string name, Dir parent)
    {
        Name = name;
        Parent = parent;
    }

    public Dir(string name)
    {
        Name = name;
        Parent = this;
    }

    public string Name;
    public Dir Parent;
    public List<Dir> Dirs = new();
    public List<Fil> Files = new();
}

class Fil
{
    public Fil(string name, int size, Dir parent)
    {
        Name = name;
        Parent = parent;
        Size = size;
    }

    public Dir Parent;
    public string Name;
    public int Size;
}