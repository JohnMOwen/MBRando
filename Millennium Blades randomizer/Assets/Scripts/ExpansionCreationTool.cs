using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class ExpansionCreationTool : MonoBehaviour
{
    string _dbPath = Application.streamingAssetsPath + "/MBExpansions.db";


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            CardSet gettingSet = GetCardWithID(0);
            Debug.Log(gettingSet.SetName);
            Debug.Log(gettingSet.SetType);
        }
    }

    /*public void SaveNewTable(System.Data.DataTable DT)
    {
        // DATABASE (Local): Formulate the SQL command.
        string strSqlCommand = "SELECT * FROM tblTest ORDER BY IdPrimary ASC;";
        SqliteCommand oLocalCommand = new SqliteCommand(strSqlCommand);

        // DATABASE (Local): Get the data records.
        SqliteDataAdapter oLocalAdapter = new SqliteDataAdapter(oLocalCommand);
        System.Data.DataSet oLocalSet = new System.Data.DataSet();
        oLocalAdapter.Fill(oLocalSet, "tblTest");

        // 
        SqliteCommandBuilder oBuilder = new SqliteCommandBuilder(oLocalAdapter);

        // Try to write to some changes.
        string strValue = oLocalSet.Tables[0].Rows[0][8].ToString();
        oLocalSet.Tables[0].Rows[0][8] = 9;
        strValue = oLocalSet.Tables[0].Rows[0][8].ToString();
        oLocalSet.AcceptChanges();
        oLocalAdapter.UpdateCommand = oBuilder.GetUpdateCommand();
        oLocalAdapter.Update(oLocalSet.Tables[0]);

        // Clean up.
        oLocalSet.Dispose();
        oLocalAdapter.Dispose();
        oLocalCommand.Dispose();
        oLocalCommand = null;
        SqliteConnection connection = new SqliteConnection($"URI=file:{_dbPath}");
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = string.Format("SELECT * FROM {0}", DT.TableName);
        SqliteDataAdapter adapter = new SqliteDataAdapter(command);
        SqliteCommandBuilder builder = new SqliteCommandBuilder(adapter);
        adapter.Update(DT);
        connection.Close();
    }*/

    public CardSet GetCardWithID(int cardID)
    {
        CardSet thisSet = new CardSet();

        SqliteConnection connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = "SELECT ExpansionName, ExpansionType FROM ExpansionList WHERE ExpansionID = " + cardID.ToString() + "";
        //command.CommandText = "SELECT ReleaseName FROM ExpansionReleases, json_each(ContainsExpansions) WHERE json_each.value='0'";// ExpansionID = " + cardID.ToString() + "";
        //command.CommandText = "SELECT json_extract(ContainsExpansions
        //command.CommandText = "CREATE VIRTUAL TABLE tempReleases USING ExpansionReleases";

        SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            thisSet.SetName = reader.GetString(0);
            thisSet.SetType = (CardSet.TypeOfSet)reader.GetInt32(1);
        }
        Debug.Log(thisSet.SetName);
        Debug.Log(thisSet.SetType);
        reader.Close();
        command.CommandText = "SELECT ReleaseName FROM ExpansionReleases, json_each(ContainsExpansions) WHERE json_each.value='0'";// ExpansionID = " + cardID.ToString() + "";
        //https://stackoverflow.com/questions/27545640/writing-json-string-from-sql-query-in-c-sharp
        reader = command.ExecuteReader();
        while (reader.Read())
        {
            Debug.Log(reader.GetString(0));
        }
        connection.Clone();
        
        return thisSet;
    }
}
