using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LayerMask blockingLayer;

    public LayerMask itemsLayer;  

    public float PlayerBeerSpeed = 7.0f;

    public float MinCustomerMoveTime = 1.5f;
    public float MaxCustomerMoveTime = 3.0f;

    public float MinCustomerStopTime = 0.5f;
    public float MaxCustomerStopTime = 2.5f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ShowAllBarTaps();
        // CheckIfPlayerIsAtTap();
    }

    private void ShowAllBarTaps()
    {
        foreach (BarTap tap in GetBarTaps())
        {
            tap.GetComponent<SpriteRenderer>().enabled = true;   
        }
    }

    private void CheckIfPlayerIsAtTap()
    {
        RaycastHit2D itemHit;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        Player player = playerObj.GetComponent<Player>();
        BoxCollider2D playerCollider = playerObj.GetComponent<BoxCollider2D>();

        playerCollider.enabled = false;
        itemHit = Physics2D.Linecast(player.transform.position, player.transform.position, itemsLayer);
        playerCollider.enabled = true;

        if (itemHit.transform != null)
        {
            // something hit, check that it was a Tap
        }
    }

    public List<BarTap> GetBarTaps()
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("BarTap");
        List<BarTap> taps = new List<BarTap>();

        foreach (GameObject obj in taggedObjects)
        {
            taps.Add(obj.GetComponent<BarTap>());
        }

        return taps;
    }

    public BarTap GetBarTapAtTapIndex(int tapIndex)
    {
        List<BarTap> availableTaps = GetBarTaps();

        return availableTaps.Where(t => t.TapIndex == tapIndex).FirstOrDefault();
    }

    public BarTap GetFirstBarTap()
    {
        List<BarTap> availableTaps = GetBarTaps();

        return availableTaps.Where(t => t.TapIndex == 1).FirstOrDefault();
    }

    public BarTap GetLastBarTap()
    {
        List<BarTap> availableTaps = GetBarTaps();

        int maxIndex = availableTaps.Max(t => t.TapIndex);

        return availableTaps.Where(t => t.TapIndex == maxIndex).FirstOrDefault();
    }


}
