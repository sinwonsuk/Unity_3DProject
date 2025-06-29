using UnityEngine;

public class CharacterSelector : MonoBehaviour
{

    public void OnMouseDown()
    {
        if (LobbyCameraManager.Instance.IsZoomedIn) return;

        // 카메라 줌
        LobbyCameraManager.Instance.ZoomToCharacter(zoomTarget);
        //UI 갱신
        LobbyUIManager.Instance.ShowCharacterInfo(characterName, characterDesc);

        CharacterSelectionManager.Instance?.SetSelectedIndex(characterIndex);
    }

    public Transform zoomTarget; // 카메라가 이동할 위치
    public int characterIndex;
    public string characterName;
    public string characterDesc;
}
