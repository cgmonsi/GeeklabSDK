using UnityEngine;

public class UnityService : MonoBehaviour
{
    private static UnityService instance;

    public static UnityService Instance
    {
        get
        {
            if (instance != null) {
                return instance;
            }

            instance = new GameObject("CoroutineService").AddComponent<UnityService>();
            var o = instance.gameObject;
            o.hideFlags = HideFlags.HideInHierarchy;
            DontDestroyOnLoad(o);

            return instance;
        }
    }
    
    public static void Log(object message)
    {
        Debug.Log(message);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}