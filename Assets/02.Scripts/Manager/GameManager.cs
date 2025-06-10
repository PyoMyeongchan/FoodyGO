using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using FoodyGo.Controller;
using UnityEngine.EventSystems;
using FoodyGo.Singletons;

namespace FoodyGo.Manager
{
    public class GameManager : Singleton_1<GameManager>
    { [Header("Game Scenes")]
        public string MapSceneName;
        
        [Header("Catch Scenes")]
        public string CatchSceneName;
        
        [Header("Splash Scenes")]
        public string SplashSceneName;
        
        [Header("Layer Names")]
        public string MonsterLayerName = "Monster";

        private Scene SplashScene;
        private Scene MapScene;
        private Scene CatchScene;
        // Use this for initialization
        IEnumerator Start()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            yield return SceneManager.LoadSceneAsync(SplashSceneName, LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync(MapSceneName, LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync(CatchSceneName, LoadSceneMode.Additive);
            
            ActiveAdditiveScene(MapSceneName);
            yield return SceneManager.UnloadSceneAsync(SplashSceneName);
        }
        
        //run when a new scene is loaded
        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode lsm)
        {
            if(scene.name == MapSceneName)
            {
                MapScene = scene;
            }
            else if (scene.name == CatchSceneName)
            {
                CatchScene = scene;
            }
        }

        public void ActiveAdditiveScene(string sceneName)
        {
            bool isActiveMapScene = sceneName.Equals(MapSceneName);
            bool isActiveCatchScene = sceneName.Equals(CatchSceneName);

            GameObject[] roots;
            roots = CatchScene.GetRootGameObjects();
            foreach (GameObject root in roots)
            {
                root.SetActive(isActiveCatchScene);
            }
            roots = MapScene.GetRootGameObjects();
            foreach (GameObject root in roots)
            {
                root.SetActive(isActiveMapScene);
            }
        }
        /// <summary>
        /// Checks if a relevant game object has been hit
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool RegisterHitGameObject(PointerEventData data)
        {
            int mask = BuildLayerMask();
            Ray ray = Camera.main.ScreenPointToRay(data.position);            
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, mask))
            {
                print("Object hit " + hitInfo.collider.gameObject.name);
                var go = hitInfo.collider.gameObject;
                HandleHitGameObject(go);

                return true;
            }
            return false;
        }

        private void HandleHitGameObject(GameObject go)
        {
            if(go.GetComponent<MonsterController>()!= null)
            {
                print("Monster hit, need to open catch scene ");
            }
        }

        private int BuildLayerMask()
        {
            return 1 << LayerMask.NameToLayer(MonsterLayerName);
        }
    }
}
