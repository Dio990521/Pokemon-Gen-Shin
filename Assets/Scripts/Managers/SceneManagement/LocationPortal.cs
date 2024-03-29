using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private FacingDirection spawnDir;
    [SerializeField] private int destinationIdentifier;

    public Transform SpawnPoint => spawnPoint;

    public bool TriggerRepeatedly => false;

    private PlayerController player;
    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        player.Character.Animator.IsMoving = false;
        StartCoroutine(Teleport());
    }

    private IEnumerator Teleport()
    {
        GameManager.Instance.PauseGame(true);
        player.Character.Animator.IsMoving = false;
        player.Character.Animator.IsRunning = false;
        AudioManager.Instance.PlaySE(SFX.GO_OUT);
        yield return Fader.FadeIn(0.5f);
        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationIdentifier == this.destinationIdentifier);
        player.Character.SetPositionAndSnapToTile(destPortal.spawnPoint.position);
        player.Character.Animator.SetFacingDirection(spawnDir);
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.PauseGame(false);
        yield return Fader.FadeOut(0.5f);

    }
}
