using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{

    [SerializeField] private int sceneToLoad = -1;
    [SerializeField] private Transform spawnPoint;

    public Transform SpawnPoint => spawnPoint;

    private PlayerController player;

    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);
        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this);
        player.Character.SetPositionAndSnapToTile(destPortal.spawnPoint.position);

        Destroy(gameObject);

    }
}
