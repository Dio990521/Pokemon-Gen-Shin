using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonGiver : MonoBehaviour, ISavable
{
    [SerializeField] private Pokemon pokemonToGive;
    [SerializeField] private Dialogue dialogue;

    private bool used = false;

    public IEnumerator GivePokemon(PlayerController player)
    {
        yield return DialogueManager.Instance.ShowDialogue(dialogue);
        pokemonToGive.Init();
        player.GetComponent<PokemonParty>().AddPokemon(pokemonToGive);
        used = true;
        string dialogueText = $"{pokemonToGive.PokemonBase.PokemonName}加入了你的队伍！";
        yield return DialogueManager.Instance.ShowDialogueText(dialogueText);
        if (GameManager.Instance.PartyScreen.Pokemons.Count >= 6)
        {
            yield return DialogueManager.Instance.ShowDialogueText($"由于队伍已满，\n{pokemonToGive.PokemonBase.PokemonName}被送进了仓库！");
        }
    }

    public bool CanBeGiven()
    {
        return pokemonToGive != null && !used;
    }

    public object CaptureState()
    {
        return used;
    }

    public void RestoreState(object state)
    {
        used = (bool)state;
    }
}
