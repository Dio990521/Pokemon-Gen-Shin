using DG.Tweening;
using Game.Tool.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public static Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public static IEnumerator FadeOut(float time)
    {
        if (image.color.a == 0)
        {
            yield break;
        }
        yield return image.DOFade(0f, time).WaitForCompletion();
    }

    public static IEnumerator FadeIn(float time)
    {
        if (image.color.a == 1)
        {
            yield break;
        }
        yield return image.DOFade(1f, time).WaitForCompletion();
    }
}
