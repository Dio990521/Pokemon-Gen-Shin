using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CountSelectorUI : MonoBehaviour
{
    [SerializeField] private Text countText;
    [SerializeField] private Text priceText;
    [SerializeField] private Image leftArrow;
    [SerializeField] private Image rightArrow;

    private bool selected;
    private int currentCount;

    private int maxCount;
    private float pricePerUnit;
    private bool _isYuanshi;

    public IEnumerator ShowSelector(int maxCount, float pricePerUnit,
        Action<int> onCountSelected, bool isYuanshi=false)
    {
        this.maxCount = maxCount;
        this.pricePerUnit = pricePerUnit;
        _isYuanshi = isYuanshi;

        selected = false;
        currentCount = 1;

        gameObject.SetActive(true);
        SetValues();
        SetArrow();

        yield return new WaitUntil(() => selected == true);

        onCountSelected?.Invoke(currentCount);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        int prevCount = currentCount;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentCount;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentCount;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentCount -= 10;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentCount += 10;
        }

        currentCount = Mathf.Clamp(currentCount, 1, maxCount);
        if (currentCount != prevCount)
        {
            SetValues();
            SetArrow();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
            selected = true;
        }
    }

    private void SetValues()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("x");
        stringBuilder.Append(currentCount);
        countText.text = stringBuilder.ToString();
        stringBuilder.Clear();
        stringBuilder.Append(pricePerUnit * currentCount);
        if (_isYuanshi)
        {
            stringBuilder.Append("ԭʯ");
        }
        else
        {
            stringBuilder.Append("��");
        }
        priceText.text = stringBuilder.ToString();
    }

    private void SetArrow()
    {
        if (currentCount == 1)
        {
            leftArrow.enabled = false;
            rightArrow.enabled = true;
        }
        else if (currentCount == maxCount)
        {
            leftArrow.enabled = true;
            rightArrow.enabled = false;
        }
        else
        {
            leftArrow.enabled = true;
            rightArrow.enabled = true;
        }
    }
}
