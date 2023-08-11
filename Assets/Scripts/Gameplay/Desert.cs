using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Desert : MonoBehaviour, IPlayerTriggerable
{
    public bool TriggerRepeatedly => true;
    public bool NeedBattle = true;

    // Start battle with 10% probability when player is on the grass
    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.IsDesert = true;
        if (NeedBattle && Random.Range(1, 101) <= 5)
        {
            player.Character.Animator.IsMoving = false;
            StartCoroutine(GameManager.Instance.StartBattle(BattleTrigger.Water));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().Character.IsDesert = false;
        }
    }
}
