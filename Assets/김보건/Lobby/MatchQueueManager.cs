using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MatchQueueManager : SimulationBehaviour
{
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public static MatchQueueManager Instance;

    public string CurrentRoomName; // 먼저 버튼 누른 사람이 설정
    public bool RoomStarted => !string.IsNullOrEmpty(CurrentRoomName);

}