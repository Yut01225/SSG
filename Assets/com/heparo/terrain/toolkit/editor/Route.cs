using UnityEngine;
using System.Collections;
using UnityEditor;

enum DropListMenu
{
    True,
    False
}

public class Route : ScriptableWizard
{
    private RouteSample routesample = null;
    //自動高さ
    private DropListMenu AutoHeight = DropListMenu.False;
    //高さ補正
    private float CorrectionHeight = 0;
    //停止時間
    private float StopTime = 0;
    //座標表示
    private DropListMenu positionView = DropListMenu.False;
    //停止時カメラ変更
    private DropListMenu StoppingAngleChange = DropListMenu.False;

    private DropListMenu UnevenFlag = DropListMenu.False;

    private float UnevenHeight = 0;

    private bool[] IsChange = new bool[8];

    void OnWizardUpdate()
    {
        int count = 0;
        foreach (bool b in IsChange)
        {
            if (b)
            {
                count++;
            }
        }
        if (count == 1 && IsChange[0])
        {
            IsChange[0] = false;
        }
        if (count == IsChange.Length - 1 && !IsChange[0])
        {
            IsChange[0] = true;
        }
    }

    /// <summary>
    /// WizardのGUIを上書きする
    /// </summary>
    protected override bool DrawWizardGUI()
    {
        base.DrawWizardGUI();
        if (routesample)
        {
            if (routesample.getList().Count > 0)
            {
                routesample = (RouteSample)EditorGUILayout.ObjectField("Route_List", routesample, typeof(RouteSample), true);
                if (IsChange[0])
                {
                    if (GUILayout.Button("一括解除"))
                    {
                        for (int i = 0; i < IsChange.Length; i++)
                        {
                            IsChange[i] = false;
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button("一括選択"))
                    {
                        for (int i = 0; i < IsChange.Length; i++)
                        {
                            IsChange[i] = true;
                        }
                    }
                }

                GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));
                if (Terrain.activeTerrain)
                {
                    IsChange[1] = EditorGUILayout.ToggleLeft("高さ自動調整を変更する", IsChange[1]);
                    if (IsChange[1])
                    {
                        AutoHeight = (DropListMenu)EditorGUILayout.EnumPopup("高さ自動調整を有効にする", (DropListMenu)AutoHeight);
                    }
                    GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));
                    IsChange[2] = EditorGUILayout.ToggleLeft("高さ補正を変更する", IsChange[2]);
                    if (IsChange[2])
                    {
                        CorrectionHeight = EditorGUILayout.FloatField("補正高さの値", CorrectionHeight);
                    }
                    GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));

                    IsChange[6] = EditorGUILayout.ToggleLeft("凹凸カメラの変更", IsChange[6]);
                    if (IsChange[6])
                    {
                        UnevenFlag = (DropListMenu)EditorGUILayout.EnumPopup("凹凸カメラを有効にする", (DropListMenu)UnevenFlag);
                    }
                    GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));
                    IsChange[7] = EditorGUILayout.ToggleLeft("高さ補正を変更する", IsChange[7]);
                    if (IsChange[7])
                    {
                        UnevenHeight = EditorGUILayout.FloatField("補正高さの値", UnevenHeight);
                    }
                    GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));
                }
                IsChange[3] = EditorGUILayout.ToggleLeft("停止時間を変更する", IsChange[3]);
                if (IsChange[3])
                {
                    StopTime = EditorGUILayout.FloatField("停止時間", StopTime);
                }
                GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));
                IsChange[4] = EditorGUILayout.ToggleLeft("座標表示を変更にする", IsChange[4]);
                if (IsChange[4])
                {
                    positionView = (DropListMenu)EditorGUILayout.EnumPopup("座標表示を有効にする", (DropListMenu)positionView);
                }
                GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));
                IsChange[5] = EditorGUILayout.ToggleLeft("停止中、方向転換を変更する", IsChange[5]);
                if (IsChange[5])
                {
                    StoppingAngleChange = (DropListMenu)EditorGUILayout.EnumPopup("停止中、方向転換する", (DropListMenu)StoppingAngleChange);
                }
            }
            else
            {
                routesample = (RouteSample)EditorGUILayout.ObjectField("Route_List", routesample, typeof(RouteSample), true);
                EditorGUILayout.HelpBox("経路配列の中身がありません！ " + "\r\n" + "配列の中身を確認してください。。", MessageType.Warning, true);
            }

        }
        else
        {
            routesample = (RouteSample)EditorGUILayout.ObjectField("Route_List", routesample, typeof(RouteSample), true);
            EditorGUILayout.HelpBox("経路配列を設定してください。", MessageType.Warning, true);
        }
        //false を返すことで OnWizardUpdate が呼び出されなくなる
        return true;
    }

    [MenuItem("Route/ALL Change")]
    static void CreateWizard()
    {
        //　ウィザードを表示
        ScriptableWizard.DisplayWizard<Route>("一括変更機能", "一括変更");
    }

    //　ウィザードの作成ボタンを押した時に実行
    void OnWizardCreate()
    {
        if (routesample && routesample.getList().Count > 0)
        {
            foreach (GameObject obj in routesample.getList())
            {
                if (obj)
                {
                    obj.GetComponent<HandleData>().route = routesample;
                    if (IsChange[1] && Terrain.activeTerrain)
                    {
                        obj.GetComponent<HandleData>().AutoHeight = ((int)AutoHeight == 0);
                    }
                    if (IsChange[2] && Terrain.activeTerrain)
                    {
                        obj.GetComponent<HandleData>().CorrectionHeight = CorrectionHeight;
                    }
                    if (IsChange[3])
                    {
                        obj.GetComponent<HandleData>().StopTime = StopTime;
                    }
                    if (IsChange[4])
                    {
                        obj.GetComponent<HandleData>().positionView = ((int)positionView == 0);
                    }
                    if (IsChange[5])
                    {
                        obj.GetComponent<HandleData>().StoppingAngleChange = ((int)StoppingAngleChange == 0);
                    }
                    if (IsChange[6] && Terrain.activeTerrain)
                    {
                        obj.GetComponent<HandleData>().UnevenFlag = ((int)UnevenFlag == 0);
                    }
                    if (IsChange[7] && Terrain.activeTerrain)
                    {
                        obj.GetComponent<HandleData>().UnevenHeight = UnevenHeight;
                    }
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
