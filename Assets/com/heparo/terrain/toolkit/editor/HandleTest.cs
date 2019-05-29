using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(HandleData))]
[CanEditMultipleObjects]
public class HandleTest : Editor
{
    //　シリアライズプロパティ
    SerializedProperty hint;
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
    SerializedProperty BezierFlag;

    void OnEnable()
    {
        //　シリアライズプロパティの取得
        hint = serializedObject.FindProperty("hint");
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
        BezierFlag = serializedObject.FindProperty("BezierFlag");
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
            //【ヒント】
            myData.setHint(EditorGUILayout.ToggleLeft("View_Hint", myData.getHint()));
            if (!myData.getHint())
            {
                EditorGUILayout.HelpBox("ヒントが必要ですか？ " + "\r\n" + "ヒントを有効にすると、各項目の動作の説明が追加されます。", MessageType.Info, true);
            }

            if (myData.route)
            {
                for (int k = 0; k < myData.route.getList().Count; k++)
                {
                    //自分を探す
                    if (myData.route.getList()[k] && myData.route.getList()[k].GetComponent<HandleData>() == myData)
                    {
                        if (k > 0 && myData.route.getList()[k - 1] && myData.route.getList()[k - 1].GetComponent<HandleData>().BezierFlag)
                        {
                            EditorGUILayout.HelpBox("ベジェ曲線の端点として利用されています。", MessageType.Info, true);
                        }
                        else
                        {
                            //【座標】
                            GUILayout.Label("【Edit_Option】");
                            myData.positionView = EditorGUILayout.ToggleLeft("View_Position", myData.positionView);
                            if (myData.getHint())
                            {
                                EditorGUILayout.HelpBox("次の目的地までの座標の差を表示します。" + "\r\n" + "表記は、(x,y,z)の順に表記されています。", MessageType.None, true);
                            }
                            EditorGUILayout.Space();
                            //【設定】
                            GUILayout.Label("【Options】");
                            myData.StoppingAngleChange = EditorGUILayout.ToggleLeft("Stopping_AngleChange", myData.StoppingAngleChange);
                            if (myData.getHint())
                            {
                                EditorGUILayout.HelpBox("停止時に方向転換できるようにします。" + "\r\n" + "有効でない場合、移動開始するまで方向転換しません。", MessageType.None, true);
                            }
                            float leftValue = -179.0f;
                            float rightValue = 179.0f;
                            EditorGUILayout.Space();
                            myData.BezierFlag = EditorGUILayout.ToggleLeft("Bezier", myData.BezierFlag);
                            if (myData.getHint())
                            {
                                EditorGUILayout.HelpBox("ベジェ曲線移動を有効にします。" + "\r\n" + "次の目的地を端点として2つ次の目的地に移動します。", MessageType.None, true);
                            }
                            EditorGUILayout.Space();
                            //【方向】
                            GUILayout.Label("【Angle】");
                            myData.NextCameraOption = (LookOption)EditorGUILayout.EnumPopup("Camera_Option", (LookOption)myData.NextCameraOption);
                            string text = "";
                            switch (myData.NextCameraOption)
                            {
                                case LookOption.NextTarget:
                                    text = "次の目的地の(x,y,z)方向に向くように追従します。" + "\r\n" + "Terrain追従が有効な場合、次の目的地の(x,z)方向のみ追従します。";
                                    break;
                                case LookOption.LookAtHorizontal:
                                    GUILayout.Label("Horizontal_Angle");
                                    myData.HorizontalAngle = EditorGUILayout.Slider(myData.HorizontalAngle, leftValue, rightValue);
                                    text = "現在の方向から水平方向にカメラを方向転換します。" + "\r\n" + "-179度から+179度まで有効です。";
                                    break;
                                case LookOption.LookAtVertical:
                                    GUILayout.Label("Vertical_Angle");
                                    myData.VerticalAngle = EditorGUILayout.Slider(myData.VerticalAngle, leftValue, rightValue);
                                    text = "現在の向きから垂直方向に指定した角度方向を変えます。" + "\r\n" + "-179度から+179度まで有効です。";
                                    break;
                                case LookOption.FreeChange:
                                    GUILayout.Label("Horizontal_Angle");
                                    myData.HorizontalAngle = EditorGUILayout.Slider(myData.HorizontalAngle, leftValue, rightValue);
                                    GUILayout.Label("Vertical_Angle");
                                    myData.VerticalAngle = EditorGUILayout.Slider(myData.VerticalAngle, leftValue, rightValue);
                                    text = "現在の方向から垂直、水平方向を同時に指定します。" + "\r\n" + "それぞれ、-179度から+179度まで有効です。";
                                    break;
                                case LookOption.DontChange:
                                    text = "次の目的地に到着するまでカメラの方向を変更しません。";
                                    break;
                                case LookOption.LookAtTarget:
                                    bool allowSceneObjects1 = !EditorUtility.IsPersistent(target);
                                    myData.LookTarget = (Transform)EditorGUILayout.ObjectField("Target_Object", myData.LookTarget, typeof(Transform), allowSceneObjects1);
                                    text = "指定したオブジェクトの方向を見続けます。" + "\r\n" + "オブジェクトを指定していない場合は、次の目的地を追従するようになります";
                                    if (!myData.LookTarget)
                                    {
                                        EditorGUILayout.HelpBox("オブジェクトが設定されていません。", MessageType.Warning, true);
                                    }
                                    break;
                            }
                            if (myData.getHint())
                            {
                                EditorGUILayout.HelpBox(text, MessageType.None, true);
                            }
                            EditorGUILayout.Space();
                            //【速度】
                            GUILayout.Label("【Speed】");
                            myData.NextMoveSpeed = EditorGUILayout.FloatField("Move_Speed", myData.NextMoveSpeed);
                            if (myData.getHint())
                            {
                                EditorGUILayout.HelpBox("次の目的地までの移動速度を指定します。" + "\r\n" + "0の場合は、デフォルトの値が適応されます。", MessageType.None, true);

                            }
                            myData.NextLookSpeed = EditorGUILayout.FloatField("Look_Speed", myData.NextLookSpeed);
                            if (myData.getHint())
                            {
                                EditorGUILayout.HelpBox("次の目的地までの方向転換の速度を指定します。" + "\r\n" + "0の場合は、デフォルトの値が適応されます。", MessageType.None, true);
                            }
                            EditorGUILayout.Space();
                            //【時間】
                            GUILayout.Label("【Time】");
                            myData.StopTime = EditorGUILayout.FloatField("Stop_Time", myData.StopTime);
                            if (myData.getHint())
                            {
                                EditorGUILayout.HelpBox("指定した時間、目的地で停止します。" + "\r\n" + "時間の単位は「秒」です。", MessageType.None, true);
                            }
                            EditorGUILayout.Space();
                            //【経路】
                            GUILayout.Label("【Route】");
                            bool allowSceneObjects = !EditorUtility.IsPersistent(target);
                            myData.route = (RouteSample)EditorGUILayout.ObjectField("Route_Object", myData.route, typeof(RouteSample), allowSceneObjects);
                            if (myData.getHint())
                            {
                                EditorGUILayout.HelpBox("経路配列を格納します。" + "\r\n" + "経路配列を格納することで編集作業がしやすくなります。", MessageType.None, true);
                            }
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
                                            for (int j = i - 1; j > -1; j--)
                                            {
                                                if (myData.route.getList()[j])
                                                {
                                                    myData.setPotision(myData.route.getList()[j].GetComponent<HandleData>().getPotision());
                                                    myData.pos = myData.route.getList()[j].GetComponent<HandleData>().getPotision();
                                                    break;
                                                }
                                            }
                                        }
                                        if (myData.getHint())
                                        {
                                            EditorGUILayout.HelpBox("前の目的地の座標に移動します。" + "\r\n" + "巻き戻しが効かないので注意して作業してください。", MessageType.None, true);
                                        }
                                        break;
                                    }
                                }
                            }
                            //Terrainが存在しているか判定
                            if (Terrain.activeTerrain)
                            {
                                GUILayout.Label("【Terrain】");
                                myData.AutoHeight = EditorGUILayout.ToggleLeft("Auto_Height", myData.AutoHeight);
                                if (myData.getHint())
                                {
                                    EditorGUILayout.HelpBox("有効にすると、リアルタイムでTerrainの表面からy座標を取得します。", MessageType.None, true);
                                }
                                if (myData.AutoHeight)
                                {
                                    myData.CorrectionHeight = EditorGUILayout.FloatField("Correction_Height", myData.CorrectionHeight);
                                    myData.setPotision(new Vector3(myData.pos.x, myData.getTerrainHigh(myData.pos.x, myData.pos.z) + myData.CorrectionHeight, myData.pos.z));
                                    if (myData.getHint())
                                    {
                                        EditorGUILayout.HelpBox("入力した数値分、Terrainの表面のy座標に追加します。" + "\r\n" + "0の場合は、terrainの表面座標が設定されます。", MessageType.None, true);
                                    }
                                }
                                EditorGUILayout.Space();
                                myData.UnevenFlag = EditorGUILayout.ToggleLeft("Uneven_Flag", myData.UnevenFlag);
                                if (myData.getHint())
                                {
                                    EditorGUILayout.HelpBox("有効にすると、次の目的地までの間、Terrainの表面を追従するカメラになります。", MessageType.None, true);
                                }
                                if (myData.UnevenFlag)
                                {
                                    myData.UnevenHeight = EditorGUILayout.FloatField("UnevenHeight", myData.UnevenHeight);
                                    if (myData.getHint())
                                    {
                                        EditorGUILayout.HelpBox("入力した数値分、Terrainの表面のy座標に追加します。" + "\r\n" + "0の場合は、terrainの表面座標が設定されます。", MessageType.None, true);
                                    }
                                }

                            }
                        }

                    }
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
        {
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

                    //自分自身かつ座標表示が有効の場合
                    if (data.route.getList()[i].GetComponent<HandleData>() == data && data.positionView)
                    {
                        Vector3 diff;
                        //Nullではない次の目的地を取得する
                        for (int j = i + 1; j < data.route.getList().Count; j++)
                        {
                            //次の座標が存在するか
                            if (data.route.getList()[j])
                            {
                                //座標の差を求める
                                diff = data.route.getList()[i].transform.position - data.route.getList()[j].transform.position;
                                //座標の差を２つのオブジェクトの中心に表示する
                                Handles.Label(data.transform.position, "(" + Mathf.Floor(diff.x * 100) / 100 + " , " + Mathf.Floor(diff.y * 100) / 100 + " , " + Mathf.Floor(diff.z * 100) / 100 + ")");
                                break;
                            }
                        }
                    }
                    //線を引くための座標配列に格納
                    points[positioncount] = data.route.getList()[i].transform.position;
                    positioncount++;
                }
                else
                {
                    positioncount++;
                }
            }
            //配列の要素数繰り返す
            for (var i = 0; i < data.route.getList().Count - 1; i++)
            {
                if (!data.route.getList()[i])
                {
                    continue;
                }
                int NextTargetIndex = i + 1;
                for (int j = NextTargetIndex; j < data.route.getList().Count; j++)
                {
                    if (!data.route.getList()[NextTargetIndex])
                    {
                        NextTargetIndex++;
                    }
                    else
                    {
                        break;
                    }
                }
                //Handles.DrawBezier(points[6], points[8], points[7], points[7], Color.red, null, 2f);

                if (data.route.getList()[i] && data.route.getList()[i].GetComponent<HandleData>().BezierFlag)
                {
                    if (i + 2 < data.route.getList().Count && data.route.getList()[i + 1] && data.route.getList()[i + 2])
                    {
                        if (data.route.getList()[i].GetComponent<HandleData>() == data)
                        {
                            //選択場所を太くする
                            Handles.DrawBezier(points[i], points[i + 2], points[i + 1], points[i + 1], new Color(1f, 0f, 1f, 1f), null, 10f);
                        }
                        else
                        {
                            //選択場所以外は細くする
                            Handles.DrawBezier(points[i], points[i + 2], points[i + 1], points[i + 1], new Color(1f, 0f, 1f, 1f), null, 3f);
                        }
                    }

                }
                else
                {
                    if (i > 0 && data.route.getList()[i - 1] && data.route.getList()[i - 1].GetComponent<HandleData>().BezierFlag)
                    {
                        if (i + 1 < data.route.getList().Count && data.route.getList()[i] && data.route.getList()[i + 1])
                        {
                            if (data.route.getList()[i].GetComponent<HandleData>() == data)
                            {
                                //選択場所を太くする
                                Handles.DrawBezier(points[i - 1], points[i + 1], points[i], points[i], new Color(1f, 0f, 1f, 1f), null, 10f);
                            }
                            else
                            {
                                //選択場所以外は細くする
                                Handles.DrawBezier(points[i - 1], points[i + 1], points[i], points[i], new Color(1f, 0f, 1f, 1f), null, 3f);
                            }
                        }
                    }
                    else
                    { //目的地から次の目的地を格納
                        Vector3[] sample = { points[i], points[NextTargetIndex] };
                        switch (data.route.getList()[i].GetComponent<HandleData>().getLookOption())
                        {
                            case LookOption.NextTarget:
                                //線の色を黄色
                                Handles.color = new Color(1f, 1f, 0f, 1f);
                                break;
                            case LookOption.LookAtHorizontal:
                                //線の色を緑
                                Handles.color = new Color(0f, 1f, 0f, 1f);
                                break;
                            case LookOption.LookAtVertical:
                                //線の色を青
                                Handles.color = new Color(0f, 0f, 1f, 1f);
                                break;
                            case LookOption.LookAtTarget:
                                //線の色を赤
                                Handles.color = new Color(1f, 0f, 0f, 1f);
                                break;
                            case LookOption.FreeChange:
                                //線の色を白
                                Handles.color = new Color(1f, 1f, 1f, 1f);
                                break;
                            case LookOption.DontChange:
                                //線の色を黒
                                Handles.color = new Color(0f, 0f, 0f, 1f);
                                break;
                        }
                        //選択した場所を判定する
                        if (data.route.getList()[i].GetComponent<HandleData>() == data)
                        {
                            //選択場所を太くする
                            Handles.DrawAAPolyLine(10f, sample);
                        }
                        else
                        {
                            //選択場所以外は細くする
                            Handles.DrawAAPolyLine(3f, sample);
                        }
                    }

                }
            }

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