using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarExit : MonoBehaviour
{
    public int TapIndex;

    public bool IsFlipped;

    public int CustomerLimit;

    public float SpawnCoolDownTime;
    private float cooldownTimer;

    public GameObject CustomerPrefab;

    public float MinOffsetX = 0.25f;
    public float MaxOffsetX = 1.2f;


    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        IsFlipped = renderer.flipX;   
        
        cooldownTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetCustomerCount() < CustomerLimit && cooldownTimer >= SpawnCoolDownTime)
        {
            SpawnCustomer();
            cooldownTimer = 0;
        }
        else
        {
            cooldownTimer += Time.deltaTime;
        }
    }

    int GetCustomerCount()
    {
        int customerCount = 0;
        GameObject[] customers = GameObject.FindGameObjectsWithTag("Customer");

        foreach (GameObject customer in customers)
        {
            Customer component = customer.GetComponent<Customer>();
            if (component.TapIndex == this.TapIndex)
            {
                customerCount++;
            }
        }

        return customerCount;
    }

    void SpawnCustomer()
    {
        int customerDir = IsFlipped ? -1 : 1;

        float customerOffsetX = Random.Range(MinOffsetX, MaxOffsetX);
        float customerOffsetY = 0.75f;

        GameObject customerObj = Instantiate(CustomerPrefab, transform.position + new Vector3(customerDir * customerOffsetX, customerOffsetY, 0), transform.rotation);
        
        Customer customer = customerObj.GetComponent<Customer>();
        customer.MoveSpeed = GameManager.instance.levelManager.GetCustomerMoveSpeed();
        customer.SlideSpeed = GameManager.instance.levelManager.GetCustomerSlideSpeed();
        customer.TapIndex = this.TapIndex;
        customer.HorionztalDir = customerDir;
        
        customer.MinMoveTime = GameManager.instance.levelManager.GetMinCustomerMoveTime();
        customer.MaxMoveTime = GameManager.instance.levelManager.GetMaxCustomerMoveTime();

        customer.MinStopTime = GameManager.instance.levelManager.GetMinCustomerStopTime();
        customer.MaxStopTime = GameManager.instance.levelManager.GetMaxCustomerStopTime();

        customer.MinSlideDistance = GameManager.instance.levelManager.GetMinCustomerSlideDistance();
        customer.MaxSlideDistance = GameManager.instance.levelManager.GetMaxCustomerSlideDistance();
    }
}
