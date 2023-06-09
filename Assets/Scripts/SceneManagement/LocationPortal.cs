using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private FacingDirection spawnDir;
    [SerializeField] private DestinationIdentifier destinationIdentifier;

    public Transform SpawnPoint => spawnPoint;

    public bool TriggerRepeatedly => false;

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
        AudioManager.instance.PlaySE(SFX.GO_OUT);
        yield return fader.FadeIn(0.65f);


        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationIdentifier == this.destinationIdentifier);
        player.Character.SetPositionAndSnapToTile(destPortal.spawnPoint.position);
        player.Character.Animator.SetFacingDirection(spawnDir);

        yield return fader.FadeOut(0.65f);
        GameManager.Instance.StartFreeRoamState();

    }
}
