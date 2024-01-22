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
            yield return DialogueManager.Instance.ShowDialogueText("我是超级小遥！\n我会随机派6只100级的宝可梦与你对战！");
            yield return DialogueManager.Instance.ShowDialogueText($"你准备好挑战我了吗？", autoClose: false);
            ChoiceState.I.Choices = new List<string>() { "占戈！", "没准备好！" };
        }
        else if (IsDoctor)
        {
            yield return DialogueManager.Instance.ShowDialogueText("我是超级小田卷博士！\n我会随机派几只100级强大的Boss与你对战！");
            yield return DialogueManager.Instance.ShowDialogueText($"你想挑战哪个难度的呢？", autoClose: false);
            ChoiceState.I.Choices = new List<string>() { "我要打3个！", "我要打4个！", "我要打5个！", "我要打6个！", "没准备好！" };
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
