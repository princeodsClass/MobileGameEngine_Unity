using UnityEngine;


/// <summary>
/// Unity Singleton.
/// <typeparam name="T"> Unity script class name </typeparam>
public class SingletonMono<T> : MonoBehaviour where T : UnityEngine.Component
{
    private static T singleton = null;
    private static bool m_bAppClosed = false;

    void OnDestroy()
    {
        singleton = null;
    }

    public static T Singleton
    {
        get
        {
            if (m_bAppClosed) return null;

            if (null == singleton)
            {
                CreateSingleton();
            }
            return singleton;
        }
        protected set
        {
            if (null != singleton) return;
            singleton = value;
            DontDestroyOnLoad(singleton.gameObject);
        }
    }

    private static void CreateSingleton()
    {
        string strID = (typeof(T)).ToString();
        Singleton = new GameObject(strID).AddComponent<T>();
    }

    public static bool IsCreated()
    {
        return singleton != null;
    }

    protected virtual void OnApplicationQuit()
    {
        m_bAppClosed = true;
    }
}

/// <summary>
/// SceneChange -> Destory
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMono_DestoryType<T> : MonoBehaviour where T : UnityEngine.Component
{
    private static T singleton = null;

    void OnDestroy()
    {
        singleton = null;
    }

    public static T Instance
    {
        get
        {
            if (null == singleton)
            {
                CreateSingleton();
            }
            return singleton;
        }
    }

    private static void CreateSingleton()
    {
        string strID = (typeof(T)).ToString();
        singleton = new GameObject(strID).AddComponent<T>();

        if (null != singleton)
        {
            //DontDestroyOnLoad(GameObject.Find(strID));
        }
    }
}