using UnityEngine;
using System.Collections.Generic;
using System;

public class UnitBlock : MonoBehaviour
{
    [SerializeField] private int maxHp = 10;
    private int hp;
    [SerializeField] private int blockCount = 2;
    private List<EnemyController> blockingEnemies = new List<EnemyController>();

    public Vector3Int placedCell; // PlayerUnit が設定する 破壊通知
    public event Action<UnitBlock> OnUnitDestroyed;

    private bool registeredToPlayer = false;

    void Start()
    {
        hp = maxHp;

        // PlayerUnit がシングルトンになっていれば自動登録を試みる
        if (PlayerUnit.Instance != null)
        {
            registeredToPlayer = PlayerUnit.Instance.RegisterPlacedUnit(gameObject);
            if (!registeredToPlayer)
            {
                Debug.LogWarning($"PlayerUnit の登録に失敗しました（上限超過）。このユニットを破棄します: {gameObject.name}");
                Destroy(gameObject);
                return;
            }
        }
    }

    // Dataから初期化（PlayerUnitから呼ぶ）
    public void Initialize(UnitSetting.UnitData data)
    {
        if (data != null)
        {
            blockCount = Mathf.Max(0, data.blockCount);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        EnemyController enemy;
        if (collision.gameObject.TryGetComponent<EnemyController>(out enemy))
        {
            if (blockingEnemies.Count < blockCount && !blockingEnemies.Contains(enemy))
            {
                blockingEnemies.Add(enemy);
                enemy.OnBlocked(this);
                Debug.Log("OnBlocked呼び出し: " + enemy.gameObject.name);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // blockingEnemiesの空きをチェック
        if (blockingEnemies.Count < blockCount && collision.TryGetComponent(out EnemyController enemy))
        {
            if (!blockingEnemies.Contains(enemy))
            {
                blockingEnemies.Add(enemy);
                enemy.OnBlocked(this);
                // EnemyController側に自分を通知（撃破時に呼び出してもらう）
                enemy.OnDestroyedByBlock += OnEnemyDestroyed;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 敵が範囲外へ出たときも解除（必要なら）
        if (collision.TryGetComponent(out EnemyController enemy) && blockingEnemies.Contains(enemy))
        {
            blockingEnemies.Remove(enemy);
            enemy.OnReleased();
            enemy.OnDestroyedByBlock -= OnEnemyDestroyed;
        }
    }

    public void OnEnemyKilled(EnemyController enemy)
    {
        if (blockingEnemies.Contains(enemy))
        {
            blockingEnemies.Remove(enemy);
            enemy.OnDestroyedByBlock -= OnEnemyDestroyed;
            Debug.Log("敵撃破によるブロック解除: " + enemy.gameObject.name);
        }
    }

    // ユニット撃破時に呼ぶ（Destroy直前）
    private void OnDestroy()
    {
        // ループ中にコレクションを変更しないよう ToArray でコピーして列挙する
        foreach (var enemy in blockingEnemies.ToArray())
        {
            if (enemy != null)
            {
                enemy.OnReleased();
                // イベントが登録されていれば解除する（安全のため try-unsubscribe）
                enemy.OnDestroyedByBlock -= OnEnemyDestroyed;
                Debug.Log("ユニット破壊に伴うブロック解除: " + enemy.gameObject.name);
            }
        }
        // PlayerUnit に自分の登録解除を依頼
        if (PlayerUnit.Instance != null && registeredToPlayer)
        {
            PlayerUnit.Instance.UnregisterPlacedUnit(gameObject);
            PlayerUnit.Instance.FreeOccupiedCell(placedCell);
        }
        blockingEnemies.Clear();
    }

    private void OnEnemyDestroyed(EnemyController enemy)
    {
        if (blockingEnemies.Contains(enemy))
        {
            blockingEnemies.Remove(enemy);
            enemy.OnDestroyedByBlock -= OnEnemyDestroyed;
        }
    }

    // 敵からダメージを受ける
    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log($"{gameObject.name} 残りHP: {hp}");
        if (hp <= 0)
        {
            // 親側へ通知して登録解除
            OnUnitDestroyed?.Invoke(this);

            // PlayerUnit へも登録解除を頼む
            if (PlayerUnit.Instance != null && registeredToPlayer)
            {
                PlayerUnit.Instance.UnregisterPlacedUnit(gameObject);
                PlayerUnit.Instance.FreeOccupiedCell(placedCell);
            }

            Destroy(gameObject);
        }
    }

    public int CurrentBlockingCount
    {
        get { return blockingEnemies.Count; }
    }

    /// このユニットのブロック可能数（外部確認用）
    public int BlockCapacity
    {
        get { return blockCount; }
    }
}
