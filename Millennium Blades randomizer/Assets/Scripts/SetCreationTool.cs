using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SetCreationTool : MonoBehaviour
{

    string _dbPath = Application.streamingAssetsPath + "/MBExpansions.db";
    [SerializeField]
    TMPro.TMP_Dropdown setListDropdown, cardTypeDropdown;
    [SerializeField]
    TMPro.TMP_InputField setName;

    // Start is called before the first frame update
    void Start()
    {
        UpdateSetListDropdown();
    }

    void UpdateSetListDropdown()
    {
        setListDropdown.ClearOptions();
        setListDropdown.AddOptions(RetrieveFullSetList());
    }

    List<string> RetrieveFullSetList()
    {
        Debug.Log("Getting list");
        List<string> setList = new List<string>();

        SqliteConnection connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = "SELECT ExpansionName FROM ExpansionList";

        SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            setList.Add(reader.GetString(0));
        }
        reader.Close();

        return setList;
    }

    public void AddSetToDatabase()
    {
        using var connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        using var trans = connection.BeginTransaction();// Async();
        
        //await trans;

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("INSERT INTO ExpansionList (ExpansionName, ExpansionType) VALUES('{0}', '{1}')", setName.text, cardTypeDropdown.value+1);
        command.ExecuteNonQuery();
        trans.Commit();
        connection.Close();

        //command.ExecuteNonQueryAsync()
        //yield return new WaitForEndOfFrame();
        UpdateSetListDropdown();
    }

    public void RemoveSetFromDatabase()
    {
        using var connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        using var trans = connection.BeginTransaction();// Async();

        //await trans;

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("DELETE FROM ExpansionList WHERE ExpansionName = '{0}'", setListDropdown.options[setListDropdown.value].text);
        command.ExecuteNonQuery();
        trans.Commit();
        connection.Close();

        //command.ExecuteNonQueryAsync()
        //yield return new WaitForEndOfFrame();
        UpdateSetListDropdown();
    }
}
