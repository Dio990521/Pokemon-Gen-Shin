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
    protected SpriteRenderer _spriteRenderer;
    protected BoxCollider2D _boxCollider;

    [SerializeField] private CutsceneName ActivateCutscene;
    [SerializeField] private bool isStep = false;

    public bool Used { get; set; } = false;

    protected virtual void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (!Used)
        {
            GameManager.Instance.PauseGame(true);
            Used = true;
            yield return PlayChestAnimation();
            if (isStep)
            {
                yield return DialogueManager.Instance.ShowDialogueText("感觉踩到了什么东西。");
            }
            if (isMoney)
            {
                Wallet.I.AddMoney(moneyAmt);
                yield return DialogueManager.Instance.ShowDialogueText($"你获得了{moneyAmt}摩拉！");
            }
            else
            {
                initiator.GetComponent<Inventory>().AddItem(item, itemAmt);
                yield return DialogueManager.Instance.ShowDialogueText($"你找到了{itemAmt}个{item.ItemName}！");
                if (ActivateCutscene != CutsceneName.None && !GameKeyManager.Instance.GetBoolValue(ActivateCutscene.ToString()))
                {
                    GameKeyManager.Instance.SetBoolValue(ActivateCutscene.ToString(), true);
                }
            }
            GameManager.Instance.PauseGame(false);

        }

    }

    private IEnumerator PlayChestAnimation()
    {
        if (animatedSprites.Count == 0)
        {
            _spriteRenderer.enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            if (!isStep)
            {
                AudioManager.Instance.PlaySE(SFX.CHEST);
            }
            int curFrame = 0;
            while (curFrame < animatedSprites.Count)
            {
                _spriteRenderer.sprite = animatedSprites[curFrame++];
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
                _spriteRenderer.enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                _spriteRenderer.sprite = animatedSprites[animatedSprites.Count - 1];
            }
        }
    }
}
