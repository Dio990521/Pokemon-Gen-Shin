using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cave : MonoBehaviour, IPlayerTriggerable
{
    public bool TriggerRepeatedly => true;

    // Start battle with 10% probability when player is on the grass
    public void OnPlayerTriggered(PlayerController player)
    {
        if (!player.AvoidWildPokemon && Random.Range(1, 101) <= 5)
        {
            player.Character.Animator.IsMoving = false;
            GameManager.Instance.StartBattle(BattleTrigger.Cave);
        }
    }
}
