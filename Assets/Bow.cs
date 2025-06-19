using UnityEngine;

public class Bow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public BowRope Rope { get; set; }

    void Start()
    {
        Rope = GetComponentInChildren<BowRope>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void CreateArrow()
    {

    }

}
