using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T current = null;
    public static T Current
    {
        get
        {
            if (!current)
                current = FindObjectOfType<T>();
            
            return current;
        }
    }
}