using UnityEngine;
using System.Collections;
using UnityEditor;


namespace RandomObj
{
    public class SetGameObject : ScriptableWizard
    {

        [SerializeField] private Terrain terrain;           //　テレイン
        [SerializeField] private string parentName;         //　親にするゲームオブジェクトの名前
        [SerializeField] private GameObject obj;            //　設置するゲームオブジェクト
        [SerializeField] private bool doubleCheckMode;          //　重なりチェックを厳しくするか簡易にするか
        [SerializeField] private bool randomMode;           //　シードを変更したランダムを生成するか？
        [SerializeField] private string fieldLayerName = "Field";       //　地面のTerrainに設定したレイヤー名
        [SerializeField] private string objLayerName = "Obj";       //　設置するゲームオブジェクトに設定したレイヤー名
        [SerializeField] private int count = 1;             //　設置する数
        [SerializeField] private bool slope;                //　地形の斜めに合わせるかどうか
        [SerializeField] private float offset = 0.0f;       //　ベースが下にない時用オフセット値上に移動させる
        [SerializeField] private bool useScale;             //　木のスケールをランダムに変更するかどうか
        [SerializeField] private float minScale = 0.1f; //　木の最小スケール
        [SerializeField] private float maxScale = 1.0f; //　木の最大スケール

        private GameObject ins;

        [MenuItem("MyMenu/SetGameObject")]
        static void CreateWizard()
        {
            //　ウィザードを表示
            ScriptableWizard.DisplayWizard<SetGameObject>("自動ゲームオブジェクト設置ツール", "ゲームオブジェクトを設置", "データの初期化");
        }
        //　ウィザードの他のボタンを押した時に実行
        void OnWizardOtherButton()
        {
            terrain = null;
            obj = null;
            doubleCheckMode = false;
            count = 1;
            slope = false;
            offset = 0.0f;
            useScale = false;
            minScale = 0.1f;
            maxScale = 1.0f;
            parentName = "";
            fieldLayerName = "Field";
            objLayerName = "Obj";
        }
        //　ウィザードで更新があった時に実行
        void OnWizardUpdate()
        {

            //　設置する木の制限を加える
            if (count > 10000)
            {
                count = 10000;
            }
            else if (count <= 0)
            {
                count = 1;
            }
            //　スケールが0以下に設定されていたら0.1fにする
            if (minScale <= 0)
            {
                minScale = 0.1f;
            }
        }
        //　ウィザードの作成ボタンを押した時に実行
        void OnWizardCreate()
        {

            //　テレインがセットされていなければエラー
            if (terrain == null)
            {
                Debug.LogError("not set terrain", this);
                return;
                //　オブジェクトがセットされていなければエラー
            }
            else if (obj == null)
            {
                Debug.LogError("not set obj", this);
                return;
            }
            else if (count <= 0)
            {
                Debug.LogError("A numerical value less than or equal to 0 is set");
                return;
            }
            else if (fieldLayerName == "")
            {
                Debug.LogError("not set fieldLayerName");
            }

            //　テレインデータを確保
            Vector3 tPos = terrain.GetPosition();
            TerrainData tData = terrain.terrainData;

            //　親の作成
            GameObject parentObj = new GameObject();

            if (parentName == "")
            {
                parentName = "objParent";
            }

            parentObj.name = parentName;

            //　親要素のUndo登録をして親を消す事で子も全部消えるようにしておく
            Undo.RegisterCreatedObjectUndo(parentObj, "Create " + parentObj.name);

            // シードを指定してランダムクラスを作成
            System.Random rand = new System.Random(Time.time.ToString().GetHashCode());

            for (var i = 0; i < count; i++)
            {

                //　X軸の位置
                float x;
                //　Z軸の位置
                float z;
                //　角度
                Quaternion rot;

                //　Terrainの大きさからランダム値を計算 
                if (randomMode)
                {
                    x = (float)(tPos.x + rand.NextDouble() * (tPos.x + tData.size.x));
                    z = (float)(tPos.z + rand.NextDouble() * (tPos.z + tData.size.z));
                    //　Y軸をランダムに回転
                    rot = Quaternion.Euler(0, (float)(rand.NextDouble() * 360), 0);
                }
                else
                {
                    x = Random.Range(tPos.x, tPos.x + tData.size.x);
                    z = Random.Range(tPos.z, tPos.z + tData.size.z);
                    //　Y軸をランダムに回転
                    rot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                }

                //　ランダムな位置を作成
                Vector3 pos = new Vector3(x, tPos.y + tData.size.y, z);

                int rePosCount = 0;
                bool RePos = false;

                if (!doubleCheckMode)
                {

                    //　自身のレイヤー名が設定されていて他の木にぶつかった場合は位置を再設定

                    if (objLayerName != "")
                    {
                        while (true)
                        {
                            if (Physics.Raycast(pos, Vector3.down, tPos.y + tData.size.y + 100f, LayerMask.GetMask(objLayerName)))
                            {
                                if (randomMode)
                                {
                                    x = (float)(tPos.x + rand.NextDouble() * (tPos.x + tData.size.x));
                                    z = (float)(tPos.z + rand.NextDouble() * (tPos.z + tData.size.z));
                                }
                                else
                                {
                                    x = Random.Range(tPos.x, tPos.x + tData.size.x);
                                    z = Random.Range(tPos.z, tPos.z + tData.size.z);
                                }
                                pos = new Vector3(x, tPos.y + tData.size.y, z);
                                Debug.Log("他の木にぶつかった");
                            }
                            else
                            {
                                break;
                            }
                            rePosCount++;

                            //　3回位置を直したらもう直さない
                            if (rePosCount > 3)
                            {
                                RePos = true;
                                break;
                            }
                        }
                    }
                }

                RaycastHit hit;

                //　地面の位置とゲームオブジェクトの位置を合わせる
                if (!RePos && Physics.Raycast(pos, Vector3.down, out hit, tPos.y + tData.size.y + 100f, LayerMask.GetMask(fieldLayerName)))
                {
                    //　高さを地面に合わせる
                    pos.y = hit.point.y;
                    //　木のインスタンスの作成
                    ins = GameObject.Instantiate(obj, pos, rot) as GameObject;

                    //　地面の傾斜に合わせて木を傾ける設定の場合は木を回転させる
                    if (slope)
                    {
                        ins.transform.rotation = Quaternion.FromToRotation(ins.transform.up, hit.normal) * ins.transform.rotation;
                    }
                    //　ランダムに木のスケールを変える場合
                    if (useScale)
                    {
                        float randomScale = 1f;
                        if (randomMode)
                        {
                            randomScale = (float)(minScale + rand.NextDouble() * maxScale);
                        }
                        else
                        {
                            randomScale = Random.Range(minScale, maxScale);
                        }
                        ins.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                        ins.transform.position += (ins.transform.up * offset) * randomScale;
                    }

                    //　ある程度重なり合っている場合は削除（指定した設置数より大幅に減る）
                    if (doubleCheckMode)
                    {
                        if (objLayerName != "")
                        {

                            Collider[] hitCol = Physics.OverlapSphere(ins.transform.position, maxScale / 2f, LayerMask.GetMask(objLayerName));

                            //　自身以外の同じレイヤー名のものにヒットしたら残念ながらインスタンスを削除する
                            if (hitCol != null)
                            {
                                bool check = false;
                                foreach (var col in hitCol)
                                {
                                    if (col.gameObject.GetInstanceID() != ins.gameObject.GetInstanceID())
                                    {
                                        check = true;
                                        Debug.Log(col.name);
                                    }
                                }
                                if (check)
                                {
                                    DestroyImmediate(ins.gameObject);
                                }
                            }
                        }
                    }
                    if (ins != null)
                    {
                        ins.transform.SetParent(parentObj.transform);
                    }
                }
            }

            Debug.Log(parentObj.transform.childCount);
        }
    }
}