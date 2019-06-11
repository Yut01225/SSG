using TMPro;
using UnityEngine;


public enum EffectType
{
    Default,
    Big,
    Invisible,
    Move,
    Flash,
    MoveBlock
}

public class TargetSystem : MonoBehaviour
{
    private bool IsBig = false;
    private float Power = 20f;
    private Vector3 RotatePower = new Vector3(0, 0, 0);
    private bool IsView;
    private Vector3 NextSize;
    private float Size = 1;

    //的の体力
    public int Hitpoint;
    //破損オブジェクトの指定
    public GameObject little;
    //体力があるときの得点
    public int ClickPoint;
    //破壊した時の得点
    public int BrokenPoint;
    //得点オブジェクト
    public GameObject PointTextObject;
    //生成開始するインデックス
    public int CretateIndex;
    //生成型
    public EffectType type;
    //変化の速度
    public float EffectSpeed;
    //座標位置の追加
    public Vector3 AddPosition;
    //最大の大きさ
    public float LimitSize;
    //開始時間
    public float StartTime;

    public bool IsRotation;

    public FloorType floorType;

    public bool right;
    public bool up;
    public bool front;
    public bool left;
    public bool down;
    public bool back;
    public float CM;

    void Start()
    {
        //色をランダムに変更
        this.GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        //自分の大きさを取得
        Size = this.transform.localScale.x;
        RotatePower = new Vector3(Random.Range(0.1f, 1) * 3, Random.Range(-1, 1) * 1, Random.Range(-1, 1) * 3);
        if (CretateIndex >= 0)
        {
            IsView = false;
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void Update()
    {
        if (MoveStatus.NextIndex == 0 || MoveStatus.NextIndex >= CretateIndex)
        {
            if (StartTime > 0)
            {
                StartTime -= Time.deltaTime * 1f;
            }
            else
            {
                if (!IsView)
                {
                    IsView = true;
                    StartTime = 0;
                    this.gameObject.GetComponent<MeshRenderer>().enabled = true;
                    switch (type)
                    {
                        default:
                        case EffectType.Default:
                            break;
                        case EffectType.Big:
                            this.gameObject.AddComponent<Bigger>().setting(LimitSize, EffectSpeed, StartTime);
                            break;
                        case EffectType.Invisible:
                            this.gameObject.AddComponent<Transparent>().setting(EffectSpeed, StartTime);
                            break;
                        case EffectType.Move:
                            this.gameObject.AddComponent<Moving>().setting(this.AddPosition, this.transform.position, EffectSpeed, StartTime);
                            break;
                        case EffectType.Flash:
                            this.gameObject.AddComponent<Flash>().setting(EffectSpeed, floorType);
                            break;
                        case EffectType.MoveBlock:
                            if (this.GetComponent<Rigidbody>() == null)
                            {
                                this.gameObject.AddComponent<Rigidbody>().isKinematic = true;
                                Debug.Log(transform.name + "のRigidbodyが未設定です");
                            }
                            this.gameObject.AddComponent<MoveBlock>().setting(right, up, front, left, down, back, CM, EffectSpeed);
                            break;
                    }
                }
            }


            if (IsRotation)
            {
                this.transform.Rotate(RotatePower);
            }
            if (IsBig)
            {
                if (NextSize.x < Size)
                {
                    this.transform.localScale = new Vector3(Size, Size, Size);
                    Size -= Time.deltaTime * 2f;
                }
                else
                {
                    this.transform.localScale = NextSize;
                    IsBig = false;
                }
            }
        }


    }

    public void HitMe()
    {
        this.Hitpoint--;
        Division();
        if (this.Hitpoint <= 0)
        {
            TextCreate("+" + BrokenPoint + "P");
            this.gameObject.layer = 0;
            Destroy(this.gameObject);
        }
        else
        {
            if (this.Hitpoint < 99)
            {
                TextCreate("+" + ClickPoint + "P");
                IsBig = true;
                NextSize = 0.9f * transform.localScale;
            }
            else
            {
                this.Hitpoint = 100;
                TextCreate("GUARD");
            }

        }
    }

    public void TextCreate(string text)
    {
        GameObject vp = Instantiate(PointTextObject, this.transform.position + new Vector3(0, 10, 0), Quaternion.identity) as GameObject;
        vp.GetComponent<TextMeshPro>().text = text;
    }

    public int getPoint()
    {
        if (1 < this.Hitpoint)
        {
            return this.ClickPoint;
        }
        else
        {
            return this.BrokenPoint;
        }

    }

    public void Division()
    {
        Vector3[] directions = {
            new Vector3 (1, -1, 1),
            new Vector3 (-1, -1, 1),
            new Vector3 (-1, -1, -1),
            new Vector3 (1, -1, -1),
            new Vector3 (1, 1, 1),
            new Vector3 (-1, 1, 1),
            new Vector3 (-1, 1, -1),
            new Vector3 (1, 1, -1)
        };
        for (int i = 0; i < 8; i++)
        {
            var obj = Instantiate(little) as GameObject;
            var cube = obj.transform;
            if (this.Hitpoint > 0)
            {
                cube.localScale = 0.3f * transform.localScale;
                cube.position = transform.TransformPoint(directions[i] / 4);
            }
            else
            {
                cube.localScale = 0.45f * transform.localScale;
                cube.position = transform.TransformPoint(directions[i] / 4);
            }
            cube.GetComponent<Renderer>().material = this.GetComponent<Renderer>().material;
            cube.GetComponent<Rigidbody>().AddForce(Power * Random.insideUnitSphere, ForceMode.Impulse);
        }
    }
}
