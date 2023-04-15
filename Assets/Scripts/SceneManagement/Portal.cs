using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{

    [SerializeField] private int sceneToLoad = -1;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private DestinationIdentifier destinationIdentifier;

    public Transform SpawnPoint => spawnPoint;

    private PlayerController player;

    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(transform.parent.gameObject);

        GameManager.Instance.PauseGame(true);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationIdentifier == this.destinationIdentifier);
        player.Character.SetPositionAndSnapToTile(destPortal.spawnPoint.position);

        GameManager.Instance.PauseGame(false);

        Destroy(transform.parent.gameObject);

    }
}

public enum DestinationIdentifier { A, B, C, D, E };
