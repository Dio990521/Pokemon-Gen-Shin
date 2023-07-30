using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Text hpText;
    private int _curHp;
    private int _maxHp;
    private float _hpScale;

    public bool IsUpdating { get; private set; }

    // Set up the Hp information
    public void SetHp(float hpNormalized, int maxHp, int curHp)
    {
        transform.localScale = new Vector3(hpNormalized, 1f);
        _hpScale = hpNormalized;
        _maxHp = maxHp;
        _curHp = curHp;
        SetHpText(_curHp);
    }

    private void SetHpText(int curHp)
    {
        if (hpText != null)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(curHp.ToString()).Append(" / ").Append(_maxHp.ToString());
            hpText.text = stringBuilder.ToString();
        }
    }

    // Hp bar animation
    public IEnumerator SetHpSmooth(float newHp, float duration)
    {
        if (newHp <= 0)
        {
            duration = 1f;
        }
        IsUpdating = true;
        _hpScale = transform.localScale.x;
        // 创建一个DOTween的整数Tween，从currentCountdown到end，持续duration秒
        Tween countdownTween = DOTween.To(() => _hpScale, x => _hpScale = x, newHp, duration)
            .SetEase(Ease.Linear)
            .OnUpdate(UpdateHpBar)
            .OnComplete(CountdownComplete);

        // 在Tween结束时自动销毁，防止内存泄漏
        countdownTween.SetAutoKill(true);
        yield return null;
    }

    public IEnumerator StartCountdownAnimation(int end, float duration)
    {
        if (end <= 0)
        {
            duration = 1f;
        }
        // 创建一个DOTween的整数Tween，从currentCountdown到end，持续duration秒
        Tween countdownTween = DOTween.To(() => _curHp, x => _curHp = x, end, duration)
            .SetEase(Ease.Linear)
            .OnUpdate(UpdateCountdownText)
            .OnComplete(CountdownComplete);

        // 在Tween结束时自动销毁，防止内存泄漏
        countdownTween.SetAutoKill(true);
        yield return null;
    }

    private void UpdateCountdownText()
    {
        SetHpText(_curHp);
    }

    private void UpdateHpBar()
    {
        transform.localScale = new Vector3(_hpScale, 1f);
    }

    private void CountdownComplete()
    {
        // 倒数结束后的逻辑
        IsUpdating = false;
    }
}
