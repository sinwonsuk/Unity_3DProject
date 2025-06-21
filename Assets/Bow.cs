using UnityEngine;

public class Bow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public BowRope Rope { get; set; }

    private void Awake()
    {
        Rope = GetComponentInChildren<BowRope>();
    }

    void Start()
    {

    }


    void Update()
    {
       
    }

    public void CreateArrow()
    {

    }

}
