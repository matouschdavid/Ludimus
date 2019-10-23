using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cardgame_FieldController : MonoBehaviour
{
    
    public Cardgame_MoveIndicator indicator;
    public int ArrayPosX;
    public int ArrayPosY;
    public int FieldDmg;
    public int MaxTimesDmg;
    public int currentTimesDmg = 0;
    public GameObject currentDmgObj;
    public bool isOccupied = false;

    public void PlaceFigureOnField(Cardgame_FigureController figure)
    {
        if (FieldDmg > 0)
        {
            figure.TakeDamage(FieldDmg);
            currentTimesDmg++;
        }

        if (currentTimesDmg >= MaxTimesDmg)
        {
            currentTimesDmg = 0;
            FieldDmg = 0;
            Destroy(currentDmgObj);
        }
    }

    public void FieldIsOnPath(Cardgame_FigureController figure)
    {
        if (FieldDmg > 0)
        {
            figure.TakeDamage((int)(FieldDmg * 0.6f));
            currentTimesDmg++;
        }

        if (currentTimesDmg >= MaxTimesDmg)
        {
            currentTimesDmg = 0;
            FieldDmg = 0;
            Destroy(currentDmgObj);
        }
    }

    public Cardgame_FigureController GetPlayerOnField()
    {
        return indicator.figureOnField;
    }

    private void OnCollisionStay(Collision collision)
    {
        isOccupied = true;
        indicator.figureOnField = collision.gameObject.GetComponent<Cardgame_FigureController>();
    }

    private void OnCollisionExit(Collision collision)
    {
        isOccupied = false;
        indicator.figureOnField = null;
    }

}
