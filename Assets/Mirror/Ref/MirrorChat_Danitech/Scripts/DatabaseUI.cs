using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;

public class DatabaseUI : MonoBehaviour
{
   [Header("UI")]
   [SerializeField] InputField Input_Query;
   [SerializeField] Text Text_DBResult;
   [SerializeField] Text Text_Log;

    private bool _connectTestComplete = false;

    [Header("ConnectionInfo")]
    [SerializeField] string _ip = "127.0.0.1";
    [SerializeField] string _dbName = "test";
    [SerializeField] string _uid = "root";
    [SerializeField] string _pwd = "1234";

    private static MySqlConnection _dbConnection;
    private bool ConnectTest()
    {
        string connectStr = $"Server={_ip};Database={_dbName};Uid={_uid};Pwd={_pwd};";

        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectStr))
            {
                _dbConnection = conn;
                conn.Open();
            }

            Text_Log.text = "DB 연결을 성공했습니다!";
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"e: {e.ToString()}");
            Text_Log.text = "DB 연결 실패!";
            return false;
        }
    }

    public void OnClick_TestDBConnect()
    {
        _connectTestComplete = ConnectTest();
    }

    public void OnSubmit_SendQuery()
    {
    }

    public void OnClick_OpenDatabaseUI()
    {
        this.gameObject.SetActive(true);
    }

    public void OnClick_CloseDatabaseUI()
    {
        this.gameObject.SetActive(false);
    }

}
