using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private DestinationIdentifier destinationIdentifier;

    public Transform SpawnPoint => spawnPoint;

    private PlayerController player;
    private Fader fader;

    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        player.Character.Animator.IsMoving = false;
        StartCoroutine(Teleport());
    }

    private IEnumerator Teleport()
    {
        GameManager.Instance.PauseGame(true);
        yield return fader.FadeIn(0.5f);


        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationIdentifier == this.destinationIdentifier);
        player.Character.SetPositionAndSnapToTile(destPortal.spawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameManager.Instance.PauseGame(false);

    }
}
