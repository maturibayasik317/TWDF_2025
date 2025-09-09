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
    private Vector3[] path; // pathData����擾�������W���i�[���邽�߂̔z��


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
        transform.DOPath(path, moveDuration).SetEase(Ease.Linear);
        //�G���S�[���ɂ����Ƃ�
        transform.DOPath(path, 1000 / speed)
    .   SetEase(Ease.Linear)
    .   OnComplete(() => {
        // �S�[�����B���̏���
        Destroy(gameObject); // �G������
        });

    }

    // Update is called once per frame
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
}
