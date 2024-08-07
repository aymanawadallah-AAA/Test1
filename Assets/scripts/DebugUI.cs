using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
    public Text debugText;

    void Update()
    {
        if (debugText != null)
        {
            string currentTeam = TeamsManager.Instance.GetCurrentTeam() == 1 ? "White" : "Black";
            string availableMoves = string.Join(", ", DiceManager.Instance.GetAvailableMoves());
            string canRoll = DiceManager.Instance.CanMove() ? "No" : "Yes";

            debugText.text = $"Current Team: {currentTeam}\n" +
                             $"Available Moves: {availableMoves}\n" +
                             $"Can Roll: {canRoll}";
        }
    }
}
