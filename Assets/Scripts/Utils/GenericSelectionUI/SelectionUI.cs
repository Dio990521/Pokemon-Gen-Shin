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

        //private float selectionTimer = 0;
        //const float selectionSpeed = 5;

        public event Action<int> OnSelected;
        public event Action OnBack;

        private SelectionType _selectionType;
        private int _gridWidth = 2;

        public void SetSelectionSettings(SelectionType selectionType, int gridWidth)
        {
            _selectionType = selectionType;
            _gridWidth = gridWidth;
        }

        public void SetItems(List<T> items)
        {
            _items = items;
            items.ForEach(i => i.Init());
            UpdateSelectionUI();
        }

        public void ResetSelection()
        {
            selectedItem = 0;
            prevSelection = -1;
            UpdateSelectionUI();
        }

        public virtual void ClampSelection()
        {
            selectedItem = Mathf.Clamp(selectedItem, 0, _items.Count - 1);
        }

        public virtual void HandleUpdate()
        {
            //UpdateSelectionTimer();
            if (_selectionType  == SelectionType.List)
            {
                HandleListSelection();
            }
            else if (_selectionType == SelectionType.Grid)
            {
                HandleGridSelection();
            }

            ClampSelection();

            if (selectedItem != prevSelection)
            {
                UpdateSelectionUI();
            }

            prevSelection = selectedItem;

            if (Input.GetButtonDown("Action"))
            {
                OnSelected?.Invoke(selectedItem);
            }
            else if (Input.GetButtonDown("Back"))
            {
                OnBack?.Invoke();
            }
        }

        public void HandleListSelection()
        {
            //float v = Input.GetAxis("Vertical");

            //if (selectionTimer == 0 && Mathf.Abs(v) > 0.2f)
            //{
            //    selectedItem += -(int)Mathf.Sign(v);
            //    selectionTimer = 1 / selectionSpeed;
            //}
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

        //private void UpdateSelectionTimer()
        //{
        //    if (selectionTimer > 0)
        //    {
        //        selectionTimer = Mathf.Clamp(selectionTimer - Time.deltaTime, 0, selectionTimer);
        //    }
        //}

    }
}

