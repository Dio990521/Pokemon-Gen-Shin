using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    [SerializeField] private GameObject menu;
    [SerializeField] private Image menuCursor;
    [SerializeField] private float cursorOffset;

    public event Action<int> OnMenuSelected;
    public event Action OnBack;

    private List<Text> menuItems;

    private int selectedItem = 0;

    private void Awake()
    {
        menuItems = menu.GetComponentsInChildren<Text>().ToList();
    }

    public void OpenMenu()
    {
        AudioManager.Instance.PlaySE(SFX.CONFIRM);
        menu.SetActive(true);
        menuCursor.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        menuCursor.gameObject.SetActive(false);
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

        selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);

        if (prevSelection != selectedItem)
        {
            menuCursor.transform.position = menuItems[selectedItem].transform.position - new Vector3(cursorOffset, 0f, 0f);
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
