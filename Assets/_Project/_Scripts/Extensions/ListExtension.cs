using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public static class ListExtension
{
    public static T First<T>(this List<T> target)
    {
        return (target.Count > 0) ? target[0] : default;
    }

    public static T Last<T>(this List<T> target)
    {
        return (target.Count > 0) ? target[target.Count - 1] : default;
    }
    
    public static T Any<T>(this List<T> target)
    {
        return (target.Count > 0) ? target[Random.Range(0, target.Count)] : default;
    }
    
    public static T ExtractAny<T>(this List<T> target)
    {
        if (target.Count > 0)
        {
            int index = Random.Range(0, target.Count);
            T value = target[index];
            target.RemoveAt(index);
            return value;
        }
        return default;
    }
    
    public static T GetAny<T>(this List<T> target, int cooldown = 0)
    {
        if (target.Count > 0)
        {
            int randLimit = target.Count - cooldown;
            int index = randLimit > 1 ? Random.Range(0, randLimit) : 0;
            target.Add(target[index]);
            target.RemoveAt(index);
            return target[target.Count - 1];
        }
        return default;
    }

    public static void MoveBackAt<T>(this List<T> target, int index)
    {
        if (target.Count > 0 && index < target.Count)
        {
            target.Add(target[index]);
            target.RemoveAt(index);
        }
    }
    
    public static void MoveBack<T>(this List<T> target, T item)
    {
        if (target.Remove(item))
            target.Add(item);
    }
    
    public static void RemoveFirst<T>(this List<T> target)
    {
        if (target.Count > 0)
            target.RemoveAt(0);
    }

    public static void RemoveRandom<T>(this List<T> target)
    {
        if (target.Count > 0)
            target.RemoveAt(Random.Range(0, target.Count));
    }

    public static void RemoveLast<T>(this List<T> target)
    {
        if (target.Count > 0)
            target.RemoveAt(target.Count - 1);
    }
    
    public static void Add<T>(this List<T> target, List<T> array)
    {
        foreach (var item in array) 
            target.Add(item);
    }
    
    public static void Remove<T>(this List<T> target, List<T> array)
    {
        foreach (var item in array) 
            target.Remove(item);
    }
    
    public static Type Type<T>(this List<T> target)
    {
        return typeof(T);
    }
}
