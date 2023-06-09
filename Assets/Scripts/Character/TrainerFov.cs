using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFov : MonoBehaviour, IPlayerTriggerable
{
    public bool TriggerRepeatedly => false;

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        GameManager.Instance.OnEnterTrainersView(GetComponentInParent<TrainerController>());
    }
}
