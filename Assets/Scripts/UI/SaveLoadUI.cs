using PokeGenshinUtils.SelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadUI : SelectionUI<DataSlotUI>
{
    [SerializeField] private GameObject _selector;
    [SerializeField] private List<DataSlotUI> _dataSlotUIs;

    private string _dataFilePath;

    

    private void Awake()
    {
        SetItems(_dataSlotUIs);
    }

    private void Init()
    {
        // 定义文件路径，这里我们将文件保存在Persistent Data Path中
        _dataFilePath = Path.Combine(Application.persistentDataPath, "pokemon-genshin-data.json");
        Debug.Log(_dataFilePath);
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

    public IEnumerator TrySave()
    {
        yield return DialogueManager.Instance.ShowDialogueText("要在这里保存吗？", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "是的", "不了" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("saveFile").Append(selectedItem);
            SavingSystem.Instance.Save(stringBuilder.ToString());
            _dataSlotUIs[selectedItem].GetComponent<Image>().color = Color.white;
            _dataSlotUIs[selectedItem].Scene.text = GameManager.Instance.CurrentScene.MapName;
            _dataSlotUIs[selectedItem].Date.text = System.DateTime.Now.ToString();
            _dataSlotUIs[selectedItem].PlayTime.text = GameManager.Instance.GamePlayTime;
            _dataSlotUIs[selectedItem].Achievement.text = $"{AchievementManager.Instance.GetTotalProgress().ToString("F1")}%";
            _dataSlotUIs[selectedItem].Active = true;
            SaveDataSlot();
        }

    }

    public IEnumerator TryLoad()
    {
        if (!_dataSlotUIs[selectedItem].Active)
        {
            yield return DialogueManager.Instance.ShowDialogueText("此处没有存档！");
            yield break;
        }
        yield return DialogueManager.Instance.ShowDialogueText("要读取这个存档吗？", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "是的", "不了" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);
        int selectedChoice = ChoiceState.I.Selection;
        if (selectedChoice == 0)
        {
            GameManager.Instance.PauseGame(true);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("saveFile").Append(selectedItem);
            yield return Fader.FadeIn(1f);
            VideoManager.Instance.Stop();
            GameManager.Instance.StateMachine.Pop();
            GameManager.Instance.StateMachine.Pop();
            yield return GameManager.Instance.LoadGame(stringBuilder.ToString());
            if (GameManager.Instance.TitleUI.activeSelf)
            {
                GameManager.Instance.TitleUI.SetActive(false);
            }
            yield return Fader.FadeOut(1f);
            GameManager.Instance.PauseGame(false);
        }

    }
}
