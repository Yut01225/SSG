using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteSample : MonoBehaviour
{
    [SerializeField]
    List<GameObject> tl = null;

    public List<GameObject> getList()
    {
        return tl;
    }

}
