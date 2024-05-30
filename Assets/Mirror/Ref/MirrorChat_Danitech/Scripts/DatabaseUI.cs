using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;
using System.Data;

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

    private string SendQuery(string queryStr, string tableName)
    {
        DataSet dataSet = OnSelectRequest(queryStr, tableName);

        return dataSet.GetXml().ToString();
    }

    public static DataSet OnSelectRequest(string query, string tableName)
    {
        try
        {
            _dbConnection.Open();

            MySqlCommand sqlCmd = new MySqlCommand();
            sqlCmd.Connection = _dbConnection;
            sqlCmd.CommandText = query;

            MySqlDataAdapter sd = new MySqlDataAdapter(sqlCmd);
            DataSet dataSet = new DataSet();
            sd.Fill(dataSet, tableName);

            _dbConnection.Close();
            return dataSet;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }

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
        string query = string.IsNullOrWhiteSpace(Input_Query.text) ? "SELECT U_Name FROM user_info"
            : Input_Query.text;

        string resultStr = SendQuery(query, "user_info");
        Text_DBResult.text = resultStr;
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
