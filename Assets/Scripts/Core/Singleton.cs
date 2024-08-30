using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;

    private static bool applicationIsQuitting = false;

    private static Transform managerObject = null;
    private static Transform managerObjectNeedDestroy = null;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting) { return null; }

            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else if (instance != this as T)
        {
            Destroy(gameObject);
        }
        
        if (managerObject == null)
        {
            GameObject obj = GameObject.Find("Managers");
            if (obj == null)
            {
                managerObject = new GameObject("Managers").transform;
                DontDestroyOnLoad(managerObject);
            }
            else
            {
                managerObject = obj.transform;
            }
        }
        if (managerObjectNeedDestroy == null)
        {
            GameObject obj = GameObject.Find("ManagersNeedDestroy");
            if (obj == null)
            {
                managerObjectNeedDestroy = new GameObject("ManagersNeedDestroy").transform;
            }
            else
            {
                managerObjectNeedDestroy = obj.transform;
            }
        }

        if (NeedDestory())
        {
            transform.parent = managerObjectNeedDestroy.transform;
        }
        else
        {
            transform.parent = managerObject.transform;
        }
        OnAwake();
    }
    
    //子类重写这个方法，不要重写Awake
    protected virtual void OnAwake() { }

    protected virtual bool NeedDestory()
    {
        return true;
    }

    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
}