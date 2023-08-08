using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum SaveLoadUIState { Select, Saving, Loading}

public class SaveLoadUI : MonoBehaviour
{
    private int selection = 0;
    private int prevSelection = -1;
    [SerializeField] private GameObject _selector;
    [SerializeField] private List<DataSlotUI> _dataSlotUIs;
    private SaveLoadUIState _state;

    private string _dataFilePath;

    private void Init()
    {
        // 定义文件路径，这里我们将文件保存在Persistent Data Path中
        _dataFilePath = Path.Combine(Application.persistentDataPath, "pokemon-genshn-data.json");

        // 调用保存和读取方法示例
        SaveLoadData saveData = ReadFromFile();
        if (saveData != null)
        {
            for (int i = 0; i < _dataSlotUIs.Count; ++i)
            {
                _dataSlotUIs[i].Restore(saveData, i);
            }
        }

    }

    // 保存数据到文件
    private void SaveToFile(SaveLoadData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(_dataFilePath, json);
    }

    // 从文件读取数据
    private SaveLoadData ReadFromFile()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            return JsonUtility.FromJson<SaveLoadData>(json);
        }
        return null;
    }

    public void Show()
    {
        Init();
        selection = 0;
        _state = SaveLoadUIState.Select;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SaveDataSlot()
    {
        SaveLoadData data = new SaveLoadData();
        for (int i = 0; i < _dataSlotUIs.Count; ++i)
        {
            data.playTimes[i] = _dataSlotUIs[i].PlayTime.text;
            data.achievements[i] = _dataSlotUIs[i].Achievement.text;
            data.scenes[i] = _dataSlotUIs[i].Scene.text;
            data.dates[i] = _dataSlotUIs[i].Date.text;
            data.actives[i] = _dataSlotUIs[i].Active;
        }
        SaveToFile(data);
    }

    public void Close()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void HandleUpdate(bool save=true)
    {
        if (_state == SaveLoadUIState.Select)
        {

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selection += 1;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selection -= 1;
            }

            selection = Mathf.Clamp(selection, 0, 2);

            if (selection != prevSelection)
            {
                UpdateUI(selection);
            }
            prevSelection = selection;


            if (Input.GetKeyDown(KeyCode.Z))
            {
                AudioManager.Instance.PlaySE(SFX.CONFIRM);
                if (save)
                {
                    _state = SaveLoadUIState.Saving;
                    StartCoroutine(TrySave());
                }
                else
                {
                    _state = SaveLoadUIState.Loading;
                    StartCoroutine(TryLoad());
                }
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                AudioManager.Instance.PlaySE(SFX.CANCEL);
                Close();
                GameManager.Instance.StartFreeRoamState();
            }
        }

    }

    private void UpdateUI(int selection)
    {
        _selector.transform.position = _dataSlotUIs[selection].SelectorPos.position;
    }

    private IEnumerator TrySave()
    {
        int selectedChoice = 0;
        yield return DialogueManager.Instance.ShowDialogueText("要在这里保存吗？",
            waitForInput: false,
            choices: new List<string>() { "是的", "不了" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex,
            cancelX: false);
        if (selectedChoice == 0)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("saveFile").Append(selection);
            SavingSystem.i.Save(stringBuilder.ToString());
            _dataSlotUIs[selection].GetComponent<Image>().color = Color.white;
            _dataSlotUIs[selection].Scene.text = GameManager.Instance.CurrentScene.MapName;
            _dataSlotUIs[selection].Date.text = System.DateTime.Now.ToString();
            _dataSlotUIs[selection].PlayTime.text = GameManager.Instance.GamePlayTime;
            _dataSlotUIs[selection].Achievement.text = $"{AchievementManager.Instance.GetTotalProgress().ToString("F1")}%";
            _dataSlotUIs[selection].Active = true;
            SaveDataSlot();
        }
        _state = SaveLoadUIState.Select;
    }

    private IEnumerator TryLoad()
    {
        if (!_dataSlotUIs[selection].Active)
        {
            _state = SaveLoadUIState.Select;
            yield break;
        }
        int selectedChoice = 0;
        yield return DialogueManager.Instance.ShowDialogueText("要读取这个存档吗？",
            waitForInput: false,
            choices: new List<string>() { "是的", "不了" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex,
            cancelX: false);
        if (selectedChoice == 0)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("saveFile").Append(selection);
            yield return GameManager.Instance.LoadGame(stringBuilder.ToString());
        }
        else
        {
            _state = SaveLoadUIState.Select;
        }
    }
}
