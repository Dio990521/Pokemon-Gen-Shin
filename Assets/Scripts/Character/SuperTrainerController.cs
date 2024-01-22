using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SuperTrainerController : TrainerController, InteractableObject
{
    public List<PokemonBase> XiaoyaoPokemons;
    public List<PokemonBase> DoctorPokemons;
    public bool IsXiaoyao;
    public bool IsDoctor;

    public int BattleCount;

    public new IEnumerator Interact(Transform initiator)
    {
        character.LookTowards(initiator.position);
        if (IsXiaoyao)
        {
            yield return DialogueManager.Instance.ShowDialogueText("���ǳ���Сң��\n�һ������6ֻ100���ı����������ս��");
            yield return DialogueManager.Instance.ShowDialogueText($"��׼������ս������", autoClose: false);
            ChoiceState.I.Choices = new List<string>() { "ռ�꣡", "û׼���ã�" };
        }
        else if (IsDoctor)
        {
            yield return DialogueManager.Instance.ShowDialogueText("���ǳ���С���ʿ��\n�һ�����ɼ�ֻ100��ǿ���Boss�����ս��");
            yield return DialogueManager.Instance.ShowDialogueText($"������ս�ĸ��Ѷȵ��أ�", autoClose: false);
            ChoiceState.I.Choices = new List<string>() { "��Ҫ��3����", "��Ҫ��4����", "��Ҫ��5����", "��Ҫ��6����", "û׼���ã�" };
        }
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);
        int selectedChoice = ChoiceState.I.Selection;

        if (IsXiaoyao)
        {
            if (selectedChoice == 0)
            {
                BattleCount = 6;
                GameManager.Instance.StartTrainerBattle(this);
            }
        }
        else if (IsDoctor)
        {
            if (selectedChoice == 0)
            {
                BattleCount = 3;
                GameManager.Instance.StartTrainerBattle(this);
            }
            else if (selectedChoice == 1)
            {
                BattleCount = 4;
                GameManager.Instance.StartTrainerBattle(this);
            }
            else if (selectedChoice == 2)
            {
                BattleCount = 5;
                GameManager.Instance.StartTrainerBattle(this);
            }
            else if (selectedChoice == 3)
            {
                BattleCount = 6;
                GameManager.Instance.StartTrainerBattle(this);
            }

        }

    }

}
