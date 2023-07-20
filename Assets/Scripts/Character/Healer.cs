using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform player, Dialogue dialogue)
    {
        int selectedChoice = 0;
        yield return DialogueManager.Instance.ShowDialogueText("Ҫ��Ҫ�ָ���С���ã�",
            choices : new List<string>() { "�õ�", "������"},
            onChoiceSelected : (choiceIndex) => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            // Yes
            yield return Fader.FadeIn(0.5f);

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
