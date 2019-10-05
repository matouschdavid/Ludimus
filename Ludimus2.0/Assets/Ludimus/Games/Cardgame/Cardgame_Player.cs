using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cardgame_Player : MonoBehaviour
{
    public List<Cardgame_FigureController> Figures;

    private Cardgame_FieldController currentField;
    private bool isGrabbing = false;
    private Camera cam;
    public bool isMoving = false;
    public Cardgame_Controller controller;
    private Cardgame_FigureController currentFigure;
    private Cardgame_CardType mode;
    public Queue<Cardgame_Card> Deck;
    public List<Cardgame_MovementCard> MovementCards;
    public List<Cardgame_CombatCard> CombatCards;
    private System.Random rnd = new System.Random();
    public bool isHuman = false;
    public List<Cardgame_Card> Hand = new List<Cardgame_Card>();
    public float MaxPowerBudget = 2.5f;
    public float MinPowerBudget = 2.5f;

    public float HealthValue;
    public float DmgValue;
    public float SpeedValue;
    public float BlockValue;

    public float minHealth;
    public float minDmg;
    public float minSpeed;
    public float minBlock;
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
        if (currentFigure == null)
            return;
        if (mode == Cardgame_CardType.Movement)
        {
            if (Input.GetMouseButtonDown(0) && !isGrabbing)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.transform.gameObject == currentFigure.gameObject)
                    {
                        if (controller.FieldList.Where(m => m.indicator.Available).Count() == 0)
                        {
                            currentFigure.transform.position = GetCurrentFieldAndReturnHitPos(hit.transform.position);
                            currentFigure = null;
                            return;
                        }

                        isGrabbing = true;
                    }
                }
            }
            else if (Input.GetMouseButton(0) && isGrabbing)
            {
                var newPos = GetWorldPositionOnPlane(Input.mousePosition, 1.548f);

                currentFigure.transform.position = Vector3.Lerp(currentFigure.transform.position, newPos, 5 * Time.deltaTime);
            }
            else if (Input.GetMouseButtonUp(0) && isGrabbing)
            {
                var nearest = GetNearestAvailableField(currentFigure.transform.position);
                MoveFigure(nearest);
            }
        }

        if (mode == Cardgame_CardType.Combat)
        {
            if (Input.GetMouseButtonDown(0) && isGrabbing)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    Debug.Log(hit.transform.gameObject.name);
                    var nearest = GetNearestAvailableField(hit.transform.position);
                    AttackEnemy(nearest, currentField);
                }
            }
        }
    }

    protected void MoveFigure(Cardgame_FieldController to)
    {
        
        int range = (int)currentFigure.Speed;
        var value = GetMovesToPoint(currentField, to, currentFigure.MovementType);
        if (value < 0)
            value = 0;

        GetFields(GetAngle(to.transform.position, currentField.transform.position), value, currentField).Where(field => field.FieldDmg > 0).ToList().ForEach(field => field.FieldIsOnPath(currentFigure));
        currentFigure.Speed -= value;
        currentFigure.transform.position = new Vector3(to.transform.position.x, currentFigure.transform.position.y, to.transform.position.z);
        to.PlaceFigureOnField(currentFigure);
        currentFigure = null;
        controller.FieldList.ForEach(m => m.indicator.ResetIndicator());
        isGrabbing = false;
    }

    public void AttackEnemy(Cardgame_FieldController enemyField, Cardgame_FieldController currentField)
    {
        if ((currentFigure.CurrentCard as Cardgame_CombatCard).DamageType == DamageType.AllInLine)
        {
            var angle = GetAngle(enemyField.transform.position, currentFigure.transform.position);
            Debug.Log(angle);
            var g = Instantiate((currentFigure.CurrentCard as Cardgame_CombatCard).Ability, currentFigure.transform.position, Quaternion.Euler(0, angle, 0));
            int range = GetMovesToPoint(enemyField, currentField, currentFigure.MovementType);
            //int range = currentFigure.CurrentCard.Range;
            g.GetComponent<Cardgame_AbilityController>().Init(currentFigure, controller, GetFields(angle, range, currentField), (currentFigure.CurrentCard as Cardgame_CombatCard).Message);
        }
        else if((currentFigure.CurrentCard as Cardgame_CombatCard).DamageType == DamageType.OnlyOne)
        {
            var angle = GetAngle(enemyField.transform.position, currentFigure.transform.position);
            Debug.Log(angle);
            var g = Instantiate((currentFigure.CurrentCard as Cardgame_CombatCard).Ability, currentFigure.transform.position, Quaternion.Euler(0, angle, 0));
            g.GetComponent<Cardgame_AbilityController>().Init(currentFigure, controller, new Cardgame_FieldController[] { enemyField }, (currentFigure.CurrentCard as Cardgame_CombatCard).Message);
        }
        else
        {
            var g = Instantiate((currentFigure.CurrentCard as Cardgame_CombatCard).Ability, currentFigure.transform.position, Quaternion.Euler(0, 0, 0));
            int range = currentFigure.CurrentCard.Range;
            g.GetComponent<Cardgame_AbilityController>().Init(currentFigure, controller, GetFields(0, range, currentField).Concat(GetFields(45, range, currentField)).Concat(GetFields(90, range, currentField)).Concat(GetFields(135, range, currentField)).Concat(GetFields(180, range, currentField)).Concat(GetFields(225, range, currentField)).Concat(GetFields(270, range, currentField)).Concat(GetFields(315, range, currentField)).ToArray(), (currentFigure.CurrentCard as Cardgame_CombatCard).Message);
        }
        isGrabbing = false;
        controller.FieldList.ForEach(m => m.indicator.ResetIndicator());
    }

    protected Cardgame_FieldController[] GetFields(float angle, int range, Cardgame_FieldController currentField)
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
                Debug.Log("Reached End of Field");
                return fields.ToArray();
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

    protected float GetAngle(Vector3 to, Vector3 from)
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

    internal void Attack(Cardgame_FigureController cardgame_FigureController, Cardgame_FieldController currentField, Cardgame_MovementType movementType)
    {
        mode = Cardgame_CardType.Combat;
        isGrabbing = true;
        currentFigure = cardgame_FigureController;
        this.currentField = currentField;
        int range = (currentFigure.CurrentCard as Cardgame_CombatCard).Range;
        ShowAvailablePositions(currentField, range, movementType, true);
    }

    internal void MoveWith(Cardgame_FigureController cardgame_FigureController, Cardgame_FieldController currentField)
    {
        mode = Cardgame_CardType.Movement;
        currentFigure = cardgame_FigureController;
        this.currentField = currentField;
        int range = (int)currentFigure.Speed;
            
        ShowAvailablePositions(currentField, range, currentFigure.MovementType, false);
    }

    protected Cardgame_FieldController GetNearestAvailableField(Vector3 pos)
    {
        var nearestIndicator = controller.FieldList.Where(m => m.indicator.Available).OrderBy(m => Vector3.Distance(m.transform.position, pos)).FirstOrDefault();
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

    public void ShowAvailablePositions(Cardgame_FieldController currentField, int range, Cardgame_MovementType currMovementType, bool ignoreOccupied)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int k = 0; k < 9; k++)
            {
                controller.Field[i, k].indicator.ResetIndicator();
                //Debug.Log("Move to " + controller.Field[i, k].name + " is possible: " + IsMovePossible(controller.Field[i, k], currentField, range, currMovementType));
                if (IsMovePossible(controller.Field[i, k], currentField, range, currMovementType) && (!controller.Field[i,k].indicator.IsOccupied() || ignoreOccupied))
                    controller.Field[i, k].indicator.Highlight();
            }
        }
    }

    protected bool IsMovePossible(Cardgame_FieldController field, Cardgame_FieldController currentField, int range, Cardgame_MovementType currMovementType)
    {
        //Debug.Log("Move to points res is: " + (GetMovesToPoint(field, currentField, currMovementType) > range));
        if (GetMovesToPoint(field, currentField, currMovementType) > range)
            return false;
        if(currMovementType == Cardgame_MovementType.Straight)
        {
            int xMoves = Math.Abs(currentField.ArrayPosX - field.ArrayPosX);
            int zMoves = Math.Abs(currentField.ArrayPosY - field.ArrayPosY);
            return xMoves == 0 || zMoves == 0;
        }
        else if(currMovementType == Cardgame_MovementType.Diagonal)
        {
            int xMoves = Math.Abs(currentField.ArrayPosX - field.ArrayPosX);
            int zMoves = Math.Abs(currentField.ArrayPosY - field.ArrayPosY);
            return xMoves == zMoves;
        }
        return false;
    }

    public int GetMovesToPoint(Cardgame_FieldController field, Cardgame_FieldController currentField, Cardgame_MovementType currMovementType)
    {
        int res = 0;
        Debug.Log("Field: " + field.ArrayPosX + " " + field.ArrayPosY);
        Debug.Log("Currentfield: " + currentField.ArrayPosX + " " + currentField.ArrayPosY);
        int xMoves = Math.Abs(currentField.ArrayPosX - field.ArrayPosX);
        int zMoves = Math.Abs(currentField.ArrayPosY - field.ArrayPosY);
        Debug.Log("Diff: " + xMoves + " " + zMoves);
        if(currMovementType == Cardgame_MovementType.Straight)
            res = xMoves + zMoves;
        else if (currMovementType == Cardgame_MovementType.Diagonal)
            res = (xMoves + zMoves) / 2;
        return res;
    }

    public void Turn()
    {
        isMoving = true;
    }

    public IEnumerator SetUp(Cardgame_Controller controller, bool shouldPlace, bool randomizeStats)
    {
        this.controller = controller;
        Deck = new Queue<Cardgame_Card>();
        foreach (var card in MovementCards)
        {
            for (int i = 0; i < 3; i++)
            {
                Deck.Enqueue(card);
            }
        }
        foreach (var card in CombatCards)
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
                Debug.Log("New figure");
                RandomizeStats(figure);
                figure.SetStats();
            }
        }
        if (shouldPlace)
        {
            foreach (var figure in Figures)
            {
                
                figure.transform.position = GetRandomPos();
                //figure.transform.position = NearestFreeField(figure.transform.position);
                figure.transform.position = new Vector3(figure.transform.position.x, 1.548f, figure.transform.position.z);
                figure.animator.enabled = true;
                figure.GetComponent<Rigidbody>().useGravity = true;
                yield return new WaitForSeconds(1.5f);
            }
        }

        
    }

    private void RandomizeStats(Cardgame_FigureController figure, int count = 0)
    {
        var dmg = minDmg + rnd.NextDouble() * 2;
        var speed = minSpeed + rnd.NextDouble() * 2;
        var block = minBlock + rnd.NextDouble() * 2;
        var health = minHealth + rnd.NextDouble() * 25;
        var currentPowerBudget = dmg * DmgValue + speed * SpeedValue + block * BlockValue + health * HealthValue;
        Debug.Log(count + " " + currentPowerBudget);
        if ((currentPowerBudget <= MaxPowerBudget && currentPowerBudget >= MinPowerBudget) || count > 100)
        {
            figure.DmgMul = (float)dmg;
            figure.SpeedMul = (float)speed;
            figure.BlockMul = (float)block;
            figure.Health = Convert.ToInt32(health);
        }
        else
        {
            RandomizeStats(figure, count + 1);
        }
    }

    private Vector3 GetRandomPos()
    {
        int x = UnityEngine.Random.Range(0, 9);
        int y = UnityEngine.Random.Range(0, 9);
        var f = controller.FieldList.Where(field => !field.indicator.IsOccupied()).Where(field => field.ArrayPosX == x && field.ArrayPosY == y).FirstOrDefault();
        if (f == null || f.indicator.IsOccupied())
            return GetRandomPos();
        return f.transform.position;
    }

    private Vector3 NearestFreeField(Vector3 figure)
    {
        var f = controller.FieldList.Where(field => !field.indicator.IsOccupied()).OrderBy(field => Vector3.Distance(field.transform.position, figure)).FirstOrDefault();
        if (f.indicator.IsOccupied())
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

    public Cardgame_Card GetNewCard(List<Cardgame_MovementCard> cards)
    {
        return cards[rnd.Next(cards.Count)];
    }
}
