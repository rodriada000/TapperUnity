using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public LayerMask blockingLayer;
    public float restartLevelDelay = 1f;
    public float ShiftTime = 0.1f;
    public float RunSpeed = 4f;
    public bool IsRunning;
    public bool IsFacingLeft;
    public bool IsShifting;

    private float inverseMoveTime;
    private Rigidbody2D rBody;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private int levelScore;

    // Start is called before the first frame update
    protected void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rBody = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1 / ShiftTime;
        animator = GetComponent<Animator>();    

        IsRunning = false;
        IsShifting = false;
        IsFacingLeft = false;
    }

    // Update is called once per frame
    void Update()
    {
        int horizontal = 0;
        int vertical = 0;
        IsRunning = false;

        horizontal = (int) Input.GetAxisRaw("Horizontal");
        vertical = (int) Input.GetAxisRaw("Vertical");   

        FlipPlayerIfNeeded(horizontal);


        if (horizontal != 0 || vertical != 0)
        {
            if (horizontal != 0)
                vertical = 0; 
  
            AttemptMove<Wall>(horizontal, vertical);
        }

        animator.SetBool("isRunning", IsRunning);
    }

    void FlipPlayerIfNeeded(int horizontalDir)
    {
        if (!IsFacingLeft && horizontalDir < 0)
        {
            IsFacingLeft = true;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
        else if (IsFacingLeft && horizontalDir > 0)
        {
            IsFacingLeft = false;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }   
    }

    
    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;



        if (hit.transform == null)
        {
            if (!IsShifting && yDir != 0)
            {
                // smooth animation movement up and down
                IsShifting = true;
                animator.SetTrigger("playerShift");
                StartCoroutine(SmoothMovement(end));
            } 
            else if (xDir != 0)
            {
                // normal movement left or right for walking
                IsRunning = true;
                Vector3 newPosition = Vector3.MoveTowards(rBody.position, end, RunSpeed * Time.deltaTime);
                rBody.MovePosition(newPosition);
            }

            return true;
        }

        return false;
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rBody.position, end, inverseMoveTime * Time.deltaTime);
            rBody.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }

        IsShifting = false;
    }

    protected virtual void AttemptMove<T> (int xDir, int yDir) where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
        {
            return;
        }

        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
        {
            // TODO: do something when collided with BarExit or Wall            
        }
    }
}
