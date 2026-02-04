using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Add SELECT checkers to all UPDATE calls
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

    static public void AddExpansionToDatabase(string expansionName)
    {
        using var connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        using var trans = connection.BeginTransaction();// Async();

        //await trans;

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("INSERT INTO ExpansionReleases (ReleaseName, ContainsExpansions) VALUES('{0}', '{1}')", expansionName, "");
        command.ExecuteNonQuery();
        trans.Commit();
        connection.Close();

        //command.ExecuteNonQueryAsync()
        //yield return new WaitForEndOfFrame();
    }

    static public void AddSetToExpansionList(string setID, string expansionName)
    {
        string IDs = GetSetIDsContainedInExpansionAsString(expansionName);

        using var connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        using var trans = connection.BeginTransaction();// Async();

        //await trans;

        SqliteCommand command = connection.CreateCommand();
        //command.CommandType = System.Data.CommandType.Text;
        //command.CommandText = string.Format("SELECT ContainsExpansions FROM ExpansionReleases WHERE ReleaseName = '{0}'", expansionName);

        //SqliteDataReader reader = command.ExecuteReader();
        //if (IDs == null)
            //return;

        if (IDs != null && IDs != "")
            setID = IDs + "," + setID;

        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("UPDATE ExpansionReleases SET ContainsExpansions = '{0}' WHERE ReleaseName = '{1}'", setID, expansionName);
        //command.CommandText = string.Format("UPDATE ExpansionReleases SET ContainsExpansions = CONCAT(ContainsExpansions, '{0}') WHERE ReleaseName = '{1}'", setID, expansionName);
        command.ExecuteNonQuery();
        trans.Commit();

        //Debug.Log(IDs);
        connection.Close();
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

    static public void UpdateExpansionNameInDatabase(string newName, string currentName)
    {
        using var connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        using var trans = connection.BeginTransaction();// Async();

        //await trans;

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("UPDATE ExpansionReleases SET ExpansionName = '{0}' WHERE ExpansionName = '{1}'", newName, currentName);
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

    static public void RemoveExpansionFromDatabase(string expansionName)
    {
        using var connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        using var trans = connection.BeginTransaction();// Async();

        //await trans;

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("DELETE FROM ExpansionReleases WHERE ReleaseName = '{0}'", expansionName);
        command.ExecuteNonQuery();
        trans.Commit();
        connection.Close();
    }

    //TODO: If we don't need setID to be an int, make it a string
    static public void RemoveSetFromExpansion(string setID, string expansionName)
    {
        List<string> IDs = GetSetIDsContainedInExpansionAsList(expansionName);
        if (IDs.Count == 0 || !IDs.Contains(setID))
            return;

        IDs.Remove(setID);

        string newIDList = "";

        for (int i = 0; i < IDs.Count; i++)
        {
            if (i > 0)
                newIDList += ",";

            newIDList = IDs[i].ToString();
        }


        using var connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        using var trans = connection.BeginTransaction();// Async();

        //await trans;

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("UPDATE ExpansionReleases SET ContainsExpansions = '{0}') WHERE ReleaseName = '{1}'", newIDList, expansionName);
        command.ExecuteNonQuery();
        trans.Commit();
        connection.Close();
    }
    static public void RemoveSetFromAllExpansions(string setID)
    {
        List<string> expansionList = GetExpansionsThatContainID(setID);
        foreach (string expansion in expansionList)
        {
            RemoveSetFromExpansion(setID, expansion);
        }
    }

    #endregion

    #region getters
    static public CardSet GetCardSetWithID(int cardID, int setType = -1)
    {
        CardSet thisSet = new CardSet();

        SqliteConnection connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        if(setType > 0)
            command.CommandText = string.Format("SELECT ExpansionName, ExpansionType FROM ExpansionList WHERE ExpansionID = '{0}' AND ExpansionType = '{1}'", cardID.ToString(), setType.ToString());
        else
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
        connection.Close();
        Expansion thisExpansion = new Expansion();

        return thisSet;
    }

    static public int GetSetIDFromName(string setName)
    {
        SqliteConnection connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = "SELECT ExpansionID FROM ExpansionList WHERE ExpansionName = '" + setName + "'";

        SqliteDataReader reader = command.ExecuteReader();
        reader.Read();
        int IDToReturn = reader.GetInt32(0);
        reader.Close();
        connection.Close();
        return IDToReturn;
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
        connection.Close();
        return setList;
    }

    static public List<string> GetFullSetListFilterByType(int setType)
    {
        Debug.Log("Getting booster list");
        List<string> setList = new List<string>();

        SqliteConnection connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        if(setType > 0)
            command.CommandText = string.Format("SELECT ExpansionName FROM ExpansionList WHERE ExpansionType = '{0}'", setType.ToString());
        else
            command.CommandText = "SELECT ExpansionName FROM ExpansionList";

        SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            setList.Add(reader.GetString(0));
        }
        reader.Close();
        connection.Close();
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
        connection.Close();
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
        connection.Close();
        return textToRetun;
    }

    static public List<CardSet> GetCardSetsContainedInExpansion(string expansionName)
    {
        List<CardSet> setsToReturn = new List<CardSet>();

        SqliteConnection connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("SELECT ContainsExpansions FROM ExpansionReleases WHERE ReleaseName = '{0}'", expansionName);

        SqliteDataReader reader = command.ExecuteReader();
        reader.Read();
        if (reader.IsDBNull(0) || reader.GetString(0) == "")
        {
            reader.Close();
            connection.Close();
            return setsToReturn;
        }
        string IDList = reader.GetString(0);

        string[] IDListArray = IDList.Split(',');
        reader.Close();

        foreach(string ID in IDListArray)
        {
            setsToReturn.Add(GetCardSetWithID(int.Parse(ID)));
        }
        connection.Close();
        return setsToReturn;
    }

    static public List<CardSet> GetCardSetsContainedInExpansionFilterBySetType(string expansionName, int setType)
    {
        List<CardSet> setsToReturn = new List<CardSet>();

        SqliteConnection connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("SELECT ContainsExpansions FROM ExpansionReleases WHERE ReleaseName = '{0}'", expansionName);

        SqliteDataReader reader = command.ExecuteReader();
        reader.Read();
        if (reader.IsDBNull(0) || reader.GetString(0) == "")
        {
            reader.Close();
            connection.Close();
            return setsToReturn;
        }
        string IDList = reader.GetString(0);

        string[] IDListArray = IDList.Split(',');
        reader.Close();

        foreach (string ID in IDListArray)
        {
            setsToReturn.Add(GetCardSetWithID(int.Parse(ID), setType));
        }
        connection.Close();
        return setsToReturn;
    }

    static public List<string> GetSetIDsContainedInExpansionAsList(string expansionName)
    {
        List<string> setsToReturn = new List<string>();

        SqliteConnection connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("SELECT ContainsExpansions FROM ExpansionReleases WHERE ReleaseName = '{0}'", expansionName);

        SqliteDataReader reader = command.ExecuteReader();
        reader.Read();
        if (reader.IsDBNull(0) || reader.GetString(0) == "")
        {
            reader.Close() ;
            connection.Close();
            return setsToReturn;
        }
        string IDList = reader.GetString(0);

        string[] IDListArray = IDList.Split(',');
        reader.Close();

        foreach (string ID in IDListArray)
        {
            setsToReturn.Add(ID);
        }
        connection.Close();
        return setsToReturn;
    }

    static public string GetSetIDsContainedInExpansionAsString(string expansionName)
    {
        SqliteConnection connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("SELECT ContainsExpansions FROM ExpansionReleases WHERE ReleaseName = '{0}'", expansionName);

        SqliteDataReader reader = command.ExecuteReader();
        reader.Read();
        if (reader.IsDBNull(0) || reader.GetString(0) == "")
        {
            reader.Close();
            connection.Close();
            return null;
        }
        string SetIDS = reader.GetString(0);

        reader.Close();
        connection.Close();
        return SetIDS;
    }

    static public List<string> GetExpansionsThatContainID(string ID)
    {
        Debug.Log("Getting expansion list");
        List<string> expansionList = new List<string>();

        SqliteConnection connection = new SqliteConnection($"URI=file:{_dbPath}");
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = "SELECT ReleaseName FROM ExpansionReleases WHERE ContainsExpansions LIKE '" + ID + "'";

        SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            expansionList.Add(reader.GetString(0));
        }
        reader.Close();
        connection.Close();
        return expansionList;
    }
    #endregion
}