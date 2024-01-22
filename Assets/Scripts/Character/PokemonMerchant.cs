using Game.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonMerchant : MonoBehaviour, InteractableObject
{
    [SerializeField] private List<PokemonBase> availablePokemons;
    [SerializeField] private Vector2 shopCameraOffset;

    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogueManager.Instance.ShowDialogueText("��ӭʹ�ó���������Ť������\n�Ƿ񻨷�10000ԭʯ������һ����δ��õı����Σ�", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "�õ�", "�´�һ��" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {
            var inventory = Inventory.GetInventory();
            if (inventory.GetItemCount(Wallet.I.Yuanshi) >= 10000)
            {
                bool canGet = false;
                PokemonBase newPokemonBase = null;
                foreach (var pokemonBase in availablePokemons)
                {
                    if (!AchievementManager.Instance.HasComplete(pokemonBase.Achievement, pokemonBase.PokemonName))
                    {
                        canGet = true;
                        newPokemonBase = pokemonBase;
                        break;
                    }
                }

                if (canGet)
                {
                    AudioManager.Instance.PlaySE(SFX.BUY);
                    inventory.RemoveItem(Wallet.I.Yuanshi, 10000);
                    Wallet.I.TakeMoney(0);
                    yield return DialogueManager.Instance.ShowDialogueText($"��ô��һ���㽫���\n�ĸ���������~");
                    AudioManager.Instance.PlaySE(SFX.XUANNAGE);
                    yield return DialogueManager.Instance.ShowDialogueText($"����Ҫѡ�ĸ��أ�����Ҫѡ�ĸ���~");
                    yield return DialogueManager.Instance.ShowDialogueText($"���ޣ������ˣ���...���ǣ�����");
                    availablePokemons.Shuffle();
                    var pokemonParty = initiator.GetComponent<PokemonParty>();
                    var newPokemon = new Pokemon(newPokemonBase, 10);
                    AudioManager.Instance.PlaySE(SFX.RECEIVE_POKEMON, true);
                    yield return DialogueManager.Instance.ShowDialogueText($"��{newPokemon.PokemonBase.PokemonName}����");
                    if (pokemonParty.AddPokemonToParty(newPokemon))
                    {
                        yield return DialogueManager.Instance.ShowDialogueText($"{newPokemon.PokemonBase.PokemonName}��Ϊ����Ļ�飡");
                    }
                    else
                    {
                        yield return DialogueManager.Instance.ShowDialogueText($"���ڶ���������\n{newPokemon.PokemonBase.PokemonName}���ͽ��˲ֿ⣡");
                    }
                }
                else
                {
                    yield return DialogueManager.Instance.ShowDialogueText($"���Ѿ��������ܲ��񵽵�\n���еı�����������ţ��");
                }


            }
            else
            {
                yield return DialogueManager.Instance.ShowDialogueText($"���ԭʯ����ร�");
            }

        }

    }


}
