using Unity.VisualScripting;
using UnityEngine;

public class AnimationHandler
{
    private Animator animator;
    private int hashAttackCount = Animator.StringToHash("AttackCount");
    private int hashWeaponCount = Animator.StringToHash("WeaponCount");
    public AnimationHandler(Animator animator)
    {
        this.animator = animator;
    }

    public int WeaponCount
    {
        get => animator.GetInteger(hashWeaponCount);
        set => animator.SetInteger(hashWeaponCount, value);
    }


    public void ChangeWeapon(ItemState itemState)
    {
        animator.SetInteger("WeaponCount", (int)itemState);
    }
    public void ChangeBowWeaponState(float val)
    {
        animator.SetFloat("BowAttackFloat", val);
    }
    public void ShootBowWeapon()
    {
        animator.SetTrigger("Shoot");
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