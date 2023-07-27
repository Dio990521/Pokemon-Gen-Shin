using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject menuCursor;

    public event Action<int> OnMenuSelected;
    public event Action OnBack;

    [SerializeField] private List<Transform> menuItemPos;

    private int selectedItem = 0;

    private void Awake()
    {
        menuCursor.SetActive(false);
    }

    public void OpenMenu()
    {
        AudioManager.Instance.PlaySE(SFX.MENU);
        menu.SetActive(true);
        menuCursor.SetActive(true);
        selectedItem = 0;
        menuCursor.transform.position = menuItemPos[selectedItem].position;
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        menuCursor.SetActive(false);
    }

    public void HandleUpdate()
    {
        int prevSelection = selectedItem;

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
            menuCursor.transform.position = menuItemPos[selectedItem].position;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            CloseMenu();
            OnMenuSelected?.Invoke(selectedItem);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            CloseMenu();
            OnBack?.Invoke();
        }

    }

}
