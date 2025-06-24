using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Zone Config")]
public class ZoneConfig : ScriptableObject
{
    [Tooltip("순차적으로 적용될 존 페이즈 리스트. 각 페이즈마다 중심, 반지름, 대기 시간, 수축 시간, 초당 데미지 설정")]
    public List<ZonePhase> phases;
}

[System.Serializable]
public class ZonePhase
{
    [Tooltip("해당 페이즈 동안 존의 중심 위치 (월드 좌표)")]
    public Vector3 center;

    [Tooltip("해당 페이즈 동안 존의 반지름. 오브젝트 스케일 = 반지름×2")]
    public float radius;

    [Tooltip("이 페이즈가 시작된 후 수축을 시작하기 전까지 기다리는 시간(초)")]
    public float waitDuration;

    [Tooltip("다음 페이즈의 중심/반지름으로 보간하는 데 걸리는 시간(초)")]
    public float shrinkDuration;

    [Tooltip("페이즈 동안 존 밖에 있는 플레이어에게 초당 적용할 데미지량")]
    public float damagePerSecond;
}
