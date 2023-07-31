using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class R_CardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    //every Card has this script on it 
    //used to control the card drag and its positions
    //sets the position of the dummy card object while the card is being dragged.
    public bool canCreateGroup;
    private Image image;
    private Vector3 prevDragPos;
    private Vector3 dragPos;
    private RectTransform canvas;
    public GameObject dummyObject,showcardObject;

    public R_CardCollision leftCollider;
    public R_SUITS cardSuit;
    public int cardNumber;
    public bool isDrop;
    public bool isTriggerShowCard;
    public bool isSingleClick = false;
    public int cardIndexForConfirm, groupIndexForConfirm;

    private void Start()
    {
        canvas = GameObject.FindObjectOfType<Canvas>().gameObject.GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    public void SetCardData(int num, R_SUITS suit, GameObject dummy)
    {
        dummyObject = dummy;
        cardNumber = num;
        cardSuit = suit;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDrop)
        {
            leftCollider.SetSelected(true);
            canCreateGroup = false;
            R_CardManager.Instance.ActivateCreateGroupUI(true);
            dragPos = this.transform.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDrop)
        {
            prevDragPos = dragPos;
            this.transform.position = new Vector3(eventData.position.x, eventData.position.y + 10, 0);
            dragPos = this.transform.position;
            Vector3 moveDelta = (prevDragPos - dragPos);
            prevDragPos = this.transform.position;

            R_CardManager.Instance.group_Btn.SetActive(false);
            R_CardManager.Instance.discard_Btn.SetActive(false);
            // R_CardManager.Instance.showArea.SetActive(false);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        R_CardManager.Instance.ActivateCreateGroupUI(false);
        if (!isDrop)
        {
            leftCollider.SetSelected(false);
            R_CardManager.Instance.ResetSelectedCard();
            //CardManager.Instance.ActivateCreateGroupUI(false);
        
            R_CardManager.Instance.showArea.SetActive(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // if (Rummy_InGameManager.instance.isMyTurn && Rummy_InGameManager.instance.isCardPulledByMe)
        // {
        //     R_CardManager.Instance.showArea.SetActive(true);
        // }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
        if (!isDrop && !isSingleClick)
        {
            cardIndexForConfirm = this.transform.GetSiblingIndex();
            groupIndexForConfirm = this.transform.parent.GetSiblingIndex();
// Debug.Log("card name = " + this.transform.name);
// Debug.Log("cardIndexForConfirm = " + cardIndexForConfirm);
// Debug.Log("groupIndexForConfirm = " + groupIndexForConfirm);

            dummyObject.transform.SetParent(this.transform.parent);
            dummyObject.transform.SetSiblingIndex(this.transform.GetSiblingIndex());
            leftCollider.gameObject.SetActive(true);
            this.transform.SetParent(canvas);
            dummyObject.SetActive(true);
           // Color color = image.color;
            //color.a = .7f;
            //image.color = color;
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 10, this.transform.position.z);
            isSingleClick = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
// Debug.Log("____________ 000 ******** " + R_CardManager.Instance.GetSelectedCardCount());
        isSingleClick = false;
        if (isTriggerShowCard)
        {
            // if (R_CardManager.Instance.Player_1_Card.Count == 14)
            // {
            //     this.transform.parent = null;
            //     Color color = image.color;
            //     color.a = 1f;
            //     image.color = color;
      
            //     dummyObject.transform.SetParent(canvas);
            //     dummyObject.SetActive(false);
            //     image.raycastTarget = false;
            //     this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            //     transform.GetChild(0).gameObject.SetActive(false);
            //     transform.GetChild(1).gameObject.SetActive(false);

            //     showcardObject = R_CardManager.Instance.showArea; //collider.gameObject;

            //     // R_CardManager.Instance.Player_1_Card.RemoveAt(R_CardManager.Instance.Player_1_Card.FindIndex(x => x.name == this.gameObject.name));
            //     R_CardManager.Instance.Player_1_Card.Remove(this.gameObject);
            //     R_CardManager.Instance.CheckForEmptyGroups();
            //     this.transform.SetParent(showcardObject.transform);
            //     this.transform.localPosition = Vector3.zero;
            //     R_CardManager.Instance.isShowingCards = true;
                
            //     R_CardManager.Instance.DragOnShowCard();
            // }
            Rummy_InGameManager.instance.ShowConfirmationPanel(this.gameObject);
        }
        
        if (!isDrop)
        {
            if (canCreateGroup)
            {
                canCreateGroup = false;
                this.transform.SetParent(R_CardManager.Instance.AddGroup());
                this.transform.SetSiblingIndex(0);
            }
            else
            {
                this.transform.SetParent(dummyObject.transform.parent);
                this.transform.SetSiblingIndex(dummyObject.transform.GetSiblingIndex());
            }
            R_CardManager.Instance.CheckForEmptyGroups();
            dummyObject.transform.SetParent(canvas);
            dummyObject.SetActive(false);
            Color color = image.color;
            if(color == new Color32(200,200,200, 255))
            {
                R_CardManager.Instance.SetSelectedCardCount(-1, this.gameObject);
                image.color = new Color32(255, 255, 255, 255);
                //color.a = .7f;
                R_CardManager.Instance.showArea.SetActive(false);
            }
            else
            {
                R_CardManager.Instance.SetSelectedCardCount(1, this.gameObject);
                image.color = new Color32(200, 200, 200, 255);
                //color.a = 1f;
                R_CardManager.Instance.showArea.SetActive(false);
            }            
            //image.color = color;
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 10, this.transform.position.z);
            if(R_CardManager.Instance.isShowingCards && showcardObject!=null)
            {
                this.transform.SetParent(showcardObject.gameObject.transform);
                this.transform.localPosition = Vector3.zero;
            }
            if(R_CardManager.Instance.GetSelectedCardCount() == 1)
            {
                // Debug.Log("____________ 1st " + R_CardManager.Instance.GetSelectedCardCount());
                // R_CardManager.Instance.discard_Btn.transform.position = new Vector2(this.transform.position.x, R_CardManager.Instance.discard_Btn.transform.position.y);
                R_CardManager.Instance.discard_Btn.transform.position = new Vector2(R_CardManager.Instance.selectedCards[0].gameObject.transform.position.x, R_CardManager.Instance.discard_Btn.transform.position.y);
                if (Rummy_InGameManager.instance.isMyTurn && Rummy_InGameManager.instance.isCardPulledByMe)
                {
                    // Debug.Log("____________ 2 ******** " + Rummy_InGameManager.instance.isMyTurn);
                    R_CardManager.Instance.discard_Btn.SetActive(true);
                    R_CardManager.Instance.showArea.SetActive(true);
                }
                R_CardManager.Instance.group_Btn.SetActive(false);
            }
            else if(R_CardManager.Instance.GetSelectedCardCount() > 1)
            {
                // Debug.Log("____________" + R_CardManager.Instance.GetSelectedCardCount());
                // R_CardManager.Instance.group_Btn.transform.position = new Vector2(this.transform.position.x, R_CardManager.Instance.group_Btn.transform.position.y); //(this.transform.position.x, 135f
                R_CardManager.Instance.group_Btn.transform.position = new Vector2(R_CardManager.Instance.selectedCards[(R_CardManager.Instance.selectedCards.Count-1)].gameObject.transform.position.x, R_CardManager.Instance.group_Btn.transform.position.y);
                R_CardManager.Instance.group_Btn.SetActive(true);
                R_CardManager.Instance.discard_Btn.SetActive(false);
                R_CardManager.Instance.showArea.SetActive(false);
            }
            else
            {
                // Debug.Log("____________ 3rd" + R_CardManager.Instance.GetSelectedCardCount());
                R_CardManager.Instance.group_Btn.SetActive(false);
                R_CardManager.Instance.ActivateCreateGroupUI(false);
                R_CardManager.Instance.discard_Btn.SetActive(false);
                if (!R_CardManager.Instance.isShowingCards)
                {
                    R_CardManager.Instance.showArea.SetActive(false);
                }
            }
        }
        if (!R_CardManager.Instance.isShowingCards)
        {
            R_CardManager.Instance.SendArrangedCard();
        }
    }
    public void GetDropCard()
    {
        if (R_CardManager.Instance.Player_1_Card.Count <= 13)
        {
            // Debug.Log("____");
            transform.parent = null;
            R_CardManager.Instance.ActivateCreateGroupUI(true);

            this.transform.SetParent(R_CardManager.Instance.handCards.GetChild(R_CardManager.Instance.handCards.childCount - 1).transform);
            this.transform.SetSiblingIndex(12);
            R_CardManager.Instance.CheckForEmptyGroups();

            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 10, this.transform.position.z);
            R_CardManager.Instance.drop_Card.RemoveAt(R_CardManager.Instance.drop_Card.Count - 1);
            image.raycastTarget = true;
            this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
            R_CardManager.Instance.Player_1_Card.Add(this.gameObject);
            isDrop = false;
        }
    }
    public void SetDummyParent(Transform parent)
    {
        if (!isDrop)
        {
            dummyObject.transform.SetParent(parent);
        }
    }

    public void SetCardIndexToFill(int index)
    {
        if (!isDrop)
        {
            dummyObject.transform.SetSiblingIndex(index);
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag == "ShowCard" && R_CardManager.Instance.isShowingCards == false)
        {
            isTriggerShowCard=true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "GroupCreator")
        {
            canCreateGroup = true;
        }
        //Droping card collider
        else if (collider.tag == "DropCard" && isDrop == false)
        {
            if ((Rummy_InGameManager.instance.isMyTurn && Rummy_InGameManager.instance.isCardPulledByMe) && R_CardManager.Instance.isShowingCards == false)
            {
                // if (R_CardManager.Instance.Player_1_Card.Count == 14)
                // {
                //     isDrop = true;
                //     this.transform.SetParent(collider.gameObject.transform);
                //     this.transform.localPosition = Vector3.zero;
                //     Color color = image.color;
                //     color.a = 1f;
                //     image.color = color;
                //     R_CardManager.Instance.drop_Card.Add(gameObject);
                //     R_CardManager.Instance.CheckForEmptyGroups();
                //     dummyObject.transform.SetParent(canvas);
                //     dummyObject.SetActive(false);
                //     image.raycastTarget = false;
                //     this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                //     transform.GetChild(0).gameObject.SetActive(false);
                //     transform.GetChild(1).gameObject.SetActive(false);

                //     R_CardManager.Instance.Player_1_Card.RemoveAt(R_CardManager.Instance.Player_1_Card.FindIndex(x => x.name == this.gameObject.name));
                //     R_CardManager.Instance.CheckForEmptyGroups();
                // }


                // code from R_CardManager.Instance.OnDiscardBtn();
                // string cardName = selectedCards[0].name;
                // string cardGroupName = selectedCards[0].transform.parent.name;
                // // Debug.Log("OnDiscardBtn selectedCardName: "+cardName);
                // // Debug.Log("OnDiscardBtn selectedCard GroupName: "+cardGroupName);
                // R_SocketController.instance.SendDiscard(cardName);

                // Player_1_Card.Remove(selectedCards[0]);
                // selectedCards.RemoveAt(0);
                // if (selectedCardCount>0) {selectedCardCount--;}

                // Destroy(handCards.Find(cardGroupName+"/"+cardName).gameObject);

                // Transform cardT = handCards.Find(cardGroupName+"/"+cardName).parent;
                // // Debug.Log("parent: "+cardT.name, cardT.gameObject);
                // // Debug.Log("parent child count: "+cardT.childCount);
                // if (cardT.childCount==2)
                // {
                //     // Debug.Log("child count: 2");
                //     Destroy(cardT.gameObject);
                // }

                // discard_Btn.SetActive(false);
                // Rummy_InGameManager.instance.isCardPulledByMe=false;
                // // Debug.Log("OnDiscardBtn after destroy");
            }
        }
        else if (collider.tag == "ShowCard" && R_CardManager.Instance.isShowingCards == false)
        {
            // Debug.Log("OnTriggerEnter2D isPointerUp ShowCard");
            // if (R_CardManager.Instance.Player_1_Card.Count == 14)
            // {
            //     this.transform.parent = null;
            //     Color color = image.color;
            //     color.a = 1f;
            //     image.color = color;
      
            //     dummyObject.transform.SetParent(canvas);
            //     dummyObject.SetActive(false);
            //     image.raycastTarget = false;
            //     this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            //     transform.GetChild(0).gameObject.SetActive(false);
            //     transform.GetChild(1).gameObject.SetActive(false);

            //     showcardObject = collider.gameObject;

            //     R_CardManager.Instance.Player_1_Card.RemoveAt(R_CardManager.Instance.Player_1_Card.FindIndex(x => x.name == this.gameObject.name));
            //     R_CardManager.Instance.CheckForEmptyGroups();
            //     this.transform.SetParent(collider.gameObject.transform);
            //     this.transform.localPosition = Vector3.zero;
            //     R_CardManager.Instance.isShowingCards = true;
            //     //R_CardManager.Instance.ActivateCreateGroupUI(false);

                
            //     R_CardManager.Instance.DragOnShowCard(this.name);
            //     isPointerUp = false;
            //     Debug.Log("OnTriggerEnter2D isPointerUp ShowCard SET isPointerUp false");
            // }
        }

    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "GroupCreator")
        {
            canCreateGroup = false;
        }
        if (collider.tag == "ShowCard" && R_CardManager.Instance.isShowingCards == false)
        {
            isTriggerShowCard=false;
        }
    }

    public void FinishConfirm()
    {
        if (R_CardManager.Instance.Player_1_Card.Count == 14)
        {
            this.transform.parent = null;
            Color color = image.color;
            color.a = 1f;
            image.color = color;
    
            dummyObject.transform.SetParent(canvas);
            dummyObject.SetActive(false);
            image.raycastTarget = false;
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);

            showcardObject = R_CardManager.Instance.showArea; //collider.gameObject;

            // R_CardManager.Instance.Player_1_Card.RemoveAt(R_CardManager.Instance.Player_1_Card.FindIndex(x => x.name == this.gameObject.name));
            R_CardManager.Instance.Player_1_Card.Remove(this.gameObject);
            R_CardManager.Instance.CheckForEmptyGroups();
            this.transform.SetParent(showcardObject.transform);
            this.transform.localPosition = Vector3.zero;
            R_CardManager.Instance.isShowingCards = true;
            
            R_CardManager.Instance.DragOnShowCard();
        }
    }
}
