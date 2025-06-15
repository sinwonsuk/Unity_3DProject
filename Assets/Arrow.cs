using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Shoot ==true)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }        
    }

    public bool Shoot { get; set; } = false;


    private float speed = 10.0f;
}
