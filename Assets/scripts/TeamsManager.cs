using UnityEngine;

public class TeamsManager : MonoBehaviour
{
    public static TeamsManager Instance;
    private int currentTeam = 1; // 1 for white, 2 for black

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
        Debug.Log("TeamsManager initialized");
    }

    public int GetCurrentTeam()
    {
        Debug.Log($"Current team: {(currentTeam == 1 ? "White" : "Black")}");
        return currentTeam;
    }

    public void SwitchTurns()
    {
        currentTeam = (currentTeam == 1) ? 2 : 1;
        ResetDiceSprites();
        Debug.Log($"Switched turns. Current team is now: {(currentTeam == 1 ? "White" : "Black")}");
    }

    private void ResetDiceSprites()
    {
        foreach (Dices dice in GameControl.Instance.mainDice)
        {
            if (dice != null)
            {
                dice.ResetToNormalSprite();
            }
        }
        Debug.Log("Reset dice sprites");
    }
}
