using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Xml.Serialization;
using TMPro;

public class SelectionHandler : MonoBehaviour
{
    public ExpansionList expansionList;

    public GameObject SetPrefab;
    public GameObject ExpansionPrefab;
    public GameObject HeaderPrefab;
    public GameObject IndividualTextPrefab;

    public GameObject scrollViewContent;
    public TMP_Dropdown numPlayersDropdown;

    public bool CollusionSetup;

    public static SelectionHandler instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        expansionList = ExpansionList.LoadData();
        Debug.Log(expansionList.expansions[0].ExpansionName);
        foreach(CardSet set in expansionList.expansions[0].mySets)
        {
            Debug.Log("Name: " + set.SetName + ", Type: " + set.SetType);
        }

        foreach(Expansion expansion in expansionList.expansions)
        {
            GameObject expansionObj = GameObject.Instantiate(ExpansionPrefab, scrollViewContent.transform);
            expansionObj.transform.GetComponentInChildren<TMP_Text>().text = expansion.ExpansionName;
            expansionObj.GetComponent<Toggle>().onValueChanged.AddListener((value) => {expansion.SetToInclude(value); });
            string header = "";
            foreach (CardSet set in expansion.mySets)
            {
                if(set.SetType.ToString() != header)
                {
                    GameObject headerObj = GameObject.Instantiate(HeaderPrefab, scrollViewContent.transform);
                    headerObj.transform.GetComponent<TMP_Text>().text = set.SetType.ToString();
                    header = set.SetType.ToString();
                }

                GameObject setObj = GameObject.Instantiate(SetPrefab, scrollViewContent.transform);
                setObj.transform.GetComponentInChildren<TMP_Text>().text = set.SetName;
                setObj.GetComponent<Toggle>().onValueChanged.AddListener((value) => { set.IncludeCardSet = value; });
            }
        }
    }

    public void GenerateRandomSets()
    {
        List<CardSet> Characters = new List<CardSet>();
        List<CardSet> StarterDecks = new List<CardSet>();
        List<CardSet> ExpansionSets = new List<CardSet>();
        List<CardSet> PremiumSets = new List<CardSet>();
        List<CardSet> MasterSets = new List<CardSet>();
        List<CardSet> BronzePromos = new List<CardSet>();
        List<CardSet> SilverPromos = new List<CardSet>();
        List<CardSet> GoldPromos = new List<CardSet>();

        foreach(Expansion expansion in expansionList.expansions)
        {
            if (expansion.IncludeExpansion)
            {
                foreach(CardSet set in expansion.mySets)
                {
                    if (set.IncludeCardSet)
                    {
                        switch (set.SetType)
                        {
                            case CardSet.TypeOfSet.Starter:
                                StarterDecks.Add(set);
                                break;
                            case CardSet.TypeOfSet.Expansion:
                                ExpansionSets.Add(set);
                                break;
                            case CardSet.TypeOfSet.Premium:
                                PremiumSets.Add(set);
                                break;
                            case CardSet.TypeOfSet.Master:
                                MasterSets.Add(set);
                                break;
                            case CardSet.TypeOfSet.BronzePromo:
                                BronzePromos.Add(set);
                                break;
                            case CardSet.TypeOfSet.SilverPromo:
                                SilverPromos.Add(set);
                                break;
                            case CardSet.TypeOfSet.GoldPromo:
                                GoldPromos.Add(set);
                                break;
                            case CardSet.TypeOfSet.Character:
                                Characters.Add(set);
                                break;
                        }
                    }
                }
            }
        }

        List<CardSet> SelectedCharacters = new List<CardSet>();
        List<CardSet> SelectedStarterDecks = new List<CardSet>();
        List<CardSet> SelectedExpansionSets = new List<CardSet>();
        List<CardSet> SelectedPremiumSets = new List<CardSet>();
        List<CardSet> SelectedMasterSets = new List<CardSet>();
        List<CardSet> SelectedBronzePromos = new List<CardSet>();
        List<CardSet> SelectedSilverPromos = new List<CardSet>();
        List<CardSet> SelectedGoldPromos = new List<CardSet>();

        int index;
        int numPlayers = int.Parse(numPlayersDropdown.options[numPlayersDropdown.value].text);

        for (int i = 0; i < numPlayers; i++)
        {
            index = Random.Range(0, Characters.Count);
            SelectedCharacters.Add(Characters[index]);
            Characters.RemoveAt(index);

            index = Random.Range(0, StarterDecks.Count);
            SelectedStarterDecks.Add(StarterDecks[index]);
            StarterDecks.RemoveAt(index);
        }

        for(int i = 0; i < (CollusionSetup ? 6 : 5); i++)
        {
            index = Random.Range(0, ExpansionSets.Count);
            SelectedExpansionSets.Add(ExpansionSets[index]);
            ExpansionSets.RemoveAt(index);
        }

        for (int i = 0; i < (CollusionSetup ? 5 : 4); i++)
        {
            index = Random.Range(0, PremiumSets.Count);
            SelectedPremiumSets.Add(PremiumSets[index]);
            PremiumSets.RemoveAt(index);
        }

        for (int i = 0; i < (CollusionSetup ? 4 : 3); i++)
        {
            index = Random.Range(0, MasterSets.Count);
            SelectedMasterSets.Add(MasterSets[index]);
            MasterSets.RemoveAt(index);
        }

        for (int i = 0; i < 2; i++)
        {
            index = Random.Range(0, BronzePromos.Count);
            SelectedBronzePromos.Add(BronzePromos[index]);
            BronzePromos.RemoveAt(index);
        }

        for (int i = 0; i < 2; i++)
        {
            index = Random.Range(0, SilverPromos.Count);
            SelectedSilverPromos.Add(SilverPromos[index]);
            SilverPromos.RemoveAt(index);
        }

        index = Random.Range(0, GoldPromos.Count);
        SelectedGoldPromos.Add(GoldPromos[index]);
        GoldPromos.RemoveAt(index);


        for(int i = scrollViewContent.transform.childCount-1; i >= 0; i--)
        {
            Destroy(scrollViewContent.transform.GetChild(i).gameObject);
        }

        for(int i = 0; i < numPlayers; i++)
        {
            GameObject pHeader = GameObject.Instantiate(HeaderPrefab, scrollViewContent.transform);
            pHeader.GetComponent<TMP_Text>().text = "Player " + (i+1);

            GameObject character = GameObject.Instantiate(IndividualTextPrefab, scrollViewContent.transform);
            character.GetComponent<TMP_Text>().text = SelectedCharacters[i].SetName;
            
            GameObject deck = GameObject.Instantiate(IndividualTextPrefab, scrollViewContent.transform);
            deck.GetComponent<TMP_Text>().text = SelectedStarterDecks[i].SetName;
        }

        GameObject header = GameObject.Instantiate(HeaderPrefab, scrollViewContent.transform);
        header.GetComponent<TMP_Text>().text = "Expansion Sets";
        for (int i = 0; i < SelectedExpansionSets.Count; i++)
        {
            GameObject set = GameObject.Instantiate(IndividualTextPrefab, scrollViewContent.transform);
            set.GetComponent<TMP_Text>().text = SelectedExpansionSets[i].SetName;
        }

        header = GameObject.Instantiate(HeaderPrefab, scrollViewContent.transform);
        header.GetComponent<TMP_Text>().text = "Premium Sets";
        for (int i = 0; i < SelectedPremiumSets.Count; i++)
        {
            GameObject set = GameObject.Instantiate(IndividualTextPrefab, scrollViewContent.transform);
            set.GetComponent<TMP_Text>().text = SelectedPremiumSets[i].SetName;
        }
        
        header = GameObject.Instantiate(HeaderPrefab, scrollViewContent.transform);
        header.GetComponent<TMP_Text>().text = "Master Sets";
        for (int i = 0; i < SelectedMasterSets.Count; i++)
        {
            

            GameObject set = GameObject.Instantiate(IndividualTextPrefab, scrollViewContent.transform);
            set.GetComponent<TMP_Text>().text = SelectedMasterSets[i].SetName;
        }

        header = GameObject.Instantiate(HeaderPrefab, scrollViewContent.transform);
        header.GetComponent<TMP_Text>().text = "Bronze Promos";
        for (int i = 0; i < SelectedBronzePromos.Count; i++)
        {
            GameObject set = GameObject.Instantiate(IndividualTextPrefab, scrollViewContent.transform);
            set.GetComponent<TMP_Text>().text = (i == 0 ? "Store promo: " : "Prize promo: ") + SelectedBronzePromos[i].SetName;
        }

        header = GameObject.Instantiate(HeaderPrefab, scrollViewContent.transform);
        header.GetComponent<TMP_Text>().text = "Silver Promos";
        for (int i = 0; i < SelectedBronzePromos.Count; i++)
        {
            GameObject set = GameObject.Instantiate(IndividualTextPrefab, scrollViewContent.transform);
            set.GetComponent<TMP_Text>().text = (i == 0 ? "Store promo :" : "Prize promo: ") + SelectedSilverPromos[i].SetName;
        }

        header = GameObject.Instantiate(HeaderPrefab, scrollViewContent.transform);
        header.GetComponent<TMP_Text>().text = "Gold Promo";
        GameObject gSet = GameObject.Instantiate(IndividualTextPrefab, scrollViewContent.transform);
        gSet.GetComponent<TMP_Text>().text = SelectedGoldPromos[0].SetName;
    }

    public void SetCollusionSetup(bool Collusion)
    {
        CollusionSetup = Collusion;
    }
}


[XmlRoot("Expansion_Data")]
public class ExpansionList
{
    private static XmlSerializer serializer = new XmlSerializer(typeof(ExpansionList));

    [XmlArray("Expansion_List")]
    [XmlArrayItem("Expansion")]
    public List<Expansion> expansions;

    public ExpansionList()
    {
        expansions = new List<Expansion>();
    }

    public static ExpansionList LoadData()
    {
        var stream = new FileStream(Application.dataPath + "/Resources/Expansion Data.xml", FileMode.Open);

        var expansionList = serializer.Deserialize(stream) as ExpansionList;

        stream.Close();

        return expansionList;
    }
}