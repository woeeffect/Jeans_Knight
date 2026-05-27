using UnityEngine;

public class Test_Animator : MonoBehaviour
{
    public Animator animator;
    public float speed;

    [ContextMenu("ChangeSpeed")]
    public void ChangeSpeed()
    {
        animator.speed = speed;
    }
}
