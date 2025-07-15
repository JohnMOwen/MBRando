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

    public CardSet GetCardWithID(int cardID)
    {
        CardSet thisSet = new CardSet();

        SqliteConnection connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = "SELECT ExpansionName, ExpansionType FROM ExpansionList WHERE ExpansionID = " + cardID.ToString() + "";
        //command.CommandText = "SELECT ReleaseName FROM ExpansionReleases, json_each(ContainsExpansions) WHERE json_each.value='0'";// ExpansionID = " + cardID.ToString() + "";


        SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            thisSet.SetName = reader.GetString(0);
            thisSet.SetType = (CardSet.TypeOfSet)reader.GetInt32(1);
        }
        //while (reader.Read())
        //{
            //Debug.Log(reader.GetString(0));
        //}
        connection.Clone();
        
        return thisSet;
    }
}
