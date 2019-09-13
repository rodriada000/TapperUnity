using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beer : MonoBehaviour
{

    public bool IsFilled;

    public int HorionztalDir;

    public int TapIndex;

    public float Speed;

    public Sprite EmptyBeerMugSprite;    

    private Rigidbody2D rBody;
    private SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (!IsFilled)
        {
            spriteRenderer.sprite = EmptyBeerMugSprite;
        }

        spriteRenderer.flipX = (HorionztalDir == -1);

        if (spriteRenderer.flipX)
        {
            // flip offset if sprite is flipped
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            Vector2 offset = collider.offset;
            offset.x *= HorionztalDir;
            collider.offset = offset;
        }
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

        Vector3 newPos = Vector3.MoveTowards(rBody.position, end, Speed * Time.deltaTime);
        rBody.MovePosition(newPos);       
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Exit") && IsFilled)
        {
            Destroy(this.gameObject);
            GameManager.instance.levelManager.PlayerThrewExtraMug = true;
        }

        if (collider.gameObject.CompareTag("BarEnd") && !IsFilled)
        {
            Destroy(this.gameObject);

            if (GameManager.instance.levelManager.IsPlayerAtBarTap(TapIndex))
            {
                // TODO: get points for getting empty mug
            }
            else
            {
                GameManager.instance.levelManager.PlayerMissedEmptyMug = true;
                // TODO: lose life, animate beer shatter
            }
        }
    }
}
