using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    [Networked] public int CurrentHp { get; private set; }
    [SerializeField] private int maxHp;

    public override void Spawned()
    {
        if (HasStateAuthority)
            CurrentHp = maxHp;

        if (Object.HasInputAuthority)
        {
            EventBus<HealthChanged>.Raise(new HealthChanged(Object.InputAuthority, CurrentHp,maxHp));
        }
    }

    public void TakeDamage(int dmg)
    {
        if (!HasStateAuthority) return; //권한을 가지고있지 않으면 리턴

        CurrentHp-=dmg;

        if (Object.HasInputAuthority)
        {
            EventBus<HealthChanged>.Raise(new HealthChanged(Object.InputAuthority, CurrentHp,maxHp));
        }
    }
}
