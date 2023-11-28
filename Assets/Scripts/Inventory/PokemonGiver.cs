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
                    yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}��Ϊ����Ļ�飡");

                }
                else
                {
                    yield return DialogueManager.Instance.ShowDialogueText($"���ڶ���������\n{pokemon.PokemonBase.PokemonName}���ͽ��˲ֿ⣡");

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
