using UnityEngine;

public class Inimigo : MonoBehaviour
{
    private Animator animator;
    private bool isMoving = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        UpdateAnimator();
    }

    void UpdateAnimator()
    {
        int transitionState = 0;

        if (isMoving)
        {
            transitionState = 1;
        }

        animator.SetInteger("transition", transitionState);
    }

    public void SetMoving(bool moving)
    {
        isMoving = moving;
    }

    public void SetAnimationState(int state)
    {
        animator.SetInteger("transition", state);
    }
}
