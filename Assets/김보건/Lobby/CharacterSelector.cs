using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public Transform zoomTarget; // ī�޶� �̵��� ��ġ
    public string characterName;
    public string characterDesc;

    public void OnMouseDown()
    {
        if (LobbyCameraManager.Instance.IsZoomedIn) return;

        // ī�޶� ��
        LobbyCameraManager.Instance.ZoomToCharacter(zoomTarget);
        //UI ����
        LobbyUIManager.Instance.ShowCharacterInfo(characterName, characterDesc);
    }
}
