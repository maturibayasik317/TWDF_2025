using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 
using System.Linq;
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


    void Start()
    {
        hp = maxHp;

        TryGetComponent(out animator);

        // �o�H���擾
        path = pathData.pathArray.Select(x => x.position).ToArray();
        // �X�^�[�g�n�_�ɓG���Z�b�g
        transform.position = pathData.positionStart.position;
        // �o�H�̑��������v�Z
        float totalDistance = CalculatePathLength(path);
        // �ړ����Ԃ��v�Z (���� �� ���x)
        float moveDuration = totalDistance / speed;
        // �o�H�ɉ����Ĉړ�
        tween = transform.DOPath(path, moveDuration)
                                .SetEase(Ease.Linear)
                                .OnWaypointChange(x => ChangeWalkingAnimation(x));
        //�G���S�[���ɂ����Ƃ�
        transform.DOPath(path, 1000 / speed)
    .   SetEase(Ease.Linear)
    .   OnComplete(() => {
        // �S�[�����B���̏���
        Destroy(gameObject); // �G������
        });

    }

    void Update()
    {
        
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
        tween.Kill(); // tween�ϐ��ɑ������Ă��鏈�����I������
        Destroy(gameObject); // �G�̔j��
    }

    // </summary>
    public void ReachedGoal()
    {
        DestroyEnemy(); // �G��j�󂷂�
    }
}
