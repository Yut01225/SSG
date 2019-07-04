using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    public GameObject Dobj;
    TargetSystem t = new TargetSystem();
    public void setting(GameObject dobj)
    {
        Dobj = dobj;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] DB = GameObject.FindGameObjectsWithTag("DB");
        
        if (DB.Length == 1)
        {
            t.getPoint();
            Debug.Log("成功");
            //Destroy(Dobj);
        }
    }
}
