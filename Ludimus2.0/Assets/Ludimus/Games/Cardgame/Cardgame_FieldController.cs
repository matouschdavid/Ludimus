using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cardgame_FieldController : MonoBehaviour
{
    public Cardgame_MoveIndicator indicator;
    public int ArrayPosX;
    public int ArrayPosY;
    public int FieldDmg;
    public int MaxTimesDmg;
    private int currentTimesDmg = 0;
    private GameObject currentDmgObj;

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

    private Cardgame_FigureController GetPlayerOnField()
    {
        indicator.IsOccupied();
        return indicator.figureOnField;
    }



    #region Spells

    public void Raise(object f)
    {
        var figure = f as Cardgame_FigureController;
        Debug.Log("Raise");
        GetComponentInChildren<Animator>().SetTrigger("Raise");
        var target = GetPlayerOnField();

        if (target == null)
            return;
        figure.Attack(target);
    }

    public void Slam(object f)
    {
        var figure = f as Cardgame_FigureController;
        var target = GetPlayerOnField();
        if (target == null)
            return;
        figure.Attack(target);
    }

    public void Spike(object f)
    {
        var figure = f as Cardgame_FigureController;
        var spikes = Instantiate(figure.player.controller.Spikes, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        FieldDmg = figure.Dmg;
        currentTimesDmg = 0;
        currentDmgObj = spikes;
    }

    public void Spin(object f)
    {
        var figure = f as Cardgame_FigureController;
        var target = GetPlayerOnField();
        if (target == null)
            return;
        figure.Attack(target);
        figure.Block += (figure.CurrentCard as Cardgame_CombatCard).Block;
    }

    #endregion

}
