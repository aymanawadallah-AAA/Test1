using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;
    private List<int> availableMoves = new List<int>();
    private Sprite[] disabledDiceSides;
    private bool canRoll = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Debug.Log("DiceManager Awake called");
    }

    private void Start()
    {
        if (GameControl.Instance == null)
        {
            GameControl.Instance = FindObjectOfType<GameControl>();
        }
        if (GameControl.Instance == null)
        {
            Debug.LogError("GameControl.Instance is null in DiceManager Start. Make sure GameControl script is attached to a GameObject in the scene.");
            return;
        }

        InitializeDice();
        ShowBonusDice(false);
        disabledDiceSides = Resources.LoadAll<Sprite>("disable_dicesides/");
        Debug.Log("DiceManager Start initialized");
    }

    private void UpdateAvailableMoves()
    {
        availableMoves.Clear();
        foreach (Dices dice in GameControl.Instance.mainDice)
        {
            if (dice != null)
            {
                availableMoves.Add(dice.CurrentValue);
            }
        }
        Debug.Log($"Available moves updated: {string.Join(", ", availableMoves)}");
    }

    public bool IsValidMove(int moveDistance)
    {
        bool isValid = availableMoves.Contains(moveDistance);
        Debug.Log($"Checking if move distance {moveDistance} is valid: {isValid}");
        return isValid;
    }

    public void UseDiceValue(int value)
    {
        int index = availableMoves.IndexOf(value);
        if (index != -1)
        {
            availableMoves.RemoveAt(index);
            DisableDice(index);
            Debug.Log($"Used dice value {value}. Remaining moves: {string.Join(", ", availableMoves)}");
        }
        if (availableMoves.Count == 0)
        {
            EndTurn();
        }
    }

    private void DisableDice(int index)
    {
        if (index >= 0 && index < GameControl.Instance.mainDice.Length)
        {
            Dices dice = GameControl.Instance.mainDice[index];
            if (dice != null)
            {
                dice.SetDisabledSprite(disabledDiceSides[dice.CurrentValue - 1]);
                Debug.Log($"Disabled dice at index {index}");
            }
        }
    }

    private void InitializeDice()
    {
        if (GameControl.Instance.mainDice == null || GameControl.Instance.bonusDice == null)
        {
            Debug.LogError("Main dice or bonus dice array is null in GameControl");
            return;
        }

        GameControl.Instance.mainDice[0] = GameObject.Find("Dice1")?.GetComponent<Dices>();
        GameControl.Instance.mainDice[1] = GameObject.Find("Dice2")?.GetComponent<Dices>();
        GameControl.Instance.bonusDice[0] = GameObject.Find("BonusDice1")?.GetComponent<Dices>();
        GameControl.Instance.bonusDice[1] = GameObject.Find("BonusDice2")?.GetComponent<Dices>();

        // Log errors if any dice are not found
        if (GameControl.Instance.mainDice[0] == null) Debug.LogError("Dice1 not found or doesn't have Dices component");
        if (GameControl.Instance.mainDice[1] == null) Debug.LogError("Dice2 not found or doesn't have Dices component");
        if (GameControl.Instance.bonusDice[0] == null) Debug.LogError("BonusDice1 not found or doesn't have Dices component");
        if (GameControl.Instance.bonusDice[1] == null) Debug.LogError("BonusDice2 not found or doesn't have Dices component");

        Debug.Log("Dice initialized");
    }

    public void RollAllMainDice()
    {
        if (!canRoll)
        {
            Debug.Log("Cannot roll dice at this time");
            return;
        }

        if (GameControl.Instance == null || GameControl.Instance.mainDice == null)
        {
            Debug.LogError("GameControl.Instance or mainDice is null in RollAllMainDice");
            return;
        }

        foreach (Dices dice in GameControl.Instance.mainDice)
        {
            dice?.RollDice();
        }

        canRoll = false;
        CheckDiceValues();
        Debug.Log($"All main dice rolled. Can roll: {canRoll}");
    }

    public void CheckDiceValues()
    {
        if (GameControl.Instance == null || GameControl.Instance.mainDice == null || GameControl.Instance.mainDice.Length < 2)
        {
            Debug.LogError("GameControl.Instance or mainDice is null or has insufficient elements in CheckDiceValues");
            return;
        }

        if (GameControl.Instance.mainDice[0] != null && GameControl.Instance.mainDice[1] != null &&
            GameControl.Instance.mainDice[0].CurrentValue == GameControl.Instance.mainDice[1].CurrentValue)
        {
            ShowBonusDice(true);
            SetBonusDiceValues(GameControl.Instance.mainDice[0].CurrentValue);
            Debug.Log("Bonus dice shown due to matching values");
        }
        else
        {
            ShowBonusDice(false);
        }

        UpdateAvailableMoves();
        Debug.Log($"Dice values checked. Available moves: {string.Join(", ", availableMoves)}");
    }

    private void ShowBonusDice(bool show)
    {
        if (GameControl.Instance == null || GameControl.Instance.bonusDice == null)
        {
            Debug.LogError("GameControl.Instance or bonusDice is null in ShowBonusDice");
            return;
        }

        foreach (Dices dice in GameControl.Instance.bonusDice)
        {
            if (dice != null && dice.gameObject != null)
            {
                dice.gameObject.SetActive(show);
                dice.IsBonus = show;
            }
        }
        Debug.Log($"Bonus dice visibility set to: {show}");
    }

    private void SetBonusDiceValues(int value)
    {
        if (GameControl.Instance == null || GameControl.Instance.bonusDice == null)
        {
            Debug.LogError("GameControl.Instance or bonusDice is null in SetBonusDiceValues");
            return;
        }

        foreach (Dices dice in GameControl.Instance.bonusDice)
        {
            dice?.SetValue(value);
        }
        Debug.Log($"Bonus dice values set to: {value}");
    }

    public List<int> GetAvailableMoves()
    {
        return new List<int>(availableMoves);
    }

    public bool CanMove()
    {
        bool canMove = !canRoll && availableMoves.Count > 0;
        Debug.Log($"CanMove: {canMove} (canRoll: {canRoll}, availableMoves: {availableMoves.Count})");
        return canMove;
    }

    private void EndTurn()
    {
        canRoll = true;
        TeamsManager.Instance.SwitchTurns();
        Debug.Log("Turn ended. Switching to next player.");
    }
}
