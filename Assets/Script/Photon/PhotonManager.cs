using System;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonManager : Photon.MonoBehaviour
{
    private string StageName;

    public GameObject Stage1CreateButton;
    public GameObject Stage2CreateButton;
    public GameObject Stage3CreateButton;

    public GameObject Stage1JoinButton;
    public GameObject Stage2JoinButton;
    public GameObject Stage3JoinButton;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("SSGv0.3");
    }

    void OnJoinedLobby()
    {
        Debug.Log("Join_Lobby");
        Stage1CreateButton.GetComponent<Button>().interactable = true;
        Stage2CreateButton.GetComponent<Button>().interactable = true;
        Stage3CreateButton.GetComponent<Button>().interactable = true;
    }

    public void CreateRoom(string stagename)
    {
        this.StageName = stagename;
        //ルームの作成設定
        RoomOptions roomOptions = new RoomOptions();
        //ロビーで見える部屋にする
        roomOptions.IsVisible = true;
        //他のプレイヤーの入室を許可する
        roomOptions.IsOpen = true;
        //入室可能最大人数を設定
        roomOptions.MaxPlayers = (byte)4;

        //ルームカスタムプロパティの設定
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            //カスタムプロパティ「ステージID」
            { "StageName",this.StageName }
        };

        roomOptions.CustomRoomPropertiesForLobby = new string[] {
            "StageName"
        };

        Debug.Log("作成しました");
        //部屋作成
        //("部屋の名前",ルームの設定,作成ロビー)
        //部屋の名前を""又はnullにするとサーバー側が自動生成してくれる。
        //作成ロビーも指定しなければデフォルトロビーに作成される。
        PhotonNetwork.CreateRoom(null, roomOptions, null);
        SceneManager.LoadScene(this.StageName);
    }

    void OnReceivedRoomListUpdate()
    {
        //ルーム一覧を取る
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        Stage1JoinButton.GetComponent<Button>().interactable = false;
        Stage2JoinButton.GetComponent<Button>().interactable = false;
        Stage3JoinButton.GetComponent<Button>().interactable = false;
        if (rooms.Length > 0)
        {
            //ルームが1件以上ある時ループでRoomInfo情報をログ出力
            foreach (RoomInfo ri in rooms)
            {
                string name = (string)ri.CustomProperties["StageName"];
                if (name.Equals("Grassland"))
                {
                    Debug.Log("ステージ１に入れます");
                    Stage1JoinButton.GetComponent<Button>().interactable = true;
                }
                if (name.Equals("City"))
                {
                    Debug.Log("ステージ２に入れます");
                    Stage2JoinButton.GetComponent<Button>().interactable = true;
                }
                if (name.Equals("SnowMountain"))
                {
                    Debug.Log("ステージ３に入れます");
                    Stage3JoinButton.GetComponent<Button>().interactable = true;
                }
            }
        }
    }

    //ルームの退室
    public void ExitRoom()
    {
        Debug.Log("Out_Room!");
        PhotonNetwork.LeaveRoom();
    }

    public void OnJoinedRoom()
    {
        Debug.Log("Join_Room!");
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        //現在の部屋を取得
        Room room = PhotonNetwork.room;
        //部屋の入出許可を禁止
        room.IsOpen = false;
        //見えない
        room.IsVisible = false;
    }

    public void JoinButton(string stagename)
    {
        Hashtable expectedCustomRoomProperties = new Hashtable() { { "StageName", stagename } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties,4);
        SceneManager.LoadScene(stagename);
    }
    public void OnCreatedRoom()
    {
        Debug.Log("Room_Create_Success!");
    }

}
