using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbandonedShip : MonoBehaviour, IPlayerTriggerable
{
    public bool TriggerRepeatedly => true;

    public void OnPlayerTriggered(PlayerController player)
    {
        if (!player.AvoidWildPokemon && Random.Range(1, 101) <= 5)
        {
            player.Character.Animator.IsMoving = false;
            GameManager.Instance.StartBattle(BattleTrigger.Ship);
        }
    }
}
