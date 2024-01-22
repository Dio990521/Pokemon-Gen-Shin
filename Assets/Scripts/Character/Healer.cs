using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform player)
    {
        yield return DialogueManager.Instance.ShowDialogueText("要不要恢复啊小妹妹？", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "好的", "不用了" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {
            // Yes
            GameManager.Instance.PauseGame(true);
            yield return Fader.FadeIn(0.5f);
            AudioManager.Instance.PlaySE(SFX.POKEMON_HEAL, true);
            var playerParty = player.GetComponent<PokemonParty>();
            playerParty.Pokemons.ForEach(p => p.Heal());
            playerParty.PartyUpdated();
            yield return new WaitForSeconds(3f);
            yield return Fader.FadeOut(0.5f);
            yield return DialogueManager.Instance.ShowDialogueText($"你的宝可梦们都恢复健康了！");
            GameManager.Instance.PauseGame(false);
        }
        else if (selectedChoice == 1)
        {
            // No
            yield return DialogueManager.Instance.ShowDialogueText($"那你快滚。");
        }
 
    }
}
