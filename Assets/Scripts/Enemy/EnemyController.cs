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
    [SerializeField, Header("最大HP")]
    private int maxHp;
    [SerializeField, Header("HP")]
    private int hp;
    private Tween tween; // DOPathメソッドの処理を代入しておく変数
    private Vector3[] path; // pathDataから取得した座標を格納するための配列
    private Animator animator;
    private GameManager gameManager;
    public EnemySetting.EnemyData enemyData;
    private bool isBlocked = false; // ブロック状態フラグ

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

    }

    void Update()
    {
        
    }

    // 敵データを初期化
    public void InitializeEnemy(PathData selectedPath, GameManager gameManager, EnemySetting.EnemyData enemyData)
    {
        this.enemyData = enemyData; // EnemyDataを代入
        speed = this.enemyData.speed; // 移動速度を設定
        maxHp = this.enemyData.maxHp; // 最大HPを設定
        this.gameManager = gameManager;
        hp = maxHp; // 現在のHPを設定
        if (TryGetComponent(out animator)) // Animatorコンポーネントを取得して代入
        {
            // Animatorコンポーネントが取得できたら、アニメーションの上書きをする
            SetUpAnimation();
        }
        path = selectedPath.pathArray.Select(x => x.position).ToArray(); // 経路を取得
        float totalDistance = CalculatePathLength(path); // 経路の総距離を計算
        float moveDuration = totalDistance / enemyData.speed; // 移動時間を計算
        // 経路に沿って移動する処理をtween変数に代入
        tween = transform.DOPath(path, moveDuration)
            .SetEase(Ease.Linear)
            .OnWaypointChange(x => ChangeWalkingAnimation(x))
            .OnComplete(() => { Destroy(gameObject); });
        Debug.Log($"生成された敵: {enemyData.name}, HP: {hp}, 速度: {speed}");
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

    // 敵の進行方向を取得してアニメを変更
    private void ChangeWalkingAnimation(int index)
    {
        // 次の移動先がない場合は処理を終了
        if (index >= path.Length - 1)
        {
            return;
        }

        // 移動先の方向を計算
        Vector2 direction = (path[index + 1] - path[index]).normalized;

        // XとY方向をアニメーターに設定
        animator.SetFloat("X", Mathf.Round(direction.x));
        animator.SetFloat("Y", Mathf.Round(direction.y));
    }

    // 味方Unitにブロックされたとき呼ぶ
    public void OnBlocked(MonoBehaviour blocker)
    {
        isBlocked = true;
        if (tween != null && tween.IsActive())
        {
            tween.Pause();
            Debug.Log($"{gameObject.name} がブロックされて停止");
        }
    }

    // 味方Unitのブロックが外れたとき呼ぶ
    public void OnReleased()
    {
        isBlocked = false;
        isBlocked = false;
        if (tween != null && tween.IsActive())
        {
            tween.Play();
            Debug.Log($"{gameObject.name} のブロック解除、再開");
        }
    }

        // アニメーションを変更
        private void SetUpAnimation()
    {
        if (enemyData.overrideController != null) // アニメーション用のデータがあれば
        {
            animator.runtimeAnimatorController = enemyData.overrideController; // アニメーションを上書きする
        }
    }

    //ダメージ計算
    public void CalcDamage(int amount)
    {
        hp = Mathf.Clamp(hp -= amount, 0, maxHp);
        Debug.Log("残りHP : " + hp);
        if (hp <= 0) // HPが0以下になったら
        {
            DestroyEnemy(); // 敵を破壊
        }
    }

    //敵の撃破
    public void DestroyEnemy()
    {
        tween.Kill(); // tween変数に代入されている処理を終了する
        Destroy(gameObject); // 敵の破壊
    }

    // </summary>
    public void ReachedGoal()
    {
        DestroyEnemy(); // 敵を破壊する
    }
}
