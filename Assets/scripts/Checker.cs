using UnityEngine;

public class Checker : MonoBehaviour
{
    public enum CheckerColor { White, Black }

    public CheckerColor Color { get; private set; }
    public int CurrentCellIndex { get; set; } = -1;

    private Vector3 originalPosition;

    private void Awake()
    {
        // Initialize based on the checker's name
        Color = gameObject.name.StartsWith("white") ? CheckerColor.White : CheckerColor.Black;
        Debug.Log($"Checker {gameObject.name} initialized with color {Color}");
    }

    private void Start()
    {
        // Initialize based on the checker's name
        Color = gameObject.name.StartsWith("white") ? CheckerColor.White : CheckerColor.Black;
        Debug.LogError($"Checker {gameObject.name} initialized with color {Color}");
    }
    public void Initialize(CheckerColor color, int startingCellIndex)
    {
        Color = color;
        CurrentCellIndex = startingCellIndex;
        originalPosition = transform.position;
    }

    public void ResetPosition()
    {
        transform.position = originalPosition;
    }
}
