using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform player, Dialogue dialogue)
    {
        yield return DialogueManager.Instance.ShowDialogueText("Ҫ��Ҫ�ָ���С���ã�", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "�õ�", "������" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {
            // Yes
            yield return Fader.FadeIn(0.5f);
            AudioManager.Instance.PlaySE(SFX.POKEMON_HEAL, true);
            var playerParty = player.GetComponent<PokemonParty>();
            playerParty.Pokemons.ForEach(p => p.Heal());
            playerParty.PartyUpdated();

            yield return Fader.FadeOut(0.5f);
            yield return DialogueManager.Instance.ShowDialogueText($"��ı������Ƕ��ָ������ˣ�");

        }
        else if (selectedChoice == 1)
        {
            // No
            yield return DialogueManager.Instance.ShowDialogueText($"����������");
        }


        
    }
}
