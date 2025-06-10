using UnityEngine;

public class AnimationHandler
{
    private Animator animator;
    private int hashAttackCount = Animator.StringToHash("AttackCount");

    public AnimationHandler(Animator animator)
    {
        this.animator = animator;
    }

    public void SetAttackBool(bool value)
    {
        animator.SetBool("Attack", value);
    }

    public void SetAttackCount(int value)
    {
        animator.SetInteger(hashAttackCount, value);
    }
}