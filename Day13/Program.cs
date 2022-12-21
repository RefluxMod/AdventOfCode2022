var lists = File.ReadAllText("input.txt")
    .Split("\r\n\r\n")
    .SelectMany(x => x.Split("\r\n"))
    .Select(CreateList)
    .ToList();

int sum = 0;
for (int i = 0; i < lists.Count; i += 2)
    if (0 < CompareLists(lists[i], lists[i + 1]))
        sum += (i / 2) + 1;

Console.WriteLine("Part1: " + sum);

var divider1 = CreateList("[[2]]");
var divider2 = CreateList("[[6]]");
lists.Add(divider1);
lists.Add(divider2);
lists.Sort(CompareLists);
Console.WriteLine("Part2: " + (lists.IndexOf(divider1) + 1) * (lists.IndexOf(divider2) + 1));

ListNode CreateList(string list)
{
    var root = new ListNode();
    var current = root;
    for (int i = 1; i < list.Length - 1; i++)
    {
        if (list[i] == '[')
            current.Add(current = new() { Parent = current });
        else if (list[i] == ']')
            current = current.Parent;
        else if (list[i] == ',')
            continue;
        else if (list[i + 1] == '0')
            current.Add(new ValueNode(current, 10));
        else if (list[i - 1] is '[' or ',')
            current.Add(new ValueNode(current, int.Parse("" + list[i])));
    }
    return root;
}

int CompareLists(ListNode l, ListNode r)
{
    int result = 0;
    for(int i = 0; i < l.Count && i < r.Count; i++)
    {
        result = (l[i], r[i]) switch
        {
            (ValueNode v1, ValueNode v2) => CompareValues(v1, v2),
            (ListNode l1, ListNode l2) => CompareLists(l1, l2),
            (ValueNode v1, ListNode l2) => CompareLists(v1.ToList(), l2),
            (ListNode l1, ValueNode v2) => CompareLists(l1, v2.ToList()),
            _ => throw new NotImplementedException(),
        };
        if (result != 0) return result;
    }
    return l.Count - r.Count;
}

int CompareValues(ValueNode l, ValueNode r) => l.Value - r.Value;

interface INode
{
    ListNode Parent { get; set; }
}

class ValueNode : INode
{
    public ValueNode(ListNode parent, int value)
    {
        Parent = parent;
        Value = value;
    }
    public int Value { get; set; }
    public ListNode Parent { get; set; }
    public ListNode ToList() => new(this) { Parent = Parent };
    public override string ToString() => Value.ToString();
}

class ListNode : List<INode>, INode
{
    public ListNode(params INode[] nodes) => AddRange(nodes);
    public ListNode Parent { get; set; }
    public override string ToString() => "[" + (Count > 0 ? this.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y) : "") + "]";
}