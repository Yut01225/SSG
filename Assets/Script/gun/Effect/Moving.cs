using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
    private float movespeed;
    private Vector3 AddPosition;
    private Vector3 TargetPosition;
    private float StartTime;

    public void setting(Vector3 ap, Vector3 tp, float speed, float time)
    {
        AddPosition = ap;
        TargetPosition = tp;
        if (speed <= 0)
        {
            movespeed = 5f;
        }
        else
        {
            movespeed = speed;
        }
        if (time <= 0)
        {
            StartTime = 0;
        }
        else
        {
            StartTime = time;
        }
    }

    void Start()
    {
        TargetPosition = this.transform.position;
        this.transform.position += AddPosition;
    }

    void Update()
    {
        if (StartTime <= 0)
        {
            transform.position = Vector3.Lerp(transform.position, TargetPosition, Time.deltaTime * movespeed);
            if (TargetPosition == this.transform.position)
            {
                Destroy(this.gameObject.GetComponent<Moving>());
            }
        }
        else
        {
            StartTime -= Time.deltaTime * 1f;
        }
    }
}
