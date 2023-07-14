using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    public bool TriggerRepeatedly => false;

    // Start battle with 10% probability when player is on the grass
    public void OnPlayerTriggered(PlayerController player)
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.15f, GameLayers.instance.GrassMask);
        if (collider != null)
        {
            collider.GetComponent<GrassEffect>().InitEffect();
            if (UnityEngine.Random.Range(1, 101) <= 5)
            {
                player.Character.Animator.IsMoving = false;
                GameManager.Instance.StartBattle(BattleTrigger.LongGrass);
            }
        }
    }
}
