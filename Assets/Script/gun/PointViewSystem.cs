using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointViewSystem : MonoBehaviour
{
    float time = 0;

    void Start()
    {
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(this.transform.position  - GameObject.Find("MoveObject").transform.position), 1f);
    }

    void Update()
    {
        time += Time.deltaTime * 1f;
        if(time < 1f)
        {
            this.transform.position += new Vector3(0f,0.5f,0f);
            this.GetComponent<TextMeshPro>().color -= new Color(0,0,0,0.04f);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
