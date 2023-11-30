using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearChoice : MonoBehaviour, IPlayerTriggerable
{
    public bool TriggerRepeatedly => true;

    public void OnPlayerTriggered(PlayerController player)
    {
        StartCoroutine(BossFight(player));
    }

    private IEnumerator BossFight(PlayerController player)
    {
        player.StopMovingAnimation();
        player.Character.Animator.SetFacingDirection(FacingDirection.Up);
        yield return DialogueManager.Instance.ShowDialogueText("（进入这扇门就要离开这个世界了...）");
        yield return DialogueManager.Instance.ShowDialogueText("（之后应该就无法回头了吧...）");
        yield return DialogueManager.Instance.ShowDialogueText("（我还有什么事没做完的吗？");
        ChoiceState.I.Choices = new List<string>() { "都做完了", "还有点事" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {

        }
        else
        {
            var dir = new Vector3(player.Character.Animator.MoveX, player.Character.Animator.MoveY);
            yield return player.Character.Move(-dir, checkCollisions: false);
        }
    }
}
