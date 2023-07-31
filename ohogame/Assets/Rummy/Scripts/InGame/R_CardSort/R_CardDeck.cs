using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class R_CardDeck : MonoBehaviour
{
    public List<R_CardSort> Cards;
    public List<R_CardSort> AvailbleCards;
    public List<int> kindsList;
    public List<int> suitList;
    public List<GameObject> GCards;

    public R_CardDeck()
    {
        Cards = new List<R_CardSort>();
        int numSuits = Enum.GetNames(typeof(RSort_Suit)).Length;
        int numKinds = Enum.GetNames(typeof(RSort_Kind)).Length;

        for (int suit = 0; suit < numSuits; suit++)
        {
            for (int kind = 0; kind < numKinds; kind++)
            {
                Cards.Add(new R_CardSort((RSort_Kind)kind, (RSort_Suit)suit));
            }
        }
    }

    public R_CardDeck(List<GameObject> str)
    {
        AvailbleCards = new List<R_CardSort>();
        kindsList = new List<int>();
        suitList = new List<int>();

        int numSuits = Enum.GetNames(typeof(RSort_Suit)).Length;
        int numKinds = Enum.GetNames(typeof(RSort_Kind)).Length;

        print(str.Count);
        for (int i = 0; i < str.Count; i++)
        {
            int count = str[i].name.Length;
            string firstChar = string.Empty;
            string secCharacter = string.Empty;
            if (count > 2)
            {
                firstChar = str[i].name.Substring(0, 2);
                secCharacter = str[i].name.Substring(str[i].name.Length-1,1);
            }
            else
            {
                // firstChar = str[i].name.Substring(0, 1);
                // secCharacter = str[i].name.Substring(str[i].name.Length - 1,1);
                if(str[i].name.Equals("JJ"))
                {
                    firstChar = "j";
                    secCharacter = "j";
                }
                else
                {
                    firstChar = str[i].name.Substring(0, 1);
                    secCharacter = str[i].name.Substring(str[i].name.Length - 1, 1);
                }
            }
           
            for (int kind = 0; kind < numKinds; kind++)
            {
                string k = (RSort_Kind)kind+"";
                if (k.Contains(firstChar))
                {
                    kindsList.Add(kind);
                }
            }
            for(int suit =0;suit < numSuits;suit++)
            {
                string k = (RSort_Suit)suit+"";
                if(k.Contains(secCharacter))
                {
                    suitList.Add(suit);
                }
            }
            
        }
        for(int s = 0;s<str.Count;s++)
        {
            AvailbleCards.Add(new R_CardSort((RSort_Kind)kindsList[s], (RSort_Suit)suitList[s]));
        }
       
    }

    public int CountCardsInDeck => Cards.Count;

    public R_CardSort DrawTopCard()
    {
        R_CardSort drawnCard = Cards[0];
        Cards.RemoveAt(0);
        return drawnCard;
    }

    public R_CardSort DrawBottomCard()
    {
        int lastCardIndex = CountCardsInDeck - 1;
        R_CardSort drawnCard = Cards[lastCardIndex];
        Cards.RemoveAt(lastCardIndex);
        return drawnCard;
    }

    public R_CardSort DrawRandomCard()
    {
        System.Random random = new System.Random();
        int randomCardIndex = random.Next(CountCardsInDeck);
        R_CardSort drawnCard = Cards[randomCardIndex];
        Cards.RemoveAt(randomCardIndex);
        return drawnCard;
    }

    public void AddCardOnTop(R_CardSort card)
    {
        if (!Cards.Contains(card))
        {
            Cards[0] = card;
            return;
        }
        throw new InvalidOperationException($"Deck already contains card {card}.");
    }

    public void AddCardOnBottom(R_CardSort card)
    {
        if (!Cards.Contains(card))
        {
            Cards.Add(card);
            return;
        }
        throw new InvalidOperationException($"Deck already contains card {card}.");
    }

    public void AddCardAtRandom(R_CardSort card)
    {
        if (!Cards.Contains(card))
        {
            System.Random random = new System.Random();
            Cards[random.Next(CountCardsInDeck)] = card;
            return;
        }
        throw new InvalidOperationException($"Deck already contains card {card}.");
    }

    public void Shuffle()
    {
        // Fisher-Yates shuffle method
        System.Random random = new System.Random();
        int n = CountCardsInDeck;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            R_CardSort randomCard = Cards[k];
            Cards[k] = Cards[n];
            Cards[n] = randomCard;
        }
    }

    public void Sort() => AvailbleCards.Sort();// Cards

    public void Sort(IComparer<R_CardSort> comparer) => AvailbleCards.Sort(comparer);//Cards

    public void SortCardList(List<GameObject> g, GameObject parent)
    {
        for(int i=0;i<AvailbleCards.Count;i++)
        {
            string str = string.Empty;
            str = AvailbleCards[i] + "";
            // if(str.Contains("jj"))
            //     str = "JJ";
            for (int j = 0; j < g.Count; j++)
            {
                if (str.Contains(g[j].name))
                {
                    print("Avail Card : " + AvailbleCards[i] + " g[j] : " + g[j].name);
                    g[j].transform.SetSiblingIndex(i + 1);
                    // print("Avail Card SetSiblingIndex : " + g[j].transform.GetSiblingIndex());
                    if (str.Equals("jj"))
                        g[j].transform.SetAsLastSibling();
                    // break;
                }
            }
        }
        // for(int i=0;i<AvailbleCards.Count;i++)
        // {
        //     // g[i] = GameObject.Find(AvailbleCards[i].ToString());
        //     // g[i].transform.SetSiblingIndex(i+1);

        //     if (AvailbleCards[i].ToString().Contains("jj"))
        //     {
        //         print("Available Cards : " + AvailbleCards[i]+"");
        //         g[i] = GameObject.Find("JJ");
        //         g[i].transform.SetSiblingIndex(i + 1);
        //     }
        //     else
        //     {
        //         g[i] = GameObject.Find(AvailbleCards[i]+"");
        //         g[i].transform.SetSiblingIndex(i + 1);
        //     }
        // }
    }

    public void WriteToConsole(bool isCreateGroup)
    {
        if (isCreateGroup)
        {
            List<List<R_CardSort>> newList = new List<List<R_CardSort>>();
            var groupByLastNamesQuery =
            from student in AvailbleCards
            group student by student.Suit into newGroup
            orderby newGroup.Key
            select newGroup;

            foreach (var nameGroup in groupByLastNamesQuery)
            {
                //Debug.Log($"Key: {nameGroup.Key}");
                foreach (var student in nameGroup)
                {
                    string k = student.Kind.ToString();
                    string k1 = string.Empty;
                    if (k.Contains("Ten10"))
                        k1 = k.Substring(k.Length - 2, 2);
                    else
                        k1 = k[k.Length - 1].ToString();
                    // Debug.Log($"{k1}{student.Suit}");
                }
                //nameGroup.ToArray<Card>();
                newList.Add(nameGroup.ToList<R_CardSort>());
            }
            string output = string.Empty;
            for (int i = 0; i < newList.Count; i++)
            {
                output += "[";
                for (int j = 0; j < newList[i].Count; j++)
                {
                    if (j == newList[i].Count - 1)
                        output += "\"" + newList[i][j] + "\"]";
                    else
                        output += "\"" + newList[i][j] + "\",";
                }
                if (i != newList.Count - 1)
                    output += ",";
            }
            print(output);
        }
        else
        {
            foreach(R_CardSort card in AvailbleCards)
            {
                string k = card.Kind.ToString();
                string k1 = string.Empty;
                if (k.Contains("Ten10"))
                    k1 = k.Substring(k.Length - 2, 2);
                else
                    k1 = k[k.Length - 1].ToString();
                // Debug.Log($"{k1}{card.Suit}");
            }

        }
    }
}
