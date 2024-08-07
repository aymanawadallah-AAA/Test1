using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    private List<List<Checker>> board;
    public static GameControl Instance;
    public bool gameOver = false;
    private List<GameObject>[] cellOccupancy = new List<GameObject>[24];

    // Dice components
    public Dices[] mainDice = new Dices[2];
    public Dices[] bonusDice = new Dices[2];

    // Board components
    public Transform[] cellPoints = new Transform[24]; // 0 to 23

    // Team components
    public GameObject[] whiteCheckers = new GameObject[15];
    public GameObject[] blackCheckers = new GameObject[15];

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        Debug.Log("GameControl Awake called");
    }

    void Start()
    {
        InitializeComponents();
        InitializeCellOccupancy();
        InitializeBoard();
        PositionCheckers();
        Debug.Log("GameControl Start initialized");
        Debug.Log("Board state after initialization:");
        for (int i = 0; i < board.Count; i++)
        {
            string checkers = string.Join(", ", board[i].Select(c => c.gameObject.name));
            Debug.Log($"Cell {i}: {checkers}");
        }
    }

    void InitializeCellOccupancy()
    {
        for (int i = 0; i < 24; i++)
        {
            cellOccupancy[i] = new List<GameObject>();
        }
        Debug.Log("Cell occupancy initialized");
    }

    void InitializeComponents()
    {
        // Initialize dice
        mainDice[0] = GameObject.Find("Dice0")?.GetComponent<Dices>();
        mainDice[1] = GameObject.Find("Dice1")?.GetComponent<Dices>();
        bonusDice[0] = GameObject.Find("BonusDice0")?.GetComponent<Dices>();
        bonusDice[1] = GameObject.Find("BonusDice1")?.GetComponent<Dices>();

        // Initialize cell points
        for (int i = 0; i < 24; i++)
        {
            cellPoints[i] = GameObject.Find($"cellpoint({i})")?.transform;
            if (cellPoints[i] == null)
            {
                Debug.LogWarning($"cellpoint({i}) not found!");
            }
        }

        // Initialize checkers
        for (int i = 0; i < 15; i++)
        {
            whiteCheckers[i] = GameObject.Find($"white_checker({i})");
            blackCheckers[i] = GameObject.Find($"black_checker({i})");
            if (whiteCheckers[i] == null)
            {
                Debug.LogWarning($"white_checker({i}) not found!");
            }
            if (blackCheckers[i] == null)
            {
                Debug.LogWarning($"black_checker({i}) not found!");
            }
        }

        Debug.Log($"Initialized {mainDice.Length} main dice, {bonusDice.Length} bonus dice, {cellPoints.Length} cell points, {whiteCheckers.Length} white checkers, and {blackCheckers.Length} black checkers");
    }

    void InitializeBoard()
    {
        board = new List<List<Checker>>(24);
        for (int i = 0; i < 24; i++)
        {
            board.Add(new List<Checker>());
        }

        // Initialize white checkers
        for (int i = 0; i < whiteCheckers.Length; i++)
        {
            if (whiteCheckers[i] != null)
            {
                Checker checker = whiteCheckers[i].GetComponent<Checker>();
                if (checker != null)
                {
                    board[0].Add(checker);
                    checker.CurrentCellIndex = 0;
                    Debug.Log($"White checker {checker.gameObject.name} initialized at cell 0");
                }
            }
        }

        // Initialize black checkers
        for (int i = 0; i < blackCheckers.Length; i++)
        {
            if (blackCheckers[i] != null)
            {
                Checker checker = blackCheckers[i].GetComponent<Checker>();
                if (checker != null)
                {
                    board[23].Add(checker);
                    checker.CurrentCellIndex = 23;
                    Debug.Log($"Black checker {checker.gameObject.name} initialized at cell 23");
                }
            }
        }

        Debug.Log("Board initialized");
    }


    public int GetCellIndex(Vector3 position)
    {
        for (int i = 0; i < cellPoints.Length; i++)
        {
            if (Vector3.Distance(position, cellPoints[i].position) < 0.1f)
            {
                return i;
            }
        }
        return -1;
    }

    public int GetNearestCellIndex(Vector3 position)
    {
        int nearestIndex = 0;
        float minDistance = float.MaxValue;
        for (int i = 0; i < cellPoints.Length; i++)
        {
            float distance = Vector3.Distance(position, cellPoints[i].position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }
        Debug.Log($"Nearest cell to position {position} is cell {nearestIndex}");
        return nearestIndex;
    }

    public void UpdateCellOccupancy(GameObject checker, int fromCell, int toCell)
    {
        if (fromCell >= 0 && fromCell < 24)
        {
            cellOccupancy[fromCell].Remove(checker);
        }
        if (toCell >= 0 && toCell < 24)
        {
            cellOccupancy[toCell].Add(checker);
        }
        Debug.Log($"Updated cell occupancy: {checker.name} moved from cell {fromCell} to cell {toCell}");
    }

    public bool IsValidCellOccupancy(int cellIndex, GameObject checker)
    {
        if (cellIndex < 0 || cellIndex >= 24) return false;
        if (cellOccupancy[cellIndex].Count == 0) return true;
        bool isWhiteChecker = checker.name.StartsWith("white");
        bool isWhiteOccupant = cellOccupancy[cellIndex][0].name.StartsWith("white");
        bool isValid = isWhiteChecker == isWhiteOccupant || cellOccupancy[cellIndex].Count == 1;
        Debug.Log($"Checking cell occupancy for {checker.name} at cell {cellIndex}: {isValid}");
        return isValid;
    }

    void PositionCheckers()
    {
        if (cellPoints[0] == null || cellPoints[23] == null)
        {
            Debug.LogError("cellPoints[0] or cellPoints[23] is null!");
            return;
        }

        // Position all white checkers at cellpoint(0)
        for (int i = 0; i < whiteCheckers.Length; i++)
        {
            if (whiteCheckers[i] != null)
            {
                Vector3 newPosition = cellPoints[0].position + new Vector3(0, (i * 0.2f), -0.00001f);
                whiteCheckers[i].transform.position = newPosition;
            }
        }

        // Position all black checkers at cellpoint(23)
        for (int i = 0; i < blackCheckers.Length; i++)
        {
            if (blackCheckers[i] != null)
            {
                Vector3 newPosition = cellPoints[23].position + new Vector3(0, (i * 0.2f), 0.0f);
                blackCheckers[i].transform.position = newPosition;
            }
        }

        Debug.Log("Checkers positioned on the board");
        for (int i = 0; i < whiteCheckers.Length; i++)
        {
            if (whiteCheckers[i] != null)
            {
                Checker checker = whiteCheckers[i].GetComponent<Checker>();
                if (checker != null)
                {
                    board[0].Add(checker);
                    checker.CurrentCellIndex = 0;
                }
            }
        }

        // Add black checkers to the board
        for (int i = 0; i < blackCheckers.Length; i++)
        {
            if (blackCheckers[i] != null)
            {
                Checker checker = blackCheckers[i].GetComponent<Checker>();
                if (checker != null)
                {
                    board[23].Add(checker);
                    checker.CurrentCellIndex = 23;
                }
            }
        }

        Debug.Log("Checkers positioned and added to the board");

    }

    public bool IsTopChecker(Checker checker, int cellIndex)
    {
        if (cellIndex == -1)
        {
            Debug.LogError($"Invalid cell index (-1) for checker {checker.gameObject.name}");
            return false;
        }

        if (cellIndex < 0 || cellIndex >= board.Count)
        {
            Debug.LogError($"Invalid cell index: {cellIndex} for checker {checker.gameObject.name}");
            return false;
        }

        if (board[cellIndex].Count == 0)
        {
            Debug.LogError($"Cell {cellIndex} is empty");
            return false;
        }

        Checker topChecker = board[cellIndex][board[cellIndex].Count - 1];
        bool isTop = topChecker == checker;

        Debug.Log($"Checking if {checker.gameObject.name} is top checker at cell {cellIndex}. Result: {isTop}");

        return isTop; 
    }

        public bool CanMoveChecker(Checker checker)
        {
            // Check if it's the correct player's turn and if there are available moves
            // You'll need to implement this based on your game rules
            return true; // Placeholder
        }

        public void MoveChecker(Checker checker, int fromCell, int toCell)
        {
            if (fromCell < 0 || fromCell >= board.Count || toCell < 0 || toCell >= board.Count)
            {
                Debug.LogError($"Invalid cell index. FromCell: {fromCell}, ToCell: {toCell}");
                return;
            }

            if (!board[fromCell].Contains(checker))
            {
                Debug.LogError($"Checker {checker.gameObject.name} not found in cell {fromCell}");
                return;
            }

            board[fromCell].Remove(checker);
            board[toCell].Add(checker);
            checker.CurrentCellIndex = toCell;

            // Update checker's position on the board
            Vector3 newPosition = cellPoints[toCell].position + new Vector3(0, board[toCell].Count * 0.2f, 0);
            checker.transform.position = newPosition;

            Debug.Log($"Moved checker {checker.gameObject.name} from cell {fromCell} to cell {toCell}");
        }

        public bool IsValidMove(Checker checker, int fromCell, int toCell)
        {
            int distance = Mathf.Abs(toCell - fromCell);
            // Check if the move distance matches an available dice value
            // Check if the target cell is valid (not occupied by opponent's checkers, etc.)
            // You'll need to implement this based on your game rules
            return true; // Placeholder
        }
        public int GetCellIndexFromPosition(Vector3 position)
        {
            float minDistance = float.MaxValue;
            int closestCellIndex = -1;

            for (int i = 0; i < cellPoints.Length; i++)
            {
                float distance = Vector3.Distance(position, cellPoints[i].position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCellIndex = i;
                }
            }

            // You might want to set a maximum distance threshold
            if (minDistance > 1f) // Adjust this value based on your game's scale
            {
                return -1; // Return -1 if the position is too far from any cell
            }

            return closestCellIndex;
        }
    } 

