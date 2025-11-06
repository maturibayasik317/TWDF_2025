using UnityEngine;

/// <summary>
/// 強化内容を永続的に保持するシンプルなシングルトン（ランタイムのみ）。
/// ここに「持ち越す強化」を集約します。UpgradeManager はここに加算していきます。
/// </summary>
public class RogueProgress : MonoBehaviour
{
    public static RogueProgress Instance { get; private set; }

    // 例: 追加で増える最大設置数（合算で反映）
    public int AdditionalMaxUnits { get; private set; } = 0;

    // ユニットの最大HPに上乗せする（絶対値）
    public int UnitHpBonus { get; private set; } = 0;

    // ユニットの攻撃力に上乗せする（絶対値）
    public int UnitAttackBonus { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // API: 各種強化を適用する（呼び出し元は UpgradeManager）
    public void AddMaxUnits(int add) { AdditionalMaxUnits += add; Debug.Log($"RogueProgress: AdditionalMaxUnits -> {AdditionalMaxUnits}"); }
    public void AddUnitHp(int add) { UnitHpBonus += add; Debug.Log($"RogueProgress: UnitHpBonus -> {UnitHpBonus}"); }
    public void AddUnitAttack(int add) { UnitAttackBonus += add; Debug.Log($"RogueProgress: UnitAttackBonus -> {UnitAttackBonus}"); }

    // リセット（例えばセーブなしでゲームを最初からやり直す時に）
    public void ResetProgress()
    {
        AdditionalMaxUnits = 0;
        UnitHpBonus = 0;
        UnitAttackBonus = 0;
    }
}