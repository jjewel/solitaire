using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Solitaire : MonoBehaviour
{
    public Sprite[] cardFaces;
    public GameObject cardPrefab;
    public GameObject[] bottomPos;
    public GameObject[] topPos;
    public GameObject deckButton;

    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    public static string[] extras = new string[] { "JR", "JB" };

    public List<string> deck;
    public List<string> discardPile = new List<string>();
    public List<string>[] bottoms;
    public List<string>[] tops;
    public List<string> tripsOnDisplay = new List<string>();
    public List<List<string>> deckTrips = new List<List<string>>();

    private List<string> bottom0 = new List<string>();
    private List<string> bottom1 = new List<string>();
    private List<string> bottom2 = new List<string>();
    private List<string> bottom3 = new List<string>();
    private List<string> bottom4 = new List<string>();
    private List<string> bottom5 = new List<string>();
    private List<string> bottom6 = new List<string>();

    private int trips;
    private int tripsRemainder;
    private int deckLocation;

    // Start is called before the first frame update
    void Start()
    {
        bottoms = new List<string>[] { bottom0, bottom1, bottom2, bottom3, bottom4, bottom5, bottom6 };
        PlayCards();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayCards()
    {
        deck = GenerateDeck();
        Shuffle(deck);
        //ShuffleSeeded(deck, 0);

        SolitaireSort();
        StartCoroutine(SolitaireDeal());
        SortDeckIntoTrips();
    }

    public static List<string> GenerateDeck(bool addExtras = false)
    {
        List<string> newDeck = new List<string>();
        foreach (string suit in suits)
        {
            foreach (string value in values)
            {
                newDeck.Add(suit + value);
            }
        }

        if (addExtras)
        {
            foreach (string extra in extras)
            {
                newDeck.Add(extra);
            }
        }

        return newDeck;
    }

    void Shuffle<T>(List<T> list)
    {
        System.Random random = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    void ShuffleSeeded<T>(List<T> list, int seed)
    {
        System.Random random = new System.Random(seed);

        int n = list.Count;
        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    IEnumerator SolitaireDeal()
    {
        for (int i = 0; i < 7; i++)
        {
            float yOffset = 0f;
            float zOffset = 0.03f;
            foreach (string card in bottoms[i])
            {
                yield return new WaitForSeconds(0.01f);
                GameObject newCard = Instantiate(
                    cardPrefab,
                    new Vector3(
                        bottomPos[i].transform.position.x,
                        bottomPos[i].transform.position.y - yOffset,
                        bottomPos[i].transform.position.z - zOffset),
                    Quaternion.identity
                );

                newCard.name = card;

                if (card == bottoms[i][bottoms[i].Count - 1])
                {
                    newCard.GetComponent<Selectable>().faceUp = true;
                }


                yOffset += 0.3f;
                zOffset += 0.03f;
                discardPile.Add(card);
            }
        }

        foreach (string card in discardPile)
        {
            if (deck.Contains(card))
            {
                deck.Remove(card);
            }
        }
        discardPile.Clear();
    }

    void SolitaireSort()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = i; j < 7; j++)
            {
                bottoms[j].Add(deck.Last<string>());
                deck.RemoveAt(deck.Count - 1);
            }
        }
    }

    public void SortDeckIntoTrips()
    {
        trips = deck.Count / 3;
        tripsRemainder = deck.Count % 3;
        deckTrips.Clear();

        int modifier = 0;
        for (int i = 0; i < trips; i++)
        {
            List<string> myTrips = new List<string>();
            for (int j = 0; j < 3; j++)
            {
                myTrips.Add(deck[j + modifier]);
            }
            deckTrips.Add(myTrips);
            modifier += 3;
        }

        if (tripsRemainder != 0)
        {
            List<string> myRemainders = new List<string>();
            modifier = 0;
            for (int k = 0; k < tripsRemainder; k++)
            {
                myRemainders.Add(deck[deck.Count - tripsRemainder + modifier]);
                modifier++;
            }
            deckTrips.Add(myRemainders);
            trips++;
        }
        deckLocation = 0;
    }

    public void DealFromDeck()
    {
        // add remaining cards to discard pile
        foreach (Transform child in deckButton.transform)
        {
            if (child.CompareTag("Card"))
            {
                discardPile.Add(child.name);
                deck.Remove(child.name);
                Destroy(child.gameObject);
            }

        }

        if (deckLocation < trips)
        {
            tripsOnDisplay.Clear();
            float xOffset = 2.5f;
            float zOffset = -0.2f;

            foreach (string card in deckTrips[deckLocation])
            {
                GameObject newTopCard = Instantiate(cardPrefab, new Vector3(deckButton.transform.position.x + xOffset, deckButton.transform.position.y, deckButton.transform.position.z + zOffset), Quaternion.identity, deckButton.transform);
                newTopCard.transform.localScale = new Vector3(1f, 1f, 1f);
                xOffset += 0.5f;
                zOffset += -0.2f;
                newTopCard.name = card;
                tripsOnDisplay.Add(card);
                newTopCard.GetComponent<Selectable>().faceUp = true;
            }
            deckLocation++;

        }
        else
        {
            RestackTopDeck();
        }
    }

    void RestackTopDeck()
    {
        foreach (string card in discardPile)
        {
            deck.Add(card);
        }
        discardPile.Clear();
        SortDeckIntoTrips();
    }
}
