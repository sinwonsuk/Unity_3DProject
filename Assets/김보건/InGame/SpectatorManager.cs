using UnityEngine;

public static class SpectatorManager
{
    /// 플레이어가 죽을때 관전 모드 진입
    public static void EnterSpectatorMode(Vector3 pos, Quaternion rot)
    {
        Vector3 spawnPos = pos + Vector3.up * 1.6f;
        SpectatorCameraController.Spawn(spawnPos, rot);

        Debug.Log("관전 모드");
    }
}
