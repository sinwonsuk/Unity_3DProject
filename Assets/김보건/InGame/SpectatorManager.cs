using UnityEngine;

public static class SpectatorManager
{
    /// �÷��̾ ������ ���� ��� ����
    public static void EnterSpectatorMode(Vector3 pos, Quaternion rot)
    {
        Vector3 spawnPos = pos + Vector3.up * 1.6f;
        SpectatorCameraController.Spawn(spawnPos, rot);

        Debug.Log("���� ���");
    }
}
