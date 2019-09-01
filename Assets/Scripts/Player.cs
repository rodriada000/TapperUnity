using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public LayerMask blockingLayer;
    public LayerMask itemsLayer;    

    public float restartLevelDelay = 1f;
    public float ShiftDelay = 0.5f;
    public float ShiftSpeed = 50f;
    public float RunSpeed = 4f;
    public bool IsRunning;
    public bool IsFacingLeft;
    public bool IsShifting;
    public int CurrentTapIndex;
    public bool IsAtCurrentBarTap;
    
    public float FillSpeed = 0.05f;
    public int FillPercent = 0;
    public bool IsFillingBeer;
    public bool IsIdleWithBeer;


    private Rigidbody2D rBody;
    private BoxCollider2D boxCollider;
    private SpriteRenderer renderer;
    private Animator animator;
    private int levelScore;

    private int horizontalInput;
    private int verticalInput;
    private bool pourPressed;
    private bool servePressed;


    // Start is called before the first frame update
    protected void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();    
        renderer = GetComponent<SpriteRenderer>();

        IsRunning = false;
        IsShifting = false;
        IsFacingLeft = false;
        IsFillingBeer = false;
        IsAtCurrentBarTap = false;

        IsIdleWithBeer = false;
        animator.SetBool("isIdleWithBeer", IsIdleWithBeer);

        IsFillingBeer = false;
        animator.SetBool("isFillingBeer", IsFillingBeer);
    }

    // Update is called once per frame
    void Update()
    {
        IsRunning = false;

        horizontalInput = (int)Input.GetAxisRaw("Horizontal");
        verticalInput = (int)Input.GetAxisRaw("Vertical");

        pourPressed = Input.GetButton("Pour");
        servePressed = Input.GetButton("Serve");

        StopFillingBeerIfMoving(horizontalInput, verticalInput);
        
        AttemptMove<Wall>(horizontalInput, verticalInput);
        animator.SetBool("isRunning", IsRunning);

        CheckIfAtBarTap();

        FlipSpriteBasedOnHorizontalInput(horizontalInput);
    }

    void LateUpdate()
    {
        if (horizontalInput == 0 && verticalInput == 0)
        {
            FillBeerIfPourPressed(pourPressed);
            HideCurrentTapIfFilling();
        }
    }

    private void HideCurrentTapIfFilling()
    {
        if (!IsFillingBeer && !IsIdleWithBeer)
        {
            return;
        }

        BarTap currentTap = GameManager.instance.levelManager.GetBarTapAtTapIndex(CurrentTapIndex);

        if (currentTap.IsPlayerAtTap && !IsShifting)
        {
            currentTap.GetComponent<SpriteRenderer>().enabled = false;
            
            Vector3 theScale = transform.localScale;
            theScale.x = currentTap.transform.localScale.x;
            transform.localScale = theScale;
        }
    }

    private void CheckIfAtBarTap()
    {
        RaycastHit2D hit;
        BarTap touchingTap = null;

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(transform.position, transform.position, itemsLayer);
        boxCollider.enabled = true;

        if (hit.transform != null && hit.transform.tag == "BarTap")
        {
            touchingTap = hit.transform.GetComponent<BarTap>();
            touchingTap.IsPlayerAtTap = true;
            IsAtCurrentBarTap = true;
        }
        else
        {
            IsAtCurrentBarTap = false;
        }
        
        // mark other taps as the player not being there
        List<BarTap> taps = GameManager.instance.levelManager.GetBarTaps();
        foreach (BarTap otherTap in taps)
        {
            if (touchingTap != null && touchingTap.TapIndex != otherTap.TapIndex)
            {
                otherTap.IsPlayerAtTap = false;
            }
            else if (touchingTap == null)
            {
                otherTap.IsPlayerAtTap = false;
            }
        }
    }

    private void StopFillingBeerIfMoving(int horizontal, int vertical)
    {
        if ((horizontal != 0 || vertical != 0) && IsFillingBeer)
        {
            IsFillingBeer = false;
            IsIdleWithBeer = false;
            FillPercent = 0;
            animator.SetBool("isFillingBeer", IsFillingBeer);
            animator.SetBool("isIdleWithBeer", IsIdleWithBeer);
        }
    }

    private void FillBeerIfPourPressed(bool pourPressed)
    {
        if (pourPressed && !IsFillingBeer && !IsIdleWithBeer)
        {
            // pouring and has not started filling beer -> start filling beer
            BarTap currentTap = GameManager.instance.levelManager.GetBarTapAtTapIndex(CurrentTapIndex);
            if (!currentTap.IsPlayerAtTap)
            {
                ShiftToNextBarTap(0); // 0 will cause to shift to current bar tap
            }
            
            HorizontalFlipSpriteBasedOnBool(currentTap.IsFlipped);

            IsFillingBeer = true;
            StartCoroutine(FillBeer());
        }
        else if (pourPressed && IsFillingBeer && IsIdleWithBeer)
        {
            // pour pressed while being idle (paused filling) -> resume filling the beer
            IsIdleWithBeer = false;
            animator.SetBool("isIdleWithBeer", IsIdleWithBeer);
            StartCoroutine(FillBeer());
        }
        else if (!pourPressed && IsFillingBeer && !IsIdleWithBeer)
        {
            // stopped pouring in the middle of filling beer -> user becomes idle
            IsIdleWithBeer = true;
            animator.SetBool("isIdleWithBeer", IsIdleWithBeer);
        }
    }

    protected IEnumerator FillBeer()
    {
        animator.SetBool("isFillingBeer", IsFillingBeer);

        while (IsFillingBeer && !IsIdleWithBeer && FillPercent <= 100)
        {
            FillPercent += 10;
            yield return new WaitForSeconds(FillSpeed);
        }
    }

    void FlipSpriteBasedOnHorizontalInput(int horizontalDir)
    {
        if (horizontalDir == 0)
        {
            return;
        }

        if (!IsFacingLeft && horizontalDir < 0)
        {
            IsFacingLeft = true;
        }
        else if (IsFacingLeft && horizontalDir > 0)
        {
            IsFacingLeft = false;
        }

        HorizontalFlipSpriteBasedOnBool(IsFacingLeft);
    }

    private void HorizontalFlipSpriteBasedOnBool(bool condition)
    {
        renderer.flipX = condition;
    }
    
    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        RaycastHit2D itemHit;

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        itemHit = Physics2D.Linecast(start, end, itemsLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            if (!IsShifting && yDir != 0)
            {
                ShiftToNextBarTap(yDir);
            } 
            else if (xDir != 0 && !IsFillingBeer)
            {
                IsRunning = true;
                Vector3 newPosition = Vector3.MoveTowards(rBody.position, end, RunSpeed * Time.deltaTime);
                rBody.MovePosition(newPosition);

                animator.SetBool("isFillingBeer", IsFillingBeer);
                animator.SetBool("isIdleWithBeer", IsIdleWithBeer);
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


        BarTap foundTap = GameManager.instance.levelManager.GetBarTapAtTapIndex(nextTapIndex);
        BarTap firstTap = GameManager.instance.levelManager.GetFirstBarTap();
        BarTap lastTap = GameManager.instance.levelManager.GetLastBarTap();

        bool isFound = (foundTap != null);

        // wrap to beginning or last tap if needed
        if (!isFound && nextTapIndex < firstTap.TapIndex) 
        {
            isFound = true;
            nextTapIndex = lastTap.TapIndex;
            foundTap = lastTap;
        } 
        else if (!isFound && nextTapIndex > lastTap.TapIndex)
        {
            isFound = true;
            nextTapIndex = firstTap.TapIndex;
            foundTap = firstTap;
        }

        if (isFound && !foundTap.IsPlayerAtTap)
        {
            CurrentTapIndex = nextTapIndex;

            BoxCollider2D tapCollider = foundTap.GetComponent<BoxCollider2D>();
            Vector3 newPos = foundTap.transform.position;
            newPos.x += tapCollider.offset.x * foundTap.transform.localScale.x;
            newPos.y += tapCollider.offset.y;

            StartCoroutine(DoShift(newPos));
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
        if (IsShifting || (xDir == 0 && yDir == 0))
        {
            // don't move if shifting or no input in X or Y direction
            return;
        }

        if (xDir != 0 && IsFillingBeer)
        {
            // don't allow running when filling beer
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
