using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//ScriptabeObjectを一元管理するためのシングルトンクラス
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
