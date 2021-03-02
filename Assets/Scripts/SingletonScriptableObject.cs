using UnityEngine;

public class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (!instance)
                instance = FindObjectOfType<T>();
            return instance;
        }
    }

    public virtual void OnEnable() {
        if (this is T)
            instance = this as T;
    }

    public virtual void OnDisable() {
        if (instance == this)
            instance = null;
    }
}