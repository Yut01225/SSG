using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfflineCursorSystem : MonoBehaviour
{
    //カーソルの画像
    private Image OtherCursor;
    //銃の射程
    public float Range = 150f;
    //有効範囲内か保持
    bool isRange;
    //四人の得点を保持(ID:0は存在しない)
    int SUMPoint;
    [SerializeField]
    public GUISkin Skin;

    public int MyID = 1;

    //Joy-Conのボタンリスト
    private static readonly Joycon.Button[] m_buttons = Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    //ジョイコンリスト
    private List<Joycon> m_joycons;
    //左
    private Joycon m_joyconL = null;
    //右
    private Joycon m_joyconR = null;
    //ボタン
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;
    //速度
    private int WideSpeed;
    private int HighSpeed;
    private float RollingSpeed = 2f;
    //射撃判定
    private bool shooting;
    public float ShotWait;
    //使用するJoy
    private Joycon UseJoyCon = null;

    string info;

    // Start is called before the first frame update
    void Start()
    {
        if (MyID <= 0)
        {
            MyID = 1;
        }
        if (MyID > 4)
        {
            MyID = 4;
        }

        //【初期設定】-----------------------
        //キャンバスを親の設定にする
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
        //射程を無効に初期化する
        isRange = false;
        //横速度の設定
        WideSpeed = 60;
        //縦速度の設定
        HighSpeed = 50;
        //カーソル位置を中央に
        resetPosition();

        //マウスカーソルを非表示に
        Cursor.visible = false;
        SetControllers();
    }

    // Update is called once per frame
    void Update()
    {
        //ボタンの初期化
        m_pressedButtonL = null;
        m_pressedButtonR = null;

        SetControllers();
        this.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, -RollingSpeed));

        //カーソル座標を変える
        changeCursorPosition();
        //レイを取得する
        Ray ray = getCursorRay();
        RaycastHit hit;

        //カーソルに的がある場合
        if (Physics.Raycast(ray, out hit, Range, LayerMask.GetMask("Target")))
        {
            //無効範囲状態 かつ 可視状態 かつ 体力がある場合
            if (!isRange && hit.collider.gameObject.GetComponent<MeshRenderer>().enabled && hit.collider.gameObject.GetComponent<TargetSystem>().Hitpoint > 0)
            {
                //カーソルの色を赤色に変更
                this.GetComponent<Image>().color = new Color(1, 0, 0);
                //有効範囲内状態にする
                isRange = true;
            }
        }
        else//カーソルに的が無い場合
        {
            //有効範囲内状態の場合
            if (isRange)
            {
                //自分の色に戻す
                this.GetComponent<Image>().color = getMyColor(MyID);
                //無効範囲状態にする
                isRange = false;
            }
        }

        if (isJoycon(UseJoyCon))//Joyconが存在する
        {
            //[Y]ボタンを押
            if (UseJoyCon.GetButtonDown(Joycon.Button.DPAD_LEFT))
            {
                resetPosition();
            }
            //[B]ボタンを押
            if (UseJoyCon.GetButtonDown(Joycon.Button.DPAD_DOWN))
            {
                HighSpeed = 150;
                WideSpeed = 180;
                RollingSpeed = 20f;
            }
            //[B]ボタンを上げたとき
            if (UseJoyCon.GetButtonUp(Joycon.Button.DPAD_DOWN))
            {
                HighSpeed = 50;
                WideSpeed = 60;
                RollingSpeed = 2f;
            }
            //[ZL]ボタンを押
            if (isRange && UseJoyCon.GetButtonDown(Joycon.Button.SHOULDER_2))
            {
                //撃つ
                Shot(hit);
                //振動
                //UseJoyCon.SetRumble(20, 100, 0.6f, 100);
            }
        }
        else//マウス
        {
            //マウスの左クリックをした かつ 有効範囲内状態の場合
            if (isRange && Input.GetButtonDown("Fire1"))
            {
                //撃つ
                Shot(hit);
            }
        }

    }

    //撃つ
    void Shot(RaycastHit hit)
    {
        //自分の場所に得点を追加
        SUMPoint += hit.collider.gameObject.GetComponent<TargetSystem>().getPoint();
        //当たった場合の動作を開始する
        if (hit.collider.gameObject.GetComponent<TargetSystem>().HitMe())
        {
            //UseJoyCon.SetRumble(160, 320, 0.6f, 200);
        }
    }

    /// <summary>
    /// 座標を画面中央に変更する。
    /// </summary>
    void resetPosition()
    {
        this.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    }

    /// <summary>
    /// IDにより色を取得する
    /// </summary>
    /// <param name="id">PhotonNetworkのプレイヤーID</param>
    /// <returns>プレイヤーに応じたColorクラス</returns>
    Color getMyColor(int id)
    {
        switch (id)
        {
            default:
            case 1://白
                return new Color(1, 1, 1);
            case 2://青
                return new Color(0, 0, 1);
            case 3://緑
                return new Color(0, 1, 0);
            case 4://黄色
                return new Color(1, 1, 0);
        }
    }

    private void SetControllers()
    {
        //Joy-conをリスト化
        m_joycons = JoyconManager.Instance.j;
        //設定されてない場合処理終了
        //if (m_joycons == null || m_joycons.Count <= 0) return;
        //LとRを取得する
        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);
        //左を優先にjoy-conを設定
        if (m_joyconL == null)
        {
            Debug.Log("Lなし");
            if (m_joyconR != null)
            {
                UseJoyCon = m_joyconR;
                info = "Joy-con 【R】";
            }
        }
        if (m_joyconR == null)
        {
            Debug.Log("Rなし");
            if (m_joyconL != null)
            {
                UseJoyCon = m_joyconL;
                info = "Joy-con 【L】";
            }
        }
        if (!isJoycon(UseJoyCon))
        {
            info = "Mouse Position";
        }
    }

    /// <summary>
    /// Joy-conのジャイロを計算した座標を取得します
    /// </summary>
    /// <param name="joycon">使用しているJoy-con</param>
    /// <returns>ジャイロの入力座標</returns>
    private Vector3 getJoyconPosition(Joycon joycon)
    {
        return new Vector3(joycon.GetGyro()[2] * WideSpeed, joycon.GetGyro()[1] * HighSpeed, 0); ;
    }

    /// <summary>
    /// Joy-conが存在するか判定する
    /// </summary>
    /// <param name="joycon">判定するJoy-con</param>
    /// <returns>存在の結果</returns>
    private bool isJoycon(Joycon joycon)
    {
        if (joycon == null)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// カーソルの座標を変更します
    /// </summary>
    private void changeCursorPosition()
    {
        if (isJoycon(UseJoyCon))
        {
            this.GetComponent<RectTransform>().position += getJoyconPosition(UseJoyCon);
        }
        else
        {
            //マウスの座標を取得する
            this.GetComponent<RectTransform>().position = Input.mousePosition;

        }
    }

    /// <summary>
    /// カーソル座標からレーザーを出力します
    /// </summary>
    /// <returns>出力したレーザーを返却します</returns>
    private Ray getCursorRay()
    {
        if (UseJoyCon == null)
        {
            //判定用のレーザーを出力する
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            return Camera.main.ScreenPointToRay(this.GetComponent<RectTransform>().position);
        }
    }

    private void OnGUI()
    {
        //プレイヤーが使用出来る大きさ(1366 /4 )
        float pw = 341;
        switch (MyID)
        {
            case 1:
                Skin.GetStyle("textfield").normal.textColor = new Color(1, 1, 1);
                break;
            case 2:
                Skin.GetStyle("textfield").normal.textColor = new Color(0, 0, 1);
                break;
            case 3:
                Skin.GetStyle("textfield").normal.textColor = new Color(0, 1, 0);
                break;
            case 4:
                Skin.GetStyle("textfield").normal.textColor = new Color(1, 1, 0);
                break;
        }
        //ウィンドウの作成
        GUI.Label(new Rect(5, 5, pw - 15, 55), "", Skin.GetStyle("window"));
        //プレイヤー情報
        GUI.Label(new Rect(5, 5, pw - 15, 20), MyID + "P_SCORE", Skin.GetStyle("textfield"));
        //得点情報
        GUI.Label(new Rect(10, 15, pw - 20, 50), string.Format("{0:D9}", SUMPoint), Skin.GetStyle("label"));
        //ウィンドウの作成
        GUI.Label(new Rect(5 + pw, 5, pw - 15, 55), "", Skin.GetStyle("window"));
        //プレイヤー情報
        GUI.Label(new Rect(5 + pw, 5, pw - 15, 20), "Controller-Type", Skin.GetStyle("textfield"));
        //得点情報
        GUI.Label(new Rect(10 + pw, 15, pw - 20, 50), info, Skin.GetStyle("label"));
    }
}
