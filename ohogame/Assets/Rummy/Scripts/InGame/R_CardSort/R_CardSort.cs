using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_CardSort : IComparable<R_CardSort>
{
    public RSort_Suit Suit;
    public RSort_Kind Kind;
    public R_CardSort(RSort_Kind Kind, RSort_Suit suit)
    {
        this.Kind = Kind;
        Suit = suit;
    }

    public int CompareTo(R_CardSort other)
    {
        if (Suit > other.Suit)
        {
            return 1;
        }
        if (Suit < other.Suit)
        {
            return -1;
        }
        return Kind > other.Kind ? 1 : -1;
    }

    public override string ToString()
    {
        string k = Kind.ToString();
        string k1 = string.Empty;
        if (k.Contains("Ten10"))
            k1 = k.Substring(k.Length - 2, 2);
        else
            k1 = k[k.Length - 1].ToString();

        return $"{k1}{Suit}";
    }

}

public enum RSort_Kind
{
    A,
    Two2,
    Three3,
    Four4,
    Five5,
    Six6,
    Seven7,
    Eight8,
    Nine9,
    Ten10,
    J,
    Q,
    K,
    j,
}

public enum RSort_Suit
{
    S,
    H,
    D,
    C,
    j,
}

public enum R_CardSortOrderMethod
{
    SuitThenKind,
    KindThenSuit,
}
