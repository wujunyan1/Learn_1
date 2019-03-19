using System.Collections;
using System.Collections.Generic;

public static class ListPool<t>
{

    static Stack<List<t>> stack = new Stack<List<t>>();

    public static List<t> Get()
    {
        if (stack.Count > 0)
        {
            return stack.Pop();
        }
        return new List<t>();
    }

    public static void Add(List<t> list)
    {
        list.Clear();
        stack.Push(list);
    }
}
