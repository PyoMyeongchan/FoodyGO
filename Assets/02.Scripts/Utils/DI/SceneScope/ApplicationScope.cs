using UnityEngine;
using UnityEngine.SceneManagement;

namespace FoodyGo.Utils.DI.SceneScope
{
    public class ApplicationScope : Scope
    {
        protected override void Awake()
        {
            base.Awake();

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                InjectAll(scene);
            };
            
            DontDestroyOnLoad(gameObject);
        }
        
        void InjectAll(Scene scene)
        {
            GameObject[] roots = scene.GetRootGameObjects();

            foreach (GameObject root in roots)
            {
                MonoBehaviour[] monoBehaviours = root.GetComponentsInChildren<MonoBehaviour>();

                foreach (MonoBehaviour monoBehaviour in monoBehaviours)
                {
                    Inject(monoBehaviour);
                }                
            }
        }
    }    
    

}


