using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private EnemyController enemyPrefab; // �G�̃v���n�u
    [SerializeField]
    private PathData pathData; // �ړ��o�H���
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
        // �X�^�[�g�n�_�v���n�u����G�𐶐�
        EnemyController enemyController = Instantiate(enemyPrefab, pathData.positionStart.position, Quaternion.identity);

    }
}
