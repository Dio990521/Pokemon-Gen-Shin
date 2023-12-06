using System.Collections;
using System.Collections.Generic;
using System.Text;
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
        _pokeball.sprite = GameManager.Instance.GetPokeSprite(pokemon.PokeballSpriteType);
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(pokemon.PokemonBase.Type1.ToString());
        if (pokemon.PokemonBase.Type2 != PokemonType.None)
        {
            stringBuilder.Append(" + ").Append(pokemon.PokemonBase.Type2.ToString());
        }
        _pokemonType.text = stringBuilder.ToString();
        _pokemonPlace.text = pokemon.CatchPlace;
        stringBuilder.Clear();
        stringBuilder.Append(pokemon.Hp.ToString()).Append("/").Append(pokemon.MaxHp.ToString());
        _hp.text = stringBuilder.ToString();
        _atk.text = pokemon.Attack.ToString();
        _dfs.text = pokemon.Defense.ToString();
        _satk.text = pokemon.SpAttack.ToString();
        _sdfs.text = pokemon.SpDefense.ToString();
        _spd.text = pokemon.Speed.ToString();
        RefreshStatuUI(pokemon);

        _curExp.text = pokemon.Exp.ToString();
        _expLeft.text = pokemon.GetNextLevelExpLeft().ToString();
        _expBar.transform.localScale = new Vector3(pokemon.GetNormalizedExp(), _expBar.transform.localScale.y, _expBar.transform.localScale.z);
        for (int i = 0; i < 4; ++i)
        {
            _moveInfoUIList[i].Clear();
        }
        for (int i = 0; i < pokemon.Moves.Count; ++i)
        {
            _moveInfoUIList[i].MoveName = pokemon.Moves[i].MoveBase.MoveName;
            _moveInfoUIList[i].MoveTypeName = pokemon.Moves[i].MoveBase.Type.ToString();
            _moveInfoUIList[i].PP = $"PP {pokemon.Moves[i].PP}/{pokemon.Moves[i].MoveBase.PP}";
            _moveInfoUIList[i].MoveTypeBG.color = ColorDB.ElementColors[pokemon.Moves[i].MoveBase.Type];
            _moveInfoUIList[i].MoveType.SetActive(true);
        }
        if (pokemon.PokemonBase.PassiveMove != null)
        {
            _moveInfoUIList[4].MoveName = pokemon.PokemonBase.PassiveMove.MoveName;
        }
        else
        {
            _moveInfoUIList[4].MoveName = "-";
        }
        _tag.sprite = _tagImage1;
        gameObject.SetActive(true);
        _infoUI1.SetActive(true);
        _infoUI2.SetActive(false);
        _buttonObject.SetActive(false);
        _selectedMove = 0;
        _state = InfoState.Info1;

    }

    private void RefreshStatuUI(Pokemon pokemon)
    {
        if (pokemon.IsBestStatus(Stat.攻击))
        {
            _atk.color = Color.blue;
        }
        else
        {
            _atk.color = Color.black;
        }
        if (pokemon.IsBestStatus(Stat.防御))
        {
            _dfs.color = Color.blue;
        }
        else
        {
            _dfs.color = Color.black;
        }
        if (pokemon.IsBestStatus(Stat.特攻))
        {
            _satk.color = Color.blue;
        }
        else
        {
            _satk.color = Color.black;
        }
        if (pokemon.IsBestStatus(Stat.特防))
        {
            _sdfs.color = Color.blue;
        }
        else
        {
            _sdfs.color = Color.black;
        }
        if (pokemon.IsBestStatus(Stat.速度))
        {
            _spd.color = Color.blue;
        }
        if (pokemon.IsBestStatus(Stat.生命))
        {
            _hp.color = Color.blue;
        }
        else
        {
            _spd.color = Color.black;
        }
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
                GameManager.Instance.StateMachine.Pop();
                AudioManager.Instance.PlaySE(SFX.CANCEL);
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
                if (_pokemon.Moves.Count > 0)
                {
                    _selectedMove = 0;
                    _state = InfoState.Detail;
                    _button.text = "X   返回";
                    _selector.SetActive(true);
                    _moveDetail.SetActive(true);
                }
                    
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                GameManager.Instance.StateMachine.Pop();
                AudioManager.Instance.PlaySE(SFX.CANCEL);
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
            if (_pokemon.PokemonBase.PassiveMove != null && _selectedMove == _pokemon.Moves.Count - 1)
            {
                _selectedMove = 4;
            }
            else
            {
                ++_selectedMove;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_pokemon.PokemonBase.PassiveMove != null && _selectedMove == 4)
            {
                _selectedMove = _pokemon.Moves.Count - 1;
            }
            else
            {
                --_selectedMove;
            }
            
        }

        if (_selectedMove == _pokemon.Moves.Count)
        {
            _selectedMove = 4;
        }
        if (_pokemon.PokemonBase.PassiveMove != null)
        {
            _selectedMove = Mathf.Clamp(_selectedMove, 0, 4);
        }
        else
        {
            _selectedMove = Mathf.Clamp(_selectedMove, 0, _pokemon.Moves.Count - 1);
        }
        UpdateUI(_selectedMove);

        if (Input.GetKeyDown(KeyCode.X))
        {
            AudioManager.Instance.PlaySE(SFX.CANCEL);
            _moveDetail.SetActive(false);
            _button.text = "Z   查看";
            _selector.SetActive(false);
            _state = InfoState.Info2;
        }
    }

    private void UpdateUI(int selectedMove)
    {
        if (selectedMove < 4)
        {
            _moveDes.text = _pokemon.Moves[selectedMove].MoveBase.Description;
            _dmg.text = _pokemon.Moves[selectedMove].MoveBase.Power.ToString();
            _acc.text = _pokemon.Moves[selectedMove].MoveBase.Accuracy.ToString();
            _selector.transform.position = _moveInfoUIList[selectedMove].SelectorPos.position;
        }
        else
        {
            _moveDes.text = _pokemon.PokemonBase.PassiveMove.Description;
            _dmg.text = "-";
            _acc.text = "-";
            _selector.transform.position = _moveInfoUIList[selectedMove].SelectorPos.position;
        }

    }

}
