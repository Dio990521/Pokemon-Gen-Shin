using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveEffects
{
    [SerializeField] private List<StatBoost> boosts;
    [SerializeField] private ConditionID status;
    [SerializeField] private ConditionID elementStatus;

    public List<StatBoost> Boosts { get { return boosts; } }
    public ConditionID Status { get { return status; } }

    public ConditionID ElementStatus { get { return elementStatus; } }
}
