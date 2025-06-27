using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class ShootObj : NetworkBehaviour
{
    [Networked] public bool isVisible { get; set; }
    [Networked] public bool flying { get; private set; }
    [Networked] public Vector3 flyDir { get; private set; }
    [Networked] TickTimer poolTimer { get; set; }
    private Transform originalParent;
    [Networked] private Vector3 originalLocalPosition { get; set; }
    [Networked] private Quaternion originalLocalRotation { get; set; }

    [SerializeField] private float speed = 10f;
    [Networked] public HandSide Side { get; set; }
    [SerializeField] ItemState itemState;

    [Header("풀/비활성화 제어")]
    [SerializeField] private ParticleSystem[] particles;
    [SerializeField] private Renderer[] renderers;

    const string PlayerTag = "Player";

    public override void Spawned()
    {
 
        isVisible = false;


        foreach (var r in renderers)
            if (r != null) r.enabled = isVisible;

        foreach (var p in particles)
        {
            if (p == null) continue;
            if (isVisible)
            {
                if (!p.isPlaying) p.Play(true);
            }
            else
            {
                if (p.isPlaying) p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                p.Clear(true);
            }
        }

        AttachToOwner(Object.InputAuthority);
    }

    public override void FixedUpdateNetwork()
    {

        foreach (var r in renderers)
            if (r != null) r.enabled = isVisible;

        foreach (var p in particles)
        {
            if (p == null) continue;
            if (isVisible)
            {
                if (!p.isPlaying) p.Play(true);
            }
            else
            {
                if (p.isPlaying) p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                p.Clear(true);
            }
        }

        if (flying)
        {

            if (transform.parent != null)
                transform.SetParent(null, true);

            transform.position += Runner.DeltaTime * speed * flyDir;

            if (poolTimer.Expired(Runner) && Object.HasStateAuthority)
            {
                flying = false;
                poolTimer = TickTimer.None;  
                isVisible = false; 
            }
        }

        else
        {
            // 부모 복귀
            if (transform.parent != originalParent)
            {
                transform.SetParent(originalParent, false);
                transform.localPosition = originalLocalPosition;
                transform.localRotation = originalLocalRotation;
            }
        }
    }

    public void ArrowShoot(Vector3 dir)
    {
        if (Object.HasStateAuthority)
        {
            flying = true;
            flyDir = (dir - transform.position).normalized;
            isVisible = true;
            poolTimer = TickTimer.CreateFromSeconds(Runner, 10f);
        }
        transform.SetParent(null, true);
        transform.forward = flyDir;
    }

    public void AttachToOwner(PlayerRef ownerRef)
    {
        var players = GameObject.FindGameObjectsWithTag(PlayerTag);
        foreach (var go in players)
        {
            var psm = go.GetComponent<PlayerStateMachine>();
            if (psm != null && psm.Object.InputAuthority == ownerRef)
            {
                Transform socket = (Side == HandSide.Right)
                    ? psm.RightHandTransform
                    : psm.LeftHandTransform;

                var charController = psm.GetComponent<CharacterController>();
                var weaponMeshCollider = GetComponentInChildren<MeshCollider>();

                if (charController != null && weaponMeshCollider != null)
                {
                    Physics.IgnoreCollision(charController, weaponMeshCollider, true);
                }

                transform.SetParent(socket, worldPositionStays: false);

                originalParent = transform.parent;
                originalLocalPosition = transform.localPosition;
                originalLocalRotation = transform.localRotation;
                return;
            }
        }
    }
}