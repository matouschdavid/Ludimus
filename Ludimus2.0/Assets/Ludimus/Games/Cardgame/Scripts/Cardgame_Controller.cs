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
    public List<Cardgame_PlayerUIController> PlayersUI;
    public Cardgame_Player currentPlayer;
    public GameObject DeathUI;
    public GameObject playerUI;
    
    public LayerMask layerMask;
    private int cardCounter = 0;
    public TextMeshProUGUI infoText;
    public GameObject infoUI;
    public Cardgame_SpellController spellController;
    private Cardgame_FigureController lastHoverFigure;

    internal IEnumerator DoDmgTo(Cardgame_FieldController[] fields, string message, Cardgame_FigureController figure, Cardgame_Card card)
    {
        Debug.Log("Use " + card.Name + " on " + fields.Length);
        currentPlayer.CurrentMana -= card.ManaCost;
        currentPlayer.playerUI.UpdateUI();
        currentPlayer.Hand.Remove(card);
        
        yield return StartCoroutine(spellController.UseCard(new SpellParams
        {
            From = figure,
            To = fields,
            Card = card as Cardgame_CombatCard
        }));
        
    }

    
    private Cardgame_FigureController lastFigure = null;
    private Cardgame_CardController lastCard = null;
    public List<Cardgame_FieldController> FieldList;
    public Cardgame_FieldController[,] Field;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        cardsSlots.Where(c => c.isActive).ToList().ForEach(c => c.GetComponent<Animator>().SetTrigger("Disabled"));
        ListToArray();
        
        
        for (int i = 0; i < Players.Count; i++)
        {
            yield return StartCoroutine(Players[i].SetUp(this, true, true, true, PlayersUI[i]));
        }
        currentPlayer = Players[0];
        var hand = ToStack(GetHandOfPlayer(currentPlayer));
        currentPlayer.Hand = hand.ToList();
        
        currentPlayer.Turn();
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
        if (!tmp.Exists(c => c.CardType == Cardgame_CardType.Spell))
        {
            tmp.RemoveAt(4);
            tmp.Add(player.Deck.FirstOrDefault(c => c.CardType == Cardgame_CardType.Spell));
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
        Field = new Cardgame_FieldController[7, 9];
        for(int i = 0; i < 7; i++)
        {
            for (int k = 0; k < 9; k++)
            {
                Field[i, k] = FieldList[k + 9 * i];

            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit h;
        if (Physics.Raycast(r, out h, 100))
        {
            if (h.transform.gameObject.CompareTag("Cardgame_Figure"))
            {
                var figure = h.transform.gameObject.GetComponent<Cardgame_FigureController>();
                if (figure != lastHoverFigure)
                {
                    figure.Hover(true);
                    if (lastHoverFigure != null)
                        lastHoverFigure.Hover(false);
                    lastHoverFigure = figure;
                }
            }
            else
            {
                if (lastHoverFigure != null)
                    lastHoverFigure.Hover(false);
                lastHoverFigure = null;
            }
        }
        else
        {
            if (lastHoverFigure != null)
                lastHoverFigure.Hover(false);
            lastHoverFigure = null;
        }
        if (currentPlayer == null || currentPlayer.isInMovement)
            return;
        if (ActiveCard != null)
        {
            if (ActiveCard.CardData.CardType == Cardgame_CardType.Combat)
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
                        if (Input.GetMouseButtonDown(0))
                        {
                            cardCounter++;
                            figure.AddCard(ActiveCard.CardData, figure.GetField(), false);
                            ActiveCard.GetComponent<Animator>().SetTrigger("Disabled");
                            ActiveCard.isActive = false;
                            ActiveCard = null;
                        }
                        else
                        {
                            currentPlayer.ShowAvailablePositions(GetField(hit.transform.position), 1, true, false, true);

                        }
                        lastFigure = figure;
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 100, ~layerMask))
                    {
                        var field = hit.transform.gameObject.GetComponent<Cardgame_FieldController>();
                        if (field != null)
                        {
                            if (field.indicator.Available)
                            {
                                Cardgame_FieldController[] fields = null;
                                var spell = ActiveCard.CardData as Cardgame_SpellCard;
                                if (spell.DamageType == DamageType.OnlyOne)
                                    fields = new Cardgame_FieldController[] { field };
                                else if (spell.DamageType == DamageType.Every)
                                {
                                    fields = FieldList.Where(f => f.indicator.Available).ToArray();
                                }
                                StartCoroutine(DoDmgTo(fields, spell.Message, null, spell));
                                ActiveCard.GetComponent<Animator>().SetTrigger("Disabled");
                                ActiveCard.isActive = false;
                                ActiveCard = null;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (lastFigure == null || lastFigure.CurrentCard == null)
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
                        currentPlayer.MoveWith(figure, figure.GetField(), false);
                        lastFigure = figure;
                    }
                }
            }
        }
        

        
        if (lastCard != ActiveCard && ActiveCard != null)
        {
            if(ActiveCard.CardData.CardType == Cardgame_CardType.Combat)
                FieldList.ForEach(m => m.indicator.ResetIndicator());
        }
        lastCard = ActiveCard;
        if (Input.GetMouseButtonDown(1) && ActiveCard != null)
        {
            if (currentPlayer.isHuman)
            {
                ActiveCard.Deactivate();
                ActiveCard = null;
            }
        }
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
        FieldList.ForEach(f => f.indicator.ResetIndicator());
        if(ActiveCard.CardData.CardType == Cardgame_CardType.Spell)
        {
            Func<Cardgame_FieldController, bool> predicate = f => f.indicator.figureOnField != null && f.indicator.figureOnField.player == currentPlayer;
            var spell = ActiveCard.CardData as Cardgame_SpellCard;
            if (spell.TargetType == TargetType.Enemy)
                predicate = f => f.indicator.figureOnField != null && f.indicator.figureOnField.player != currentPlayer;
            else if (spell.TargetType == TargetType.All)
                predicate = f => true;
            FieldList.Where(predicate).ToList().ForEach(field =>
            {
                field.indicator.ActivateIndicator(false);
                Debug.Log(field.name);
            });
        }
    }

    private int playerIndex = 0;

    public float MovementSpeed;
    public Transform playersListUI;

    public void EndTurn()
    {
        FieldList.ForEach(m => m.indicator.ResetIndicator());
        Debug.Log("Handsize: " + Players[playerIndex].Hand.Count);
        foreach (var card in Players[playerIndex].Hand)
        {
            Players[playerIndex].Deck.Enqueue(card);
        }
        Debug.Log("Next Turn");
        if (Players[playerIndex].isHuman)
        {

            cardsSlots.Where(c => c.isActive).ToList().ForEach(c => c.GetComponent<Animator>().SetTrigger("Disabled"));
        }
        playerIndex++;
        if (playerIndex == Players.Count)
            playerIndex = 0;
        currentPlayer = Players[playerIndex];
        var hand = ToStack(GetHandOfPlayer(currentPlayer));
        currentPlayer.Hand = hand.ToList();
        foreach (var figure in currentPlayer.Figures)
        {
            figure.Range = figure.SpeedPoints;
        }
        if (currentPlayer.isHuman)
        {
            if (Players.Where(p => !p.isHuman).Count() == 1)
            {
                if (Players.Where(p => !p.isHuman).Max(p => p.Figures.Count) <= 1)
                {
                    var pos = Vector3.one * 100;
                    pos.y = 5.966f;
                    var newEnemy = Instantiate(Enemy, pos, Quaternion.identity);
                    var newEnemyUI = Instantiate<GameObject>(playerUI, playersListUI);
                    var ui = newEnemyUI.GetComponent<Cardgame_PlayerUIController>();
                    PlayersUI.Add(ui);
                    StartCoroutine(newEnemy.GetComponent<Cardgame_Player>().SetUp(this, true, true, false, ui));
                    Players.Add(newEnemy.GetComponent<Cardgame_Player>());
                    SetText("New Enemy spawned");
                    
                }
            }
            currentPlayer.Turn();
        }
        else
        {
            var otherFigures = Players.Where(p => p.isHuman).Select(p => p.Figures).SelectMany(p => p).ToList();
            StartCoroutine((currentPlayer as Cardgame_Bot).Turn(otherFigures));
        }
    }

    private object Instantiate(GameObject playerUI, object playersListUI)
    {
        throw new NotImplementedException();
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
        return FieldList.Where(field => !field.isOccupied).OrderByDescending(field => Players[0].GetMovesToPoint(field, Players[0].Figures.OrderByDescending(figure => Players[0].GetMovesToPoint(field, figure.GetField(), false)).FirstOrDefault().GetField(), false)).FirstOrDefault();
    }
}
