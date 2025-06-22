using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationHandler
{
    private NetworkMecanimAnimator animator;
    private int hashAttackCount = Animator.StringToHash("AttackCount");
    private int hashWeaponCount = Animator.StringToHash("WeaponCount");


    PlayerStateMachine playerStateMachine;

    public AnimationHandler(NetworkMecanimAnimator animator,PlayerStateMachine playerStateMachine)
    {
        this.animator = animator;
        this.playerStateMachine = playerStateMachine;
    }

    public int WeaponCount
    {
        get => animator.Animator.GetInteger(hashWeaponCount);
        set => animator.Animator.SetInteger(hashWeaponCount, value);
    }
    public int RollCount
    {
        get => playerStateMachine.RollCount;      // 네트워크 프로퍼티 읽기
        set => playerStateMachine.RollCount = value;  // 네트워크 프로퍼티 쓰기
    }

    public void ChangeWeapon(ItemState itemState)
    {
        animator.Animator.SetInteger("WeaponCount", (int)itemState);
    }
    public void ChangeRoll(int RollCount)
    {
        animator.Animator.SetInteger("RollCount", RollCount);
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