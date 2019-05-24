using UnityEngine;
using System.Collections;
using UnityEditor;

public class Route : ScriptableWizard
{
    [SerializeField] private RouteSample routesample;
    [SerializeField] private bool AutoHeight;
    [SerializeField] private float CorrectionHeight;
    [SerializeField] private float StopTime;

    //　初期値
    private RouteSample initroutesample;
    private bool initAutoHeight;
    private float initCorrectionHeight;
    private float initStopTime;

    void OnWizardUpdate()
    {
        helpString = "初期設定の項目は変更されません。" +"\r\n"+ "進行経路は必ず設定してください。";
        position = new Rect(200f, 200f, 600f, 400f);
    }

    void Awake()
    {
        //　初期値
        initroutesample = null;
        initAutoHeight = false;
        initCorrectionHeight = 0;
        initStopTime = 0;
    }

    [MenuItem("Route/config")]
    static void CreateWizard()
    {
        //　ウィザードを表示
        ScriptableWizard.DisplayWizard<Route>("進行経路一括変更機能", "一括変更");
    }

        //　ウィザードの作成ボタンを押した時に実行
        void OnWizardCreate()
    {
        if (routesample)
        {
            foreach (GameObject obj in routesample.getList())
            {
                Debug.Log("進行経路を更新しました");
                obj.GetComponent<HandleData>().route = routesample;
                if(initAutoHeight != AutoHeight)
                {
                    obj.GetComponent<HandleData>().AutoHeight = AutoHeight;
                    Debug.Log("高さ自動設定にしました");
                }
                if (initCorrectionHeight != CorrectionHeight)
                {
                    obj.GetComponent<HandleData>().CorrectionHeight = CorrectionHeight;
                    Debug.Log("高さ補正を設定しました");
                }
                if (initStopTime != StopTime)
                {
                    obj.GetComponent<HandleData>().StopTime = StopTime;
                    Debug.Log("停止時間を設定しました");
                }
            }
        }
        else
        {
            Debug.Log("経路オブジェクトが選択されていません！");
        }
            
    }

    //　ウィザードの他のボタンを押した時に実行
    void OnWizardOtherButton()
    {
        //　実行したい処理
    }
}
