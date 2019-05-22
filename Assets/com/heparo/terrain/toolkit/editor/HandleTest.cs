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
    }

    public override void OnInspectorGUI()
    {
        //　シリアライズオブジェクトの更新
        serializedObject.Update();
        //　targetでデータを取得しキャスト
        HandleData myData = (HandleData)target;
        GUILayout.Label("【Edit_Option】");
        myData.positionView = EditorGUILayout.ToggleLeft("View_Position", myData.positionView);
        EditorGUILayout.Space();
        GUILayout.Label("【Options】");
        myData.StoppingAngleChange = EditorGUILayout.ToggleLeft("Stopping_AngleChange", myData.StoppingAngleChange);

        float leftValue = -90.0f;
        float rightValue = 90.0f;

        myData.NextCameraOption = (LookOption)EditorGUILayout.EnumPopup("Camera_Option", (LookOption)myData.NextCameraOption);
        switch (myData.NextCameraOption)
        {
            case LookOption.NextTarget:
                break;
            case LookOption.LookAtHorizontal:
                EditorGUILayout.Space();
                GUILayout.Label("【Angle】");
                GUILayout.Label("Horizontal_Angle");
                myData.HorizontalAngle = EditorGUILayout.Slider(myData.HorizontalAngle, leftValue, rightValue);
                break;
            case LookOption.LookAtVertical:
                EditorGUILayout.Space();
                GUILayout.Label("【Angle】");
                GUILayout.Label("Vertical_Angle");
                myData.VerticalAngle = EditorGUILayout.Slider(myData.VerticalAngle, leftValue, rightValue);
                break;
            case LookOption.DontLook:
                break;
        }
        EditorGUILayout.Space();
        GUILayout.Label("【Speed】");
        myData.NextMoveSpeed = EditorGUILayout.FloatField("Move_Speed", myData.NextMoveSpeed);
        myData.NextLookSpeed = EditorGUILayout.FloatField("Look_Speed", myData.NextLookSpeed);
        EditorGUILayout.Space();
        GUILayout.Label("【Time】");
        myData.StopTime = EditorGUILayout.FloatField("Stop_Time", myData.StopTime);
    }

        void OnSceneGUI()
    {
        HandleData data = (HandleData)target;
        data.pos = data.transform.position;
        data.rot = data.transform.rotation;
        data.scale = data.transform.localScale;


        //配列オブジェクトが存在するか
        if (data.s)
        {
            //線の色を指定
            Handles.color = new Color(1f, 1f, 0f, 1f);
            //座標配列の初期設定
            Vector3[] points = new Vector3[data.s.getList().Count];
            //配列の保存先
            int positioncount = 0;
            //配列の数だけ繰り返す
            for (var i = 0; i < data.s.getList().Count; i++)
            {
                //指定したオブジェクトが存在するか
                if (data.s.getList()[i])
                {
                    string text = "";
                    switch (data.s.getList()[i].GetComponent<HandleData>().getLookOption())
                    {
                        case LookOption.NextTarget:
                            text = "N";
                            break;
                        case LookOption.LookAtHorizontal:
                            //水平方向に向きを変える
                            text = "H : " + (data.s.getList()[i].GetComponent<HandleData>().getHorizontalAngle() + 90);
                            break;
                        case LookOption.LookAtVertical:
                            //垂直方向に向きを変える
                            text = "V : " + data.s.getList()[i].GetComponent<HandleData>().getVerticalAngle();
                            break;
                        case LookOption.DontLook:
                            text = "D";
                            break;
                    }
                    Handles.Label((data.s.getList()[i].transform.position + new Vector3(0f,2.5f,0f)),text);

                    //タイマーを設定している
                    if(data.s.getList()[i].GetComponent<HandleData>().getStopTime() > 0)
                    {
                        Handles.Label((data.s.getList()[i].transform.position + new Vector3(0f, -2.5f, 0f)), "Stop : " + data.s.getList()[i].GetComponent<HandleData>().getStopTime() + "s");
                    }

                    //座標表示が有効か
                    if (i < data.s.getList().Count - 1 && data.positionView)
                    {
                        Vector3 diff;
                        for (int j = i + 1; j < data.s.getList().Count; j++)
                        {
                            //次の座標が存在するか
                            if (data.s.getList()[j])
                            {
                                //座標の差を求める
                                diff = data.s.getList()[i].transform.position - data.s.getList()[j].transform.position;
                                //座標の差を２つのオブジェクトの中心に表示する
                                Handles.Label((data.s.getList()[i].transform.position - diff / 2), "(" + Mathf.Floor(diff.x * 100) / 100 + " , " + Mathf.Floor(diff.y * 100) / 100 + " , " + Mathf.Floor(diff.z * 100) / 100 + ")");
                                break;
                            }
                        }
                    }
                    //線を引くための座標配列に格納
                    points[positioncount] = data.s.getList()[i].transform.position;
                    positioncount++;
                }

            }
            //　アンチエイリアスの線を繋ぐ
            Handles.DrawAAPolyLine(5f,points);
        }

    }


}