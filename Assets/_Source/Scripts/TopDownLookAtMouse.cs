using UnityEngine;

public class TopDownLookAtMouse : MonoBehaviour
{
    [SerializeField] private float m_RotationSpeed = 720f;

    private Camera m_MainCamera;

    public float RotationSpeed
    {
        get => this.m_RotationSpeed;
        set => this.m_RotationSpeed = Mathf.Max(0f, value);
    }

    private void Awake()
    {
        this.m_MainCamera = Camera.main;
    }

    private void Update()
    {
        if (this.m_MainCamera == null) this.m_MainCamera = Camera.main;
        if (this.m_MainCamera == null) return;

        Ray ray = this.m_MainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return;

        Vector3 targetPosition = hit.point;
        targetPosition.y = transform.position.y;

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude <= 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        if (this.m_RotationSpeed <= 0f)
        {
            transform.rotation = targetRotation;
            return;
        }

        float maxDegreesDelta = this.m_RotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxDegreesDelta);
    }
}
