using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ワープ
public class WarpPoint : MonoBehaviour
{
    Vector3 tmp;
    Quaternion tmp1;
    [SerializeField]
    List<int> wp;
    [SerializeField]
    List<Transform> warpPoint;

     public void setting(List<int> wplist, List<Transform> warppoint)
    {
        wp = wplist;

        warpPoint = warppoint;
    }

    private void Start()
    {    
        //tmp = warpPoint[0].gameObject.transform.position;
        //tmp1 = warpPoint[0].gameObject.transform.localRotation; 
    }

    public void Update()
    {
       int point = 0;
        
        for (int i = wp.Count; 0 < i; i--)
        {
            if (MoveStatus.OldIndex == wp[point])
            {
                if(tmp != warpPoint[point].gameObject.transform.position)
                {
                tmp = warpPoint[point].gameObject.transform.position;
                gameObject.transform.position = new Vector3(tmp.x, tmp.y, tmp.z);

                tmp1 = warpPoint[point].gameObject.transform.localRotation;
                gameObject.transform.rotation = tmp1;
                }
                
            }
            point++;               
        }        
    }        
}
