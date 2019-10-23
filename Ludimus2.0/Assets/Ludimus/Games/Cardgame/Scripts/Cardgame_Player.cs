using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cardgame_Player : MonoBehaviour
{
    public List<Cardgame_FigureController> Figures;

    private Cardgame_FieldController currentField;
    private Camera cam;
    public bool isMoving = false;
    public Cardgame_Controller controller;
    private Cardgame_FigureController currentFigure;
    public Queue<Cardgame_Card> Deck;
    public List<Cardgame_CombatCard> CombatCards;
    public List<Cardgame_SpellCard> SpellCards;
    private System.Random rnd = new System.Random();
    public bool isHuman = false;
    public List<Cardgame_Card> Hand = new List<Cardgame_Card>();
    public float MaxPowerBudget = 2.5f;
    public float MinPowerBudget = 2.5f;

    public float HealthValue;
    public float DmgValue;
    public float SpeedValue;
    public float BlockValue;
    public float ManaValue;

    public float minHealth;
    public float minDmg;
    public float minSpeed;
    public float minBlock;
    public float minMana;
    public bool inCombat = false;
    public float sizeMul = 100;
    public bool isInMovement = false;
    public int MaxMana = 0;
    public int CurrentMana = 0;
    private Cardgame_FieldController lastField;
    public Cardgame_PlayerUIController playerUI;

    public string Name;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        controller = GameObject.Find("GameController").GetComponent<Cardgame_Controller>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHuman)
            return;
        if (isInMovement)
            return;
        if (controller.spellController.InSpell)
            return;
        if (controller.currentPlayer != this)
            return;
            //if (currentFigure == null)
            //    return;
            if (!inCombat) {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100) && currentFigure != null)
                {
                    var newPos = GetWorldPositionOnPlane(Input.mousePosition, 5.966f);
                    var nearest = GetNearestAvailableField(newPos);
                    if (nearest.isOccupied)
                    {
                        if (nearest.indicator.figureOnField != currentFigure)
                        {
                            if (Figures.Contains(nearest.indicator.figureOnField))
                            {
                                MoveWith(nearest.indicator.figureOnField, nearest, false);
                            }
                        }
                        else
                        {
                            currentFigure = null;
                            controller.FieldList.ForEach(m => m.indicator.ResetIndicator());
                            isMoving = false;
                        }
                    }
                    else
                    {
                        if (nearest != currentField && controller.FieldList.Where(f => f.indicator.Available).Contains(nearest))
                        {
                            if(!isInMovement)
                                StartCoroutine(MoveFigure(nearest, true));
                            
                        }
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                
                currentFigure = null;
                isMoving = false;
                controller.FieldList.ForEach(m => m.indicator.ResetIndicator());
            }


            if (isMoving)
            {
                var newPos = GetWorldPositionOnPlane(Input.mousePosition, 5.966f);
                var nearest = GetNearestAvailableField(newPos);
                nearest.indicator.Highlight();
                if(lastField != null)
                {
                    if (lastField != nearest)
                        lastField.indicator.Delight();
                }
                lastField = nearest;
            }
        }
        

        if (inCombat)
        {
            var newPos = GetWorldPositionOnPlane(Input.mousePosition, 5.966f);
            var nearest = GetNearestAvailableField(newPos);
            if(nearest.indicator.Available)
                nearest.indicator.ActivateIndicator(true);
            if (lastField != null)
            {
                if (lastField != nearest)
                {
                    if(lastField.indicator.Available)
                        lastField.indicator.Highlight();
                    else
                    {
                        lastField.indicator.ResetIndicator();
                    }
                }
            }
            lastField = nearest;
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                
                if (Physics.Raycast(ray, out hit, 100))
                {
                    Debug.Log(hit.transform.gameObject.name);

                    var field = GetNearestAvailableField(hit.transform.position);
                    if(field.indicator.Available)
                        StartCoroutine(AttackEnemy(field, currentField));
                }
            }
        }
    }

    public IEnumerator MoveFigure(Cardgame_FieldController to, bool loseRange)
    {
        isInMovement = true;
        int range = (int)currentFigure.Range;
        var value = GetMovesToPoint(currentField, to, false);
        if (value < 0)
            value = 0;

        //.Where(field => field.FieldDmg > 0).ToList().ForEach(field => field.FieldIsOnPath(currentFigure));
        //var fieldsOnPath = GetFields(GetAngle(to.transform.position, currentField.transform.position), value, currentField).ToList();
        var fieldsOnPath = new List<Cardgame_FieldController>();
        fieldsOnPath.Add(currentField);
        int xMoves = currentField.ArrayPosX - to.ArrayPosX;
        int zMoves = currentField.ArrayPosY - to.ArrayPosY;

        fieldsOnPath.AddRange(GetFieldsInDirection(xMoves < 0 ? 1 : -1, 0, Mathf.Abs(xMoves), currentField));
        fieldsOnPath.AddRange(GetFieldsInDirection(0, zMoves < 0 ? 1 : -1, Mathf.Abs(zMoves), fieldsOnPath.Last()));
        fieldsOnPath = fieldsOnPath.Distinct().ToList();
        fieldsOnPath.RemoveAt(0);
        //currentFigure.Range -= value;
        foreach (var field in fieldsOnPath)
        {
            if(loseRange)
                currentFigure.Range--;
            var endPos = new Vector3(field.transform.position.x, currentFigure.transform.position.y, field.transform.position.z);
            do
            {
                yield return new WaitForEndOfFrame();
                currentFigure.transform.position = Vector3.Lerp(currentFigure.transform.position, endPos, controller.MovementSpeed * Time.deltaTime);
            } while (Vector3.Distance(endPos, currentFigure.transform.position) > 0.05f);
            
            currentFigure.transform.position = endPos;
        }
        
        
        
        to.PlaceFigureOnField(currentFigure);
        currentFigure = null;
        isMoving = false;
        isInMovement = false;
        controller.FieldList.ForEach(m => m.indicator.ResetIndicator());
    }

    public IEnumerator AttackEnemy(Cardgame_FieldController enemyField, Cardgame_FieldController currentField)
    {
        Debug.Log("AttackEnemy");
        if ((currentFigure.CurrentCard as Cardgame_CombatCard).DamageType == DamageType.AllInLine)
        {
            var angle = GetAngle(enemyField.transform.position, currentFigure.transform.position);
            //Debug.Log(angle);
            var g = Instantiate((currentFigure.CurrentCard as Cardgame_CombatCard).Ability, currentFigure.transform.position, Quaternion.Euler(0, angle, 0));
            int range = currentFigure.CurrentCard.Range;
            //int range = currentFigure.CurrentCard.Range;
            yield return StartCoroutine(g.GetComponent<Cardgame_AbilityController>().Init(currentFigure, controller, GetFields(angle, range, currentField), (currentFigure.CurrentCard as Cardgame_CombatCard).Message, currentFigure.CurrentCard));
        }
        else if((currentFigure.CurrentCard as Cardgame_CombatCard).DamageType == DamageType.OnlyOne)
        {
            var angle = GetAngle(enemyField.transform.position, currentFigure.transform.position);
            var g = Instantiate((currentFigure.CurrentCard as Cardgame_CombatCard).Ability, currentFigure.transform.position, Quaternion.Euler(0, angle, 0));
            yield return StartCoroutine(g.GetComponent<Cardgame_AbilityController>().Init(currentFigure, controller, new Cardgame_FieldController[] { enemyField }, (currentFigure.CurrentCard as Cardgame_CombatCard).Message, currentFigure.CurrentCard));
        }
        else if ((currentFigure.CurrentCard as Cardgame_CombatCard).DamageType == DamageType.AllAround)
        {
            var g = Instantiate((currentFigure.CurrentCard as Cardgame_CombatCard).Ability, currentFigure.transform.position, Quaternion.Euler(0, 0, 0));
            int range = currentFigure.CurrentCard.Range;
            yield return StartCoroutine(g.GetComponent<Cardgame_AbilityController>().Init(currentFigure, controller, GetFields(0, range, currentField).Concat(GetFields(45, range, currentField)).Concat(GetFields(90, range, currentField)).Concat(GetFields(135, range, currentField)).Concat(GetFields(180, range, currentField)).Concat(GetFields(225, range, currentField)).Concat(GetFields(270, range, currentField)).Concat(GetFields(315, range, currentField)).ToArray(), (currentFigure.CurrentCard as Cardgame_CombatCard).Message, currentFigure.CurrentCard));
        }
        inCombat = false;
        currentFigure.CurrentCard = null;
        currentFigure = null;
        controller.FieldList.ForEach(m => m.indicator.ResetIndicator());
    }

    public Cardgame_FieldController[] GetFields(float angle, int range, Cardgame_FieldController currentField)
    {
        switch (angle)
        {
            case 0:
                return GetFieldsInDirection(1, 0, range, currentField);
            case 45:
                return GetFieldsInDirection(1, 1, range, currentField);
            case 90:
                return GetFieldsInDirection(0, 1, range, currentField);
            case 135:
                return GetFieldsInDirection(-1, 1, range, currentField);
            case 180:
                return GetFieldsInDirection(-1, 0, range, currentField);
            case 225:
                return GetFieldsInDirection(-1, -1, range, currentField);
            case 270:
                return GetFieldsInDirection(0, -1, range, currentField);
            case 315:
                return GetFieldsInDirection(1, -1, range, currentField);
            default:
                
                throw new Exception("Unknown angle");
        }
    }

    protected Cardgame_FieldController[] GetFieldsInDirection(int x, int y, int range, Cardgame_FieldController currentField)
    {
        
        List<Cardgame_FieldController> fields = new List<Cardgame_FieldController>();
        int startX = currentField.ArrayPosX;
        int startY = currentField.ArrayPosY;
        for (int i = 0; i < range; i++)
        {
            if (startY + y >= 9 || startX + x >= 9 || startY + y < 0 || startX + x < 0)
            {
                return fields.Where(f => f != currentField).ToArray();
            }
            var elem = controller.FieldList.FirstOrDefault(f => f.ArrayPosX == startX + x && f.ArrayPosY == startY + y);
            if(elem != null)
                fields.Add(elem);
            startX += x;
            startY += y;
        }
        return fields.ToArray();
    }

    protected Vector3 GetCurrentFieldAndReturnHitPos(Vector3 startPos)
    {
        Ray ray = new Ray(startPos, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, ~controller.layerMask))
        {
            currentField = hit.transform.gameObject.GetComponent<Cardgame_FieldController>();
            return hit.transform.position;
        }
        return Vector3.zero;
    }

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float y)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.up, new Vector3(0, y, 0));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

    public float GetAngle(Vector3 to, Vector3 from)
    {
        to = new Vector3(Mathf.Round(to.x), to.y, Mathf.Round(to.z));
        from = new Vector3(Mathf.Round(from.x), from.y, Mathf.Round(from.z));
        if (to.x < from.x)
        {
            if (to.z == from.z)
                return 180;
            if (to.z < from.z)
                return 225;
            if (to.z > from.z)
                return 135;
        }

        if (to.x > from.x)
        {
            if (to.z == from.z)
                return 0;
            if (to.z < from.z)
                return 315;
            if (to.z > from.z)
                return 45;
        }

        if (to.x == from.x)
        {
            if (to.z < from.z)
                return 270;
            if (to.z > from.z)
                return 90;
        }
        return -1;
    }

    internal void Attack(Cardgame_FigureController cardgame_FigureController, Cardgame_FieldController currentField, bool enemy)
    {
        inCombat = true;
        currentFigure = cardgame_FigureController;
        this.currentField = currentField;
        int range = (currentFigure.CurrentCard as Cardgame_CombatCard).Range;
        ShowAvailablePositions(currentField, 1, true, enemy, true);
        controller.FieldList.Where(f => f.indicator.Available).ToList().ForEach(f => f.indicator.Highlight());
    }

    public void MoveWith(Cardgame_FigureController cardgame_FigureController, Cardgame_FieldController currentField, bool enemy)
    {
        isMoving = true;
        inCombat = false;
        currentFigure = cardgame_FigureController;
        this.currentField = currentField;
        int range = currentFigure.Range;
            
        ShowAvailablePositions(currentField, range, !enemy, enemy, false);
    }

    protected Cardgame_FieldController GetNearestAvailableField(Vector3 pos)
    {
        var nearestIndicator = controller.FieldList.OrderBy(m => Vector3.Distance(m.transform.position, pos)).FirstOrDefault();
        return nearestIndicator;
    }

    private float GetZoomMul()
    {
        var startPos = cam.ScreenToWorldPoint(Input.mousePosition);
        var newPos = startPos;
        float counter = 15;
        do
        {
            newPos = startPos + cam.transform.forward * (0.1f + counter);
            counter += 0.1f;
        } while (newPos.y > 0.5f);

        return counter;
    }

    public void ShowAvailablePositions(Cardgame_FieldController currentField, int range, bool ignoreOccupied, bool enemy, bool diagonal)
    {
        for (int i = 0; i < 7; i++)
        {
            for (int k = 0; k < 9; k++)
            {
                controller.Field[i, k].indicator.ResetIndicator();
                
                if (IsMovePossible(controller.Field[i, k], currentField, range, diagonal) && (!controller.Field[i,k].isOccupied || ignoreOccupied))
                    controller.Field[i, k].indicator.ActivateIndicator(enemy);
            }
        }
    }

    protected bool IsMovePossible(Cardgame_FieldController field, Cardgame_FieldController currentField, int range, bool diagonal)
    {
        //Debug.Log("Move to points res is: " + (GetMovesToPoint(field, currentField, currMovementType) > range));
        if (GetMovesToPoint(field, currentField, diagonal) > range)
            return false;
        
        return true;
    }

    public int GetMovesToPoint(Cardgame_FieldController field, Cardgame_FieldController currentField, bool diagonal)
    {
        int res = 0;
        //Debug.Log("Field: " + field.ArrayPosX + " " + field.ArrayPosY);
        //Debug.Log("Currentfield: " + currentField.ArrayPosX + " " + currentField.ArrayPosY);
        int xMoves = Math.Abs(currentField.ArrayPosX - field.ArrayPosX);
        int zMoves = Math.Abs(currentField.ArrayPosY - field.ArrayPosY);
        if (diagonal && xMoves == zMoves)
        {
            res = xMoves;
        }
        else
        {
            res = xMoves + zMoves;
        }
        return res;
    }

    public void Turn()
    {
        int index = 0;
        foreach (var cardSlot in controller.cardsSlots)
        {
            cardSlot.gameObject.SetActive(true);
            cardSlot.Deactivate();
            cardSlot.SetUp(Hand[index], true);
            index++;
        }
        CurrentMana += Figures.Count;
        CurrentMana = Math.Min(CurrentMana, MaxMana);
        playerUI.UpdateUI();
        inCombat = false;
        isMoving = false;
    }

    public IEnumerator SetUp(Cardgame_Controller controller, bool shouldPlace, bool randomizeStats, bool atStart, Cardgame_PlayerUIController playerUI)
    {
        this.controller = controller;
        Deck = new Queue<Cardgame_Card>();
        foreach (var card in CombatCards)
        {
            for (int i = 0; i < 2; i++)
            {
                Deck.Enqueue(card);
            }
        }
        foreach (var card in SpellCards)
        {
            for (int i = 0; i < 2; i++)
            {
                Deck.Enqueue(card);
            }
        }
        Deck = ToQueue(Shuffle(Deck.ToList()));
        
        if (randomizeStats)
        {

            foreach (var figure in Figures)
            {
                RandomizeStats(figure);
                figure.SetStats();
            }
        }
        MaxMana = Figures.Sum(f => f.ManaPoints);
        playerUI.SetUp(this);
        this.playerUI = playerUI;
        if (shouldPlace)
        {
            foreach (var figure in Figures)
            {
                figure.Range = figure.SpeedPoints;
                figure.transform.position = GetRandomPos();
                var y = -1.5f;
                if (!atStart)
                    y = 6;
                figure.transform.position = new Vector3(figure.transform.position.x, y, figure.transform.position.z);
                figure.animator.enabled = true;
                yield return new WaitForSeconds(0.5f);
            }
        }
        
    }

    private void RandomizeStats(Cardgame_FigureController figure, int count = 0)
    {
        //var dmg = minDmg + rnd.NextDouble() * 2;
        //var speed = minSpeed + rnd.NextDouble() * 2;
        //var block = minBlock + rnd.NextDouble() * 2;
        //var health = minHealth + rnd.NextDouble() * 25;
        //var mana = minMana + rnd.NextDouble() * 2;
        //var currentPowerBudget = dmg * DmgValue + speed * SpeedValue + block * BlockValue + health * HealthValue + mana * ManaValue;
        ////Debug.Log(count + " " + currentPowerBudget);
        //if ((currentPowerBudget <= MaxPowerBudget && currentPowerBudget >= MinPowerBudget) || count > 100)
        //{
        //    figure.DmgMul = (float)dmg;
        //    figure.SpeedMul = (float)speed;
        //    figure.BlockMul = (float)block;
        //    figure.Health = Convert.ToInt32(health);
        //    figure.Mana = Convert.ToInt32(mana);
        //}
        //else
        //{
        //    RandomizeStats(figure, count + 1);
        //}

        figure.SpeedPoints = 2;
        figure.ManaPoints = 1;
        figure.DmgPoints = 0;
        figure.DefensePoints = 0;
        figure.Health = 25;
    }

    private Vector3 GetRandomPos()
    {
        int x = UnityEngine.Random.Range(0, 9);
        int y = UnityEngine.Random.Range(0, 9);
        var f = controller.FieldList.Where(field => !field.isOccupied).Where(field => field.ArrayPosX == x && field.ArrayPosY == y).FirstOrDefault();
        if (f == null || f.isOccupied)
            return GetRandomPos();
        return f.transform.position;
    }

    private Vector3 NearestFreeField(Vector3 figure)
    {
        var f = controller.FieldList.Where(field => !field.isOccupied).OrderBy(field => Vector3.Distance(field.transform.position, figure)).FirstOrDefault();
        if (f.isOccupied)
            return NearestFreeField(figure);
        return f.transform.position;
    }

    public Queue<T> ToQueue<T>(List<T> list)
    {
        Queue<T> q = new Queue<T>();
        foreach (T t in list)
            q.Enqueue(t);

        return q;
    }

    public List<T> Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int num = rnd.Next(list.Count);
            T temp = list[i];
            list[i] = list[num];
            list[num] = temp;
        }
        return list;
    }

    public Cardgame_Card GetNewCard(List<Cardgame_CombatCard> cards)
    {
        return cards[rnd.Next(cards.Count)];
    }
}
