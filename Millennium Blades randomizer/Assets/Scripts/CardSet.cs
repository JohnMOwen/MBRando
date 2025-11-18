using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSet
{

    public enum TypeOfSet
    {
        Core = 1,
        Starter = 2,
        Expansion = 3,
        Premium = 4,
        Master = 5,
        BronzePromo = 6,
        SilverPromo = 7,
        GoldPromo = 8,
        Character = 9
    }

    public string SetName;
    public TypeOfSet SetType;

    [System.Xml.Serialization.XmlIgnore]
    public bool IncludeCardSet = true;

    public void SetInclude(bool include)
    {
        IncludeCardSet = include;
        System.Array.Find(SelectionHandler.instance.scrollViewContent.GetComponentsInChildren<TMPro.TMP_Text>(), x => x.text == SetName).transform.GetComponentInParent<UnityEngine.UI.Toggle>().SetIsOnWithoutNotify(IncludeCardSet);
    }
}