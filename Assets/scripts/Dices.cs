using System.Collections;
using UnityEngine;

public class Dices : MonoBehaviour
{
    private Sprite[] diceSides;
    private SpriteRenderer rend;
    private bool coroutineAllowed = true;
    private Sprite normalSprite;
    public int CurrentValue { get; private set; }
    public bool IsBonus { get; set; }

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        rend = GetComponent<SpriteRenderer>();
        if (rend == null)
        {
            Debug.LogError($"SpriteRenderer not found on {gameObject.name}");
            return;
        }

        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        if (diceSides == null || diceSides.Length == 0)
        {
            Debug.LogError("Failed to load dice side sprites");
            return;
        }

        CurrentValue = 6;
        SetValue(CurrentValue);
        Debug.Log($"Dice {gameObject.name} initialized with value {CurrentValue}");
    }

    private void OnMouseDown()
    {
        Debug.Log($"Mouse down on dice {gameObject.name}");
        if (GameControl.Instance != null && !GameControl.Instance.gameOver && coroutineAllowed && !IsBonus)
        {
            DiceManager.Instance.RollAllMainDice();
        }
    }

    public void RollDice()
    {
        if (coroutineAllowed)
        {
            StartCoroutine("RollTheDice");
            Debug.Log($"Started rolling dice {gameObject.name}");
        }
    }

    private IEnumerator RollTheDice()
    {
        coroutineAllowed = false;
        int randomDiceSide = 0;
        for (int i = 0; i <= 20; i++)
        {
            randomDiceSide = Random.Range(0, 6);
            SetValue(randomDiceSide + 1);
            yield return new WaitForSeconds(0.05f);
        }

        CurrentValue = randomDiceSide + 1;
        coroutineAllowed = true;

        Debug.Log($"Dice {gameObject.name} rolled. Final value: {CurrentValue}");

        if (!IsBonus)
        {
            DiceManager.Instance.CheckDiceValues();
        }
    }

    public void SetValue(int value)
    {
        if (rend == null || diceSides == null || diceSides.Length == 0)
        {
            Debug.LogError($"Cannot set value for dice {gameObject.name}. SpriteRenderer or diceSides not initialized.");
            return;
        }

        CurrentValue = value;
        if (value < 1 || value > diceSides.Length)
        {
            Debug.LogError($"Invalid dice value: {value}. Must be between 1 and {diceSides.Length}");
            return;
        }

        rend.sprite = diceSides[value - 1];
        Debug.Log($"Dice {gameObject.name} value set to {value}");
    }

    public void SetDisabledSprite(Sprite disabledSprite)
    {
        if (rend != null)
        {
            normalSprite = rend.sprite;
            rend.sprite = disabledSprite;
            Debug.Log($"Dice {gameObject.name} set to disabled sprite");
        }
    }

    public void ResetToNormalSprite()
    {
        if (rend != null && normalSprite != null)
        {
            rend.sprite = normalSprite;
            Debug.Log($"Dice {gameObject.name} reset to normal sprite");
        }
    }
}
