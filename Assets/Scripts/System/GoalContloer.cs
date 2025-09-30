using UnityEngine;
using UnityEngine.UI;

public class GoalContloer : MonoBehaviour
{
    [SerializeField, Header("最大HP")] private int maxHp = 3; // 防衛拠点の最大HP
    [SerializeField, Header("現在のHP")] private int currentHp; // 防衛拠点の現在のHP
    [SerializeField, Header("HP表示スライダー")] private Slider hpSlider; // HPを表示するスライダー

    private void Start()
    {
        // ゲーム開始時にHPを最大値に設定
        currentHp = maxHp;
        // HPスライダーを更新
        UpdateHpSlider();
    }

    
    // 敵が防衛拠点に接触したときの処理
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 敵と接触した場合
        if (collision.TryGetComponent(out EnemyController enemy))
        {
            // ダメージを受ける
            TakeDamage();
            // 敵にゴール到達を通知する
            if (enemy != null)
            {
               enemy.ReachedGoal();
            }
        }
    }

    // ダメージを受ける
    public void TakeDamage()
    {
        // HPを1減らす
        currentHp--;
        // HPスライダーを更新
        UpdateHpSlider();

        // HPが0以下になったらゲームオーバー
        if (currentHp <= 0)
        {
            GameOver();
        }
    }

    // HPスライダーを更新する
    private void UpdateHpSlider()
    {
        // スライダーの最大値を設定
        hpSlider.maxValue = maxHp;
        // スライダーの値を現在のHPに設定
        hpSlider.value = currentHp;
    }

    // ゲームオーバー処理
    private void GameOver()
    {
        // ゲームオーバー処理を記述
        Debug.Log("ゲームオーバー");
    }
}
