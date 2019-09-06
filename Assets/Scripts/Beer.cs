using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beer : MonoBehaviour
{

    public bool IsFilled;

    public int HorionztalDir;

    public float Speed;

    public LayerMask BlockingLayer;
    public LayerMask defaultMask;
    

    private Rigidbody2D rBody;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // this.IsFilled = false;
        // this.HorionztalDir = 0;

        rBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(HorionztalDir, 0);
        RaycastHit2D hit;

        hit = Physics2D.Linecast(start, end, BlockingLayer);

        if (hit.transform == null)
        {
            Vector3 newPos = Vector3.MoveTowards(rBody.position, end, Speed * Time.deltaTime);
            rBody.MovePosition(newPos);

            
        }

    }
}
