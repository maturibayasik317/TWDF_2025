using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private int targetFrameRate = 60; // �t���[�����[�g�̖ڕW�l

    [SerializeField] private EnemySpawner enemySpawner;
    public bool isSpawning; // �G�𐶐����邩�ǂ����𐧌䂷��t���O
    public int spawnInterval; // �G�𐶐�����Ԋu�i�P�ʂ̓t���[���j
    public int spawnedEnemyCount; // ����܂łɐ������ꂽ�G�̐�
    public int maxSpawnCount; // �G�̍ő吶����

    // ���݃V�[����ɑ��݂���i�������Ă���j�G�̐�
    private int aliveEnemyCount = 0;

    [Header("UI")]
    [SerializeField] private GameObject stageClearObject; // StageClear �\���p�I�u�W�F�N�g�iInspector�Ɋ����āA�����͔�\���j
    
    void Awake()
    {
        // �V���O���g��������
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("������ GameManager �����݂��܂��B�Â��C���X�^���X��j�����܂��B");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        FixFrameRate(); // �t���[�����[�g���Œ�

        // StageClear��\���i
        if (stageClearObject != null)
        {
            stageClearObject.SetActive(false);
        }
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

    // �Q�[�����J�n����iUI��GameStart�{�^������Ăԁj
    public void StartGame()
    {
        if (isSpawning)
        {
            Debug.Log("���ɃQ�[���J�n�ς݂ł�");
            return;
        }

        Debug.Log("GameManager: StartGame called");

        // �G�̐���������
        isSpawning = true;
        spawnedEnemyCount = 0;
        aliveEnemyCount = 0;

        if (enemySpawner != null)
        {
            StartCoroutine(enemySpawner.ManageSpawning());
        }
        else
        {
            Debug.LogWarning("EnemySpawner ���Z�b�g����Ă��܂���BInspector�� enemySpawner �����蓖�ĂĂ��������B");
        }

        // ���j�b�g�z�u�����iPlayerUnit �����݂���ꍇ�j
        if (PlayerUnit.Instance != null)
        {
            PlayerUnit.Instance.SetAllowPlacement(true);
        }
    }


    // �G���X�|�[�������Ƃ��ɌĂԁiSpawner�܂���EnemyController�̏���������Ăԁj
    public void RegisterSpawnedEnemy(EnemyController enemy)
    {
        spawnedEnemyCount++;
        aliveEnemyCount++;
        Debug.Log($"GameManager: RegisterSpawnedEnemy -> spawnedTotal={spawnedEnemyCount}, alive={aliveEnemyCount}");
        CheckSpawnLimit();
    }

    // �G�����S�����Ƃ��ɌĂԁiEnemyController.DestroyEnemy ����Ăԁj
    public void NotifyEnemyDestroyed(EnemyController enemy)
    {
        aliveEnemyCount = Mathf.Max(0, aliveEnemyCount - 1);
        Debug.Log($"GameManager: NotifyEnemyDestroyed -> alive={aliveEnemyCount}");
        CheckStageClear();
    }

    // �G�̏���List�ɒǉ�
    public void AddEnemyToList()
    {
        spawnedEnemyCount++; // ���������G�̐��𑝂₷
    }

    // �G�̐���������ɒB���������m�F
    private void CheckSpawnLimit()
    {
        if (spawnedEnemyCount >= maxSpawnCount)
        {
            isSpawning = false;
            Debug.Log("GameManager: spawn limit reached, stopping spawn");
        }
    }

    // �X�e�[�W�N���A����i�X�|�[���I���������G0�j
    private void CheckStageClear()
    {
        // spawnedEnemyCount >= maxSpawnCount �����邱�ƂŃX�|�[�����S�ďI����Ă��邩�m�F����
        if (!isSpawning && spawnedEnemyCount >= maxSpawnCount && aliveEnemyCount <= 0)
        {
            ShowStageClear();
        }
    }

    // �X�e�[�W�N���A�\��
    private void ShowStageClear()
    {
        Debug.Log("GameManager: Stage Clear!");
        if (stageClearObject != null)
        {
            stageClearObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("stageClearObject �� Inspector �ɐݒ肳��Ă��܂���B");
        }
    }

    // �f�o�b�O�p�̃Q�b�^�[�i�O���m�F�p�j
    public int GetAliveEnemyCount() => aliveEnemyCount;
    public int GetSpawnedEnemyCount() => spawnedEnemyCount;
}
