using UnityEngine;

public class CheckerDragDrop : MonoBehaviour
{
    private GameControl gameControl;
    private Checker checker;
    private int currentCellIndex;
    private Vector3 dragOffset;
    private Camera mainCamera;
    private bool isDragging = false;

    void Start()
    {
        gameControl = FindObjectOfType<GameControl>();
        if (gameControl == null)
        {
            Debug.LogError($"GameControl not found for {gameObject.name}");
            enabled = false;
            return;
        }

        checker = GetComponent<Checker>();
        if (checker == null)
        {
            Debug.LogError($"Checker component not found on {gameObject.name}");
            enabled = false;
            return;
        }

        currentCellIndex = checker.CurrentCellIndex;
        if (currentCellIndex == -1)
        {
            Debug.LogError($"CurrentCellIndex not set for {gameObject.name}. Attempting to find correct cell.");
            currentCellIndex = gameControl.GetCellIndexFromPosition(transform.position);
            if (currentCellIndex != -1)
            {
                checker.CurrentCellIndex = currentCellIndex;
                Debug.Log($"Set CurrentCellIndex for {gameObject.name} to {currentCellIndex}");
            }
            else
            {
                Debug.LogError($"Unable to determine cell for {gameObject.name}. Disabling CheckerDragDrop.");
                enabled = false;
                return;
            }
        }

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError($"Main camera not found for {gameObject.name}");
            enabled = false;
            return;
        }

        Debug.Log($"CheckerDragDrop initialized for {gameObject.name} at cell {currentCellIndex}");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            EndDrag();
        }
        else if (isDragging)
        {
            DragChecker();
        }
    }

    void TryStartDrag()
    {
        if (gameControl == null || checker == null)
        {
            Debug.LogError($"GameControl or Checker is null for {gameObject.name}");
            return;
        }

        if (currentCellIndex == -1)
        {
            Debug.LogError($"CurrentCellIndex is -1 for {gameObject.name}. Cannot start drag.");
            return;
        }

        if (gameControl.IsTopChecker(checker, currentCellIndex))
        {
            if (gameControl.CanMoveChecker(checker))
            {
                Debug.Log($"{gameObject.name} can be moved from cell {currentCellIndex}");
                Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = transform.position.z;
                dragOffset = transform.position - mousePosition;
                isDragging = true;
                Debug.Log($"Started dragging {gameObject.name} from cell {currentCellIndex}");
            }
            else
            {
                Debug.Log($"{gameObject.name} cannot be moved from cell {currentCellIndex}");
            }
        }
        else
        {
            Debug.Log($"{gameObject.name} is not the top checker at cell {currentCellIndex}");
        }
    }

    void DragChecker()
    {
        if (mainCamera == null)
        {
            Debug.LogError($"Main camera is null for {gameObject.name}");
            return;
        }

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;
        transform.position = mousePosition + dragOffset;
    }

    void EndDrag()
    {
        if (gameControl == null || checker == null)
        {
            Debug.LogError($"GameControl or Checker is null for {gameObject.name}");
            return;
        }

        isDragging = false;
        int targetCellIndex = gameControl.GetCellIndexFromPosition(transform.position);

        Debug.Log($"Attempting to move {gameObject.name} from cell {currentCellIndex} to cell {targetCellIndex}");

        if (gameControl.IsValidMove(checker, currentCellIndex, targetCellIndex))
        {
            try
            {
                gameControl.MoveChecker(checker, currentCellIndex, targetCellIndex);
                currentCellIndex = targetCellIndex;
                Debug.Log($"{gameObject.name} moved from cell {currentCellIndex} to cell {targetCellIndex}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error moving checker: {e.Message}");
                checker.ResetPosition();
            }
        }
        else
        {
            checker.ResetPosition();
            Debug.Log($"{gameObject.name} returned to original position at cell {currentCellIndex}");
        }
    }
}
