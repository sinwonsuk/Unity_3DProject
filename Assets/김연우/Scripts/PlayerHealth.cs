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
        // ���� ������ �ִ� �ν��Ͻ������� ����
        if (HasStateAuthority)
        {
            Health -= amount;
            Debug.Log($"[Zone] {gameObject.name} took {amount:F2} damage. Current Health: {Health:F2}");

            if (Health <= 0f)
            {
                Debug.Log($"[Zone] {gameObject.name} died from zone damage.");
                // TODO: ��� ó�� (������/���� ��)
            }
        }
    }
}
