using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PartyMenu : MonoBehaviour
{
    [SerializeField] private GameObject _partyMenuCursor;
    [SerializeField] private List<Transform> menuItemPos;
    private Pokemon _pokemon;

    public event Action<int, Pokemon> OnMenuSelected;
    public event Action OnBack;

    private int selectedItem = 0;
    private int prevSelection = -1;

    public void Show(Pokemon pokemon)
    {
        _pokemon = pokemon;
        selectedItem = 0;
        gameObject.SetActive(true);
        _partyMenuCursor.SetActive(true);
        _partyMenuCursor.transform.position = menuItemPos[selectedItem].position;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        _partyMenuCursor.SetActive(false);
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++selectedItem;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            --selectedItem;
        }

        selectedItem = Mathf.Clamp(selectedItem, 0, menuItemPos.Count - 1);
        if (prevSelection != selectedItem)
        {
            _partyMenuCursor.transform.position = menuItemPos[selectedItem].position;
        }
        prevSelection = selectedItem;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Close();
            OnMenuSelected?.Invoke(selectedItem, _pokemon);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            AudioManager.Instance.PlaySE(SFX.CANCEL);
            Close();
            OnBack?.Invoke();
        }

    }

}
