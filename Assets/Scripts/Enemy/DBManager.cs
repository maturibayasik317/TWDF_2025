using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//ScriptabeObject���ꌳ�Ǘ����邽�߂̃V���O���g���N���X
public class DBManager : MonoBehaviour
{
    public static DBManager instance;
    public EnemySetting enemySetting;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
