using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beer : MonoBehaviour
{

    public bool IsFilled;

    public bool IsShattered;
    public bool IsFalling;

    public int HorionztalDir;

    public int TapIndex;

    public float Speed;

    public Sprite EmptyBeerMugSprite;    

    private Rigidbody2D rBody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (!IsFilled)
        {
            animator.SetTrigger("emptyBeer");
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

        IsShattered = false;
        IsFalling = false;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        if (IsShattered || IsFalling)
        {
            return;
        }

        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(HorionztalDir, 0);

        Vector3 newPos = Vector3.MoveTowards(rBody.position, end, Speed * Time.deltaTime);
        rBody.MovePosition(newPos);       
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Exit") && IsFilled)
        {
            StartCoroutine(ShatterBeer());
        }

        if (collider.gameObject.CompareTag("BarEnd") && !IsFilled)
        {
            if (GameManager.instance.levelManager.IsPlayerAtBarTap(TapIndex))
            {
                Destroy(this.gameObject);
                GameManager.instance.AddToCurrentPlayerScore(ScoreKey.EmptyMug);
            }
            else
            {
                StartCoroutine(DropBeerFromBar());
            }
        }
    }

    protected IEnumerator DropBeerFromBar()
    {
        float waitTime = 0f;
        while (waitTime < 0.45f)
        {
            yield return null;
            waitTime += Time.deltaTime;
            Move();  
        }

        TriggerPlayerMissAnimation();
        
        IsFalling = true;
        animator.SetTrigger("beerFall");

        waitTime = 0f;
    
        while (waitTime < 1.0f)
        {
            yield return null;
            waitTime += Time.deltaTime;

            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(0, -1f);

            Vector3 newPos = Vector3.MoveTowards(rBody.position, end, Speed * Time.deltaTime);
            rBody.MovePosition(newPos);   
        }

        animator.SetTrigger("shatterBeer");
        
        yield return new WaitForSeconds(0.75f);

        GameManager.instance.levelManager.PlayerMissedEmptyMug = true;

        Destroy(this.gameObject);
    }

    protected IEnumerator ShatterBeer()
    {
        IsShattered = true;
        animator.SetTrigger("shatterBeer");

        TriggerPlayerMissAnimation();

        yield return new WaitForSeconds(1.0f);

        GameManager.instance.levelManager.PlayerThrewExtraMug = true;

        Destroy(this.gameObject);
    }

    private void TriggerPlayerMissAnimation()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerObj.GetComponent<Player>().TriggerMissAnimation();
        }
    }
}
