using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int targetFrameRate = 60; // �t���[�����[�g�̖ڕW�l


    [SerializeField] private EnemySpawner enemySpawner;
    public bool isSpawning; // �G�𐶐����邩�ǂ����𐧌䂷��t���O
    public int spawnInterval; // �G�𐶐�����Ԋu�i�P�ʂ̓t���[���j
    public int spawnedEnemyCount; // ����܂łɐ������ꂽ�G�̐�
    public int maxSpawnCount; // �G�̍ő吶����
    void Awake()
    {
        FixFrameRate(); // �t���[�����[�g���Œ�
    }

    void Start()
    {
        isSpawning = true; // �G�𐶐��\�ɂ���
        StartCoroutine(enemySpawner.ManageSpawning());
    }

    // �t���[�����[�g���Œ�
    private void FixFrameRate()
    {
        QualitySettings.vSyncCount = 0; // V-Sync�i���������j�𖳌���
        Application.targetFrameRate = targetFrameRate;
    }

    // �G�̏���List�ɒǉ�
    public void AddEnemyToList()
    {
        spawnedEnemyCount++; // ���������G�̐��𑝂₷
    }

    // �G�̐���������ɒB���������m�F
    public void CheckSpawnLimit()
    {
        if (spawnedEnemyCount >= maxSpawnCount) // �G�̍ő吶�����𒴂�����
        {
            isSpawning = false; // �G�𐶐��s�ɂ���
        }
    }
}
