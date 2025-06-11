using UnityEngine;

namespace FoodyGo.Utils.DI.SceneScope
{
    public class SceneScope : Scope
    {
        protected override void InjectAll()
        {
            GameObject[] roots = gameObject.scene.GetRootGameObjects();

            foreach (GameObject root in roots)
            {
                MonoBehaviour[] monoBehaviours = root.GetComponentsInChildren<MonoBehaviour>();

                foreach (MonoBehaviour monoBehaviour in monoBehaviours)
                {
                    Inject(monoBehaviour);
                }                
            }
        }

        protected override void Inject(object target)
        {
            
        }
    }    
}


