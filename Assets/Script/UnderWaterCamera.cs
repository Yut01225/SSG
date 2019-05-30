using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using UnityEditor;

public class UnderWaterCamera : MonoBehaviour
{

    private enum WaterMode
    {
        FogAndBlur,
        UIAndBlur
    }

    //　水中にカメラがある時のモード
    [SerializeField] private WaterMode mode = WaterMode.FogAndBlur;
    //　水ゲームオブジェクト
    public Transform water;
    //　水中表現にUIのパネルを使用する場合は設定
    public GameObject waterPanel;
    //　水中表現にUIのパネルを使用する場合の水のRect
    private RectTransform waterRect;
    //　カメラが水と接触しているかどうか
    private bool isInWater = false;
    //　カメラ表示をボケさせるスクリプト
    private BlurOptimized blur;
    //　ノーマルのFogの色
    private Color normalFogColor;
    //　ノーマルのFogの深さ
    private float normalFogDensity;
    //　水中のFogの色
    private Color inWaterFogColor = new Color(0.8f, 0.8f, 1.0f, 0.3f);
    //　水中のFogの深さ
    private float inWaterFogDensity = 0.03f;

    void Start()
    {
        waterRect = waterPanel.GetComponent<RectTransform>();
        blur = GetComponent<BlurOptimized>();
        normalFogColor = RenderSettings.fogColor;
        normalFogDensity = RenderSettings.fogDensity;
    }

    void Update()
    {
        //　UIとBlurを使った水中表現
        if (mode == WaterMode.UIAndBlur)
        {
            if (isInWater)
            {
                //　カメラの中心が水の位置より高い時
                if (transform.position.y > water.position.y)
                {
                    blur.enabled = false;
                    waterRect.localScale = new Vector3(waterRect.localScale.x, Camera.main.WorldToViewportPoint(water.position).y, waterRect.localScale.z);
                }
                else
                {
                    blur.enabled = true;
                    waterRect.localScale = new Vector3(1f, 1f, 1f);
                }
            }
            //　FogとBlurを使った水中表現
        }
        else if (mode == WaterMode.FogAndBlur)
        {
            if (isInWater)
            {
                blur.enabled = true;
                RenderSettings.fog = true;
                RenderSettings.fogColor = inWaterFogColor;
                RenderSettings.fogDensity = inWaterFogDensity;
            }
            else
            {
                blur.enabled = false;
                RenderSettings.fogColor = normalFogColor;
                RenderSettings.fogDensity = normalFogDensity;
            }
        }
    }

    //　カメラと水が接触した時
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Water")
        {
            if (mode == WaterMode.UIAndBlur)
            {
                waterPanel.SetActive(true);
            }
            isInWater = true;
        }
    }
    //　カメラが水から出た時
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Water")
        {
            if (mode == WaterMode.UIAndBlur)
            {
                waterPanel.SetActive(false);
            }
            isInWater = false;
            blur.enabled = false;
        }
    }
}
