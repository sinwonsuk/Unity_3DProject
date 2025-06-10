using TMPro;
using UnityEngine;

public class Gold : MonoBehaviour
{
    [SerializeField] private TMP_Text showGold;
    private int gold;

    void Update()
    {
       showGold.text = ($"{gold}");
    }

    public void getGold(int _money)
    {
        gold+= _money;
    }

    public void useGold(int _money)
    {
        gold -= _money;
    }
}
