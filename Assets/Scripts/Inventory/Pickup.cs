using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, InteractableObject
{
    [SerializeField] private ItemBase item;

    public bool Used { get; set; } = false;

    public IEnumerator Interact(Transform initiator)
    {
        if (!Used)
        {
            initiator.GetComponent<Inventory>().AddItem(item);
            Used = true;

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            yield return DialogueManager.Instance.ShowDialogueText($"ƒ„’“µΩ¡À{item.ItemName}£°");
        }
        
    }
}
