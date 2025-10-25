using UnityEngine;
using UnityEngine.UI;

public class GoalContloer : MonoBehaviour
{
    [SerializeField, Header("�ő�HP")] private int maxHp = 3; // �h�q���_�̍ő�HP
    [SerializeField, Header("���݂�HP")] private int currentHp; // �h�q���_�̌��݂�HP
    [SerializeField, Header("HP�\���X���C�_�[")] private Slider hpSlider; // HP��\������X���C�_�[

    private void Start()
    {
        // �Q�[���J�n����HP���ő�l�ɐݒ�
        currentHp = maxHp;
        // HP�X���C�_�[���X�V
        UpdateHpSlider();
    }

    
    // �G���h�q���_�ɐڐG�����Ƃ��̏���
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �G�ƐڐG�����ꍇ
        if (collision.TryGetComponent(out EnemyController enemy))
        {
            // �_���[�W���󂯂�
            TakeDamage();
            // �G�ɃS�[�����B��ʒm����
            if (enemy != null)
            {
               enemy.ReachedGoal();
            }
        }
    }

    // �_���[�W���󂯂�
    public void TakeDamage()
    {
        // HP��1���炷
        currentHp--;
        // HP�X���C�_�[���X�V
        UpdateHpSlider();

        // HP��0�ȉ��ɂȂ�����Q�[���I�[�o�[
        if (currentHp <= 0)
        {
            GameOver();
        }
    }

    // HP�X���C�_�[���X�V����
    private void UpdateHpSlider()
    {
        // �X���C�_�[�̍ő�l��ݒ�
        hpSlider.maxValue = maxHp;
        // �X���C�_�[�̒l�����݂�HP�ɐݒ�
        hpSlider.value = currentHp;
    }

    // �Q�[���I�[�o�[����
    public void GameOver()
    {
        // �Q�[���I�[�o�[�������L�q
        Debug.Log("�Q�[���I�[�o�[");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}
