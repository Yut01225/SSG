using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    public string Tagname;
    public TargetSystem Ts;
    public int CompPoint;
    public int TriggerCount;
    public bool bosscreate = false;
    public GameObject Boss;



    void Start()
    {
        Boss.SetActive(false);

        if (Ts == null)
        {
            Ts = this.GetComponent<TargetSystem>();
        }
        
        if (TriggerCount < 1)
        {
            TriggerCount = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] chain = GameObject.FindGameObjectsWithTag(Tagname);

        if (chain.Length == TriggerCount)
        {
            if (bosscreate)
            {
                Boss.SetActive(true);
            }

            Ts.BrokenPoint += CompPoint;
            Destroy(this);
        }
    }
}
