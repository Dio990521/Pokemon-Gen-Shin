using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MoveToActorAction : CutsceneAction
{
    [SerializeField] private CutsceneActor _source;
    [SerializeField] private CutsceneActor _target;

    public override IEnumerator Play()
    {
        var sourceCharacter = _source.GetCharacter();
        var targetCharacter = _target.GetCharacter();

        if (_source.IsPlayer)
        {
            GameManager.Instance.PlayerController.StartMovingAnimation();
        }

        int xDistance =  (int)(targetCharacter.transform.position.x - sourceCharacter.transform.position.x);
        int yDistance = (int)(targetCharacter.transform.position.y - sourceCharacter.transform.position.y);
        if (xDistance == 0)
        {
            if (yDistance < 0)
            {
                yDistance += 1;
            }
            else
            {
                yDistance -= 1;
            }
        }
        else if (yDistance == 0)
        {
            if (xDistance < 0)
            {
                xDistance += 1;
            }
            else
            {
                xDistance -= 1;
            }
        }
        else
        {
            if (xDistance < 0)
            {
                xDistance += 1;
            }
            else
            {
                xDistance -= 1;
            }
        }
        List<Vector2> movePatterns = new()
        {
            new Vector2(xDistance, 0),
            new Vector2(0, yDistance)
        };
        foreach (var movePattern in movePatterns)
        {
            yield return sourceCharacter.Move(movePattern, checkCollisions: false);
        }
        sourceCharacter.Animator.SetFacingDirectionToTarget(targetCharacter);
        if (_source.IsPlayer)
        {
            GameManager.Instance.PlayerController.StopMovingAnimation();
        }
    }
}
