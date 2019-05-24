using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LookOption
{
    NextTarget,//次の目標
    LookAtHorizontal,//左右方向
    LookAtVertical,//上下方向
    LookAtTarget,//指定オブジェクトの方向
    FreeChange,//自由変更
    DontChange//カメラが動かない
}

public class MoveSample : MonoBehaviour
{
    //ルート配列
    public RouteSample route;

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

    //でこぼこを有効
    public bool IsUneven;
    public float CorrectionHeight;

    void Start()
    {
        //存在するまで繰り返す
        for (int i = index; i < route.getList().Count; i++)
        {
            //リストが存在するか判定
            if (route.getList()[i])
            {
                //リストの一番最初を開始位置にする
                oldindex = i;
                //最初のポイントに移動する
                this.transform.position = route.getList()[oldindex].transform.position;
                break;
            }
        }
        //存在するまで繰り返す
        for (int i = oldindex + 1; i < route.getList().Count; i++)
        {
            //リストが存在するか判定
            if (route.getList()[i])
            {
                //次の目的地に設定する
                index = i;
                //瞬時に次の目的地に方向を変える
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(route.getList()[index].transform.position - transform.position), 1f);
                break;
            }
        }
        //目的地までの差分を計算する
        CalculationDiff(route.getList()[index].transform.position, this.transform.position);
        //初期のタイマーを取得する
        this.Timer = route.getList()[oldindex].GetComponent<HandleData>().getStopTime();
    }

    void Update()
    {
        //タイマーがセットされている
        if (Timer > 0)
        {
            //カウントダウンを行う
            Timer -= Time.deltaTime * 1f;
            //停止中方向転換が有効か判定
            if (route.getList()[oldindex].GetComponent<HandleData>().getIsStoppingAngleChange())
            {
                //向きを変える
                ChangeLook();
            }
        }
        else
        {
            //配列が存在するか
            if (index < route.getList().Count)
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
        if (route.getList()[index].transform.position.x < this.transform.position.x)
        {
            this.transform.position -= new Vector3(diffX * Time.deltaTime, 0, 0);
            //加算後に目標を超えた場合
            if (route.getList()[index].transform.position.x > this.transform.position.x)
            {
                this.transform.position = new Vector3(route.getList()[index].transform.position.x, this.transform.position.y, this.transform.position.z);
            }
        }
        //目標が小さい場合
        if (route.getList()[index].transform.position.x > this.transform.position.x)
        {
            this.transform.position += new Vector3(diffX * Time.deltaTime, 0, 0);
            //減算後に目標を超えた場合
            if (route.getList()[index].transform.position.x < this.transform.position.x)
            {
                this.transform.position = new Vector3(route.getList()[index].transform.position.x, this.transform.position.y, this.transform.position.z);
            }
        }
    }
    //Y座標を変更する
    void Y_PositionChange()
    {
        if (IsUneven)
        {
           float terrainHeight = Terrain.activeTerrain.terrainData.GetInterpolatedHeight(this.transform.position.x / Terrain.activeTerrain.terrainData.size.x,this.transform.position.z / Terrain.activeTerrain.terrainData.size.z);
            this.transform.position = new Vector3(this.transform.position.x, terrainHeight + CorrectionHeight, this.transform.position.z);
        }
        else
        {
            //目標が大きい場合
            if (route.getList()[index].transform.position.y < this.transform.position.y)
            {
                this.transform.position -= new Vector3(0, diffY * Time.deltaTime, 0);
                //加算後に目標を超えた場合
                if (route.getList()[index].transform.position.y > this.transform.position.y)
                {
                    this.transform.position = new Vector3(this.transform.position.x, route.getList()[index].transform.position.y, this.transform.position.z);
                }
            }
            //目標が小さい場合
            if (route.getList()[index].transform.position.y > this.transform.position.y)
            {
                this.transform.position += new Vector3(0, diffY * Time.deltaTime, 0);
                //減算後に目標を超えた場合
                if (route.getList()[index].transform.position.y < this.transform.position.y)
                {
                    this.transform.position = new Vector3(this.transform.position.x, route.getList()[index].transform.position.y, this.transform.position.z);
                }
            }
        }

    }
    //Z座標を変更する
    void Z_PositionChange()
    {
        //目標が大きい場合
        if (route.getList()[index].transform.position.z < this.transform.position.z)
        {
            this.transform.position -= new Vector3(0, 0, diffZ * Time.deltaTime);
            //加算後に目標を超えた場合
            if (route.getList()[index].transform.position.z > this.transform.position.z)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, route.getList()[index].transform.position.z);
            }
        }
        //目標が小さい場合
        if (route.getList()[index].transform.position.z > this.transform.position.z)
        {
            this.transform.position += new Vector3(0, 0, diffZ * Time.deltaTime);
            //減算後に目標を超えた場合
            if (route.getList()[index].transform.position.z < this.transform.position.z)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, route.getList()[index].transform.position.z);
            }
        }
    }

    void CheckPositions()
    {
        //座標がすべて一致した時
        if (route.getList()[index].transform.position.x == this.transform.position.x && route.getList()[index].transform.position.z == this.transform.position.z)
        {
            if (route.getList()[index].transform.position.y == this.transform.position.y || IsUneven)
            {
                //現在地を保存する
                oldindex = index;
                //リストの最後でない場合
                if (++index < route.getList().Count)
                {
                    //存在するまで繰り返す
                    for (int i = index; i < route.getList().Count; i++)
                    {
                        //リストが存在するか判定
                        if (route.getList()[i])
                        {
                            //存在しているので次の目的地をその場所まで飛ばす
                            index = i;
                            //目的地までの差分を計算する
                            CalculationDiff(route.getList()[index].transform.position, this.transform.position);
                            //タイマーをセットする
                            this.Timer = route.getList()[oldindex].GetComponent<HandleData>().getStopTime();
                            //目的地までのカメラの設定
                            switch (route.getList()[oldindex].GetComponent<HandleData>().getLookOption())
                            {
                                case LookOption.LookAtHorizontal://水平方向に向きを変える
                                                                 //現在の方向から指定数値を取得する
                                    target = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + route.getList()[oldindex].GetComponent<HandleData>().getHorizontalAngle(), transform.localEulerAngles.z);
                                    break;
                                case LookOption.LookAtVertical://向けたい方向を設定する
                                                               //現在の方向から指定数値を取得する
                                    target = Quaternion.Euler(transform.localEulerAngles.x + route.getList()[oldindex].GetComponent<HandleData>().getVerticalAngle(), transform.localEulerAngles.y, transform.localEulerAngles.z);
                                    break;
                                case LookOption.FreeChange:
                                    target = Quaternion.Euler(transform.localEulerAngles.x + route.getList()[oldindex].GetComponent<HandleData>().getVerticalAngle(), transform.localEulerAngles.y + route.getList()[oldindex].GetComponent<HandleData>().getHorizontalAngle(), transform.localEulerAngles.z);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }
                        //NULLで配列の最後までループした場合
                        if (route.getList().Count - 1 <= i)
                        {
                            //終了させる
                            index = route.getList().Count;
                        }
                    }
                }
            }

        }
    }

    //向きを変える
    void ChangeLook()
    {
        //目的値から振り向き速度を取得
        float lookspeed = (route.getList()[oldindex].GetComponent<HandleData>().getLookSpeed() / 1000);
        if (lookspeed <= 0)//設定していない場合
        {
            //距離に応じた速度を設定
            lookspeed = 0.1f / CalculationTime(route.getList()[index].transform.position, transform.position);
        }
        //目的地の条件を取得
        switch (route.getList()[oldindex].GetComponent<HandleData>().getLookOption())
        {
            case LookOption.NextTarget://次の目的地に向きを変える
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(route.getList()[index].transform.position - transform.position), lookspeed);
                break;
            case LookOption.LookAtHorizontal://水平方向に向きを変える
                target = Quaternion.Euler(transform.localEulerAngles.x, target.eulerAngles.y, transform.localEulerAngles.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, lookspeed);
                break;
            case LookOption.LookAtVertical://向けたい方向を設定する
            case LookOption.FreeChange://フリーカメラ
                transform.rotation = Quaternion.Slerp(transform.rotation, target, lookspeed);
                break;
            case LookOption.LookAtTarget://ターゲット追従
                //相対ベクトルを求める
                var aim = this.route.getList()[oldindex].GetComponent<HandleData>().getLookTarget().transform.position - this.transform.position;
                //向きを変える
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(aim), lookspeed);
                break;
            case LookOption.DontChange://動かさない
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
        float speed = route.getList()[oldindex].GetComponent<HandleData>().getMoveSpeed();
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
}
