using UnityEngine;
using Fusion;
using System.Collections.Generic;

public class PlayerHealth : NetworkBehaviour
{
    [Networked]
    public float Health { get; set; } = 100f;
    public static readonly List<PlayerHealth> All = new List<PlayerHealth>();

    void Awake() => All.Add(this);
    void OnDestroy() => All.Remove(this);
    public void TakeDamage(float amount)
    {
        // 서버 권한이 있는 인스턴스에서만 변경
        if (HasStateAuthority)
        {
            Health -= amount;
            Debug.Log($"[Zone] {gameObject.name} took {amount:F2} damage. Current Health: {Health:F2}");

            if (Health <= 0f)
            {
                Debug.Log($"[Zone] {gameObject.name} died from zone damage.");
                // TODO: 사망 처리 (리스폰/제거 등)
            }
        }
    }
}
