using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PokeGenshinUtils.SelectionUI
{

    public enum SelectionType { List, Grid}

    public class SelectionUI<T> : MonoBehaviour where T : ISelectableItem
    {
        protected List<T> _items;
        protected int selectedItem = 0;
        protected int prevSelection = -1;

        public event Action<int> OnSelected;
        public event Action OnBack;

        private SelectionType _selectionType;
        private int _gridWidth = 2;

        public bool AllowUpdate = true;

        public void SetSelectionSettings(SelectionType selectionType, int gridWidth)
        {
            _selectionType = selectionType;
            _gridWidth = gridWidth;
        }

        public void SetItems(List<T> items)
        {
            AllowUpdate = true;
            _items = items;
            items.ForEach(i => i.Init());
            UpdateSelectionUI();
        }

        public virtual void ResetSelection()
        {
            selectedItem = 0;
            prevSelection = -1;
            UpdateSelectionUI();
        }

        public virtual void ClampSelection()
        {
            selectedItem = Mathf.Clamp(selectedItem, 0, _items.Count - 1);
        }

        public virtual void HandleUpdate(bool allowCancel=true)
        {
            if (AllowUpdate)
            {
                if (_selectionType == SelectionType.List)
                {
                    HandleListSelection();
                }
                else if (_selectionType == SelectionType.Grid)
                {
                    HandleGridSelection();
                }

                ClampSelection();

                if (_items.Count > 0 && selectedItem != prevSelection)
                {
                    UpdateSelectionUI();
                }

                prevSelection = selectedItem;

                if (Input.GetButtonDown("Action"))
                {
                    AudioManager.Instance.PlaySE(SFX.CONFIRM);
                    OnSelected?.Invoke(selectedItem);
                }
                else if (allowCancel && Input.GetButtonDown("Back"))
                {
                    AudioManager.Instance.PlaySE(SFX.CANCEL);
                    OnBack?.Invoke();
                }
            }
        }

        public virtual void HandleListSelection()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedItem += 1;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedItem -= 1;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedItem += 1;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedItem -= 1;
            }

        }

        public void HandleGridSelection()
        {
            
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedItem += _gridWidth;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedItem -= _gridWidth;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedItem += 1;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedItem -= 1;
            }
        }

        public virtual void UpdateSelectionUI()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].OnSelectionChanged(i == selectedItem);
            }
        }

    }
}

