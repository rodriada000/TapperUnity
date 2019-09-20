using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LayerMask blockingLayer;

    public LayerMask itemsLayer;

    public LevelSettings CurrentLevel;

    public List<LevelSettings> AllLevels;

    public bool PlayerMissedCustomer;
    public bool PlayerMissedEmptyMug;
    public bool PlayerThrewExtraMug;


    void Awake()
    {
        InitAllLevelSettings();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ShowAllBarTaps();
        CheckIfPlayerLost();
    }

    private void CheckIfPlayerLost()
    {
        if (PlayerMissedCustomer || PlayerMissedEmptyMug || PlayerThrewExtraMug)
        {
            GameManager.instance.PlayerLost();
        }
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

    public bool IsPlayerAtBarTap(int tapIndex)
    {
        return GetBarTapAtTapIndex(tapIndex).IsPlayerAtTap;
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

    public float GetPlayerBeerSpeed()
    {
        return CurrentLevel.PlayerBeerSpeed;
    }

    public float GetCustomerMoveSpeed()
    {
        return CurrentLevel.CustomerMoveSpeed;
    }

    public float GetCustomerSlideSpeed()
    {
        return CurrentLevel.CustomerSlideSpeed;
    }

    public float GetMinCustomerStopTime()
    {
        return CurrentLevel.CustomerStopTimes[0];
    }

    public float GetMaxCustomerStopTime()
    {
        return CurrentLevel.CustomerStopTimes[1];
    }

    public float GetMinCustomerMoveTime()
    {
        return CurrentLevel.CustomerMoveTimes[0];
    }

    public float GetMaxCustomerMoveTime()
    {
        return CurrentLevel.CustomerMoveTimes[1];
    }

    public float GetMinCustomerSlideDistance()
    {
        return CurrentLevel.CustomerSlideDistances[0];
    }

    public float GetMaxCustomerSlideDistance()
    {
        return CurrentLevel.CustomerSlideDistances[1];
    }

    private void InitAllLevelSettings()
    {
        AllLevels = new List<LevelSettings>();

        LevelSettings level1 = new LevelSettings()
        {
            Level = 1,
            PlayerBeerSpeed = 6.0f,
            CustomerMoveSpeed = 2f,
            CustomerSlideSpeed = 5
        };
        level1.SetCustomerMoveTimes(0.5f, 1.0f);
        level1.SetCustomerStopTimes(1.5f, 2f);
        level1.SetCustomerDrinkTimes(1.0f, 1.75f);
        level1.SetCustomerSlideDistances(6.0f, 7.0f);

        level1.AddCustomersToBarTap(1, new List<float>() { 0.25f });
        level1.AddCustomersToBarTap(2, new List<float>() { 0.25f });
        level1.AddCustomersToBarTap(3, new List<float>() { 0.25f });
        level1.AddCustomersToBarTap(4, new List<float>() { 0.25f });

        AllLevels.Add(level1);

        LevelSettings level2 = new LevelSettings()
        {
            Level = 2,
            PlayerBeerSpeed = 6.0f,
            CustomerMoveSpeed = 1.5f,
            CustomerSlideSpeed = 6
        };
        level2.SetCustomerMoveTimes(0.5f, 1.0f);
        level2.SetCustomerStopTimes(1f, 1.5f);
        level2.SetCustomerDrinkTimes(1.0f, 2.0f);
        level2.SetCustomerSlideDistances(6.0f, 8.0f);

        level2.AddCustomersToBarTap(1, new List<float>() { 0.25f, 1.25f });
        level2.AddCustomersToBarTap(2, new List<float>() { 0.25f, 1.25f });
        level2.AddCustomersToBarTap(3, new List<float>() { 0.25f, 1.25f });
        level2.AddCustomersToBarTap(4, new List<float>() { 0.25f, 1.25f });

        AllLevels.Add(level2);
    }
}

public class LevelSettings
{
    public int Level;

    public Dictionary<int, List<float>> StartingCustomers;

    public float PlayerBeerSpeed = 7.0f;

    public float CustomerMoveSpeed;

    public float CustomerSlideSpeed;

    public List<float> CustomerMoveTimes;

    public List<float> CustomerStopTimes; 

    public List<float> CustomerDrinkTimes;

    public List<float> CustomerSlideDistances;

    public void SetCustomerMoveTimes(float min, float max)
    {
        CustomerMoveTimes = new List<float>();
        CustomerMoveTimes.Add(min);
        CustomerMoveTimes.Add(max);
    }

    public void SetCustomerStopTimes(float min, float max)
    {
        CustomerStopTimes = new List<float>();
        CustomerStopTimes.Add(min);
        CustomerStopTimes.Add(max);
    }

    public void SetCustomerDrinkTimes(float min, float max)
    {
        CustomerDrinkTimes = new List<float>();
        CustomerDrinkTimes.Add(min);
        CustomerDrinkTimes.Add(max);
    }

    public void SetCustomerSlideDistances(float min, float max)
    {
        CustomerSlideDistances = new List<float>();
        CustomerSlideDistances.Add(min);
        CustomerSlideDistances.Add(max);
    }

    public LevelSettings()
    {
        StartingCustomers = new Dictionary<int, List<float>>();
    }

    public void AddCustomersToBarTap(int bartap_index, List<float> customer_offsets)
    {
        if (StartingCustomers == null)
        {
            StartingCustomers = new Dictionary<int, List<float>>();
        }

        if (StartingCustomers.ContainsKey(bartap_index) == false)
        {
            StartingCustomers.Add(bartap_index, customer_offsets);
        }
    }
}