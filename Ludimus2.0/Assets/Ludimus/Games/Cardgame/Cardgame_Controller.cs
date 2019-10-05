using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cardgame_Controller : MonoBehaviour
{
    public GameObject Enemy;
    public List<Cardgame_CardController> cardsSlots;
    
    public Cardgame_CardController ActiveCard;
    public List<Cardgame_Player> Players;
    public Cardgame_Player currentPlayer;
    public GameObject DeathUI;
    
    
    public LayerMask layerMask;
    private int cardCounter = 0;
    public TextMeshProUGUI infoText;
    public GameObject infoUI;


    public GameObject Spikes;

    internal void DoDmgTo(Cardgame_FieldController[] fields, string message, Cardgame_FigureController figure)
    {
        foreach (var field in fields)
        {
            Debug.Log("Send message " + message + " from " + figure.name + " to " + field.name);
            Debug.Log("Deal " + figure.Dmg + " dmg");
            field.SendMessage(message, figure);
        }
        figure.Dmg = 0;
    }

    
    private Cardgame_FigureController lastFigure = null;
    private Cardgame_CardController lastCard = null;
    public List<Cardgame_FieldController> FieldList;
    public Cardgame_FieldController[,] Field;
    // Start is called before the first frame update
    void Start()
    {
        ListToArray();
        foreach (var player in Players)
        {
            StartCoroutine(player.SetUp(this, true, true));
        }
        currentPlayer = Players[0];
        var hand = ToStack(GetHandOfPlayer(currentPlayer));
        foreach (var cardSlot in cardsSlots)
        {
            cardSlot.Deactivate();
            cardSlot.SetUp(hand.Pop());
        }
    }

    private List<Cardgame_Card> GetHandOfPlayer(Cardgame_Player player)
    {
        List<Cardgame_Card> tmp = new List<Cardgame_Card>();
        foreach (var cardSlot in cardsSlots)
        {
            tmp.Add(player.Deck.Dequeue());
        }
        if (!tmp.Exists(c => c.CardType == Cardgame_CardType.Combat))
        {
            tmp.RemoveAt(2);
            tmp.Add(player.Deck.FirstOrDefault(c => c.CardType == Cardgame_CardType.Combat));
        }
        if (!tmp.Exists(c => c.CardType == Cardgame_CardType.Movement))
        {
            tmp.RemoveAt(4);
            tmp.Add(player.Deck.FirstOrDefault(c => c.CardType == Cardgame_CardType.Movement));
        }
        return tmp;
    }

    public Stack<T> ToStack<T>(List<T> list)
    {
        Stack<T> stack = new Stack<T>();
        foreach (T t in list)
            stack.Push(t);

        return stack;
    }

    private void ListToArray()
    {
        Field = new Cardgame_FieldController[9, 9];
        for(int i = 0; i < 9; i++)
        {
            for (int k = 0; k < 9; k++)
            {
                Field[i, k] = FieldList[k + 9 * i];

            }
        }
    }


    private Cardgame_CardController c;
    private Cardgame_FigureController f;
    // Update is called once per frame
    void Update()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit h;

        if (Physics.Raycast(r, out h, 100))
        {
            if (h.transform.gameObject.CompareTag("Cardgame_Card"))
            {
                if(c != null)
                    c.Hover(false);
                c = h.transform.gameObject.GetComponent<Cardgame_CardController>();
                c.Hover(true);
            }
            else
            {
                if (c != null)
                    c.Hover(false);
            }

            if (h.transform.gameObject.CompareTag("Cardgame_Figure"))
            {
                if (f != null)
                    f.Hover(false);
                f = h.transform.gameObject.GetComponent<Cardgame_FigureController>();
                f.Hover(true);
            }
            else
            {
                if (f != null)
                    f.Hover(false);
            }
        }
        else
        {
            if (c != null)
                c.Hover(false);
            if (f != null)
                f.Hover(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                var obj = hit.transform.gameObject.GetComponent<Cardgame_FigureController>();
                if (obj == null || ActiveCard == null)
                    return;
                cardCounter++;
                obj.AddCard(ActiveCard.CardData, obj.GetField(), obj.MovementType);
                ActiveCard.animator.SetTrigger("Shrink");
                ActiveCard.isActive = false;
                //StartCoroutine(MoveCardToFigure(ActiveCard, obj));
                ActiveCard = null;
            }
        }

        if(ActiveCard != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform.gameObject.CompareTag("Cardgame_Figure"))
                {
                    var figure = hit.transform.gameObject.GetComponent<Cardgame_FigureController>();
                    if (!Players[playerIndex].Figures.Contains(figure))
                        return;
                    if (lastFigure == null || figure != lastFigure)
                        FieldList.ForEach(m => m.indicator.ResetIndicator());
                    var currMovementType = Cardgame_MovementType.Straight;
                    if (ActiveCard.CardData.CardType == Cardgame_CardType.Movement)
                        currMovementType = (ActiveCard.CardData as Cardgame_MovementCard).MovementType;
                    else
                        currMovementType = figure.MovementType;

                    var range = 0;
                    if (ActiveCard.CardData.CardType == Cardgame_CardType.Movement)
                        range = (int)(ActiveCard.CardData.Range * figure.SpeedMul) + (int)figure.Speed;
                    else
                        range = (ActiveCard.CardData as Cardgame_CombatCard).Range;
                    currentPlayer.ShowAvailablePositions(GetField(hit.transform.position), range, currMovementType, ActiveCard.CardData.CardType == Cardgame_CardType.Combat);
                        

                    lastFigure = figure;
                }
            }
        }

        if(lastCard != ActiveCard && ActiveCard != null)
        {
            FieldList.ForEach(m => m.indicator.ResetIndicator());
        }
        lastCard = ActiveCard;
    }

    internal void ShowDeathScreen()
    {
        DeathUI.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private Cardgame_FieldController GetField(Vector3 startPos)
    {
        Ray ray = new Ray(startPos, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, ~layerMask))
        {
            return hit.transform.gameObject.GetComponent<Cardgame_FieldController>();
        }
        return null;
    }

    private IEnumerator MoveCardToFigure(Cardgame_CardController activeCard, Cardgame_FigureController obj)
    {
        activeCard.transform.parent = GameObject.Find("Canvas").transform;
        do
        {
            activeCard.transform.position = Vector3.Lerp(activeCard.transform.position, Camera.main.WorldToScreenPoint(obj.transform.position), 3 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        } while (Vector3.Distance(activeCard.transform.position, Camera.main.WorldToScreenPoint(obj.transform.position)) > 1);
        Destroy(activeCard.gameObject);
    }

    public void ActivateCard(Cardgame_CardController controller)
    {
        if(ActiveCard != null)
            ActiveCard.Deactivate();
        controller.Activate();
        ActiveCard = controller;
    }

    private int playerIndex = 0;
    public void EndTurn()
    {
        foreach (var card in Players[playerIndex].Hand)
        {
            Players[playerIndex].Deck.Enqueue(card);
        }
        Debug.Log("Next Turn");
        if (Players[playerIndex].isHuman)
        {
            
            cardsSlots.Where(c => c.isActive).ToList().ForEach(c => c.animator.SetTrigger("Shrink"));
        }
        playerIndex++;
        if (playerIndex == Players.Count)
            playerIndex = 0;
        currentPlayer = Players[playerIndex];
        var hand = ToStack(GetHandOfPlayer(currentPlayer));
        currentPlayer.Hand = hand.ToList();
        if (currentPlayer.isHuman)
        {
            if (Players.Where(p => !p.isHuman).Count() == 1)
            {
                if (Players.Where(p => !p.isHuman).Max(p => p.Figures.Count) <= 1)
                {
                    var pos = Vector3.one * 100;
                    pos.y = 1.548f;
                    var newEnemy = Instantiate(Enemy, pos, Quaternion.identity);
                    StartCoroutine(newEnemy.GetComponent<Cardgame_Player>().SetUp(this, true, true));
                    Players.Add(newEnemy.GetComponent<Cardgame_Player>());
                    SetText("New Enemy spawned");
                    
                }
            }
            foreach (var cardSlot in cardsSlots)
            {
                cardSlot.Deactivate();
                cardSlot.SetUp(hand.Pop());
            }
        }
        else
        {
            var otherFigures = Players.Where(p => p.isHuman).Select(p => p.Figures).SelectMany(p => p).ToList();
            StartCoroutine((currentPlayer as Cardgame_Bot).Turn(hand.ToList(), otherFigures));
        }
    }

    public void SetText(string s)
    {
        infoUI.SetActive(true);
        infoText.text = s;
        StartCoroutine(ResetText());
    }

    public IEnumerator ResetText()
    {
        yield return new WaitForSeconds(3);
        infoUI.SetActive(false);
        infoText.text = "";
    }

    private Cardgame_FieldController GetFreeField()
    {
        return FieldList.Where(field => !field.indicator.IsOccupied()).OrderByDescending(field => Players[0].GetMovesToPoint(field, Players[0].Figures.OrderByDescending(figure => Players[0].GetMovesToPoint(field, figure.GetField(), Cardgame_MovementType.Straight)).FirstOrDefault().GetField(), Cardgame_MovementType.Straight)).FirstOrDefault();
    }
}
