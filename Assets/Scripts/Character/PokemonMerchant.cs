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
        yield return DialogueManager.Instance.ShowDialogueText("欢迎使用超级宝可梦扭蛋机！\n是否花费10000原石随机获得一个还未获得的宝可梦？", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "好的", "下次一定" };
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
                    yield return DialogueManager.Instance.ShowDialogueText($"那么这一次你将获得\n哪个宝可梦呢~");
                    AudioManager.Instance.PlaySE(SFX.XUANNAGE);
                    yield return DialogueManager.Instance.ShowDialogueText($"到底要选哪个呢，到底要选哪个呢~");
                    yield return DialogueManager.Instance.ShowDialogueText($"唔噢，出来了！这...这是！！！");
                    availablePokemons.Shuffle();
                    var pokemonParty = initiator.GetComponent<PokemonParty>();
                    var newPokemon = new Pokemon(newPokemonBase, 10);
                    AudioManager.Instance.PlaySE(SFX.RECEIVE_POKEMON, true);
                    yield return DialogueManager.Instance.ShowDialogueText($"是{newPokemon.PokemonBase.PokemonName}！！");
                    if (pokemonParty.AddPokemonToParty(newPokemon))
                    {
                        yield return DialogueManager.Instance.ShowDialogueText($"{newPokemon.PokemonBase.PokemonName}成为了你的伙伴！");
                    }
                    else
                    {
                        yield return DialogueManager.Instance.ShowDialogueText($"由于队伍已满，\n{newPokemon.PokemonBase.PokemonName}被送进了仓库！");
                    }
                }
                else
                {
                    yield return DialogueManager.Instance.ShowDialogueText($"你已经集齐了能捕获到的\n所有的宝可梦啦！！牛！");
                }


            }
            else
            {
                yield return DialogueManager.Instance.ShowDialogueText($"你的原石不够喔！");
            }

        }

    }


}
