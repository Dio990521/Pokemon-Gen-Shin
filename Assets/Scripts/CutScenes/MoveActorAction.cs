using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveActorAction : CutsceneAction
{
    [SerializeField] private Character character;
    [SerializeField] private List<Vector2> movePatterns;
}
