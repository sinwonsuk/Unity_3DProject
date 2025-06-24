using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Zone Config")]
public class ZoneConfig : ScriptableObject
{
    [Tooltip("���������� ����� �� ������ ����Ʈ. �� ������� �߽�, ������, ��� �ð�, ���� �ð�, �ʴ� ������ ����")]
    public List<ZonePhase> phases;
}

[System.Serializable]
public class ZonePhase
{
    [Tooltip("�ش� ������ ���� ���� �߽� ��ġ (���� ��ǥ)")]
    public Vector3 center;

    [Tooltip("�ش� ������ ���� ���� ������. ������Ʈ ������ = ��������2")]
    public float radius;

    [Tooltip("�� ����� ���۵� �� ������ �����ϱ� ������ ��ٸ��� �ð�(��)")]
    public float waitDuration;

    [Tooltip("���� �������� �߽�/���������� �����ϴ� �� �ɸ��� �ð�(��)")]
    public float shrinkDuration;

    [Tooltip("������ ���� �� �ۿ� �ִ� �÷��̾�� �ʴ� ������ ��������")]
    public float damagePerSecond;
}
