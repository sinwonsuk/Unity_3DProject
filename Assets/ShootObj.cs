using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class ShootObj : NetworkBehaviour
{
    [Networked] public bool isVisible { get; set; }
    [Networked] public bool flying { get; set; }
    [Networked] public Vector3 flyDir { get;  set; }
    [Networked] TickTimer poolTimer { get; set; }
    private Transform originalParent;
    [Networked] private Vector3 originalLocalPosition { get; set; }
    [Networked] private Quaternion originalLocalRotation { get; set; }

    [SerializeField] private float speed = 10f;
    [Networked] public HandSide Side { get; set; }
    [SerializeField] ItemState itemState;

    [SerializeField] private LayerMask hitLayers;

    [Header("풀/비활성화 제어")]
    [SerializeField] private ParticleSystem[] particles;
    [SerializeField] private Renderer[] renderers;

    [SerializeField] private int attackDamage;
    [Networked] float time { get; set; }

    [Networked] ItemClass state { get; set; }


    const string PlayerTag = "Player";

    public WeaponInfoConfig infoConfig;
    public override void Spawned()
    {
 
        isVisible = false;


        //foreach (var r in renderers)
        //    if (r != null) r.enabled = isVisible;

        //foreach (var p in particles)
        //{
        //    if (p == null) continue;
        //    if (isVisible)
        //    {
        //        if (!p.isPlaying) p.Play(true);
        //    }
        //    else
        //    {
        //        if (p.isPlaying) p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        //        p.Clear(true);
        //    }
        //}


        AttachToOwner(Object.InputAuthority);
    }

    public override void FixedUpdateNetwork()
    {

        if (!Object.HasStateAuthority)
            return;

        if (flying)
        {
            switch (state)
            {
                case ItemClass.None:
                    break;
                case ItemClass.One:
                    transform.localScale = infoConfig.ScaleOne;
                    break;
                case ItemClass.Two:
                    transform.localScale = infoConfig.ScaleTwo;
                    break;
                case ItemClass.Three:
                    transform.localScale = infoConfig.ScaleThree;
                    break;
                default:
                    break;
            }


            // 1) 현재 위치와 이번 틱 이동 벡터 계산
            Vector3 origin = transform.position;
            Vector3 displacement = flyDir * speed * Runner.DeltaTime;
            float distance = displacement.magnitude;

            // 2) 레이캐스트로 충돌 체크
            if (Physics.Raycast(origin, flyDir, out RaycastHit hit, distance, hitLayers, QueryTriggerInteraction.Ignore))
            {
 
                // (b) 충돌 대상에 따라 처리
                int layer = hit.collider.gameObject.layer;
                int playerLayer = LayerMask.NameToLayer("Player");
                int groundLayer = LayerMask.NameToLayer("Ground");

                if (layer == playerLayer)
                {
                    var ui = hit.collider.transform.parent.GetComponent<PlayerHealth>();
                    if (ui != null)
                        ui.TakeDamages(infoConfig.Attack);
                }

                // (c) 땅에 닿거나 플레이어에 맞으면 멈추기
                flying = false;
                poolTimer = TickTimer.None;
                isVisible = false;
            }
            else
            {
                // 3) 빗나갔으면 그냥 이동
                transform.position += displacement;

                // 4) 풀 복귀 타이밍 체크
                if (poolTimer.Expired(Runner))
                {
                    flying = false;
                    poolTimer = TickTimer.None;
                    isVisible = false;
                }
            }

            // 5) 디버그용 레이 시각화
            Debug.DrawRay(origin, flyDir * distance, Color.cyan, 1f);
        }
        else
        {
            // 부모 복귀 로직 (변경 없음)
            if (transform.parent != originalParent)
            {
                transform.SetParent(originalParent, false);
                transform.localPosition = originalLocalPosition;
                transform.localRotation = originalLocalRotation;
            }
        }
    }

    public void ArrowShoot(Vector3 dir,ItemClass itemClass)
    {
        if (Object.HasStateAuthority)
        {
            flying = true;
            flyDir = (dir - transform.position).normalized;
            isVisible = true;
            poolTimer = TickTimer.CreateFromSeconds(Runner, 10f);
        }

        isVisible = true;
        transform.SetParent(null, true);
        transform.forward = flyDir;


        state = itemClass;

 

    }
    public void OnTriggerEnter(Collider collider)
    {

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

                originalParent = socket;
                originalLocalPosition = transform.localPosition;
                originalLocalRotation = transform.localRotation;
                return;
            }
        }
    }

    public override void Render()
    {
        base.Render();

        // 네트워크로 동기화된 isVisible 을 읽어서 매 프레임 비주얼 업데이트
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
    }
}