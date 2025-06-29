using UnityEngine;

public class CharacterSelector : MonoBehaviour
{

    public void OnMouseDown()
    {
        if (LobbyCameraManager.Instance.IsZoomedIn) return;

        // ī�޶� ��
        LobbyCameraManager.Instance.ZoomToCharacter(zoomTarget);
        //UI ����
        LobbyUIManager.Instance.ShowCharacterInfo(characterName, characterDesc);

        CharacterSelectionManager.Instance?.SetSelectedIndex(characterIndex);
    }

    public Transform zoomTarget; // ī�޶� �̵��� ��ġ
    public int characterIndex;
    public string characterName;
    public string characterDesc;
}
