using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ZoneController : NetworkBehaviour
{
    [Header("�ڱ��� ������ (�ڱ� �ڽ�)")]
    [SerializeField] private NetworkPrefabRef zonePrefab;

    [Header("�ڱ��� ����")]
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
            // ù ������ ����
            BeginPhase(0);
        }
    }

    private void Start()
    {
        if (Runner != null && Runner.IsServer && !HasStateAuthority)
        {
            // ���� ���� �� �ڱ����� �������� ����
            Runner.Spawn(zonePrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("[ZoneController] �ڱ��� ��Ʈ��ũ�� ������");
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

        Debug.Log($"[Zone] Phase {index + 1} ����!");
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