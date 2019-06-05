using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//x縦ｙ横ｚ奥回転
public class Roller : MonoBehaviour
{

    public float TargetSpeed = 1.0f;
    public float Rollx = 0f;
    public float Rolly = 0f;
    public float Rollz = 0f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(new Vector3(0, 0, 0));
        // ワールドのy軸に沿って1秒間に180度回転Time.deltaTime
        transform.Rotate(new Vector3(Rollx, Rolly, Rollz) * Time.deltaTime * TargetSpeed, Space.World);
    }
}