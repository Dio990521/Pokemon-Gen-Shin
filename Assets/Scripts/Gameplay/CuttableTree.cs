using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CuttableTree : MonoBehaviour, InteractableObject, ISavable
{

    [SerializeField] private List<Sprite> sprites;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    public bool Cut { get; set; } = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogueManager.Instance.ShowDialogueText("��������������Ա���ϵ�������ƻ���");

        var pokemonWithCut = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.PokemonBase.Type1 == PokemonType.��);
        if (pokemonWithCut != null )
        {
            yield return DialogueManager.Instance.ShowDialogueText($"��{pokemonWithCut.PokemonBase.PokemonName}��������", autoClose: false);
            ChoiceState.I.Choices = new List<string>() { "��ء", "����" };
            yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

            int selectedChoice = ChoiceState.I.Selection;

            if (selectedChoice == 0)
            {
                Cut = true;
                yield return DialogueManager.Instance.ShowDialogueText($"{pokemonWithCut.PokemonBase.PokemonName}ʹ������������һ����");
                AudioManager.Instance.PlaySE(SFX.CUT_TREE);
                int curFrame = 0;
                while (curFrame < sprites.Count)
                {
                    spriteRenderer.sprite = sprites[curFrame++];
                    yield return new WaitForSeconds(0.1f);
                }
                spriteRenderer.enabled = false;
                boxCollider.enabled = false;
            }

        }
    }

    public object CaptureState()
    {
        return Cut;
    }

    public void RestoreState(object state)
    {
        Cut = (bool)state;
        if (Cut)
        {
            spriteRenderer.enabled = false;
            boxCollider.enabled = false;
        }
    }
}
