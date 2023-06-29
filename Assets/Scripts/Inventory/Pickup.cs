using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, InteractableObject, ISavable
{
    [SerializeField] private ItemBase item;
    [SerializeField] private List<Sprite> animatedSprites;
    private SpriteRenderer spriteRenderer;

    public bool Used { get; set; } = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (!Used)
        {
            initiator.GetComponent<Inventory>().AddItem(item);
            Used = true;

            if (animatedSprites.Count == 0)
            {
                spriteRenderer.enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                AudioManager.instance.PlaySE(SFX.CHEST);
                int curFrame = 0;
                while (curFrame < animatedSprites.Count)
                {
                    spriteRenderer.sprite = animatedSprites[curFrame];
                    curFrame++;
                    yield return new WaitForSeconds(0.1f);
                }
            }

            yield return DialogueManager.Instance.ShowDialogueText($"ÄãÕÒµ½ÁË{item.ItemName}£¡");
        }
        
    }

    public object CaptureState()
    {
        return Used;
    }

    public void RestoreState(object state)
    {
        Used = (bool)state;
        if (Used)
        {
            if (animatedSprites.Count == 0)
            {
                spriteRenderer.enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                spriteRenderer.sprite = animatedSprites[animatedSprites.Count - 1];
            }
        }
    }
}
