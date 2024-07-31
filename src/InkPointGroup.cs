namespace FlatlinerDOA.Controls;

public record InkPointGroup(List<InkPoint> Points, InkPointGroupOptions copy) : InkPointGroupOptions(copy);
