using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDestroySystem : MonoBehaviour
{
    private Vector3 localGravity = new Vector3(0,-45,0);
    private Rigidbody rb;
    private float Size;
    private float SmallSpeed;
    private Vector3 RotatePower;
    private Material m;

    void FixedUpdate()
    {
        setLocalGravity();
    }

    void setLocalGravity()
    {
        rb.AddForce(localGravity, ForceMode.Acceleration);
    }

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.useGravity = false;
        Size = this.transform.localScale.x;
        SmallSpeed = Random.Range(0.75f, 2f);
        RotatePower = new Vector3(Random.Range(-1, 1) * 6, Random.Range(-1, 1) * 2, Random.Range(-1, 1) * 6);

        m = this.gameObject.GetComponent<Renderer>().material;
        m.SetFloat("_Mode", (float)2);
        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m.SetInt("_ZWrite", 0);
        m.DisableKeyword("_ALPHATEST_ON");
        m.EnableKeyword("_ALPHABLEND_ON");
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m.renderQueue = 3000;
    }

    void Update()
    {
        this.transform.Rotate(RotatePower);
        if (Size <= 0f)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m.color -= new Color(m.color.r * Time.deltaTime, m.color.g * Time.deltaTime, m.color.b * Time.deltaTime, 0.45f * Time.deltaTime);
            if (this.gameObject.GetComponent<Renderer>().material.color.a < 0)
            {
                Destroy(this.gameObject);
            }
            this.transform.localScale = new Vector3(Size, Size, Size);
            Size -= Time.deltaTime * SmallSpeed;

        }
    }
}
