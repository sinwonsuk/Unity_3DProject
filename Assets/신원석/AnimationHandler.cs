using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationHandler
{
    private NetworkMecanimAnimator animator;
    private int hashAttackCount = Animator.StringToHash("AttackCount");
    private int hashWeaponCount = Animator.StringToHash("WeaponCount");



    public AnimationHandler(NetworkMecanimAnimator animator)
    {
        this.animator = animator;
    }

    public int WeaponCount
    {
        get => animator.Animator.GetInteger(hashWeaponCount);
        set => animator.Animator.SetInteger(hashWeaponCount, value);
    }


    public void ChangeWeapon(ItemState itemState)
    {
        animator.Animator.SetInteger("WeaponCount", (int)itemState);
    }
    public void ChangeBowWeaponState(float val)
    {
        animator.Animator.SetFloat("BowAttackFloat", val);
    }

    public void ChangeMagicAttackState(float val)
    {
        animator.Animator.SetFloat("MagicAttack", val);
    }

    public void ShootBowWeapon()
    {
        animator.Animator.SetTrigger("Shoot");
    }

    public void SetAttackBool(bool value)
    {
        animator.Animator.SetBool("Attack", value);
    }

    public void SetAttackCount(int value)
    {
        animator.Animator.SetInteger("AttackCount", value);
    }
}