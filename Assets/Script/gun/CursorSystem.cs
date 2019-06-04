using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSystem : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursor = null;
    [SerializeField]
    private Texture2D cursor2 = null;
    [SerializeField]
    public GUISkin Skin;
    float timer;
    string timetext;
    int SUMPoint = 0;
    public float Range;
    bool isRange;

    void Start()
    {
        isRange = false;
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.ForceSoftware);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetButtonDown("Fire1"))
        {
            if (isRange)
            {
                Shot(ray);
            }

        }
        if (Physics.Raycast(ray, out hit, Range, LayerMask.GetMask("Target")))
        {
            if (!isRange && hit.collider.gameObject.GetComponent<MeshRenderer>().enabled && hit.collider.gameObject.GetComponent<TargetSystem>().Hitpoint > 0)
            {
                Cursor.SetCursor(cursor2, new Vector2(cursor2.width / 2, cursor.height / 2), CursorMode.ForceSoftware);
                isRange = true;
            }
        }
        else
        {
            if (isRange)
            {
                Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.ForceSoftware);
                isRange = false;
            }
        }
    }

    void Shot(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Range, LayerMask.GetMask("Target")))
        {
            if (hit.collider.gameObject.GetComponent<TargetSystem>().Hitpoint > 0)
            {
                SUMPoint += hit.collider.gameObject.GetComponent<TargetSystem>().getPoint();
                hit.collider.gameObject.GetComponent<TargetSystem>().HitMe();
            }
        }
    }

    private void OnGUI()
    {
        GUI.TextField(new Rect(10, 10, 260, 60), "SCORE:", Skin.GetStyle("label"));
        GUI.TextField(new Rect(260, 10, 510, 60), string.Format("{0:D10}", SUMPoint), Skin.GetStyle("label"));
    }
}
