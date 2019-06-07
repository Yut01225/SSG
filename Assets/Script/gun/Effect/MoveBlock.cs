using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//エレベーター
public class MoveBlock : MonoBehaviour
{

    private Rigidbody rb;
    private Vector3 defaultPos;
    public bool x = false;
    public bool y = false;
    public bool z = false;
    public bool x2 = false;
    public bool y2 = false;
    public bool z2 = false;
    public float cm = 0f;
    public float speed = 0f;
    float sum;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        defaultPos = transform.position;
        transform.position += new Vector3(3f, 0, 0);
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