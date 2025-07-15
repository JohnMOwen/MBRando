using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSet
{

    public enum TypeOfSet
    {
        Core,
        Starter,
        Expansion,
        Premium,
        Master,
        BronzePromo,
        SilverPromo,
        GoldPromo,
        Character
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