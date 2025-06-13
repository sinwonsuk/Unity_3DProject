using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MatchQueueManager : SimulationBehaviour
{
    public static MatchQueueManager Instance;

    public string CurrentRoomName; // ���� ��ư ���� ����� ����
    public bool RoomStarted => !string.IsNullOrEmpty(CurrentRoomName);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}