using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private EnemyController enemyPrefab; // �G�̃v���n�u
    [SerializeField]
    private PathData[] pathDataArray; // �ړ��o�H���̔z��
    [SerializeField]
    private GameManager gameManager;


    // �G�̐����Ǘ�
    public IEnumerator ManageSpawning()
    {
        int timer = 0; // �^�C�}�[�̏�����
        while (gameManager.isSpawning) // �G�𐶐��\�Ȃ��
        {
            timer++; // �^�C�}�[���Z
            if (timer > gameManager.spawnInterval) // �^�C�}�[���G�����Ԋu�𒴂�����
            {
                timer = 0; // �^�C�}�[���Z�b�g
                Spawn(); // �G����
                gameManager.AddEnemyToList(); // �G�̏���List�ɒǉ�
                gameManager.CheckSpawnLimit();  // �ő吶�����𒴂�����G�̐�����~
            }
            yield return null;
        }
    }

    // �G�̐���
    public void Spawn()
    {
        // �����_���Ȍo�H��I��
        PathData selectedPath = pathDataArray[Random.Range(0, pathDataArray.Length)];

        // �X�^�[�g�n�_�v���n�u����G�𐶐�
        EnemyController enemyController = Instantiate(enemyPrefab, selectedPath.positionStart.position, Quaternion.identity);

        // �����_���ȓG��I��
        int enemyId = Random.Range(0, DBManager.instance.enemySetting.enemyDataList.Count);
        // �o�H����������

        // �G�f�[�^�̏�����
        enemyController.InitializeEnemy(selectedPath, gameManager, DBManager.instance.enemySetting.enemyDataList[enemyId]);

    }
}
