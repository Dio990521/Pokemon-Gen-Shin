using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform player, Dialogue dialogue)
    {
        int selectedChoice = 0;
        yield return DialogueManager.Instance.ShowDialogueText("要不要恢复啊小妹妹？",
            choices : new List<string>() { "好的", "不用了"},
            onChoiceSelected : (choiceIndex) => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            // Yes
            yield return Fader.i.FadeIn(0.5f);

            var playerParty = player.GetComponent<PokemonParty>();
            playerParty.Pokemons.ForEach(p => p.Heal());
            playerParty.PartyUpdated();

            yield return Fader.i.FadeOut(0.5f);
            yield return DialogueManager.Instance.ShowDialogueText($"你的宝可梦们都恢复健康了！");

        }
        else if (selectedChoice == 1)
        {
            // No
            yield return DialogueManager.Instance.ShowDialogueText($"那你快滚啊。");
        }


        
    }
}
