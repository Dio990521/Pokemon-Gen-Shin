using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new passive move")]
public class PassiveMoveBase : ScriptableObject
{
    [SerializeField] private string moveName;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private PassiveMoveType passiveMoveType;
    public List<PassiveMoveEffect> PassiveMoveEffects;

    public string MoveName { get => moveName; set => moveName = value; }
    public string Description { get => description; set => description = value; }
    public PassiveMoveType PassiveMoveType { get => passiveMoveType; set => passiveMoveType = value; }

}

[Serializable]
public class PassiveMoveEffect
{
    public PassiveMoveType PassiveMoveType;
    public float Effectiveness;
}
