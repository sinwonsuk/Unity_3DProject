using UnityEngine;

public class playertest : MonoBehaviour
{
    int adad = 0;

    private void OnEnable()
    {
        EventBus<Eventplatertest>.OnEvent += asdadasd;    
    }

    private void OnDisable()
    {
        EventBus<Eventplatertest>.OnEvent -= asdadasd;
    }
    void asdadasd(Eventplatertest adada)
    {
        adada.playertest.adad = 5;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        EventBus<Eventplatertest>.Raise(new Eventplatertest());
    }
}
