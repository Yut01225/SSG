using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    [SerializeField]
    List<GameObject> tl = null;

    public List<GameObject> getList()
    {
        return tl;
    }

}
