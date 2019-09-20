using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public GameObject BeerPrefab;
    public int TapIndex;

    public int HorionztalDir = 1;

    public float MoveSpeed = 2.0f;

    public float SlideSpeed = 8.0f;

    public float MinSlideDistance = 2f;
    public float MaxSlideDistance = 4f;

    public float MinDrinkTime = 1f;
    public float MaxDrinkTime = 2f;

    public bool IsSliding;
    public bool IsDistracted;
    public bool IsDrinking;


    public bool CanDrink
    {
        get
        {
            return !IsSliding && !IsDrinking && !IsDistracted;
        }
    }

    private BoxCollider2D boxCollider;
    private Rigidbody2D rBody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float currentMoveTime;
    public float RandomMoveTime;

    public bool IsStopped;

    public float MinMoveTime = 1.5f;
    public float MaxMoveTime = 3.0f;

    public float MinStopTime = 0.5f;
    public float MaxStopTime = 2.5f;
    private bool ReturnBeerOnNextUpdate = false;


    // Start is called before the first frame update
    void Start()
    {
        IsDistracted = false;
        IsDrinking = false;
        IsSliding = false;
        IsStopped = false;
        ReturnBeerOnNextUpdate = false;

        currentMoveTime = 0;
        RandomMoveTime = 0;

        boxCollider = GetComponent<BoxCollider2D>();
        rBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();    

        spriteRenderer.flipX = (HorionztalDir == -1);
    }

    // Update is called once per frame
    void Update()
    {
        if (ReturnBeerOnNextUpdate)
        {
            StartCoroutine(SpawnEmptyBeerMug());
            ReturnBeerOnNextUpdate = false;
            return;
        }

        if (RandomMoveTime == 0)
        {
            RandomMoveTime = Random.Range(MinMoveTime, MaxMoveTime);
        }

        MoveForward();
        
        if (currentMoveTime >= RandomMoveTime)
        {            
            StartCoroutine(DelayMovement());
        }
    }

    protected IEnumerator DelayMovement()
    {
        IsStopped = true;

        currentMoveTime = 0;
        RandomMoveTime = Random.Range(MinMoveTime, MaxMoveTime);

        float randomStopTime = Random.Range(MinStopTime, MaxStopTime);

        yield return new WaitForSeconds(randomStopTime);

        IsStopped = false;
    }

    void MoveForward()
    {
        if (IsDrinking || IsSliding || IsDistracted)
        {
            return;
        }

        if (IsStopped)
        {
            return;
        }

        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(HorionztalDir, 0);

        Vector3 newPos = Vector3.MoveTowards(rBody.position, end, MoveSpeed * Time.deltaTime);
        rBody.MovePosition(newPos);

        currentMoveTime += Time.deltaTime;
    }

    void StartSliding()
    {
        if (IsDrinking || IsSliding)
        {
            return;
        }

        StartCoroutine(SlideBack());
    }

    protected IEnumerator SlideBack()
    {
        IsSliding = true;

        float randomSlideDist = Random.Range(MinSlideDistance, MaxSlideDistance);

        Vector2 startPosition = transform.position;
        Vector2 finalPosition = startPosition + new Vector2(HorionztalDir * -1 * randomSlideDist, 0);


        float distanceThreshold = 0.15f;

        while (Vector2.Distance(transform.position, finalPosition) > distanceThreshold)
        {
            Vector2 currentPos = transform.position;
            Vector2 end = currentPos + new Vector2(HorionztalDir * -1, 0);

            Vector2 nextPosition = Vector3.MoveTowards(rBody.position, end, SlideSpeed * Time.deltaTime);
            rBody.MovePosition(nextPosition);

            yield return null;
        }

        IsSliding = false;

        yield return DrinkBeer();
    }

    protected IEnumerator DrinkBeer()
    {
        IsDrinking = true;
        animator.SetBool("isDrinking", IsDrinking);

        float randomDrinkTime = Random.Range(MinDrinkTime, MaxDrinkTime);
        yield return new WaitForSeconds(randomDrinkTime);

        IsDrinking = false;
        animator.SetBool("isDrinking", IsDrinking);

        ReturnBeerOnNextUpdate = true;
    }

    protected IEnumerator SpawnEmptyBeerMug()
    {
        // wait until customer is finished sliding or drinking before spawning beer
        while (IsSliding || IsDrinking)
        {
            yield return null;
        }

        float beerOffsetX = 0.25f;
        float beerOffsetY = -0.35f;

        GameObject beerObj = Instantiate(BeerPrefab, transform.position + new Vector3(beerOffsetX * HorionztalDir, beerOffsetY, 0), transform.rotation);
        Beer beer = beerObj.GetComponent<Beer>();
        beer.HorionztalDir = HorionztalDir;
        beer.IsFilled = false;
        beer.Speed = GameManager.instance.levelManager.GetPlayerBeerSpeed() * 0.5f;
        beer.TapIndex = this.TapIndex;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        
        if (collider.gameObject.CompareTag("Beer") && CanDrink)
        {
            Beer beer = collider.GetComponent<Beer>();
            if (beer.IsFilled) 
            {
                Destroy(beer.gameObject);
                StartSliding();
            }
        }

        if (collider.gameObject.CompareTag("Exit") && (IsDrinking || IsSliding))
        {
            Destroy(this.gameObject);
            GameManager.instance.AddToCurrentPlayerScore(ScoreKey.Customer);            
        }

        if (collider.gameObject.CompareTag("BarEnd") && !IsDrinking)
        {
            GameManager.instance.levelManager.PlayerMissedCustomer = true;
        }
    }
}
