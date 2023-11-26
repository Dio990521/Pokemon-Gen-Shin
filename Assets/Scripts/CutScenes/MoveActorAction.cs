using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveActorAction : CutsceneAction
{
    [SerializeField] private CutsceneActor actor;
    [SerializeField] private List<Vector2> movePatterns;
    [SerializeField] private float _moveSpeed;

    public override IEnumerator Play()
    {
        var character = actor.GetCharacter();
        if (_moveSpeed != 0)
        {
            character.MoveSpeed = _moveSpeed;
        }
        if (actor.IsPlayer)
        {
            GameManager.Instance.PlayerController.StartMovingAnimation();
        }
        foreach (var movePattern in movePatterns)
        {
            yield return character.Move(movePattern, checkCollisions: false);
        }
        if (actor.IsPlayer)
        {
            GameManager.Instance.PlayerController.StopMovingAnimation();
        }
    }
}

[System.Serializable]
public class CutsceneActor
{
    [SerializeField] private bool isPlayer;
    [SerializeField] private Character character;

    public Character GetCharacter() => (isPlayer) ? PlayerController.I.Character : character;
    public bool IsPlayer => isPlayer;
}
