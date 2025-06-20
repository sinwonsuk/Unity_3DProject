using System.Collections;
using System.Collections.Generic;
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
    private int _currentPhaseIndex = 0;

    public override void Spawned()
    {
        base.Spawned();

        // ȣ��Ʈ(����)�� �� ���� �� ���� ����
        if (Runner.IsServer)
        {
            // ��Ʈ��ũ ������Ʈ�� �� ���־� Spawn
            var netObj = Runner.Spawn(zonePrefab, Vector3.zero, Quaternion.identity);
            _zoneInstance = netObj.gameObject;

            // SphereCollider(trigger) Ȯ��
            _zoneCollider = _zoneInstance.GetComponent<SphereCollider>();
            if (_zoneCollider == null)
                _zoneCollider = _zoneInstance.AddComponent<SphereCollider>();

            _zoneCollider.isTrigger = true;

            // �ڷ�ƾ���� ����� ���� ����
            StartCoroutine(ManageZone());
        }
    }

    private IEnumerator ManageZone()
    {
        var phases = zoneConfig.phases;

        for (int i = 0; i < phases.Count; i++)
        {
            _currentPhaseIndex = i;
            var phase = phases[i];

            // 1) �ش� �������� �ʱ� ����
            SetZone(phase.center, phase.radius);

            // 2) wait ����: while ������ �ٲ㼭 �� ������ ������ ����
            float waitElapsed = 0f;
            while (waitElapsed < phase.waitDuration)
            {
                ApplyDamage(Time.deltaTime);
                waitElapsed += Time.deltaTime;
                yield return null;
            }

            // 3) shrink ���� (���� �ڵ�)
            if (i < phases.Count - 1)
            {
                var next = phases[i + 1];
                float shrinkElapsed = 0f;
                while (shrinkElapsed < phase.shrinkDuration)
                {
                    float t = shrinkElapsed / phase.shrinkDuration;
                    Vector3 center = Vector3.Lerp(phase.center, next.center, t);
                    float radius = Mathf.Lerp(phase.radius, next.radius, t);

                    SetZone(center, radius);
                    ApplyDamage(Time.deltaTime);

                    shrinkElapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }
    }


    private void SetZone(Vector3 center, float radius)
    {
        _zoneInstance.transform.position = center;
        _zoneInstance.transform.localScale = Vector3.one * radius * 2f;
        _zoneCollider.center = Vector3.zero;
        _zoneCollider.radius = radius;
    }

    private void ApplyDamage(float deltaTime)
    {
        float dps = zoneConfig.phases[_currentPhaseIndex].damagePerSecond;

        foreach (var player in PlayerHealth.All)
        {
            float dist = Vector3.Distance(player.transform.position, _zoneInstance.transform.position);
            if (dist > _zoneCollider.radius)
                player.TakeDamage(dps * deltaTime);
        }
    }

}
