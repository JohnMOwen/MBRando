using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

public class Expansion
{
    public string ExpansionName;

    public CardSet[] mySets;

    public List<int> mySetsList = new List<int>();

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
