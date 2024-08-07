using UnityEngine;

public class ForceZPosition : MonoBehaviour
{
    public float desiredZPosition = 0.1f;

    void Update()
    {
        Vector3 currentPosition = transform.position;
        if (currentPosition.z != desiredZPosition)
        {
            transform.position = new Vector3(currentPosition.x, currentPosition.y, desiredZPosition);
        }
    }
}
