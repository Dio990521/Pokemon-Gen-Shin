using PokeGenshinUtils.SelectionUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PokeStorageUI : SelectionUI<RedSelectorUI>
{
    [SerializeField] private List<RedSelectorUI> _redSelectors;
    [SerializeField] private Text _messageText;

    public Text MessageText { get => _messageText; set => _messageText = value; }

    private void Start()
    {
        SetItems(_redSelectors);
    }

}
