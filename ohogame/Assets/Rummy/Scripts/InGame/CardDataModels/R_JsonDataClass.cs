using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class R_JsonDataClass
{
    public int systime;
    public R_LoadCards value;
    public string type;
}

[System.Serializable]
public class R_LoadCards
{
    public List<string> cards;
}
