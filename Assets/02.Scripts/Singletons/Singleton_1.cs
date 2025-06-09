using UnityEngine;

namespace FoodyGo.Singletons
{
    public abstract class Singleton_1<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    T component = FindAnyObjectByType<T>();

                    if (component == null)
                    {
                        GameObject empty = new GameObject(typeof(T).Name);
                        component = empty.AddComponent<T>();
                    }
                    
                    _instance = component;
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
        
        private static T _instance;
    }
}
