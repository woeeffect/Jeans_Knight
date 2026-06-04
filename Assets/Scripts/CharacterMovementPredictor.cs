using UnityEngine;

public class PredictedMovementCursor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform targetCharacter;
    [SerializeField] private Transform cursorObject;

    [Header("Distance")]
    [SerializeField] private float minDistance = 0f;
    [SerializeField] private float maxDistance = 3f;

    [Header("Movement")]
    [SerializeField] private float movementSmoothness = 8f;
    [SerializeField] private float returnSmoothness = 6f;

    [Header("Speed Settings")]
    [SerializeField] private float maxCharacterSpeed = 8f;

    [Tooltip("Позволяет нелинейно менять дистанцию в зависимости от скорости")]
    [SerializeField]
    private AnimationCurve speedToDistanceCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Vector3 currentOffset;
    private Vector3 previousPosition;

    private void Start()
    {
        if (targetCharacter == null)
        {
            Debug.LogError($"{name}: Target Character is not assigned.");
            enabled = false;
            return;
        }

        if (cursorObject == null)
        {
            Debug.LogError($"{name}: Cursor Object is not assigned.");
            enabled = false;
            return;
        }

        previousPosition = targetCharacter.position;
    }

    private void LateUpdate()
    {
        float deltaTime = Time.deltaTime;

        if (deltaTime <= 0f)
            return;

        Vector3 currentPosition = targetCharacter.position;

        Vector3 velocity =
            (currentPosition - previousPosition) / deltaTime;

        previousPosition = currentPosition;

        velocity.y = 0f;

        float speed = velocity.magnitude;

        Vector3 targetOffset = Vector3.zero;

        if (speed > 0.01f)
        {
            Vector3 moveDirection = velocity.normalized;

            float normalizedSpeed =
                Mathf.Clamp01(speed / maxCharacterSpeed);

            float distanceFactor =
                speedToDistanceCurve.Evaluate(normalizedSpeed);

            float distance =
                Mathf.Lerp(minDistance, maxDistance, distanceFactor);

            targetOffset = moveDirection * distance;

            currentOffset = Vector3.Lerp(
                currentOffset,
                targetOffset,
                movementSmoothness * deltaTime);
        }
        else
        {
            currentOffset = Vector3.Lerp(
                currentOffset,
                Vector3.zero,
                returnSmoothness * deltaTime);
        }

        cursorObject.position = currentPosition + currentOffset;
    }
}