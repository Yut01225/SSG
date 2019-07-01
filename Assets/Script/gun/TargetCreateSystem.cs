using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCreateSystem : Photon.MonoBehaviour
{
    GameObject[] TargetList;

    private void Start()
    {
        StartCoroutine("GameStart");
    }

    IEnumerator GameStart()
    {
        //ネットワーク安定のため、時間を置く
        yield return new WaitForSeconds(1f);
        CreateTarget();
    }

    void CreateTarget()
    {
        //ターゲットを配列化
        TargetList = GameObject.FindGameObjectsWithTag("Target");

        String ObjectName = "";
        foreach (GameObject obj in TargetList)
        {
            switch (obj.GetComponent<TargetSystem>().PrefabIndex)
            {
                case 0:
                    ObjectName = "3DOBJ/Mato/ME1";
                    break;
                case 1:
                    ObjectName = "3DOBJ/SR/SR1";
                    break;
                case 2:
                    ObjectName = "3DOBJ/Mato/ME2";
                    break;
                case 3:
                    ObjectName = "3DOBJ/Mato/ME4";
                    break;
                case 4:
                    ObjectName = "3DOBJ/Mato/ME5";
                    break;
                case 5:
                    ObjectName = "3DOBJ/Mato/ME6";
                    break;
                case 6:
                    ObjectName = "3DOBJ/Mato/ME";
                    break;
                case 7:
                    ObjectName = "3DOBJ/SR/3DObjTargetSR";
                    break;
                case 8:
                    ObjectName = "3DOBJ/Train/3DObjTarget tr";
                    break;
                case 9:
                    ObjectName = "3DOBJ/BigCubes";
                    break;
            }
            Debug.Log(ObjectName + "を作成");
            //PhotonObjectとして生成
            GameObject oc = (GameObject)PhotonNetwork.Instantiate(ObjectName, new Vector3(0, 0, 0), Quaternion.identity, 0);
            //値をコピー
            oc.GetComponent<TargetSystem>().setStatus(obj.GetComponent<TargetSystem>().getStatus());
            oc.transform.position = obj.transform.position;
            oc.transform.parent = obj.transform.parent;
            oc.transform.rotation = obj.transform.localRotation;
            //通常オブジェクトを破壊
            Destroy(obj);
        }
        //生成インデックスでの並び替え
        //Array.Sort(TargetList, (a, b) => a.GetComponent<TargetSystem>().CretateIndex - b.GetComponent<TargetSystem>().CretateIndex);
     
    }
}
