using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteIcon : MonoBehaviour
{
    private Image _image;
    private Text _text;
    [SerializeField] private float _offsetY;
    private Vector2 _startPos;
    private bool _isActive;

    private void Awake()
    {
        _isActive = false;
        _startPos = transform.localPosition;
        _image = GetComponent<Image>();
        _text = GetComponentInChildren<Text>();
    }

    public IEnumerator RouteIntroAnim(string mapName)
    {
        if (!_isActive)
        {
            _isActive = true;
            _text.text = mapName;
            _image.enabled = true;
            _text.enabled = true;
            transform.localPosition = _startPos;
            yield return transform.DOLocalMoveY(_startPos.y - _offsetY, 1f).WaitForCompletion();
            yield return new WaitForSeconds(3f);
            yield return transform.DOLocalMoveY(_startPos.y, 1f).WaitForCompletion();
            _image.enabled = false;
            _text.enabled = false;
            _isActive = false;
        }
    }
}
