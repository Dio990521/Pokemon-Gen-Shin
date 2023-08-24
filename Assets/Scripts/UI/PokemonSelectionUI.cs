using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public enum PokemonSelectionState { Choosing, Confirm }

public class PokemonSelectionUI : MonoBehaviour
{
    [SerializeField] private List<AnimatedSprite> _pokeBalls;
    [SerializeField] private List<Pokemon> _selectablePokemons;
    [SerializeField] private GameObject _display;
    [SerializeField] private Text _message;
    [SerializeField] private ChoiceBox _choiceBox;
    private int _selection = 0;
    private int _prevSelection = -1;
    private PokemonSelectionState _state;
    private Vector3 _endPos;

    public void Show()
    {
        _endPos = _display.transform.position;
        _state = PokemonSelectionState.Choosing;
        StartPokeBallAnim(_selection);
        gameObject.SetActive(true);
        _message.text = "小田卷博士遇到麻烦了！\n请选择一个宝可梦帮助他吧！";
    }

    public void Close()
    {
        gameObject.SetActive(false);
        //GameManager.Instance.StartFreeRoamState();
    }

    public void HandleUpdate()
    {
        if (_state == PokemonSelectionState.Choosing)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                ++_selection;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                --_selection;
            }

            _selection = Mathf.Clamp(_selection, 0, 2);
            if (_prevSelection != _selection)
            {
                StartPokeBallAnim(_selection);
            }
            _prevSelection = _selection;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                _display.transform.GetChild(0).GetComponent<Image>().sprite = _selectablePokemons[_selection].PokemonBase.FrontSprite;
                _display.SetActive(true);
                StartDisplayAnim();
                _state = PokemonSelectionState.Confirm;
                _message.text = $"确定选择{_selectablePokemons[_selection].PokemonBase.PokemonName}吗？";
                StartCoroutine(HandleConfirm());
            }

        }
        else if (_state == PokemonSelectionState.Confirm)
        {
            
        }

    }

    private IEnumerator HandleConfirm()
    {
        int selectedChoice = 0;
        //yield return _choiceBox.ShowChoices(new List<string>() { "就决定是你了！", "让我再看一看！" },
        //    onChoiceSelected: choiceIndex => selectedChoice = choiceIndex,
        //    cancelX: false);
        yield return null;

        if (selectedChoice == 0)
        {
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
            _selectablePokemons[_selection].Init();
            PokemonParty.GetPlayerParty().AddPokemon(_selectablePokemons[_selection]);
            Close();
        }
        else if (selectedChoice == 1)
        {
            AudioManager.Instance.PlaySE(SFX.CANCEL);
            _state = PokemonSelectionState.Choosing;
            _choiceBox.gameObject.SetActive(false);
            _display.SetActive(false);
            _message.text = "小田卷博士遇到麻烦了！\n请选择一个宝可梦帮助他吧！";
        }
    }

    private void StartPokeBallAnim(int selection)
    {
        for (int i = 0;  i < _pokeBalls.Count; ++i)
        {
            if (i == selection)
            {
                _pokeBalls[i].enabled = true;
                _pokeBalls[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                _pokeBalls[i].enabled = false;
                _pokeBalls[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            
        }
    }

    private void StartDisplayAnim()
    {
        _display.transform.position = _pokeBalls[_selection].transform.position;
        _display.transform.localScale = new Vector3(0f, 0f, 0f);
        Vector3 initialScale = new Vector3(0f, 0f, 0f);
        Vector3 targetScale = new Vector3(1f, 1f, 1f);
        float duration = 1f;

        var sequence = DOTween.Sequence();
        sequence.Append(_display.transform.DOScale(targetScale, duration)
            .From(initialScale)
            .SetEase(Ease.OutBack));
        sequence.Join(_display.transform.DOMove(_endPos, duration));
        
    }
}
