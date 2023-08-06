using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DataSlotUI : MonoBehaviour
{
    [SerializeField] private Text _playTime;
    [SerializeField] private Text _achievement;
    [SerializeField] private Text _scene;
    [SerializeField] private Text _date;
    [SerializeField] private Transform _selectorPos;
    private bool _active;

    public Transform SelectorPos { get { return _selectorPos; } }
    public Text PlayTime { get => _playTime; set => _playTime = value; }
    public Text Achievement { get => _achievement; set => _achievement = value; }
    public Text Scene { get => _scene; set => _scene = value; }
    public Text Date { get => _date; set => _date = value; }
    public bool Active { get => _active; set => _active = value; }

    public void Restore(SaveLoadData saveData, int index)
    {
        _playTime.text = saveData.playTimes[index];
        _achievement.text = saveData.achievements[index];
        _scene.text = saveData.scenes[index];
        _date.text = saveData.dates[index];
        _active = saveData.actives[index];
        if (_active)
        {
            GetComponent<Image>().color = Color.white;
        }
    }
}
