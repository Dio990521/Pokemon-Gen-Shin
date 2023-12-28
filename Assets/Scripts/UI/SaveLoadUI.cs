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
        // �����ļ�·�����������ǽ��ļ�������Persistent Data Path��
        _dataFilePath = Path.Combine(Application.persistentDataPath, "pokemon-genshin-data.json");
        Debug.Log(_dataFilePath);
        // ���ñ���Ͷ�ȡ����ʾ��
        SaveLoadData saveData = ReadFromFile();
        if (saveData != null)
        {
            for (int i = 0; i < _dataSlotUIs.Count; ++i)
            {
                _dataSlotUIs[i].Restore(saveData, i);
            }
        }

    }

    // �������ݵ��ļ�
    private void SaveToFile(SaveLoadData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(_dataFilePath, json);
    }

    // ���ļ���ȡ����
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
        yield return DialogueManager.Instance.ShowDialogueText("Ҫ�����ﱣ����", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "�ǵ�", "����" };
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
            yield return DialogueManager.Instance.ShowDialogueText("�˴�û�д浵��");
            yield break;
        }
        yield return DialogueManager.Instance.ShowDialogueText("Ҫ��ȡ����浵��", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "�ǵ�", "����" };
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
