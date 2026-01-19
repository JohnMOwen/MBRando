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
    [SerializeField]
    TMPro.TMP_Text setListType;
    // Start is called before the first frame update
    void Start()
    {
        UpdateSetListDropdown();
    }

    void UpdateSetListDropdown()
    {
        setListDropdown.ClearOptions();
        setListDropdown.AddOptions(DatabaseHandler.GetFullSetList());
        UpdateSetListSetType();
    }

    public void AddSetToDatabase()
    {
        DatabaseHandler.AddSetToDatabase(setName.text, cardTypeDropdown.value+1);
        
        UpdateSetListDropdown();
    }

    public void RemoveSetFromDatabase()
    {
        DatabaseHandler.RemoveSetFromAllExpansions(DatabaseHandler.GetSetIDFromName(setListDropdown.options[setListDropdown.value].text).ToString());

        DatabaseHandler.RemoveSetFromDatabase(setListDropdown.options[setListDropdown.value].text);

        UpdateSetListDropdown();
    }

    public void UpdateSetInDatabase()
    {
        DatabaseHandler.UpdateSetInDatabase(setName.text, cardTypeDropdown.value + 1, setListDropdown.options[setListDropdown.value].text);

        UpdateSetListDropdown();
    }

    public void UpdateSetListSetType()
    {
        setListType.text = DatabaseHandler.GetExpansionTypeFromName(setListDropdown.options[setListDropdown.value].text);
    }
}
