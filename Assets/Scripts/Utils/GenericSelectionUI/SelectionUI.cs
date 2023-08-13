using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PokeGenshinUtils.SelectionUI
{
    public class SelectionUI<T> : MonoBehaviour where T : ISelectableItem
    {
        private List<T> _items;
        private float selectedItem = 0;
        private float prevSelection = -1;

        //private float selectionTimer = 0;
        //const float selectionSpeed = 5;

        public void SetItems(List<T> items)
        {
            _items = items;
            UpdateSelectionUI();
        }

        public void ResetSelection()
        {
            selectedItem = 0;
            prevSelection = -1;
            UpdateSelectionUI();
        }

        public virtual void HandleUpdate()
        {
            //UpdateSelectionTimer();
            HandleListSelection();
            selectedItem = Mathf.Clamp(selectedItem, 0, _items.Count - 1);

            if (selectedItem != prevSelection)
            {
                UpdateSelectionUI();
            }

            prevSelection = selectedItem;
        }

        public void HandleListSelection(bool isGrid=false)
        {
            //float v = Input.GetAxis("Vertical");

            //if (selectionTimer == 0 && Mathf.Abs(v) > 0.2f)
            //{
            //    selectedItem += -(int)Mathf.Sign(v);
            //    selectionTimer = 1 / selectionSpeed;
            //}
            if (isGrid)
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    ++selectedItem;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    --selectedItem;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    ++selectedItem;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    --selectedItem;
                }
            }

        }

        public void UpdateSelectionUI()
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

