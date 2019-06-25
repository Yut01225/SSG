using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherCursorSystem : Photon.MonoBehaviour
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

    //オブジェクトが自分のものか
    private PhotonView MyView;
    private int MyID = 0;
    //色を保持
    private Color MyColor;

    //Joy-Con
    private static readonly Joycon.Button[] m_buttons = Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;

    private GameObject cur;

    void Start()
    {
        //射程を無効に初期化する
        isRange = false;
        //オブジェクトの画面情報を取得する
        MyView = GetComponent<PhotonView>();
        //キャンバスを親の設定にする
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
        //自分以外のカーソル色を変更
        if (MyView.isMine)
        {
            MyID = PhotonNetwork.player.ID;
            MyView.RPC("SendID", PhotonTargets.Others, MyID);
            //通常のマウスカーソルを非表示にする
            Cursor.visible = false;
            //自分より先のプレイヤー側の色を変更する
            MyView.RPC("ChangeCursor", PhotonTargets.Others, PhotonNetwork.player.ID);
            //入室順にカーソルの色を決定する
            MyColor = getMyColor(PhotonNetwork.player.ID);
            //カーソル色を変更
            this.GetComponent<Image>().color = MyColor;

            //Joy-Con
            m_joycons = JoyconManager.Instance.j;

            if (m_joycons == null || m_joycons.Count <= 0) return;

            m_joyconL = m_joycons.Find(c => c.isLeft);
            m_joyconR = m_joycons.Find(c => !c.isLeft);


        }
    }

    //入室を検知
    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        //自分のカーソルの場合
        if (MyView.isMine)
        {
            //自分の色を変更するように命令する
            MyView.RPC("ChangeCursor", PhotonTargets.Others, PhotonNetwork.player.ID);
            MyView.RPC("SendID", PhotonTargets.Others, MyID);
            MyView.RPC("SendPoint", PhotonTargets.Others, SUMPoint);
        }
    }

    [PunRPC]
    public void SendID(int id)
    {
        MyID = id;
    }

    [PunRPC]
    public void SendPoint(int point)
    {
        SUMPoint = point;
    }
    void Update()
    {
        //Joy-Con
        m_pressedButtonL = null;
        m_pressedButtonR = null;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        foreach (var button in m_buttons)
        {
            if (m_joyconL.GetButton(button))
            {
                m_pressedButtonL = button;
            }

            if (m_joyconR.GetButton(button))
            {
                m_pressedButtonR = button;
            }
        }

        const float MOVE_PER_CLOCK = 0.01f;
        Vector3 joyconGyro;

        // Joy-Conのジャイロ値を取得
        joyconGyro = m_joyconR.GetGyro();
        Quaternion sheepQuaternion = cur.transform.rotation;
        // Joy-Conを横持ちした時にうまいこと向きが変わるようになっています。
        sheepQuaternion.x += -joyconGyro[0] * MOVE_PER_CLOCK;
        sheepQuaternion.y -= -joyconGyro[2] * MOVE_PER_CLOCK;
        sheepQuaternion.z -= -joyconGyro[1] * MOVE_PER_CLOCK;
        cur.transform.rotation = sheepQuaternion;

        // 右Joy-ConのBボタンを押すと羊の向きがリセットされます。
        if (m_joyconR.GetButtonDown(m_buttons[0]))
        {
            cur.transform.rotation = new Quaternion(0, 0, 0, 0);
        }


        //自分のカーソルの場合
        if (MyView.isMine)
        {
            //マウスの座標を取得する
            this.GetComponent<RectTransform>().position = Input.mousePosition;
            //判定用のレーザーを出力する
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //的にカーソルがある場合
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
                //マウスの左クリックをした かつ 有効範囲内状態の場合
                if (Input.GetButtonDown("Fire1") && isRange)
                {
                    //撃つ
                    Shot(hit);
                }
            }
            else//的にカーソルが無い場合
            {
                //有効範囲内状態の場合
                if (isRange)
                {
                    //自分の色に戻す
                    this.GetComponent<Image>().color = MyColor;
                    //無効範囲状態にする
                    isRange = false;
                }
            }
        }
    }

    //撃つ
    void Shot(RaycastHit hit)
    {
        //自分の場所に得点を追加
        SUMPoint += hit.collider.gameObject.GetComponent<TargetSystem>().getPoint();
        //当たった場合の動作を開始する
        hit.collider.gameObject.GetComponent<TargetSystem>().HitMe();
        MyView.RPC("SendPoint", PhotonTargets.Others, SUMPoint);
    }

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

    //カーソルの色を変える
    [PunRPC]
    public void ChangeCursor(int id)
    {
        switch (id)
        {
            case 1://白(半透明)
                this.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                break;
            case 2://青(半透明)
                this.GetComponent<Image>().color = new Color(0, 0, 1, 0.5f);
                break;
            case 3://緑(半透明)
                this.GetComponent<Image>().color = new Color(0, 1, 0, 0.5f);
                break;
            case 4://黄色(半透明)
                this.GetComponent<Image>().color = new Color(1, 1, 0, 0.5f);
                break;
        }
    }


    private void OnGUI()
    {
        //プレイヤーが使用出来る大きさ(1366 /4 )
        float pw = 341;
        switch (MyID)
        {
            case 1:
                Skin.GetStyle("textfield").normal.textColor = new Color(1,1,1);
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
        GUI.Label(new Rect(5 + pw * (MyID - 1), 5, pw - 15, 55), "", Skin.GetStyle("window"));
        //プレイヤー情報
        GUI.Label(new Rect(5 + pw * (MyID - 1), 5, pw - 15, 20), MyID + "P_SCORE", Skin.GetStyle("textfield"));
        //得点情報
        GUI.Label(new Rect(10 + pw * (MyID - 1), 15, pw - 20, 50), string.Format("{0:D10}", SUMPoint), Skin.GetStyle("label"));
    }

}
