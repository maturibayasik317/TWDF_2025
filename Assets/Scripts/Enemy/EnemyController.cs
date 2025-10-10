using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 
using System.Linq;
using System;
public class EnemyController : MonoBehaviour
{
    [SerializeField, Header("�ړ��o�H�̏��")]
    private PathData pathData;
    [SerializeField, Header("�ړ����x")]
    private float speed;
    [SerializeField, Header("�ő�HP")]
    private int maxHp;
    [SerializeField, Header("HP")]
    private int hp;
    private Tween tween; // DOPath���\�b�h�̏����������Ă����ϐ�
    private Vector3[] path; // pathData����擾�������W���i�[���邽�߂̔z��
    private Animator animator;
    private GameManager gameManager;
    public EnemySetting.EnemyData enemyData;
    private bool isBlocked = false; // �u���b�N��ԃt���O
    private UnitBlock blockingUnit;
    public event Action<EnemyController> OnDestroyedByBlock;

    void Start()
    {

        // �o�H���擾
        path = pathData.pathArray.Select(x => x.position).ToArray();
        // �X�^�[�g�n�_�ɓG���Z�b�g
        transform.position = pathData.positionStart.position;
        // �o�H�̑��������v�Z
        float totalDistance = CalculatePathLength(path);
        // �ړ����Ԃ��v�Z (���� �� ���x)
        float moveDuration = totalDistance / speed;
        // �o�H�ɉ����Ĉړ�

    }

    void Update()
    {
        
    }

    // �G�f�[�^��������
    public void InitializeEnemy(PathData selectedPath, GameManager gameManager, EnemySetting.EnemyData enemyData)
    {
        this.enemyData = enemyData; // EnemyData����
        speed = this.enemyData.speed; // �ړ����x��ݒ�
        maxHp = this.enemyData.maxHp; // �ő�HP��ݒ�
        this.gameManager = gameManager;
        hp = maxHp; // ���݂�HP��ݒ�
        if (TryGetComponent(out animator)) // Animator�R���|�[�l���g���擾���đ��
        {
            // Animator�R���|�[�l���g���擾�ł�����A�A�j���[�V�����̏㏑��������
            SetUpAnimation();
        }
        path = selectedPath.pathArray.Select(x => x.position).ToArray(); // �o�H���擾
        float totalDistance = CalculatePathLength(path); // �o�H�̑��������v�Z
        float moveDuration = totalDistance / enemyData.speed; // �ړ����Ԃ��v�Z
        // �o�H�ɉ����Ĉړ����鏈����tween�ϐ��ɑ��
        tween = transform.DOPath(path, moveDuration)
            .SetEase(Ease.Linear)
            .OnWaypointChange(x => ChangeWalkingAnimation(x))
            .OnComplete(() => { Destroy(gameObject); });
        Debug.Log($"�������ꂽ�G: {enemyData.name}, HP: {hp}, ���x: {speed}");
    }

    // �o�H�̑��������v�Z
    private float CalculatePathLength(Vector3[] path)
    {
        float length = 0f;
        for (int i = 0; i < path.Length - 1; i++)
        {
            // �e�Z�O�����g�̋������v�Z���č��v
            length += Vector3.Distance(path[i], path[i + 1]);
        }
        return length;
    }

    // �G�̐i�s�������擾���ăA�j����ύX
    private void ChangeWalkingAnimation(int index)
    {
        // ���̈ړ��悪�Ȃ��ꍇ�͏������I��
        if (index >= path.Length - 1)
        {
            return;
        }

        // �ړ���̕������v�Z
        Vector2 direction = (path[index + 1] - path[index]).normalized;

        // X��Y�������A�j���[�^�[�ɐݒ�
        animator.SetFloat("X", Mathf.Round(direction.x));
        animator.SetFloat("Y", Mathf.Round(direction.y));
    }

    // ����Unit�Ƀu���b�N���ꂽ�Ƃ��Ă�
    public void OnBlocked(MonoBehaviour blocker)
    {
        isBlocked = true;
        if (tween != null && tween.IsActive())
        {
            tween.Pause();
            Debug.Log($"{gameObject.name} ���u���b�N����Ē�~");
        }
    }

    // ����Unit�̃u���b�N���O�ꂽ�Ƃ��Ă�
    public void OnReleased()
    {
        isBlocked = false;
        blockingUnit = null;
        if (tween != null && tween.IsActive())
        {
            tween.Play();
            Debug.Log($"{gameObject.name} �̃u���b�N�����A�ĊJ");
        }
    }

        // �A�j���[�V������ύX
        private void SetUpAnimation()
    {
        if (enemyData.overrideController != null) // �A�j���[�V�����p�̃f�[�^�������
        {
            animator.runtimeAnimatorController = enemyData.overrideController; // �A�j���[�V�������㏑������
        }
    }

    //�_���[�W�v�Z
    public void CalcDamage(int amount)
    {
        hp = Mathf.Clamp(hp -= amount, 0, maxHp);
        Debug.Log("�c��HP : " + hp);
        if (hp <= 0) // HP��0�ȉ��ɂȂ�����
        {
            DestroyEnemy(); // �G��j��
        }
    }

    //�G�̌��j
    public void DestroyEnemy()
    {
        // Destroy���O�ɒʒm
        OnDestroyedByBlock?.Invoke(this);
        tween.Kill(); // tween�ϐ��ɑ������Ă��鏈�����I������
        Destroy(gameObject); // �G�̔j��
    }

    // </summary>
    public void ReachedGoal()
    {
        DestroyEnemy(); // �G��j�󂷂�
    }
}
