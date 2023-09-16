using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject _moveType;
    [SerializeField] private Image _moveTypeBG;
    [SerializeField] private Text _moveTypeName;
    [SerializeField] private Text _moveName;
    [SerializeField] private Text _pp;
    [SerializeField] private Transform _selectorPos;

    public string MoveTypeName { get { return _moveTypeName.text; } set { _moveTypeName.text = value; } }
    public string MoveName { get { return _moveName.text; } set { _moveName.text = value; } }
    public string PP { get { return _pp.text; } set { _pp.text = value; } }
    public Image MoveTypeBG { get { return _moveTypeBG; } set { _moveTypeBG = value; } }
    public GameObject MoveType { get { return _moveType; } }
    public Transform SelectorPos { get { return _selectorPos; } }

    public void Clear()
    {
        MoveName = "-";
        PP = "- -";
        MoveType.SetActive(false);
    }
}
