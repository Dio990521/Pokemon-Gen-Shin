using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearChoice : MonoBehaviour, InteractableObject
{
    public bool TriggerRepeatedly => true;

    public IEnumerator Interact(Transform initiator)
    {
        var player = initiator.GetComponent<PlayerController>();
        player.StopMovingAnimation();
        player.Character.Animator.SetFacingDirection(FacingDirection.Up);
        yield return DialogueManager.Instance.ShowDialogueText("�����������ž�Ҫ�뿪���������...��");
        yield return DialogueManager.Instance.ShowDialogueText("��֮��Ӧ�þ��޷���ͷ�˰�...��");
        yield return DialogueManager.Instance.ShowDialogueText("���һ���ʲô��û������𣿣�", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "��������", "���е���" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {
            GameManager.Instance.PlayEnding();
        }

    }

}
