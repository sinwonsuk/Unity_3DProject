using UnityEngine;

public class BloodAnim : MonoBehaviour
{
    public GameObject bloodCanvas;
    public void DestroyBloodObject()
    {
        Destroy(bloodCanvas);
    }
}
