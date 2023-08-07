using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.U2D.Path;
using UnityEngine;
using UnityEngine.UI;

public enum InfoState { Info1, Info2, Detail }

public class PokemonInfoUI : MonoBehaviour
{

    private InfoState _state;
    private int _selectedMove;
    private Pokemon _pokemon;

    [SerializeField] private GameObject _infoUI1;
    [SerializeField] private GameObject _infoUI2;
    [SerializeField] private Sprite _tagImage1;
    [SerializeField] private Sprite _tagImage2;
    [SerializeField] private Image _tag;
    [SerializeField] private GameObject _selector;

    [Header("PokemonBase")]
    [SerializeField] private Image _pokemonImage;
    [SerializeField] private Text _pokemonName;
    [SerializeField] private Text _pokemonLevel;
    [SerializeField] private Image _pokeball;

    [Header("Info1")]
    [SerializeField] private Text _pokemonType;
    [SerializeField] private Text _pokemonPlace;
    [SerializeField] private GameObject _expBar;
    [SerializeField] private Text _hp;
    [SerializeField] private Text _atk;
    [SerializeField] private Text _dfs;
    [SerializeField] private Text _satk;
    [SerializeField] private Text _sdfs;
    [SerializeField] private Text _spd;
    [SerializeField] private Text _curExp;
    [SerializeField] private Text _expLeft;

    [Header("Info2")]
    [SerializeField] private List<MoveInfoUI> _moveInfoUIList;
    [SerializeField] private GameObject _moveDetail;
    [SerializeField] private Text _dmg;
    [SerializeField] private Text _acc;
    [SerializeField] private Text _moveDes;
    [SerializeField] private Text _button;
    [SerializeField] private GameObject _buttonObject;


    public void Show(Pokemon pokemon)
    {
        _pokemon = pokemon;
        _pokemonImage.sprite = pokemon.PokemonBase.FrontSprite;
        _pokemonName.text = pokemon.PokemonBase.PokemonName;
        _pokemonLevel.text = $"Lv.{pokemon.Level}";
        _pokeball.sprite = pokemon.PokeballSprite;
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(pokemon.PokemonBase.Type1.ToString());
        if (pokemon.PokemonBase.Type2 != PokemonType.None)
        {
            stringBuilder.Append(" + ").Append(pokemon.PokemonBase.Type2.ToString());
        }
        _pokemonType.text = stringBuilder.ToString();
        _pokemonPlace.text = GameManager.Instance.CurrentScene.MapName;
        stringBuilder.Clear();
        stringBuilder.Append(pokemon.Hp.ToString()).Append("/").Append(pokemon.MaxHp.ToString());
        _hp.text = stringBuilder.ToString();
        _atk.text = pokemon.PokemonBase.Attack.ToString();
        _dfs.text = pokemon.PokemonBase.Defense.ToString();
        _satk.text = pokemon.PokemonBase.SpAttack.ToString();
        _sdfs.text = pokemon.PokemonBase.SpDefense.ToString();
        _spd.text = pokemon.PokemonBase.Speed.ToString();
        _curExp.text = pokemon.Exp.ToString();
        _expLeft.text = pokemon.GetNextLevelExpLeft().ToString();
        _expBar.transform.localScale = new Vector3(pokemon.GetNormalizedExp(), _expBar.transform.localScale.y, _expBar.transform.localScale.z);
        for (int i = 0; i < pokemon.Moves.Count; ++i)
        {
            _moveInfoUIList[i].MoveName = pokemon.Moves[i].MoveBase.MoveName;
            _moveInfoUIList[i].MoveTypeName = pokemon.Moves[i].MoveBase.Type.ToString();
            _moveInfoUIList[i].PP = $"PP {pokemon.Moves[i].PP}/{pokemon.Moves[i].MoveBase.PP}";
            _moveInfoUIList[i].MoveTypeBG.color = Color.gray;
            _moveInfoUIList[i].MoveType.SetActive(true);
        }
        _tag.sprite = _tagImage1;
        gameObject.SetActive(true);
        _infoUI1.SetActive(true);
        _infoUI2.SetActive(false);
        _buttonObject.SetActive(false);
        _selectedMove = 0;
        _state = InfoState.Info1;

    }

    private void Close()
    {
        GameManager.Instance.State = GameState.PartyScreen;
        AudioManager.Instance.PlaySE(SFX.CANCEL);
        gameObject.SetActive(false);
    }

    public void HandleUpdate()
    {
        if (_state == InfoState.Info1)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _infoUI2.SetActive(true);
                _buttonObject.SetActive(true);
                _selector.SetActive(false);
                _moveDetail.SetActive(false);
                _tag.sprite = _tagImage2;
                _state = InfoState.Info2;
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                Close();
            }
        }
        else if (_state == InfoState.Info2)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _infoUI2.SetActive(false);
                _buttonObject.SetActive(false);
                _tag.sprite = _tagImage1;
                _state = InfoState.Info1;
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                AudioManager.Instance.PlaySE(SFX.CONFIRM);
                _selectedMove = 0;
                _state = InfoState.Detail;
                _button.text = "X   ·µ»Ø";
                _selector.SetActive(true);
                _moveDetail.SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                Close();
            }
        }
        else if (_state == InfoState.Detail)
        {
            HandleMoveSelect();
        }

    }

    private void HandleMoveSelect()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++_selectedMove;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            --_selectedMove;
        }

        _selectedMove = Mathf.Clamp(_selectedMove, 0, _pokemon.Moves.Count - 1);
        UpdateUI(_selectedMove);

        if (Input.GetKeyDown(KeyCode.X))
        {
            AudioManager.Instance.PlaySE(SFX.CANCEL);
            _moveDetail.SetActive(false);
            _button.text = "Z   ²é¿´";
            _selector.SetActive(false);
            _state = InfoState.Info2;
        }
    }

    private void UpdateUI(int selectedMove)
    {
        _moveDes.text = _pokemon.Moves[selectedMove].MoveBase.Description;
        _dmg.text = _pokemon.Moves[selectedMove].MoveBase.Power.ToString();
        _acc.text = _pokemon.Moves[selectedMove].MoveBase.Accuracy.ToString();
        _selector.transform.position = _moveInfoUIList[selectedMove].SelectorPos.position;
    }

}
