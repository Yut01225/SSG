using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transparent : MonoBehaviour
{
    private float TransparentSpeed;
    private float StartTime;

    public void setting(float effectspeed, float starttime)
    {
        if (effectspeed <= 0)
        {
            TransparentSpeed = 1f;
        }
        else
        {
            TransparentSpeed = effectspeed;
        }
        if (starttime <= 0)
        {
            StartTime = 0;
        }
        else
        {
            StartTime = starttime;
        }
    }
    void Start()
    {
        Material m = this.gameObject.GetComponent<Renderer>().material;
        m.SetFloat("_Mode", (float)2);
        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m.SetInt("_ZWrite", 0);
        m.DisableKeyword("_ALPHATEST_ON");
        m.EnableKeyword("_ALPHABLEND_ON");
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m.renderQueue = 3000;
        m.color = new Vector4(m.color.r, m.color.g, m.color.b, 0);
    }
    void Update()
    {
        if (StartTime <= 0)
        {
            if (this.gameObject.GetComponent<Renderer>().material.color.a <= 1)
            {
                this.gameObject.GetComponent<Renderer>().material.color += new Color(0, 0, 0, TransparentSpeed * Time.deltaTime);

            }
            else
            {
                Destroy(this.gameObject.GetComponent<Transparent>());
            }
        }
        else
        {
            StartTime -= Time.deltaTime * 1f;
        }




    }
}
