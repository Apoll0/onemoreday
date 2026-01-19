

using UnityEngine;

public static class ArrayExtension
{
    public static T Last<T>(this T[] array)
    {
        return (array.Length > 0) ? array[array.Length - 1] : default;
    }
    
    public static T Any<T>(this T[] array)
    {
        return (array.Length > 0) ? array[Random.Range(0, array.Length)] : default;
    }

    public static T GetAny<T>(this T[] array, int cooldown = 0)
    {
        if (cooldown == 0)
            return Any(array);
        
        if (array.Length > 0)
        {
            int randLimit = array.Length - cooldown;
            int index = randLimit > 1 ? Random.Range(0, randLimit) : 0;
            T buffer = array[index];
            for (int i = index; i < array.Length - 1; i++)
                array[i] = array[i + 1];
            array[array.Length - 1] = buffer;
            return buffer;
        }
        return default;
    }
    
    public static void MoveBackAt<T>(this T[] array, int index)
    {
        if (index >= 0 && index < array.Length)
        {
            T tmp = array[index];
            for (int i = index; i < array.Length - 1; i++)
                array[i] = array[i + 1];
            array[array.Length - 1] = tmp;
        }
    }
    
    public static void MoveBack<T>(this T[] array, T element)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(element))
                continue;
            MoveBackAt(array, i);
            break;
        }
    }
    
    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        T[] result = new T[length];
        if (length > array.Length)
            length = array.Length;
        System.Array.Copy(array, offset, result, 0, length);
        return result;
    }
    
    public static T[] SubArray<T>(this T[] array, int length)
    {
        T[] result = new T[length];
        System.Array.Copy(array, 0, result, 0, length);
        return result;
    }
    
    public static bool Contains<T>(this T[] array, T element)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(element))
                return true;
        }
        return false;
    }
    
    public static bool Contains<T>(this T[] array, T element, int count)
    {
        if (count > array.Length)
            count = array.Length;
        for (int i = 0; i < count; i++)
        {
            if (array[i].Equals(element))
                return true;
        }
        return false;
    }

    public static int IndexOf<T>(this T[] array, T element)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if(array[i].Equals(element))
                return i;
        }
        
        return -1;
    }
}
