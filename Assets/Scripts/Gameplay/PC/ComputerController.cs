using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Tool.Singleton;
using static UnityEditor.Progress;
using System;

public enum ComputerState { Menu, BuyYuanshi, Buying, Room, Busy }

public class ComputerController : Singleton<ComputerController>
{
    [SerializeField] private WalletUI walletUI;
    [SerializeField] private ShopUI yuanshiShopUI;
    [SerializeField] private CountSelectorUI countSelectorUI;
    [SerializeField] private GameObject pokemonRoom;

    public event Action OnStart;
    public event Action OnFinish;
    private ComputerState state;
    private Inventory _inventory;
    private Computer _computer;

    private void Start()
    {
        _inventory = Inventory.GetInventory();
    }

    public IEnumerator Boost(Computer computer)
    {
        AudioManager.Instance.PlaySE(SFX.PC_ON);
        _computer = computer;
        OnStart?.Invoke();
        yield return StartMenuState();
    }

    private IEnumerator StartMenuState()
    {
        state = ComputerState.Menu;
        int selectedChoice = 0;
        yield return DialogueManager.Instance.ShowDialogueText("ʹ�õ�����Щʲô�أ�",
            waitForInput: false,
            choices: new List<string>() { "ԭʯ��ֵ", "ԭʯ�̵�", "�����βֿ�" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            AudioManager.Instance.PlaySE(SFX.PC_OPERATE);
            state = ComputerState.BuyYuanshi;
            walletUI.Show();
            yield return BuyYuanshi();
        }
        else if (selectedChoice == 1)
        {
            AudioManager.Instance.PlaySE(SFX.PC_OPERATE);
            GameManager.Instance.State = GameState.Computer;
            state = ComputerState.Buying;
            walletUI.Show();
            //yuanshiShopUI.Show(_computer.AvailableItems, (item) => StartCoroutine(BuyItem(item)),
            //() => StartCoroutine(OnBackFromBuying()));
        }
        else if (selectedChoice == 2)
        {
            AudioManager.Instance.PlaySE(SFX.PC_OPERATE);
            state = ComputerState.Room;
            pokemonRoom.gameObject.SetActive(true);
        }
        else if (selectedChoice == -1)
        {
            AudioManager.Instance.PlaySE(SFX.PC_OFF);
            OnFinish?.Invoke();
            yield break;
        }
    }

    public void HandleUpdate()
    {
        if (state == ComputerState.Buying)
        {
            yuanshiShopUI.HandleUpdate();
        }
        else if (state == ComputerState.Room)
        {

        }
    }

    private IEnumerator BuyYuanshi()
    {
        yield return DialogueManager.Instance.ShowDialogueText("С����飬�����������ע����ơ�\n��ע�⣺�����񲻻���׹��ε�Ӫ���������Ӱ�졣");
        int selectedChoice = 0;
        yield return DialogueManager.Instance.ShowDialogueText("��Ҫ��ֵ����ԭʯ�أ�",
            waitForInput: false,
            choices: new List<string>() { "60", "300", "980", "1980", "3280", "6480" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);
        switch (selectedChoice)
        {
            case -1:
                walletUI.Close();
                yield return StartMenuState();
                break;
            case 0:
                yield return BuyYuanshiConfirm(60);
                break;
            case 1:
                yield return BuyYuanshiConfirm(300);
                break;
            case 2:
                yield return BuyYuanshiConfirm(980);
                break;
            case 3:
                yield return BuyYuanshiConfirm(1980);
                break;
            case 4:
                yield return BuyYuanshiConfirm(3280);
                break;
            case 5:
                yield return BuyYuanshiConfirm(6480);
                break;
        }
    }

    private IEnumerator BuyYuanshiConfirm(int yuanshiAmount)
    {
        DialogueManager.Instance.CloseDialog();
        state = ComputerState.Busy;
        int selectedChoice = 0;
        int totalPrice = yuanshiAmount * 10;

        yield return DialogueManager.Instance.ShowDialogueText($"��ֵ{yuanshiAmount}ԭʯ��Ҫ����{totalPrice}Ħ����\nȷ�ϳ�ֵ��",
        waitForInput: false,
        choices: new List<string>() { "�ݺݵس�", "�����Լ�" },
        onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            if (Wallet.i.HasMoney(totalPrice))
            {
                _inventory.AddItem(Wallet.i.Yuanshi, yuanshiAmount);
                Wallet.i.TakeMoney(totalPrice);
                yield return DialogueManager.Instance.ShowDialogueText($"��л�ݹˣ�봽�Ŀ��ֹ��ס��");
            }
            else
            {
                yield return DialogueManager.Instance.ShowDialogueText($"��û����ô��Ħ����");
            }

        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"���ĺã���Ҫ���������");
        }
        walletUI.Close();
        state = ComputerState.BuyYuanshi;
    }

    private IEnumerator BuyItem(ItemBase item)
    {
        state = ComputerState.Busy;
        yield return DialogueManager.Instance.ShowDialogueText($"����ٸ���", waitForInput: false, autoClose: false);

        int countToBuy = 1;
        yield return countSelectorUI.ShowSelector(99, item.YuanshiPrice,
            (selectedCount) => countToBuy = selectedCount);

        DialogueManager.Instance.CloseDialog();
        int totalPrice = item.YuanshiPrice * countToBuy;
        if (Inventory.GetInventory().GetItemCount(Wallet.i.Yuanshi) >= totalPrice)
        {
            int selectedChoice = 0;
            yield return DialogueManager.Instance.ShowDialogueText($"һ����Ҫ����{totalPrice}ԭʯ������",
            waitForInput: false,
            choices: new List<string>() { "��ء", "����" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

            if (selectedChoice == 0)
            {
                _inventory.AddItem(item, countToBuy);
                Inventory.GetInventory().RemoveItem(Wallet.i.Yuanshi, totalPrice);
                Wallet.i.TakeMoney(0);
                yield return DialogueManager.Instance.ShowDialogueText($"��л�ݹˣ��´�������");
            }
        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"��û����ô��ԭʯ��");
        }
        state = ComputerState.Buying;
    }

    private IEnumerator OnBackFromBuying()
    {
        yuanshiShopUI.Close();
        walletUI.Close();
        yield return StartMenuState();
    }


}
