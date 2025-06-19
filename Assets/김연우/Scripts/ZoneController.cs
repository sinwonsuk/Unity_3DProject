using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ZoneController : NetworkBehaviour
{
    [Header("자기장 프리팹 (자기 자신)")]
    [SerializeField] private NetworkPrefabRef zonePrefab;

    [Header("자기장 설정")]
    [SerializeField] private ZoneConfig config;

    private GameObject zoneVisual;

    [Networked] private int phaseIndex { get; set; }
    [Networked] private float zoneRadius { get; set; }
    [Networked] private Vector3 zoneCenter { get; set; }
    [Networked] private float shrinkStartTime { get; set; }

    private ZonePhase currentPhase => config.phases[phaseIndex];
    private float elapsed => (float)(Runner.SimulationTime - shrinkStartTime);
    private float shrinkLerp => Mathf.Clamp01(elapsed / currentPhase.shrinkDuration);

    private Vector3 startCenter;
    private float startRadius;
    private bool shrinking;

    public override void Spawned()
    {
        zoneVisual = transform.Find("Visual")?.gameObject;

        if (HasStateAuthority)
        {
            // 첫 페이즈 시작
            BeginPhase(0);
        }
    }

    private void Start()
    {
        if (Runner != null && Runner.IsServer && !HasStateAuthority)
        {
            // 최초 실행 시 자기장을 동적으로 생성
            Runner.Spawn(zonePrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("[ZoneController] 자기장 네트워크로 생성됨");
        }
    }

    private void Update()
    {
        if (!HasStateAuthority) return;

        if (shrinking)
        {
            zoneCenter = Vector3.Lerp(startCenter, currentPhase.center, shrinkLerp);
            zoneRadius = Mathf.Lerp(startRadius, currentPhase.radius, shrinkLerp);

            if (shrinkLerp >= 1f)
            {
                shrinking = false;
                Invoke(nameof(AdvancePhase), currentPhase.waitDuration);
            }
        }

        UpdateZoneVisual();
        DamagePlayersOutsideZone();
    }

    void BeginPhase(int index)
    {
        if (index >= config.phases.Count) return;

        phaseIndex = index;
        zoneCenter = startCenter = config.phases[index].center;
        zoneRadius = startRadius = (index == 0) ? config.phases[0].radius : zoneRadius;
        shrinkStartTime = (float)Runner.SimulationTime;
        shrinking = true;

        Debug.Log($"[Zone] Phase {index + 1} 시작!");
    }

    void AdvancePhase()
    {
        BeginPhase(phaseIndex + 1);
    }

    void DamagePlayersOutsideZone()
    {
        foreach (var playerRef in Runner.ActivePlayers)
        {
            if (Runner.TryGetPlayerObject(playerRef, out var obj))
            {
                var dist = Vector3.Distance(obj.transform.position, zoneCenter);
                if (dist > zoneRadius)
                {
                    if (obj.TryGetComponent(out PlayerHealth hp))
                    {
                        float dps = currentPhase.damagePerSecond;
                        hp.ApplyDamage(dps * Time.deltaTime);
                    }
                }
            }
        }
    }

    void UpdateZoneVisual()
    {
        if (zoneVisual != null)
        {
            zoneVisual.transform.position = zoneCenter;
            zoneVisual.transform.localScale = Vector3.one * zoneRadius * 2f;
        }
    }
}