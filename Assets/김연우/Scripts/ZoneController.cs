using System.Collections;
using UnityEngine;
using Fusion;

public class ZoneController : NetworkBehaviour
{
    [Header("�� ������ ����")]
    [SerializeField] private ZoneConfig zoneConfig;

    [Header("��Ʈ��ũ �� ������")]
    [SerializeField] private NetworkPrefabRef zonePrefab;

    private GameObject _zoneInstance;
    private SphereCollider _zoneCollider;
    private int _currentPhaseIndex;

    public override void Spawned()
    {
        if (!Runner.IsServer)
            return;

        // �� ������Ʈ ����
        var netObj = Runner.Spawn(zonePrefab, Vector3.zero, Quaternion.identity);
        _zoneInstance = netObj.gameObject;
        _zoneCollider = _zoneInstance.GetComponent<SphereCollider>() ?? _zoneInstance.AddComponent<SphereCollider>();
        _zoneCollider.isTrigger = true;

        // ������ ���� �� ������ ƽ ����
        StartCoroutine(ManageZone());
        StartCoroutine(DamageTicker());
    }

    private IEnumerator ManageZone()
    {
        var phases = zoneConfig.phases;
        for (int i = 0; i < phases.Count; i++)
        {
            _currentPhaseIndex = i;
            var phase = phases[i];

            // �� ��ġ �� ũ�� ����
            _zoneInstance.transform.position = phase.center;
            _zoneInstance.transform.localScale = Vector3.one * phase.radius * 2f;
            _zoneCollider.center = Vector3.zero;
            _zoneCollider.radius = phase.radius;

            // ��� ����
            yield return new WaitForSeconds(phase.waitDuration);

            // ���� ����
            if (i < phases.Count - 1)
            {
                var next = phases[i + 1];
                float shrinkElapsed = 0f;
                while (shrinkElapsed < phase.shrinkDuration)
                {
                    float t = shrinkElapsed / phase.shrinkDuration;
                    Vector3 center = Vector3.Lerp(phase.center, next.center, t);
                    float radius = Mathf.Lerp(phase.radius, next.radius, t);

                    _zoneInstance.transform.position = center;
                    _zoneInstance.transform.localScale = Vector3.one * radius * 2f;
                    _zoneCollider.radius = radius;

                    shrinkElapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }
    }

    /// <summary>
    /// 1�� �������� ���� ������ DPS ����
    /// </summary>
    private IEnumerator DamageTicker()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            ApplyZoneDamagePerSecond();
        }
    }

    private void ApplyZoneDamagePerSecond()
    {
        var phase = zoneConfig.phases[_currentPhaseIndex];
        int damage = Mathf.CeilToInt(phase.damagePerSecond);

        foreach (var health in PlayerHealth.All)
        {
            if (!health.HasStateAuthority)
                continue;

            float dist = Vector3.Distance(health.transform.position, _zoneInstance.transform.position);
            if (dist > _zoneCollider.radius)
                health.TakeDamage(damage);
        }
    }
}
