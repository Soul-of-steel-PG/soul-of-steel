using System;
using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T> {
    private static T _instance;

    public static T Instance {
        get {
            if (_instance != null) return _instance;
            
            _instance = FindObjectOfType<T>();
            if (_instance != null) return _instance;
            
            GameObject singletonObject = new($"Singleton - {typeof(T).Name}");
            _instance = singletonObject.AddComponent<T>();

            return _instance;
        }
        protected set => _instance = value;
    }
    
    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = (T)this;
        DontDestroyOnLoad(gameObject);
    }

    public static bool HasInstance() {
        return _instance != null;
    }

    protected virtual void OnDestroy() {
        if (_instance == this) {
            _instance = null;
        }
    }
}