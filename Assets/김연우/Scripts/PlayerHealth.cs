using Fusion;
using UnityEngine;
public class PlayerHealth : NetworkBehaviour
{
    [Networked] public float Health { get; set; } = 100f;

    public void ApplyDamage(float amount)
    {
        if (!Object.HasStateAuthority) return;

        Health -= amount;
        if (Health <= 0f)
        {
            Debug.Log($"{Object.InputAuthority} died!");
            // »ç¸Á Ã³¸®
        }
    }
}
