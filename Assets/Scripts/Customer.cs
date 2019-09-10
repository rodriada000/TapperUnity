using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{

    public int HorionztalDir = 1;

    public float MoveSpeed = 2.0f;

    public float SlideSpeed = 8.0f;

    public float SlideTime = 1.0f;

    public bool IsDistracted;

    public bool IsDrinking;

    public LayerMask BeerLayer;
    public LayerMask defaultMask;
    

    private BoxCollider2D boxCollider;
    private Rigidbody2D rBody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        IsDistracted = false;
        IsDrinking = false;

        boxCollider = GetComponent<BoxCollider2D>();
        rBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        MoveForward();
        StartSlidingIfDrinking();
    }

    void MoveForward()
    {
        if (IsDrinking)
        {
            return;
        }

        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(HorionztalDir, 0);

        Vector3 newPos = Vector3.MoveTowards(rBody.position, end, MoveSpeed * Time.deltaTime);
        rBody.MovePosition(newPos);
    }

    void StartSlidingIfDrinking()
    {
        if (!IsDrinking)
        {
            return;
        }

        StartCoroutine(SlideBack());
    }

    protected IEnumerator SlideBack()
    {
        float startTime = 0.0f;

        while (startTime < SlideTime)
        {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(HorionztalDir * -1, 0);

            Vector3 newPos = Vector3.MoveTowards(rBody.position, end, SlideSpeed * Time.deltaTime);
            rBody.MovePosition(newPos);

            startTime += Time.deltaTime;
            yield return null;
        }

        IsDrinking = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Beer") && !IsDrinking)
        {
            IsDrinking = true;
            Destroy(collider.gameObject);
            animator.SetBool("isDrinking", IsDrinking);
        }
    }
}
