using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int targetFrameRate = 60; // �t���[�����[�g�̖ڕW�l

    void Awake()
    {
        FixFrameRate(); // �t���[�����[�g���Œ�
    }

    // �t���[�����[�g���Œ�
    private void FixFrameRate()
    {
        QualitySettings.vSyncCount = 0; // V-Sync�i���������j�𖳌���
        Application.targetFrameRate = targetFrameRate;
    }
}
