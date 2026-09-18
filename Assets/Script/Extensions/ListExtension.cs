using System.Collections.Generic;
using UnityEngine;

public static class ListExtension
{
    public static T GetRandomElement<T> (this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public static T GetAndRemoveRandomElement<T>(this List<T> list)
    {
        T element = list[Random.Range(0, list.Count)];
        list.Remove(element);
        return element;
    }
}
