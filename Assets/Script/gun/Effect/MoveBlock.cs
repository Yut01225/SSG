using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//移動する
public class MoveBlock : MonoBehaviour
{

    private Rigidbody rb;
    private Vector3 defaultPos;
  
    public float speed;
    public float cm = 0f;
    float sum;
    public bool x = false;
    public bool y = false;
    public bool z = false;
    public bool x2 = false;
    public bool y2 = false;
    public bool z2 = false;
    

    public void setting(bool right, bool up, bool front, bool left, bool down, bool back, float CM, float effectspeed)
    {
        if (effectspeed <= 0)
        {
            speed = 1f;
        }
        else
        {
            speed = effectspeed;
        }
        x = right;
        y = up;
        z = front;
        x2 = left;
        y2 = down;
        z2 = back;
        cm = CM;

}


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        defaultPos = transform.position;
        //transform.position += new Vector3(3f, 0, 0);
    }


void FixedUpdate()
    {
        sum += Time.deltaTime * speed;
        if (x == true)
        {
            rb.MovePosition(new Vector3(defaultPos.x + Mathf.PingPong(sum, cm), defaultPos.y, defaultPos.z));
        }
        if (y == true)
        {
            rb.MovePosition(new Vector3(defaultPos.x, defaultPos.y + Mathf.PingPong(sum, cm), defaultPos.z));
        }
        if (z == true)
        {
            rb.MovePosition(new Vector3(defaultPos.x, defaultPos.y, defaultPos.z + Mathf.PingPong(sum, cm)));
        }
        if (x2 == true)
        {
            rb.MovePosition(new Vector3(defaultPos.x - Mathf.PingPong(sum, cm), defaultPos.y, defaultPos.z));
        }
        if (y2 == true)
        {
            rb.MovePosition(new Vector3(defaultPos.x, defaultPos.y - Mathf.PingPong(sum, cm), defaultPos.z));
        }
        if (z2 == true)
        {
            rb.MovePosition(new Vector3(defaultPos.x, defaultPos.y, defaultPos.z - Mathf.PingPong(sum, cm)));
        }
    }
}