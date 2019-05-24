using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(HandleData))]
[CanEditMultipleObjects]
public class HandleTest : Editor
{
    //　シリアライズプロパティ
    SerializedProperty positionView;
    SerializedProperty StoppingAngleChange;
    SerializedProperty NextCameraOption;
    SerializedProperty NextMoveSpeed;
    SerializedProperty NextLookSpeed;
    SerializedProperty HorizontalAngle;
    SerializedProperty VerticalAngle;
    SerializedProperty StopTime;
    SerializedProperty route;
    SerializedProperty LookTarget;
    SerializedProperty AutoHeight;
    SerializedProperty CorrectionHeight;
    SerializedProperty UnevenFlag;
    SerializedProperty UnevenHeight;

    void OnEnable()
    {
        //　シリアライズプロパティの取得
        positionView = serializedObject.FindProperty("positionView");
        StoppingAngleChange = serializedObject.FindProperty("StoppingAngleChange");
        NextCameraOption = serializedObject.FindProperty("NextCameraOption");
        positionView = serializedObject.FindProperty("NextMoveSpeed");
        StoppingAngleChange = serializedObject.FindProperty("NextLookSpeed");
        positionView = serializedObject.FindProperty("HorizontalAngle");
        StoppingAngleChange = serializedObject.FindProperty("VerticalAngle");
        NextCameraOption = serializedObject.FindProperty("StopTime");
        route = serializedObject.FindProperty("route");
        LookTarget = serializedObject.FindProperty("LookTarget");
        AutoHeight = serializedObject.FindProperty("AutoHeight");
        CorrectionHeight = serializedObject.FindProperty("CorrectionHeight");
        UnevenFlag = serializedObject.FindProperty("UnevenFlag");
        UnevenHeight = serializedObject.FindProperty("UnevenHeight");
    }

    public override void OnInspectorGUI()
    {
        //　シリアライズオブジェクトの更新
        serializedObject.Update();

        //　targetでデータを取得しキャスト
        HandleData myData = (HandleData)target;
        //配列を設定しているか判定
        if (myData.route)
        {
            GUILayout.Label("【Edit_Option】");
            myData.positionView = EditorGUILayout.ToggleLeft("View_Position", myData.positionView);
            EditorGUILayout.HelpBox("目的地までの距離の差を(x,y,z)表記で表示します。", MessageType.None, true);
            EditorGUILayout.Space();
            GUILayout.Label("【Options】");
            myData.StoppingAngleChange = EditorGUILayout.ToggleLeft("Stopping_AngleChange", myData.StoppingAngleChange);
            EditorGUILayout.HelpBox("停止中に、方向転換を有効にします。", MessageType.None, true);
            float leftValue = -179.0f;
            float rightValue = 179.0f;
            EditorGUILayout.Space();
            GUILayout.Label("【Angle】");
            myData.NextCameraOption = (LookOption)EditorGUILayout.EnumPopup("Camera_Option", (LookOption)myData.NextCameraOption);
            switch (myData.NextCameraOption)
            {
                case LookOption.NextTarget:
                    EditorGUILayout.HelpBox("次の目的地に方向を変えます。", MessageType.None, true);
                    break;
                case LookOption.LookAtHorizontal:
                    GUILayout.Label("Horizontal_Angle");
                    myData.HorizontalAngle = EditorGUILayout.Slider(myData.HorizontalAngle, leftValue, rightValue);
                    EditorGUILayout.HelpBox("現在の向きから水平方向に指定した角度方向を変えます。", MessageType.None, true);
                    break;
                case LookOption.LookAtVertical:
                    GUILayout.Label("Vertical_Angle");
                    myData.VerticalAngle = EditorGUILayout.Slider(myData.VerticalAngle, leftValue, rightValue);
                    EditorGUILayout.HelpBox("現在の向きから垂直方向に指定した角度方向を変えます。", MessageType.None, true);
                    break;
                case LookOption.FreeChange:
                    GUILayout.Label("Horizontal_Angle");
                    myData.HorizontalAngle = EditorGUILayout.Slider(myData.HorizontalAngle, leftValue, rightValue);
                    GUILayout.Label("Vertical_Angle");
                    myData.VerticalAngle = EditorGUILayout.Slider(myData.VerticalAngle, leftValue, rightValue);
                    EditorGUILayout.HelpBox("垂直、水平方向を同時に指定できます。。", MessageType.None, true);
                    break;
                case LookOption.DontChange:
                    EditorGUILayout.HelpBox("次の目的地まで方向を変えません。", MessageType.None, true);
                    break;
                case LookOption.LookAtTarget:
                    bool allowSceneObjects1 = !EditorUtility.IsPersistent(target);
                    myData.LookTarget = (Transform)EditorGUILayout.ObjectField("Target_Object", myData.LookTarget, typeof(Transform), allowSceneObjects1);
                    EditorGUILayout.HelpBox("指定したオブジェクトの方向を見続けます。", MessageType.None, true);
                    if (!myData.LookTarget)
                    {
                        EditorGUILayout.HelpBox("オブジェクトが設定されていません。", MessageType.Warning, true);
                    }

                    break;
            }
            EditorGUILayout.Space();
            GUILayout.Label("【Speed】");
            myData.NextMoveSpeed = EditorGUILayout.FloatField("Move_Speed", myData.NextMoveSpeed);
            EditorGUILayout.HelpBox("次の目的地までの間、移動速度を指定します。" + "\r\n" + "0の場合は、デフォルトの値が適応されます。", MessageType.None, true);
            myData.NextLookSpeed = EditorGUILayout.FloatField("Look_Speed", myData.NextLookSpeed);
            EditorGUILayout.HelpBox("次の目的地までの間、方向転換の速度を指定します。" + "\r\n" + "0の場合は、デフォルトの値が適応されます。", MessageType.None, true);
            EditorGUILayout.Space();
            GUILayout.Label("【Time】");
            myData.StopTime = EditorGUILayout.FloatField("Stop_Time", myData.StopTime);
            EditorGUILayout.HelpBox("指定した時間、目的地で停止します。", MessageType.None, true);
            EditorGUILayout.Space();
            GUILayout.Label("【Route】");
            bool allowSceneObjects = !EditorUtility.IsPersistent(target);
            myData.route = (RouteSample)EditorGUILayout.ObjectField("Route_Object", myData.route, typeof(RouteSample), allowSceneObjects);

            //エラー対策用配列チェック
            if (myData.route)
            {
                for (int i = myData.route.getList().Count - 1; i > 0; i--)
                {
                    if (myData.route.getList()[i] && myData.route.getList()[i].GetComponent<HandleData>() == myData && i > 0)
                    {
                        GUILayout.Label("【Additional_Features】");
                        if (GUILayout.Button("Go_PreviousTarget"))
                        {
                            myData.setPotision(myData.route.getList()[i - 1].GetComponent<HandleData>().getPotision());

                        }
                        EditorGUILayout.HelpBox("前の目的地の座標に移動します。", MessageType.None, true);
                        break;
                    }
                }
            }
            //Terrainが存在しているか判定
            if (Terrain.activeTerrain)
            {
                myData.AutoHeight = EditorGUILayout.ToggleLeft("Auto_Height", myData.AutoHeight);
                if (myData.AutoHeight)
                {
                    myData.CorrectionHeight = EditorGUILayout.FloatField("Correction_Height", myData.CorrectionHeight);
                    myData.setPotision(new Vector3(myData.pos.x, myData.getTerrainHigh(myData.pos.x, myData.pos.z) + myData.CorrectionHeight, myData.pos.z));
                    EditorGUILayout.HelpBox("補正値で高さを調整することもできます。", MessageType.None, true);
                }
                else
                {
                    EditorGUILayout.HelpBox("有効にすると、Terrainから高さを取得します。", MessageType.None, true);
                }
                myData.UnevenFlag = EditorGUILayout.ToggleLeft("Uneven_Flag", myData.UnevenFlag);
                if (myData.UnevenFlag)
                {
                    myData.UnevenHeight = EditorGUILayout.FloatField("UnevenHeight", myData.UnevenHeight);
                }

            }

        }
        else//配列が存在しない場合
        {
            GUILayout.Label("【Route】");
            bool allowSceneObjects = !EditorUtility.IsPersistent(target);
            myData.route = (RouteSample)EditorGUILayout.ObjectField("Route_Object", myData.route, typeof(RouteSample), allowSceneObjects);
            EditorGUILayout.HelpBox("ルート配列が指定されていません。", MessageType.Warning, true);
        }
    }

    void OnSceneGUI()
    {
        HandleData data = (HandleData)target;
        data.pos = data.transform.position;
        data.rot = data.transform.rotation;
        data.scale = data.transform.localScale;

        //配列オブジェクトが存在するか
        if (data.route)
        {//線の色を指定
            Handles.color = new Color(1f, 1f, 0f, 1f);
            //座標配列の初期設定
            Vector3[] points = new Vector3[data.route.getList().Count];
            //配列の保存先
            int positioncount = 0;
            //配列の数だけ繰り返す
            for (var i = 0; i < data.route.getList().Count; i++)
            {
                //指定したオブジェクトが存在するか
                if (data.route.getList()[i])
                {
                    string text = "";
                    switch (data.route.getList()[i].GetComponent<HandleData>().getLookOption())
                    {
                        case LookOption.NextTarget:
                            text = "N";
                            break;
                        case LookOption.LookAtHorizontal:
                            //水平方向に向きを変える
                            text = "H : " + (data.route.getList()[i].GetComponent<HandleData>().getHorizontalAngle());
                            break;
                        case LookOption.LookAtVertical:
                            //垂直方向に向きを変える
                            text = "V : " + data.route.getList()[i].GetComponent<HandleData>().getVerticalAngle();
                            break;
                        case LookOption.LookAtTarget:
                            text = "T";
                            break;
                        case LookOption.FreeChange:
                            text = "FH: " + (data.route.getList()[i].GetComponent<HandleData>().getHorizontalAngle()) + "\r\n" + "FV: " + data.route.getList()[i].GetComponent<HandleData>().getVerticalAngle();
                            break;
                        case LookOption.DontChange:
                            text = "D";
                            break;
                    }
                    //オブジェクトより少し上に表示
                    Handles.Label((data.route.getList()[i].transform.position + new Vector3(0f, 2.5f, 0f)), text);

                    //タイマーを設定している
                    if (data.route.getList()[i].GetComponent<HandleData>().getStopTime() > 0)
                    {
                        Handles.Label((data.route.getList()[i].transform.position + new Vector3(0f, -2.5f, 0f)), "Stop : " + data.route.getList()[i].GetComponent<HandleData>().getStopTime() + "s");
                    }

                    //座標表示が有効か
                    if (i < data.route.getList().Count - 1 && data.positionView)
                    {
                        Vector3 diff;
                        for (int j = i + 1; j < data.route.getList().Count; j++)
                        {
                            //次の座標が存在するか
                            if (data.route.getList()[j])
                            {
                                //座標の差を求める
                                diff = data.route.getList()[i].transform.position - data.route.getList()[j].transform.position;
                                //座標の差を２つのオブジェクトの中心に表示する
                                Handles.Label((data.route.getList()[i].transform.position - diff / 2), "(" + Mathf.Floor(diff.x * 100) / 100 + " , " + Mathf.Floor(diff.y * 100) / 100 + " , " + Mathf.Floor(diff.z * 100) / 100 + ")");
                                break;
                            }
                        }
                    }
                    //線を引くための座標配列に格納
                    points[positioncount] = data.route.getList()[i].transform.position;
                    positioncount++;
                }

            }
            //　アンチエイリアスの線を繋ぐ
            Handles.DrawAAPolyLine(5f, points);
        }

        //選択ツールを無効にする
        Tools.current = Tool.None;
        data.transform.position = PositionHandle(data.AutoHeight, data.transform, data.getTerrainHigh(data.pos.x, data.pos.z) + data.CorrectionHeight);
    }

    //3点アローの設定
    Vector3 PositionHandle(bool isUneven, Transform transform, float yh)
    {
        //選択された座標からPositionを取得
        var position = transform.position;

        //凸凹が有効の場合
        if (isUneven)
        {
            //選択された座標を外部のy座標に変更
            position.y = yh;
        }
        else//凸凹ではない場合
        {
            //ハンドルから取得
            Handles.color = Handles.yAxisColor;
            position = Handles.Slider(position, transform.up);
        }
        //その他のハンドル設定
        Handles.color = Handles.xAxisColor;
        position = Handles.Slider(position, transform.right);
        Handles.color = Handles.zAxisColor;
        position = Handles.Slider(position, transform.forward);

        return position;
    }
}