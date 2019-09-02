using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarTap : MonoBehaviour
{
    public int TapIndex;

    public bool IsPlayerAtTap;

    public bool IsFlipped;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        IsFlipped = renderer.flipX;
        IsPlayerAtTap = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetShiftPositionVector()
    {
        BoxCollider2D tapCollider = GetComponent<BoxCollider2D>();
        Vector3 positionWithOffset = this.transform.position;
        positionWithOffset.x += tapCollider.offset.x;
        positionWithOffset.y += tapCollider.offset.y;

        return positionWithOffset;
    }
}
