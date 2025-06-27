using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using System.Linq;

public class RoomHUD : MonoBehaviour
{
    void Start() => runner = UnityEngine.Object.FindFirstObjectByType<NetworkRunner>();

    void OnGUI()
    {
        if (runner == null || runner.SessionInfo == null) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 150), GUI.skin.box);
        GUILayout.Label($"Scene  : {SceneManager.GetActiveScene().name}");
        GUILayout.Label($"Session: {runner.SessionInfo.Name}");
        GUILayout.Label($"Players: {runner.ActivePlayers.Count()}");

        foreach (var p in runner.ActivePlayers)
        {
            bool isHost = runner.IsServer && runner.LocalPlayer == p;
            string label = isHost ? $"HOST (PlayerRef {p.PlayerId})" : $"PlayerRef {p.PlayerId}";
            GUILayout.Label(label);
        }

        GUILayout.EndArea();
    }

    NetworkRunner runner;
}