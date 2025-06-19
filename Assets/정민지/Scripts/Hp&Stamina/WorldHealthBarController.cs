using Fusion;
using UnityEngine;

public class WorldHealthBarController : NetworkBehaviour
{
    [SerializeField] private Canvas worldCanvas;

    public override void Spawned()
    {
        // �� ĳ�����̸� �Ӹ� �� ü�¹� ��Ȱ��ȭ
        if (Object.HasInputAuthority)
        {
            worldCanvas.enabled = false;
        }
    }
}
