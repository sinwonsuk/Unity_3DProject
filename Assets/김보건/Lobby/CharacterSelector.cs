using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public Transform zoomTarget; // 카메라가 이동할 위치
    public string characterName;
    public string characterDesc;

    public void OnMouseDown()
    {
        if (LobbyCameraManager.Instance.IsZoomedIn) return;

        // 카메라 줌
        LobbyCameraManager.Instance.ZoomToCharacter(zoomTarget);
        //UI 갱신
        LobbyUIManager.Instance.ShowCharacterInfo(characterName, characterDesc);
    }
}
