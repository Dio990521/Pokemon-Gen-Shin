using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenuUI : MonoBehaviour
{
    public List<GameObject> Selectors;
    public Slider BGMSlider;
    public Slider SFXSlider;
    public Button DoubleSpeedButton;

    public Sprite OnButtonImage;
    public Sprite OffButtonImage;

    protected int selectedItem = 0;
    public bool IsDoubleSpeed = false;

    public event Action OnBack;
    public event Action<int> OnSelected;

    public void Show()
    {
        gameObject.SetActive(true);
        selectedItem = 0;
        DoubleSpeedButton.image.sprite = IsDoubleSpeed ? OnButtonImage : OffButtonImage;
        UpdateSelector(selectedItem);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            AudioManager.Instance.PlaySE(SFX.CURSOR);
            selectedItem += 1;
            selectedItem = Mathf.Clamp(selectedItem, 0, Selectors.Count - 1);
            UpdateSelector(selectedItem);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            AudioManager.Instance.PlaySE(SFX.CURSOR);
            selectedItem -= 1;
            selectedItem = Mathf.Clamp(selectedItem, 0, Selectors.Count - 1);
            UpdateSelector(selectedItem);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
            HandleOption(selectedItem, false);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
            HandleOption(selectedItem, true);
        }

        if (Input.GetButtonDown("Action"))
        {
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
            OnSelected?.Invoke(selectedItem);
        }
        else if (Input.GetButtonDown("Back"))
        {
            AudioManager.Instance.PlaySE(SFX.CANCEL);
            OnBack?.Invoke();
        }
    }

    public void UpdateSelector(int selectedItem)
    {
        for (int i = 0; i < Selectors.Count; i++)
        {
            if (i == selectedItem)
            {
                Selectors[i].SetActive(true);
            }
            else
            {
                Selectors[i].SetActive(false);
            }
        }
    }

    public void HandleOption(int selectedItem, bool isLeftPush)
    {
        switch (selectedItem)
        {
            case 0: // bgm volume
                var curBGMValue = BGMSlider.value;
                if (isLeftPush)
                {
                    curBGMValue -= 0.1f;
                }
                else
                {
                    curBGMValue += 0.1f;
                }
                curBGMValue = Mathf.Clamp(curBGMValue, 0, 1);
                BGMSlider.value = curBGMValue;
                AudioManager.Instance.ChangeMusicPlayerVol(curBGMValue);
                break;
            case 1: // sfx volume
                var curSFXValue = SFXSlider.value;
                if (isLeftPush)
                {
                    curSFXValue -= 0.1f;
                }
                else
                {
                    curSFXValue += 0.1f;
                }
                curSFXValue = Mathf.Clamp(curSFXValue, 0, 1);
                SFXSlider.value = curSFXValue;
                AudioManager.Instance.ChangeSfxPlayerVol(curSFXValue);
                break;
            case 2: // double speed
                if (isLeftPush)
                {
                    DoubleSpeedButton.image.sprite = OffButtonImage;
                    GameManager.Instance.ChangeTimeScale(1f);
                    IsDoubleSpeed = false;
                }
                else
                {
                    DoubleSpeedButton.image.sprite = OnButtonImage;
                    GameManager.Instance.ChangeTimeScale(2f);
                    IsDoubleSpeed = true;
                }
                break;
            case 3: // language
                break;
        }
    }


}
