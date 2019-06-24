using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(TargetSystem))]
[CanEditMultipleObjects]
public class TargetEditor : Editor
{
    SerializedProperty ms;
    SerializedProperty Hitpoint;
    SerializedProperty little;
    SerializedProperty ClickPoint;
    SerializedProperty BrokenPoint;
    SerializedProperty PointTextObject;
    SerializedProperty CretateIndex;
    SerializedProperty type;
    SerializedProperty EffectSpeed;
    SerializedProperty AddPosition;
    SerializedProperty LimitSize;
    SerializedProperty StartTime;
    SerializedProperty IsRotation;
    SerializedProperty floorType;
    SerializedProperty right;
    SerializedProperty up;
    SerializedProperty front;
    SerializedProperty left;
    SerializedProperty down;
    SerializedProperty back;
    SerializedProperty CM;
    SerializedProperty wp;
    SerializedProperty warpPoint;
    SerializedProperty Tsize;
    SerializedProperty Changecolor;
    SerializedProperty td;
    SerializedProperty d;
    SerializedProperty PrefabIndex;

    void OnEnable()
    {
        //　シリアライズプロパティの取得
        ms = serializedObject.FindProperty("ms");
        Hitpoint = serializedObject.FindProperty("Hitpoint");
        little = serializedObject.FindProperty("little");
        ClickPoint = serializedObject.FindProperty("ClickPoint");
        BrokenPoint = serializedObject.FindProperty("BrokenPoint");
        PointTextObject = serializedObject.FindProperty("PointTextObject");
        CretateIndex = serializedObject.FindProperty("CretateIndex");
        type = serializedObject.FindProperty("type");
        EffectSpeed = serializedObject.FindProperty("EffectSpeed");
        AddPosition = serializedObject.FindProperty("AddPosition");
        LimitSize = serializedObject.FindProperty("LimitSize");
        StartTime = serializedObject.FindProperty("StartTime");
        IsRotation = serializedObject.FindProperty("IsRotation");
        floorType = serializedObject.FindProperty("floorType");
        right = serializedObject.FindProperty("right");
        up = serializedObject.FindProperty("up");
        front = serializedObject.FindProperty("front");
        left = serializedObject.FindProperty("left");
        down = serializedObject.FindProperty("down");
        back = serializedObject.FindProperty("back");
        CM = serializedObject.FindProperty("CM");
        wp = serializedObject.FindProperty("wp");
        warpPoint = serializedObject.FindProperty("warpPoint");
        Tsize = serializedObject.FindProperty("Tsize");
        Changecolor = serializedObject.FindProperty("Changecolor");
        td = serializedObject.FindProperty("td");
        d = serializedObject.FindProperty("d");
        PrefabIndex = serializedObject.FindProperty("PrefabIndex");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        TargetSystem myData = (TargetSystem)target;

        myData.PrefabIndex = EditorGUILayout.IntField("プレハブ番号", myData.PrefabIndex);

        bool allowSceneObjects = !EditorUtility.IsPersistent(target);
        GUILayout.Label("【基本設定】");
        myData.Hitpoint = EditorGUILayout.IntField("的の耐久値", myData.Hitpoint);
        myData.ClickPoint = EditorGUILayout.IntField("耐久減少時の得点", myData.ClickPoint);
        myData.BrokenPoint = EditorGUILayout.IntField("破壊時の得点", myData.BrokenPoint);
        myData.CretateIndex = EditorGUILayout.IntField("生成する目的地番号", myData.CretateIndex);
        myData.IsRotation = EditorGUILayout.ToggleLeft("回転を有効にする", myData.IsRotation);
        myData.Tsize = EditorGUILayout.ToggleLeft("Scaleを変化させる", myData.Tsize);
        myData.Changecolor = EditorGUILayout.ToggleLeft("色をランダムに変化させる", myData.Changecolor);
        myData.td = EditorGUILayout.ToggleLeft("一括破壊設定", myData.td);
        if (myData.td)
        {
            myData.d = (GameObject)EditorGUILayout.ObjectField("破壊オブジェクト", myData.d, typeof(GameObject), allowSceneObjects);
        }
        EditorGUILayout.HelpBox("移動オブジェクトが指定したインデックスに到達したときに出現します。", MessageType.Info, true);
        EditorGUILayout.Space();
        GUILayout.Label("【オブジェクト設定】");

        myData.little = (GameObject)EditorGUILayout.ObjectField("破片用オブジェ", myData.little, typeof(GameObject), allowSceneObjects);
        EditorGUILayout.HelpBox("破損や破壊時に飛び散る破片オブジェクトを指定します。", MessageType.Info, true);
        myData.PointTextObject = (GameObject)EditorGUILayout.ObjectField("得点表示用オブジェ", myData.PointTextObject, typeof(GameObject), allowSceneObjects);
        EditorGUILayout.HelpBox("文字を表示するためのオブジェクトを指定します。", MessageType.Info, true);
        EditorGUILayout.Space();
        GUILayout.Label("【出現設定】");
        myData.type = (EffectType)EditorGUILayout.EnumPopup("出現効果", (EffectType)myData.type);
        switch (myData.type)
        {
            case EffectType.Default:
                EditorGUILayout.HelpBox("最初から画面上に表示します。特殊効果はありません。", MessageType.Info, true);
                break;
            case EffectType.Big:
                EditorGUILayout.HelpBox("大きくなりながら、画面に現れます。" + "\r\n" + "効果の速度:10f,変化後の最大サイズ:8f が初期値となっています。", MessageType.Info, true);
                myData.EffectSpeed = EditorGUILayout.FloatField("効果の速度", myData.EffectSpeed);
                myData.StartTime = EditorGUILayout.FloatField("開始時間", myData.StartTime);
                myData.LimitSize = EditorGUILayout.FloatField("変化後の最大サイズ", myData.LimitSize);
                break;
            case EffectType.Invisible:
                EditorGUILayout.HelpBox("透明な状態から徐々に現れます。" + "\r\n" + "効果の速度:1f が初期値となっています。", MessageType.Info, true);
                myData.EffectSpeed = EditorGUILayout.FloatField("効果の速度", myData.EffectSpeed);
                myData.StartTime = EditorGUILayout.FloatField("開始時間", myData.StartTime);
                break;
            case EffectType.Move:
                EditorGUILayout.HelpBox("指定した座標分ずれた位置に移動します。その後、配置場所に戻ってきます。" + "\r\n" + "効果の速度:5f が初期値となっています。", MessageType.Info, true);
                myData.EffectSpeed = EditorGUILayout.FloatField("効果の速度", myData.EffectSpeed);
                myData.StartTime = EditorGUILayout.FloatField("開始時間", myData.StartTime);
                myData.AddPosition = EditorGUILayout.Vector3Field("出現する座標", myData.AddPosition);
                break;
            case EffectType.Flash:
                myData.EffectSpeed = EditorGUILayout.FloatField("効果の速度", myData.EffectSpeed);
                myData.floorType = (FloorType)EditorGUILayout.EnumPopup("出現タイプ", (FloorType)myData.floorType);
                break;
            case EffectType.MoveBlock:
                myData.EffectSpeed = EditorGUILayout.FloatField("効果の速度", myData.EffectSpeed);
                myData.CM = EditorGUILayout.FloatField("移動距離", myData.CM);
                myData.right = EditorGUILayout.ToggleLeft("X", myData.right);
                myData.up = EditorGUILayout.ToggleLeft("Y", myData.up);
                myData.front = EditorGUILayout.ToggleLeft("Z", myData.front);
                myData.left = EditorGUILayout.ToggleLeft("-X", myData.left);
                myData.down = EditorGUILayout.ToggleLeft("-Y", myData.down);
                myData.back = EditorGUILayout.ToggleLeft("-Z", myData.back);
                break;
            case EffectType.Warp:
                GUILayout.Label("ワープ回数");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("wp"), true);
                EditorGUILayout.Space();
                GUILayout.Label("ワープポイント");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("warpPoint"), true);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
