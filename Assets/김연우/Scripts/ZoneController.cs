using System.Collections;
using UnityEngine;
using Fusion;

public class ZoneController : NetworkBehaviour
{
    [Header("존 페이즈 설정")]
    [SerializeField] private ZoneConfig zoneConfig;

    [Header("네트워크 존 프리팹")]
    [SerializeField] private NetworkPrefabRef zonePrefab;

    private GameObject _zoneInstance;
    private SphereCollider _zoneCollider;
    private int _currentPhaseIndex;

    public override void Spawned()
    {
        if (!Runner.IsServer)
            return;

        // 존 오브젝트 생성
        var netObj = Runner.Spawn(zonePrefab, Vector3.zero, Quaternion.identity);
        _zoneInstance = netObj.gameObject;
        _zoneCollider = _zoneInstance.GetComponent<SphereCollider>() ?? _zoneInstance.AddComponent<SphereCollider>();
        _zoneCollider.isTrigger = true;

        // 페이즈 로직 및 데미지 틱 시작
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

            // 존 위치 및 크기 설정
            _zoneInstance.transform.position = phase.center;
            _zoneInstance.transform.localScale = Vector3.one * phase.radius * 2f;
            _zoneCollider.center = Vector3.zero;
            _zoneCollider.radius = phase.radius;

            // 대기 구간
            yield return new WaitForSeconds(phase.waitDuration);

            // 수축 구간
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
    /// 1초 간격으로 현재 페이즈 DPS 적용
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
