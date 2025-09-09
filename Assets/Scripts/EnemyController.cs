using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 
using System.Linq;
public class EnemyController : MonoBehaviour
{
    [SerializeField, Header("移動経路の情報")]
    private PathData pathData;
    [SerializeField, Header("移動速度")]
    private float speed;
    private Vector3[] path; // pathDataから取得した座標を格納するための配列


    void Start()
    {
        // 経路を取得
        path = pathData.pathArray.Select(x => x.position).ToArray();
        // スタート地点に敵をセット
        transform.position = pathData.positionStart.position;
        // 経路の総距離を計算
        float totalDistance = CalculatePathLength(path);
        // 移動時間を計算 (距離 ÷ 速度)
        float moveDuration = totalDistance / speed;
        // 経路に沿って移動
        transform.DOPath(path, moveDuration).SetEase(Ease.Linear);
        //敵がゴールについたとき
        transform.DOPath(path, 1000 / speed)
    .   SetEase(Ease.Linear)
    .   OnComplete(() => {
        // ゴール到達時の処理
        Destroy(gameObject); // 敵を消す
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 経路の総距離を計算
    private float CalculatePathLength(Vector3[] path)
    {
        float length = 0f;
        for (int i = 0; i < path.Length - 1; i++)
        {
            // 各セグメントの距離を計算して合計
            length += Vector3.Distance(path[i], path[i + 1]);
        }
        return length;
    }
}
