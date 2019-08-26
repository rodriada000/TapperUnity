using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public LayerMask blockingLayer;
    public float restartLevelDelay = 1f;
    public float ShiftDelay = 0.5f;
    public float ShiftSpeed = 50f;
    public float RunSpeed = 4f;
    public bool IsRunning;
    public bool IsFacingLeft;
    public bool IsShifting;
    public int CurrentTapIndex;


    private Rigidbody2D rBody;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private int levelScore;

    // Start is called before the first frame update
    protected void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rBody = GetComponent<Rigidbody2D>();
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

        FlipSpriteBasedOnHorizontalInput(horizontal);

        AttemptMove<Wall>(horizontal, vertical);

        animator.SetBool("isRunning", IsRunning);
    }

    void FlipSpriteBasedOnHorizontalInput(int horizontalDir)
    {
        bool doFlip = false;

        if (!IsFacingLeft && horizontalDir < 0)
        {
            IsFacingLeft = true;
            doFlip = true;
        }
        else if (IsFacingLeft && horizontalDir > 0)
        {
            IsFacingLeft = false;
            doFlip = true;
        }

        if (doFlip)
        {
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
                ShiftToNextBarTap(yDir);
            } 
            else if (xDir != 0)
            {
                IsRunning = true;
                Vector3 newPosition = Vector3.MoveTowards(rBody.position, end, RunSpeed * Time.deltaTime);
                rBody.MovePosition(newPosition);
            }

            return true;
        }

        return false;
    }

    private void ShiftToNextBarTap(int yDir)
    {
        if (IsShifting)
        {
            return; // don't do anything if already shifting
        }
        
        int nextTapIndex = CurrentTapIndex;

        if (yDir < 0)
        {
            nextTapIndex ++;
        }
        else if (yDir > 0)
        {
            nextTapIndex --;
        }

        GameObject[] taps = GameObject.FindGameObjectsWithTag("BarTap");

        BarTap foundTap = null;
        bool isFound = false;

        BarTap firstTap = null;
        BarTap lastTap = null;

        for (int i = 0; i < taps.Length; i++)
        {
            foundTap = taps[i].GetComponent<BarTap>();

            if (foundTap.tapIndex == 1)
            {
                firstTap = foundTap;
            }

            if (lastTap == null || foundTap.tapIndex > lastTap.tapIndex)
            {
                lastTap = foundTap;
            }

            if (foundTap.tapIndex == nextTapIndex)
            {
                isFound = true;
                break;
            }
        }

        if (!isFound && nextTapIndex < firstTap.tapIndex) 
        {
            isFound = true;
            nextTapIndex = lastTap.tapIndex;
            foundTap = lastTap;
        } 
        else if (!isFound && nextTapIndex > lastTap.tapIndex)
        {
            isFound = true;
            nextTapIndex = firstTap.tapIndex;
            foundTap = firstTap;
        }

        if (isFound)
        {
            CurrentTapIndex = nextTapIndex;
            StartCoroutine(DoShift(foundTap.transform.position));
        }

    }

    protected IEnumerator DoShift(Vector3 end)
    {
        float halfShiftTime = ShiftDelay * 0.5f;

        IsShifting = true;
        animator.SetTrigger("playerShift");

        yield return new WaitForSeconds(halfShiftTime);

        Vector3 newPosition = Vector3.MoveTowards(rBody.position, end, ShiftSpeed);
        rBody.MovePosition(newPosition);

        yield return new WaitForSeconds(halfShiftTime);

        IsShifting = false;
    }

    protected virtual void AttemptMove<T> (int xDir, int yDir) where T : Component
    {
        if (xDir == 0 && yDir == 0)
        {
            return;
        }

        if (xDir != 0)
        {
            yDir = 0;
        }

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
