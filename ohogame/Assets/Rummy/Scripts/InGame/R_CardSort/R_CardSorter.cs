using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class R_CardSorter : IComparer<R_CardSort>
{
    public R_CardSortOrderMethod SortBy = R_CardSortOrderMethod.SuitThenKind;

    public int Compare(R_CardSort x, R_CardSort y)
    {
        if (SortBy == R_CardSortOrderMethod.SuitThenKind)
        {
            if (x.Suit > y.Suit)
            {
                return 1;
            }
            if (x.Suit < y.Suit)
            {
                return -1;
            }
            return x.Kind > y.Kind ? 1 : -1;
        }
        if (SortBy == R_CardSortOrderMethod.KindThenSuit)
        {
            if (x.Kind > y.Kind)
            {
                return 1;
            }
            if (x.Kind < y.Kind)
            {
                return -1;
            }
            return x.Suit > y.Suit ? 1 : -1;
        }
        throw new NotImplementedException($"CardOrderMethod {SortBy} is not implemented.");
    }
}
