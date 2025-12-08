using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DatabaseHandler
{
    static string _dbPath = Application.streamingAssetsPath + "/MBExpansions.db";

    #region adders

    static public void AddSetToDatabase(string expansionName, int expansionType)
    {
        using var connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        using var trans = connection.BeginTransaction();// Async();

        //await trans;

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("INSERT INTO ExpansionList (ExpansionName, ExpansionType) VALUES('{0}', '{1}')", expansionName, expansionType);
        command.ExecuteNonQuery();
        trans.Commit();
        connection.Close();

        //command.ExecuteNonQueryAsync()
        //yield return new WaitForEndOfFrame();
    }

    #endregion

    #region updaters
    static public void UpdateSetInDatabase(string newName, int newType, string currentName)
    {
        using var connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        using var trans = connection.BeginTransaction();// Async();

        //await trans;

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("UPDATE ExpansionList SET ExpansionName = '{0}', ExpansionType = '{1}' WHERE ExpansionName = '{2}'", newName, newType, currentName);
        command.ExecuteNonQuery();
        trans.Commit();
        connection.Close();
    }
    #endregion

    #region removers
    static public void RemoveSetFromDatabase(string whichSet)
    {
        using var connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        using var trans = connection.BeginTransaction();// Async();

        //await trans;

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("DELETE FROM ExpansionList WHERE ExpansionName = '{0}'", whichSet);
        command.ExecuteNonQuery();
        trans.Commit();
        connection.Close();
    }
    #endregion

    #region getters
    static public CardSet GetCardSetWithID(int cardID)
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
        //Debug.Log("set name: " + thisSet.SetName);
        //Debug.Log("set type: " + thisSet.SetType);
        reader.Close();

        Expansion thisExpansion = new Expansion();

        return thisSet;
    }

    static public List<string> GetFullSetList()
    {
        Debug.Log("Getting booster list");
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

    static public List<string> GetFullExpansionList()
    {
        Debug.Log("Getting expansion list");
        List<string> expansionList = new List<string>();

        SqliteConnection connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = "SELECT ReleaseName FROM ExpansionReleases";

        SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            expansionList.Add(reader.GetString(0));
        }
        reader.Close();

        return expansionList;
    }

    static public string GetExpansionTypeFromName(string expansionName)
    {
        string textToRetun = "";
        SqliteConnection connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("SELECT ExpansionType FROM ExpansionList WHERE ExpansionName = '{0}'", expansionName);
        //command.CommandText = "SELECT ReleaseName FROM ExpansionReleases, json_each(ContainsExpansions) WHERE json_each.value='0'";// ExpansionID = " + cardID.ToString() + "";
        //command.CommandText = "SELECT json_extract(ContainsExpansions
        //command.CommandText = "CREATE VIRTUAL TABLE tempReleases USING ExpansionReleases";

        SqliteDataReader reader = command.ExecuteReader();
        reader.Read();
        textToRetun = ((CardSet.TypeOfSet)reader.GetInt32(0)).ToString();
        reader.Close();

        return textToRetun;
    }

    static public List<CardSet> GetSetsContainedInExpansion(string expansionName)
    {
        List<CardSet> setsToReturn = new List<CardSet>();

        SqliteConnection connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("SELECT ContainsExpansions FROM ExpansionReleases WHERE ReleaseName = '{0}'", expansionName);

        SqliteDataReader reader = command.ExecuteReader();
        reader.Read();
        string IDList = reader.GetString(0);

        string[] IDListArray = IDList.Split(',');
        reader.Close();

        foreach(string ID in IDListArray)
        {
            setsToReturn.Add(GetCardSetWithID(int.Parse(ID)));
        }

        return setsToReturn;
    }
    #endregion
}