using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using System.Linq;

public class RoomHUD : MonoBehaviour
{
    NetworkRunner runner;
    void Start() => runner = UnityEngine.Object.FindFirstObjectByType<NetworkRunner>();

    void OnGUI()
    {
        if (runner == null || runner.SessionInfo == null) return;

        GUILayout.BeginArea(new Rect(10, 10, 250, 100), GUI.skin.box);
        GUILayout.Label($"Scene  : {SceneManager.GetActiveScene().name}");
        GUILayout.Label($"Session: {runner.SessionInfo.Name}");
        GUILayout.Label($"Players: {runner.ActivePlayers.Count()}");
        foreach (var p in runner.ActivePlayers)
            GUILayout.Label($" PlayerRef {p.PlayerId}");
        GUILayout.EndArea();
    }
}