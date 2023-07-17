using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, InteractableObject, ISavable
{
    [Header("ItemChest")]
    [SerializeField] private ItemBase item;
    [SerializeField] private List<Sprite> animatedSprites;
    [SerializeField] private int itemAmt = 1;

    [Header("MoneyChest")]
    [SerializeField] private bool isMoney = false;
    [SerializeField] private int moneyAmt;
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
            Used = true;
            yield return PlayChestAnimation();
            if (initiator.gameObject.layer == GameLayers.instance.StepInteractableLayer)
            {
                yield return DialogueManager.Instance.ShowDialogueText("感觉踩到了什么东西。");
            }
            if (isMoney)
            {
                Wallet.i.AddMoney(moneyAmt);
                yield return DialogueManager.Instance.ShowDialogueText($"你获得了{moneyAmt}摩拉！");
            }
            else
            {
                initiator.GetComponent<Inventory>().AddItem(item, itemAmt);
                yield return DialogueManager.Instance.ShowDialogueText($"你找到了{itemAmt}个{item.ItemName}！");
            }
            
        }
        
    }

    private IEnumerator PlayChestAnimation()
    {
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
                spriteRenderer.sprite = animatedSprites[curFrame++];
                yield return new WaitForSeconds(0.1f);
            }
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
