using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{

    [SerializeField] private List<SceneDetails> connectedScenes;
    [SerializeField] private BGM sceneMusic = BGM.NONE;

    public BGM SceneMusic => sceneMusic;

    public bool IsLoaded {  get; private set; }

    private List<SavableEntity> savableEntities;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(LoadScene());
            GameManager.Instance.SetCurrentScene(this);

            if (sceneMusic != BGM.NONE)
            {
                AudioManager.instance.PlayMusic(sceneMusic, fade: true);
            }

            // Load all connected scenes
            foreach (var scene in connectedScenes)
            {
                StartCoroutine(scene.LoadScene());
            }

            // Unload the scenes that are no longer connected
            var prevScene = GameManager.Instance.PrevScene;
            if (prevScene != null)
            {
                var prevLoadedScenes = prevScene.connectedScenes;
                foreach (var scene in prevLoadedScenes)
                {
                    if (!connectedScenes.Contains(scene) && scene != this)
                    {
                        scene.UnloadScene();
                    }
                }

                if (!connectedScenes.Contains(prevScene))
                {
                    prevScene.UnloadScene();
                }
                
            
            }
        }
    }

    public IEnumerator LoadScene()
    {
        if (!IsLoaded)
        {
            IsLoaded = true;
            var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);

            operation.completed += (AsyncOperation op) =>
            {
                savableEntities = GetSavableEntitiesInScene();
                SavingSystem.i.RestoreEntityStates(savableEntities);
            };

        }
        yield return null;
    }

    public void UnloadScene()
    {
        if (IsLoaded)
        {
            SavingSystem.i.CaptureEntityStates(savableEntities);
            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }

    public List<SavableEntity> GetSavableEntitiesInScene()
    {
        var curScene = SceneManager.GetSceneByName(gameObject.name);
        return FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == curScene).ToList();
    }
}
