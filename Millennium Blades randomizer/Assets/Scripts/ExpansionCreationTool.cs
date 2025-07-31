using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class ExpansionCreationTool : MonoBehaviour
{
    string _dbPath = Application.streamingAssetsPath + "/MBExpansions.db";


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
        Debug.Log("set name: " + thisSet.SetName);
        Debug.Log("set type: " + thisSet.SetType);
        reader.Close();

        Expansion thisExpansion = new Expansion();

        command.CommandText = "SELECT ReleaseName, ContainsExpansions FROM ExpansionReleases WHERE ReleaseID = '3' ";// ExpansionID = " + cardID.ToString() + "";
        //https://stackoverflow.com/questions/27545640/writing-json-string-from-sql-query-in-c-sharp
        reader = command.ExecuteReader();
        while (reader.Read())
        {
            thisExpansion.ExpansionName = reader.GetString(0);
            string[] expansionContains = reader.GetString(1).Split(',');
            foreach (string expansion in expansionContains)
            {
                thisExpansion.mySetsList.Add(int.Parse(expansion));
            }
        }
        connection.Clone();
        
        return thisSet;
    }

    public void CreateNewSetEntry()
    {

    }
}
