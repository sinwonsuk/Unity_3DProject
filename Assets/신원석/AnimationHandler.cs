using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationHandler
{
    private NetworkMecanimAnimator animator;

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
    public int HitCount
    {
        get => playerStateMachine.HitCount;      // 네트워크 프로퍼티 읽기
        set => playerStateMachine.HitCount = value;  // 네트워크 프로퍼티 쓰기
    }


    public void ChangeWeapon(ItemState itemState)
    {
        switch (itemState)
        {
            case ItemState.none:
                WeaponCount = 0;
                animator.Animator.SetTrigger("NoWeaponEquip");
                break;
            case ItemState.Sword:
                WeaponCount = 1;
                animator.Animator.SetTrigger("KatanaEquip");
                break;
            case ItemState.Harberd:
                WeaponCount = 2;
                animator.Animator.SetTrigger("HalberdEquip");
                break;
            case ItemState.Bow:
                WeaponCount = 3;
                animator.Animator.SetTrigger("BowEquip");
                break;
            case ItemState.FireMagic:
                WeaponCount = 4;
                playerStateMachine.WeaponManager.magicState = ItemState.FireMagic;
                animator.Animator.SetTrigger("MagicEquip");
                break;
            case ItemState.IceMagic:
                WeaponCount = 4;
                playerStateMachine.WeaponManager.magicState = ItemState.IceMagic;
                animator.Animator.SetTrigger("MagicEquip");
                break;
            case ItemState.ElectricMagic:
                WeaponCount = 4;
                playerStateMachine.WeaponManager.magicState = ItemState.ElectricMagic;
                animator.Animator.SetTrigger("MagicEquip");
                break;
            case ItemState.FireBall:
                WeaponCount = 4;
                playerStateMachine.WeaponManager.magicState = ItemState.FireMagic;
                animator.Animator.SetTrigger("MagicEquip");
                break;
            case ItemState.IceBall:
                WeaponCount = 4;
                playerStateMachine.WeaponManager.magicState = ItemState.IceMagic;
                animator.Animator.SetTrigger("MagicEquip");
                break;
            case ItemState.ElectricBall:
                WeaponCount = 4;
                playerStateMachine.WeaponManager.magicState = ItemState.ElectricMagic;
                animator.Animator.SetTrigger("MagicEquip");
                break;
            case ItemState.HpPotion:
                WeaponCount = 5;
                playerStateMachine.WeaponManager.potionState = PotionState.HpPotion;
                animator.Animator.SetTrigger("PotionEquip");
                break; // 추가된 break 문  
            case ItemState.StaminaPotion:
                WeaponCount = 5;
                playerStateMachine.WeaponManager.potionState = PotionState.StaminaPotion;
                animator.Animator.SetTrigger("PotionEquip");
                break; // 추가된 break 문  
            default:
                break;
        }
    }
    public void ChangeRoll(int RollCount)
    {
        animator.Animator.SetInteger("RollCount", RollCount);
    }

    public void ChangeBowWeaponState(float val)
    {
        animator.Animator.SetFloat("BowAttackFloat", val);
    }
    public void ChangeDeathState()
    {
        animator.SetTrigger("DeathTrigger");
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
    public void SetPoitionBool(bool value)
    {
        animator.Animator.SetBool("Potion", value);
    }

    public void SetHitBool(bool value)
    {
        animator.Animator.SetBool("Hit", value);
    }

    public void SetAttackCount(int value)
    {
        animator.Animator.SetInteger("AttackCount", value);
    }
    public void SetHitCount(int value)
    {
        animator.Animator.SetInteger("HitCount", value);
    }
    public void SetHitTrigger()
    {
        animator.Animator.SetTrigger("HitTrigger");
    }
}