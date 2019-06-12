using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ワープ
public class WarpPoint : MonoBehaviour
{

    
    Vector3 tmp;
    [SerializeField]
    List<int> wp;
    [SerializeField]
    List<GameObject> warpPoint;



    private void Start()
    {
        tmp = warpPoint[0].gameObject.transform.position;
    }


    public void Update()
    {
       int point = 0;
        
        for (int i = wp.Count; 0 < i; i--)
        {
            if (MoveStatus.NextIndex == wp[point])
            {
                tmp = warpPoint[point].gameObject.transform.position;
                gameObject.transform.position = new Vector3(tmp.x, tmp.y, tmp.z);
                
            }
            point++;            
            
        }
        
    }        
    
}
