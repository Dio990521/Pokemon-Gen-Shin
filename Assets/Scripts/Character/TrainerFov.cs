using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFov : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        GameManager.Instance.OnEnterTrainersView(GetComponentInParent<TrainerController>());
    }
}
