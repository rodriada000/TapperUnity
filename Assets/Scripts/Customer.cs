using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public int TapIndex;

    public int HorionztalDir = 1;

    public float MoveSpeed = 2.0f;

    public float SlideSpeed = 8.0f;

    public float SlideDistance = 4f;

    public bool IsSliding;
    public bool IsDistracted;

    public bool IsDrinking;

    public float DrinkTime = 1.0f;

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


    // Start is called before the first frame update
    void Start()
    {
        IsDistracted = false;
        IsDrinking = false;
        IsSliding = false;
        IsStopped = false;

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
        if (!IsDrinking && !IsSliding)
        {
            return;
        }

        StartCoroutine(SlideBack());
    }

    protected IEnumerator SlideBack()
    {
        IsSliding = true;

        Vector2 startPosition = transform.position;
        Vector2 finalPosition = startPosition + new Vector2(HorionztalDir * -1 * SlideDistance, 0);


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
    }

    protected IEnumerator DrinkBeer(GameObject beer)
    {
            Destroy(beer);

            IsDrinking = true;
            animator.SetBool("isDrinking", IsDrinking);

            yield return new WaitForSeconds(DrinkTime);

            IsDrinking = false;
            animator.SetBool("isDrinking", IsDrinking);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Beer") && !IsDrinking && !IsDistracted)
        {
            StartCoroutine(DrinkBeer(collider.gameObject));
            StartSliding();
        }

        if (collider.gameObject.CompareTag("Exit") && (IsDrinking || IsSliding))
        {
            Destroy(this.gameObject);
            // TODO: get points for getting rid of customer
        }
    }
}
