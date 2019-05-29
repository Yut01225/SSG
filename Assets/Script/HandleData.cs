using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandleData : MonoBehaviour
{
    public static bool Hint;

    //オブジェクトの各種データ
    public Vector3 pos = Vector3.zero;
    public Quaternion rot = Quaternion.identity;
    public Vector3 scale = Vector3.one;

    //【Edit用変数】
    public RouteSample route;//ルート配列
    public bool positionView;//座標表示
    public bool StoppingAngleChange;//停止中方向転換するか
    public bool AutoHeight;//自動で高さを変更する

    //カメラの動かし方
    public LookOption NextCameraOption;

    //目標値までの速度
    public float NextMoveSpeed;
    //振り向きまでの速度
    public float NextLookSpeed;

    //水平方向のカメラ角度
    public float HorizontalAngle;
    //垂直方向のカメラ角度
    public float VerticalAngle;

    //目標地点で止まる時間
    public float StopTime;

    public Transform LookTarget;

    //補正用高さ
    public float CorrectionHeight;

    //補正用
    public bool UnevenFlag;

    //補正用高さ
    public float UnevenHeight;

    //ベジェ曲線
    public bool BezierFlag;

    public bool getHint()
    {
        return Hint;
    }

    public void setHint(bool hint)
    {
        Hint = hint;
    }

    //各種データの更新用
    public void DataUpdate()
    {
        transform.rotation = rot;
        transform.position = pos;
        transform.localScale = scale;
    }

    public Transform getLookTarget()
    {
        return this.LookTarget;
    }

    public LookOption getLookOption()
    {
        return this.NextCameraOption;
    }

    public float getStopTime()
    {
        return this.StopTime;
    }
    public float getMoveSpeed()
    {
        return this.NextMoveSpeed;
    }
    public float getLookSpeed()
    {
        return this.NextLookSpeed;
    }
    public float getHorizontalAngle()
    {
        return this.HorizontalAngle;
    }
    public float getVerticalAngle()
    {
        return -this.VerticalAngle;
    }
    public bool getIsStoppingAngleChange()
    {
        return this.StoppingAngleChange;
    }
    public Vector3 getPotision()
    {
        return this.transform.localPosition;
    }

    public float getTerrainHigh(float posx, float posz)
    {
        if (Terrain.activeTerrain)
        {
            return Terrain.activeTerrain.terrainData.GetInterpolatedHeight(
         posx / Terrain.activeTerrain.terrainData.size.x,
         posz / Terrain.activeTerrain.terrainData.size.z);
        }
        else
        {
            return 0;
        }


    }
    public void setPotision(Vector3 newPosition)
    {
        this.transform.localPosition = newPosition;
    }
}