using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bigger : MonoBehaviour
{
    private float mySize;
    private float LimitSize;
    private float BiggerSpeed;
    private float StartTime;
    private bool IsSmall = true;

    public void setting(float limitsize, float effectspeed, float starttime)
    {
        if (limitsize <= 0)
        {
            LimitSize = 8f;
        }
        else
        {
            LimitSize = limitsize;
        }
        if (effectspeed <= 0)
        {
            BiggerSpeed = 10f;
        }
        else
        {
            BiggerSpeed = effectspeed;
        }
        if (starttime <= 0)
        {
            StartTime = 0;
        }
        else
        {
            StartTime = starttime;
        }
    }

    void Start()
    {
        this.transform.localScale = new Vector3(0, 0, 0);
    }

    void Update()
    {
        if (StartTime <= 0)
        {
            if (IsSmall)
            {
                mySize += Time.deltaTime * BiggerSpeed;
                this.transform.localScale = new Vector3(mySize, mySize, mySize);
                if (mySize > LimitSize)
                {
                    this.transform.localScale = new Vector3(LimitSize, LimitSize, LimitSize);
                    IsSmall = false;
                    Destroy(this.gameObject.GetComponent<Bigger>());
                }
            }
        }
        else
        {
            StartTime -= Time.deltaTime * 1f;
        }



    }
}
