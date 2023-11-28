using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonGiver : MonoBehaviour, ISavable
{
    [SerializeField] private List<Pokemon> pokemonsToGive;
    [SerializeField] private Dialogue dialogue;

    private bool used = false;

    public IEnumerator GivePokemon(PlayerController player)
    {
        yield return DialogueManager.Instance.ShowDialogue(dialogue);
        used = true;
        AudioManager.Instance.PlaySE(SFX.RECEIVE_POKEMON);
        foreach (var pokemon in pokemonsToGive)
        {
            if (!PokemonParty.GetPlayerParty().HasPokemon(pokemon))
            {
                pokemon.Init();
                if (player.GetComponent<PokemonParty>().AddPokemonToParty(pokemon))
                {
                    yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}成为了你的伙伴！");

                }
                else
                {
                    yield return DialogueManager.Instance.ShowDialogueText($"由于队伍已满，\n{pokemon.PokemonBase.PokemonName}被送进了仓库！");

                }
            }
        }

        
    }

    public bool CanBeGiven()
    {
        return pokemonsToGive.Count > 0 && !used;
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
