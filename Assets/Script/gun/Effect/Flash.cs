using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//　透明フロアのタイプ
public enum FloorType
{
    alwaysTra,                  //　通れない壁
    sometimesTraAndCollider,    //　透明時には接触も出来ない
    sometimesTra,               //　透明時に接触は出来る
};

//透明処理
public class Flash : MonoBehaviour
{
    //　透明フロアのタイプ
    [SerializeField]
    private FloorType Type;
    /*　透明になる間隔時間
    [SerializeField]
    private float transparentTime = 5f;
    */
    //　床の状態が変化してからの時間
    public float nowTime = 0f;
    //　見た目表示コンポーネント
    private MeshRenderer mesh;
    //　床のコライダ
    private Collider col;

    private float TransparentSpeed;

    public void setting(float effectspeed, FloorType floorType)
    {
        if (effectspeed <= 0)
        {
            TransparentSpeed = 1f;
        }
        else
        {
            TransparentSpeed = effectspeed;
        }

         Type = floorType;
    }


    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
        //　透明フロアのタイプが常に透明であれば表示コンポーネントを無効化
        if (Type == FloorType.alwaysTra)
        {
            mesh.enabled = false;
        }
    }

    void Update()
    {

        //　時間計測
        nowTime += Time.deltaTime;

        //　フロアタイプが透明関連
        if (Type == FloorType.sometimesTraAndCollider || Type == FloorType.sometimesTra)
        {
            //　床が透明になる間隔時間を超えたら

            if (nowTime >= TransparentSpeed)
            {
                //　表示コンポーネントを反転
                mesh.enabled = !mesh.enabled;
                //　透明時に接触も出来ないタイプの場合はコライダも反転
                if (Type == FloorType.sometimesTraAndCollider)
                {
                    col.enabled = !col.enabled;
                }
                nowTime = 0f;
            }
        }
    }
}
