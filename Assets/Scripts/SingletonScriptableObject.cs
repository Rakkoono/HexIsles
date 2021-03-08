using System.Linq;
using UnityEngine;

public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T current = null;
    public static T Current
    {
        get
        {
            if (!current)
                current = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
            
            return current;
        }
    }
}