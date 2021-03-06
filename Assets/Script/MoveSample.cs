﻿using System.Collections;
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

    //待機用カウントダウンタイマー
    private float Timer = 0;
    float t = 0;

    //でこぼこを有効
    public bool IsUneven;
    public float CorrectionHeight;

    public int index;

    void Start()
    {
        MoveStatus.NextIndex = index;
        //存在するまで繰り返す
        for (int i = MoveStatus.NextIndex; i < route.getList().Count; i++)
        {
            //リストが存在するか判定
            if (route.getList()[i])
            {
                //リストの一番最初を開始位置にする
                MoveStatus.OldIndex = i;
                //最初のポイントに移動する
                this.transform.position = route.getList()[MoveStatus.OldIndex].transform.position;
                break;
            }
        }
        ChengeUneven();
        //存在するまで繰り返す
        for (int i = MoveStatus.OldIndex + 1; i < route.getList().Count; i++)
        {
            //リストが存在するか判定
            if (route.getList()[i])
            {
                //次の目的地に設定する
                MoveStatus.NextIndex = i;
                if (!IsUneven)
                {
                    //瞬時に次の目的地に方向を変える
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(route.getList()[MoveStatus.NextIndex].transform.position - transform.position), 1f);
                }

                break;
            }
        }
        //目的地までの差分を計算する
        CalculationDiff(route.getList()[MoveStatus.NextIndex].transform.position, this.transform.position);
        //初期のタイマーを取得する
        this.Timer = route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().getStopTime();


    }

    void Update()
    {
        //タイマーがセットされている
        if (Timer > 0)
        {
            //カウントダウンを行う
            Timer -= Time.deltaTime * 1f;
            //停止中方向転換が有効か判定
            if (route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().getIsStoppingAngleChange())
            {
                //向きを変える
                ChangeLook();
            }
        }
        else
        {
            //配列が存在するか
            if (MoveStatus.NextIndex < route.getList().Count)
            {

                //向きを変える
                ChangeLook();

                if (route.getList()[MoveStatus.NextIndex - 1].GetComponent<HandleData>().BezierFlag)
                {
                    float speed = route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().getMoveSpeed();
                    if (speed <= 0)
                    {
                        speed = DefaultMoveSpeed;
                    }
                    t += Time.deltaTime * (speed / 200);


                    Vector3 targetposition = GetPoint(route.getList()[MoveStatus.NextIndex - 1].transform.localPosition, route.getList()[MoveStatus.NextIndex].transform.localPosition, route.getList()[MoveStatus.NextIndex + 1].transform.localPosition, t);
                    if (route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().UnevenFlag)
                    {
                        if (IsUneven && Terrain.activeTerrain)
                        {
                            float terrainHeight = Terrain.activeTerrain.terrainData.GetInterpolatedHeight(this.transform.position.x / Terrain.activeTerrain.terrainData.size.x, this.transform.position.z / Terrain.activeTerrain.terrainData.size.z);
                            targetposition = new Vector3(targetposition.x, terrainHeight + CorrectionHeight, targetposition.z);
                        }
                    }
                    transform.position = targetposition;
                }
                else
                {
                    //各座標を更新
                    X_PositionChange();
                    Y_PositionChange();
                    Z_PositionChange();
                }


                //目標の座標に到達しているか
                if (CheckPositions())
                {
                    //目標を次に変更
                    changeNextTarget();
                }
            }
        }
    }

    Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        var a = Vector3.Lerp(p0, p1, t); // 緑色の点1
        var b = Vector3.Lerp(p1, p2, t); // 緑色の点2

        return Vector3.Lerp(a, b, t);   // 青色の点1
    }

    /// <summary>
    /// 自分自身を、目的地のX座標に近づける。
    /// </summary>
    void X_PositionChange()
    {
        //目標が大きい場合
        if (route.getList()[MoveStatus.NextIndex].transform.position.x < this.transform.position.x)
        {
            this.transform.position -= new Vector3(diffX * Time.deltaTime, 0, 0);
            //加算後に目標を超えた場合
            if (route.getList()[MoveStatus.NextIndex].transform.position.x > this.transform.position.x)
            {
                this.transform.position = new Vector3(route.getList()[MoveStatus.NextIndex].transform.position.x, this.transform.position.y, this.transform.position.z);
            }
        }
        //目標が小さい場合
        if (route.getList()[MoveStatus.NextIndex].transform.position.x > this.transform.position.x)
        {
            this.transform.position += new Vector3(diffX * Time.deltaTime, 0, 0);
            //減算後に目標を超えた場合
            if (route.getList()[MoveStatus.NextIndex].transform.position.x < this.transform.position.x)
            {
                this.transform.position = new Vector3(route.getList()[MoveStatus.NextIndex].transform.position.x, this.transform.position.y, this.transform.position.z);
            }
        }
    }
    /// <summary>
    /// 自分自身を、目的地のY座標に近づける。
    /// </summary>
    void Y_PositionChange()
    {
        if (IsUneven && Terrain.activeTerrain)
        {
            float terrainHeight = Terrain.activeTerrain.terrainData.GetInterpolatedHeight(this.transform.position.x / Terrain.activeTerrain.terrainData.size.x, this.transform.position.z / Terrain.activeTerrain.terrainData.size.z);
            this.transform.position = new Vector3(this.transform.position.x, terrainHeight + CorrectionHeight, this.transform.position.z);
        }
        else
        {
            //目標が大きい場合
            if (route.getList()[MoveStatus.NextIndex].transform.position.y < this.transform.position.y)
            {
                this.transform.position -= new Vector3(0, diffY * Time.deltaTime, 0);
                //加算後に目標を超えた場合
                if (route.getList()[MoveStatus.NextIndex].transform.position.y > this.transform.position.y)
                {
                    this.transform.position = new Vector3(this.transform.position.x, route.getList()[MoveStatus.NextIndex].transform.position.y, this.transform.position.z);
                }
            }
            //目標が小さい場合
            if (route.getList()[MoveStatus.NextIndex].transform.position.y > this.transform.position.y)
            {
                this.transform.position += new Vector3(0, diffY * Time.deltaTime, 0);
                //減算後に目標を超えた場合
                if (route.getList()[MoveStatus.NextIndex].transform.position.y < this.transform.position.y)
                {
                    this.transform.position = new Vector3(this.transform.position.x, route.getList()[MoveStatus.NextIndex].transform.position.y, this.transform.position.z);
                }
            }
        }

    }
    /// <summary>
    /// 自分自身を、目的地のZ座標に近づける。
    /// </summary>
    void Z_PositionChange()
    {
        //目標が大きい場合
        if (route.getList()[MoveStatus.NextIndex].transform.position.z < this.transform.position.z)
        {
            this.transform.position -= new Vector3(0, 0, diffZ * Time.deltaTime);
            //加算後に目標を超えた場合
            if (route.getList()[MoveStatus.NextIndex].transform.position.z > this.transform.position.z)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, route.getList()[MoveStatus.NextIndex].transform.position.z);
            }
        }
        //目標が小さい場合
        if (route.getList()[MoveStatus.NextIndex].transform.position.z > this.transform.position.z)
        {
            this.transform.position += new Vector3(0, 0, diffZ * Time.deltaTime);
            //減算後に目標を超えた場合
            if (route.getList()[MoveStatus.NextIndex].transform.position.z < this.transform.position.z)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, route.getList()[MoveStatus.NextIndex].transform.position.z);
            }
        }
    }

    void ChengeUneven()
    {
        if (Terrain.activeTerrain)
        {
            if (this.IsUneven != route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().UnevenFlag)
            {
                this.IsUneven = route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().UnevenFlag;
            }
            if (this.CorrectionHeight != route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().UnevenHeight)
            {
                this.CorrectionHeight = route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().UnevenHeight;
            }
        }

    }

    /// <summary>
    /// 目標地点に到達しているか判定する。
    /// </summary>
    /// <returns>到達している場合True</returns>
    bool CheckPositions()
    {
        if (route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().BezierFlag)
        {
            //座標のX.Y座標が一致した
            if (route.getList()[MoveStatus.NextIndex + 1].transform.position.x == this.transform.position.x && route.getList()[MoveStatus.NextIndex + 1].transform.position.z == this.transform.position.z)
            {
                if (route.getList()[MoveStatus.NextIndex + 1].transform.position.y == this.transform.position.y || IsUneven)
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            //座標のX.Y座標が一致した
            if (route.getList()[MoveStatus.NextIndex].transform.position.x == this.transform.position.x && route.getList()[MoveStatus.NextIndex].transform.position.z == this.transform.position.z)
            {
                if (route.getList()[MoveStatus.NextIndex].transform.position.y == this.transform.position.y || IsUneven)
                {
                    return true;
                }
            }
            return false;
        }

    }
    /// <summary>
    /// 次の目的地に座標を変更する
    /// </summary>
    void changeNextTarget()
    {
        if (route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().BezierFlag)
        {
            //現在地を保存する
            MoveStatus.OldIndex = MoveStatus.NextIndex + 1;
            MoveStatus.NextIndex++;
            t = 0;
        }
        else
        {
            //現在地を保存する
            MoveStatus.OldIndex = MoveStatus.NextIndex;
        }
        Debug.Log(MoveStatus.OldIndex);
        //リストの最後でない場合
        if (++MoveStatus.NextIndex < route.getList().Count)
        {
            //存在するまで繰り返す
            for (int i = MoveStatus.NextIndex; i < route.getList().Count; i++)
            {
                //リストが存在するか判定
                if (route.getList()[i])
                {
                    //存在しているので次の目的地をその場所まで飛ばす
                    MoveStatus.NextIndex = i;
                    //目的地までの差分を計算する
                    CalculationDiff(route.getList()[MoveStatus.NextIndex].transform.position, this.transform.position);
                    //タイマーをセットする
                    this.Timer = route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().getStopTime();
                    //目的地までのカメラの設定
                    switch (route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().getLookOption())
                    {
                        case LookOption.LookAtHorizontal://水平方向
                            target = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().getHorizontalAngle(), transform.localEulerAngles.z);
                            break;
                        case LookOption.LookAtVertical://垂直方向
                            target = Quaternion.Euler(transform.localEulerAngles.x + route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().getVerticalAngle(), transform.localEulerAngles.y, transform.localEulerAngles.z);
                            break;
                        case LookOption.FreeChange://水平・垂直方向
                            target = Quaternion.Euler(transform.localEulerAngles.x + route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().getVerticalAngle(), transform.localEulerAngles.y + route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().getHorizontalAngle(), transform.localEulerAngles.z);
                            break;
                        default://その他の設定
                            break;
                    }
                    ChengeUneven();
                    break;


                }
                //NULLで配列の最後までループした場合
                if (route.getList().Count - 1 <= i)
                {
                    //終了させる
                    MoveStatus.NextIndex = route.getList().Count;
                }
            }
        }
        Destroy(route.getList()[MoveStatus.OldIndex - 1].gameObject);
    }
    /// <summary>
    /// 向く方向を設定する
    /// </summary>
    void ChangeLook()
    {
        //目的値から振り向き速度を取得
        float lookspeed = (route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().getLookSpeed() / 1000);
        if (lookspeed <= 0)//設定していない場合
        {
            //距離に応じた速度を設定
            lookspeed = 0.1f / CalculationTime(route.getList()[MoveStatus.NextIndex].transform.position, transform.position);
        }
        //目的地の条件を取得
        switch (route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().getLookOption())
        {
            case LookOption.NextTarget://次の目的地に向きを変える
                if (Terrain.activeTerrain)
                {
                    if (IsUneven)
                    {
                        if (route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().BezierFlag)
                        {
                            Vector3 targetLookRotation = (transform.position + route.getList()[MoveStatus.NextIndex + 1].transform.position) - transform.position - transform.position;
                            targetLookRotation.y = 0f;
                            if (targetLookRotation != Vector3.zero)
                            {
                                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetLookRotation), lookspeed);
                            }
                        }
                        else
                        {
                            Vector3 UnevenPosition = route.getList()[MoveStatus.NextIndex].transform.position - transform.position;
                            UnevenPosition.y = 0;
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(UnevenPosition), lookspeed);
                        }
                    }
                    else
                    {
                        if (route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().BezierFlag)
                        {
                            Vector3 targetLookRotation = (transform.position + route.getList()[MoveStatus.NextIndex + 1].transform.position) - transform.position - transform.position;
                            if (targetLookRotation != Vector3.zero)
                            {
                                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetLookRotation), lookspeed);
                            }
                        }
                        else
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(route.getList()[MoveStatus.NextIndex].transform.position - transform.position), lookspeed);
                        }
                    }
                }
                else
                {
                    if (route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().BezierFlag)
                    {
                        Vector3 targetLookRotation = (transform.position + route.getList()[MoveStatus.NextIndex + 1].transform.position) - transform.position - transform.position;
                        if (targetLookRotation != Vector3.zero)
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetLookRotation), lookspeed);
                        }
                    }
                    else
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(route.getList()[MoveStatus.NextIndex].transform.position - transform.position), lookspeed);
                    }
                }
                break;
            case LookOption.LookAtHorizontal://水平方向に向きを変える
                target = Quaternion.Euler(transform.localEulerAngles.x, target.eulerAngles.y, transform.localEulerAngles.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, lookspeed);
                break;
            case LookOption.LookAtVertical://垂直方向に向きを変える
            case LookOption.FreeChange://水平・垂直方向を変更
                transform.rotation = Quaternion.Slerp(transform.rotation, target, lookspeed);
                break;
            case LookOption.LookAtTarget://ターゲット追従
                if (this.route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().getLookTarget())
                {
                    //相対ベクトルを求める
                    var aim = this.route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().getLookTarget().transform.position - this.transform.position;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(aim), lookspeed);
                    break;
                }
                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(route.getList()[MoveStatus.NextIndex].transform.position - transform.position), lookspeed);
                }
                break;
            case LookOption.DontChange://動かさない
                break;
        }
    }

    /// <summary>
    /// 二点間の距離の差を、絶対値で表示する
    /// </summary>
    /// <param name="StartPoint">開始点</param>
    /// <param name="EndPoint">終了点</param>
    /// <returns>二点間の距離</returns>
    float Diff(float StartPoint, float EndPoint)
    {
        if (StartPoint < EndPoint)
        {
            return EndPoint - StartPoint;
        }
        else
        {
            return StartPoint - EndPoint;
        }
    }

    /// <summary>
    /// 移動にひつような距離を求める
    /// </summary>
    /// <param name="StartVector">開始場所の座標</param>
    /// <param name="EndVector">終了場所の座標</param>
    void CalculationDiff(Vector3 StartVector, Vector3 EndVector)
    {
        float time = CalculationTime(StartVector, EndVector);
        //かかる時間から移動すべき距離を求める
        diffX = Diff(StartVector.x, EndVector.x) / time;
        diffY = Diff(StartVector.y, EndVector.y) / time;
        diffZ = Diff(StartVector.z, EndVector.z) / time;
    }

    /// <summary>
    /// 三座標の中から最長距離の移動にかかる時間を求める
    /// </summary>
    /// <param name="StartVector">開始場所の座標</param>
    /// <param name="EndVector">終了場所の座標</param>
    /// <returns>最長距離の移動にかかる時間</returns>
    float CalculationTime(Vector3 StartVector, Vector3 EndVector)
    {
        //移動速度を目標ポイントから取得する
        float speed = route.getList()[MoveStatus.OldIndex].GetComponent<HandleData>().getMoveSpeed();
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
