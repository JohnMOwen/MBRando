using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

public class Expansion
{
    public string ExpansionName;

    [XmlArray("Set_List")]
    [XmlArrayItem("CardSet")]
    public CardSet[] mySets;

    [XmlIgnore]
    public bool IncludeExpansion = true;

    public void SetToInclude(bool include)
    {
        IncludeExpansion = include;
        foreach(CardSet set in mySets)
        {
            set.SetInclude(IncludeExpansion);
        }
    }
}
