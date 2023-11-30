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
        yield return DialogueManager.Instance.ShowDialogueText("�����������ž�Ҫ�뿪���������...��");
        yield return DialogueManager.Instance.ShowDialogueText("��֮��Ӧ�þ��޷���ͷ�˰�...��");
        yield return DialogueManager.Instance.ShowDialogueText("���һ���ʲô��û�������");
        ChoiceState.I.Choices = new List<string>() { "��������", "���е���" };
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
