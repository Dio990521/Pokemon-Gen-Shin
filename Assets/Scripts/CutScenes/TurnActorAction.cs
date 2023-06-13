using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnActorAction : CutsceneAction
{
    [SerializeField] private CutsceneActor actor;
    [SerializeField] private FacingDirection direction;

    public override IEnumerator Play()
    {
        actor.GetCharacter().Animator.SetFacingDirection(direction);
        yield break;
    }
}
