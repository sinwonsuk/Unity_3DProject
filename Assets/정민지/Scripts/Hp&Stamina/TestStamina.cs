using UnityEngine;

public class TestStamina : MonoBehaviour
{
    public void OnclickStamina()
    {
        EventBus<UseStamina>.Raise(new UseStamina(10));
    }
}
