using UnityEngine;

public static class Extensions
{
    public static T RandomElement<T>(this T[] array) => array[Random.Range(0, array.Length)];
}