using UnityEngine;

public class TopDownLookAtMouse : MonoBehaviour
{
    private Camera m_MainCamera;

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
        transform.rotation = targetRotation;
    }
}
