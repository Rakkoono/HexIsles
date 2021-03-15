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
            {
#if UNITY_EDITOR
                current = Resources.LoadAll<T>("").FirstOrDefault();
#else
                current = Resources.FindObjectsOfTypeAll<T>("").FirstOrDefault();
#endif
            }

            return current;
        }
    }
}