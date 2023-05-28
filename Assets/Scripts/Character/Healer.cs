using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform player, Dialogue dialogue)
    {
        yield return DialogueManager.Instance.ShowDialogue(dialogue);

        yield return Fader.i.FadeIn(0.5f);

        var playerParty = player.GetComponent<PokemonParty>();
        playerParty.Pokemons.ForEach(p => p.Heal());
        playerParty.PartyUpdated();

        yield return Fader.i.FadeOut(0.5f);
    }
}
