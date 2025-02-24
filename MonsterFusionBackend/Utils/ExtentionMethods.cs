using System.Collections.Generic;
using System.Linq;

public static class ExtentsionMethods
{
    public static T GetRandom<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable == null || enumerable.Count() == 0)
            return default(T);
        int n = RandomUtils.Range(0, enumerable.Count());
        return enumerable.ElementAt(n);
    }
    public static void Confuse<T>(this IList<T> list)
    {
        if (list == null)
            return;

        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = RandomUtils.Range(0, list.Count);

            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    public static T GetLastItem<T>(this List<T> List)
    {
        if (List.Count == 0) return default(T);
        return List[List.Count - 1];
    }
    static T GetLastItem<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable.Count() == 0) return default(T);
        return enumerable.ElementAt(enumerable.Count() - 1);
    }
    public static List<T> GetRandom<T>(this IEnumerable<T> enumerable, int count)
    {
        if (enumerable == null)
            return null;

        if (enumerable.Count() <= count)
        {
            return enumerable.ToList();
        }

        List<T> ret = new List<T>();
        while (ret.Count < count)
        {
            int n = RandomUtils.Range(0, enumerable.Count());
            T e = enumerable.ElementAt(n);
            if (!ret.Contains(e))
            {
                ret.Add(e);
            }
        }

        return ret;
    }
}
