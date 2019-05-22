using UnityEngine;
using System.Collections;



public class HandleData : MonoBehaviour
{
    //オブジェクトの各種データ
    public Vector3 pos = Vector3.zero;
    public Quaternion rot = Quaternion.identity;
    public Vector3 scale = Vector3.one;

    //【Edit用変数】
    public Sample s;//ルート配列
    public bool positionView;//座標表示
    public bool StoppingAngleChange;//停止中方向転換するか

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

    //各種データの更新用
    public void DataUpdate()
    {
        transform.rotation = rot;
        transform.position = pos;
        transform.localScale = scale;
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
        float angle = this.HorizontalAngle;
        if (angle >= 0)
        {
            while (angle > 90)
            {
                angle -= 90;
            }
        }
        else
        {
            while (angle < -90)
            {
                angle += 90;
            }
        }
        return angle - 90;
    }
    public float getVerticalAngle()
    {
        float angle = this.VerticalAngle;
        if (angle >= 0)
        {
            while (angle > 90)
            {
                angle -= 90;
            }
        }
        else
        {
            while (angle < -90)
            {
                angle += 90;
            }
        }
        return angle;
    }
    public bool getIsStoppingAngleChange()
    {
        return this.StoppingAngleChange;
    }

}