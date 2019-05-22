using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LookOption
{
    NextTarget,//次の目標
    LookAtHorizontal,//左右方向
    LookAtVertical,//上下方向
    DontLook//カメラが動かない
}

public class MoveSample : MonoBehaviour
{
    //ルート配列
    public Sample s;

    //各種速度
    public float DefaultMoveSpeed;//移動速度

    //目的地までの座標
    private float diffX;
    private float diffY;
    private float diffZ;

    //現在の方向情報
    private Quaternion now_rot;
    //向きたい方向情報
    private Quaternion target;

    //次の目的地
    public int index = 0;
    //現在の場所
    private int oldindex = -1;

    //待機用カウントダウンタイマー
    private float Timer = 0;

    void Start()
    {
        //存在するまで繰り返す
        for (int i = index; i < s.getList().Count; i++)
        {
            //リストが存在するか判定
            if (s.getList()[i])
            {
                //リストの一番最初を開始位置にする
                oldindex = i;
                //最初のポイントに移動する
                this.transform.position = s.getList()[oldindex].transform.position;
                break;
            }
        }
        //存在するまで繰り返す
        for (int i = oldindex + 1; i < s.getList().Count; i++)
        {
            //リストが存在するか判定
            if (s.getList()[i])
            {
                //次の目的地に設定する
                index = i;
                //瞬時に次の目的地に方向を変える
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(s.getList()[index].transform.position - transform.position), 1f);
                break;
            }
        }
        //目的地までの差分を計算する
        CalculationDiff(s.getList()[index].transform.position, this.transform.position);
    }

    void Update()
    {
        //タイマーがセットされている
        if (Timer > 0)
        {
            //カウントダウンを行う
            Timer -= Time.deltaTime * 1f;
            //停止中方向転換が有効か判定
            if (s.getList()[oldindex].GetComponent<HandleData>().getIsStoppingAngleChange())
            {
                //向きを変える
                ChangeLook();
            }
        }
        else
        {
            //配列が存在するか
            if (index < s.getList().Count)
            {
                //向きを変える
                ChangeLook();
                //【X座標】
                X_PositionChange();
                //【Y座標】
                Y_PositionChange();
                //【Z座標】
                Z_PositionChange();
                //【X,Y,Zが目的地に完全一致しているか】
                CheckPositions();
            }
        }
    }

    //X座標を変更する
    void X_PositionChange()
    {
        //目標が大きい場合
        if (s.getList()[index].transform.position.x < this.transform.position.x)
        {
            this.transform.position -= new Vector3(diffX * Time.deltaTime, 0, 0);
            //加算後に目標を超えた場合
            if (s.getList()[index].transform.position.x > this.transform.position.x)
            {
                this.transform.position = new Vector3(s.getList()[index].transform.position.x, this.transform.position.y, this.transform.position.z);
            }
        }
        //目標が小さい場合
        if (s.getList()[index].transform.position.x > this.transform.position.x)
        {
            this.transform.position += new Vector3(diffX * Time.deltaTime, 0, 0);
            //減算後に目標を超えた場合
            if (s.getList()[index].transform.position.x < this.transform.position.x)
            {
                this.transform.position = new Vector3(s.getList()[index].transform.position.x, this.transform.position.y, this.transform.position.z);
            }
        }
    }
    //Y座標を変更する
    void Y_PositionChange()
    {
        //目標が大きい場合
        if (s.getList()[index].transform.position.y < this.transform.position.y)
        {
            this.transform.position -= new Vector3(0, diffY * Time.deltaTime, 0);
            //加算後に目標を超えた場合
            if (s.getList()[index].transform.position.y > this.transform.position.y)
            {
                this.transform.position = new Vector3(this.transform.position.x, s.getList()[index].transform.position.y, this.transform.position.z);
            }
        }
        //目標が小さい場合
        if (s.getList()[index].transform.position.y > this.transform.position.y)
        {
            this.transform.position += new Vector3(0, diffY * Time.deltaTime, 0);
            //減算後に目標を超えた場合
            if (s.getList()[index].transform.position.y < this.transform.position.y)
            {
                this.transform.position = new Vector3(this.transform.position.x, s.getList()[index].transform.position.y, this.transform.position.z);
            }
        }
    }
    //Z座標を変更する
    void Z_PositionChange()
    {
        //目標が大きい場合
        if (s.getList()[index].transform.position.z < this.transform.position.z)
        {
            this.transform.position -= new Vector3(0, 0, diffZ * Time.deltaTime);
            //加算後に目標を超えた場合
            if (s.getList()[index].transform.position.z > this.transform.position.z)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, s.getList()[index].transform.position.z);
            }
        }
        //目標が小さい場合
        if (s.getList()[index].transform.position.z > this.transform.position.z)
        {
            this.transform.position += new Vector3(0, 0, diffZ * Time.deltaTime);
            //減算後に目標を超えた場合
            if (s.getList()[index].transform.position.z < this.transform.position.z)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, s.getList()[index].transform.position.z);
            }
        }
    }
    //
    void CheckPositions()
    {
        //座標がすべて一致した時
        if (s.getList()[index].transform.position.x == this.transform.position.x && s.getList()[index].transform.position.y == this.transform.position.y && s.getList()[index].transform.position.z == this.transform.position.z)
        {
            //現在地を保存する
            oldindex = index;
            //リストの最後でない場合
            if (++index < s.getList().Count)
            {
                //存在するまで繰り返す
                for (int i = index; i < s.getList().Count; i++)
                {
                    //リストが存在するか判定
                    if (s.getList()[i])
                    {
                        //存在しているので次の目的地をその場所まで飛ばす
                        index = i;
                        //目的地までの差分を計算する
                        CalculationDiff(s.getList()[index].transform.position, this.transform.position);
                        //タイマーをセットする
                        this.Timer = s.getList()[oldindex].GetComponent<HandleData>().getStopTime();
                        break;
                    }
                    if (s.getList().Count - 1 <= i)
                    {
                        index = s.getList().Count;
                    }
                }
            }
        }
    }

    //向きを変える
    void ChangeLook()
    {
        //目的値から振り向き速度を取得
        float lookspeed = (s.getList()[oldindex].GetComponent<HandleData>().getLookSpeed() / 1000);
        if (lookspeed <= 0)//設定していない場合
        {
            //距離に応じた速度を設定
            lookspeed = 0.1f / CalculationTime(s.getList()[index].transform.position, transform.position);
        }
        //目的地の条件を取得
        switch (s.getList()[oldindex].GetComponent<HandleData>().getLookOption())
        {
            case LookOption.NextTarget:
                //次の目的地に向きを変える
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(s.getList()[index].transform.position - transform.position), lookspeed);
                break;
            case LookOption.LookAtHorizontal:
                //水平方向に向きを変える
                target = Quaternion.Euler(new Vector3(transform.localEulerAngles.x, s.getList()[oldindex].GetComponent<HandleData>().getHorizontalAngle(), transform.localEulerAngles.z));
                //現在の方向を取得する
                now_rot = transform.rotation;
                //角度差が無いか判定
                if (Quaternion.Angle(now_rot, target) <= 1)
                {
                    transform.rotation = target;
                }
                else
                {
                    if (s.getList()[oldindex].GetComponent<HandleData>().getHorizontalAngle() > -90)
                    {
                        transform.Rotate(new Vector3(0, lookspeed * 100,0));
                    }
                    else
                    {
                        transform.Rotate(new Vector3(0,-lookspeed * 100, 0));
                    }
                }
                break;
            case LookOption.LookAtVertical:
                //向けたい方向を設定する
                target = Quaternion.Euler(new Vector3(-s.getList()[oldindex].GetComponent<HandleData>().getVerticalAngle(), transform.localEulerAngles.y, transform.localEulerAngles.z));
                //現在の方向を取得する
                now_rot = transform.rotation;
                //角度差が無いか判定
                if (Quaternion.Angle(now_rot, target) <= 1)
                {
                    transform.rotation = target;
                }
                else
                {
                    if (s.getList()[oldindex].GetComponent<HandleData>().getVerticalAngle() > 0)
                    {
                        transform.Rotate(new Vector3(-lookspeed * 100, 0, 0));
                    }
                    else
                    {
                        transform.Rotate(new Vector3(lookspeed * 100, 0, 0));
                    } 
                }
                break;
            case LookOption.DontLook:
                //動かさない
                break;
        }
    }

    //座標の差分を求める
    float Diff(float StartPoint, float EndPoint)
    {
        //目標が開始より大きい
        if (StartPoint < EndPoint)
        {
            return EndPoint - StartPoint;
        }
        else
        {
            return StartPoint - EndPoint;
        }
    }

    //移動する距離を求める
    void CalculationDiff(Vector3 StartVector, Vector3 EndVector)
    {
        //最長距離からかかる時間を求める
        float time = CalculationTime(StartVector, EndVector);
        //かかる時間から移動すべき距離を求める
        diffX = Diff(StartVector.x, EndVector.x) / time;
        diffY = Diff(StartVector.y, EndVector.y) / time;
        diffZ = Diff(StartVector.z, EndVector.z) / time;
    }

    //移動にかかる時間求める
    float CalculationTime(Vector3 StartVector, Vector3 EndVector)
    {
        //移動速度を目標ポイントから取得する
        float speed = s.getList()[oldindex].GetComponent<HandleData>().getMoveSpeed();
        //移動速度が設定されていない
        if (speed <= 0)
        {
            //初期設定速度にする
            speed = DefaultMoveSpeed;
        }
        //xの差を求める
        float Maxdiff = Diff(StartVector.x, EndVector.x);
        //yの差が最大を超える場合
        if (Maxdiff < Diff(StartVector.y, EndVector.y))
        {
            Maxdiff = Diff(StartVector.y, EndVector.y);
        }
        //zの差が最大を超える場合
        if (Maxdiff < Diff(StartVector.z, EndVector.z))
        {
            Maxdiff = Diff(StartVector.z, EndVector.z);
        }
        //最長距離からかかる時間を求める
        return (Maxdiff / speed);
    }

    float AngleCorrection(float angle)
    {
        while (angle > 180)
        {
                angle -= 360;
        }
        return angle;
    }
}
