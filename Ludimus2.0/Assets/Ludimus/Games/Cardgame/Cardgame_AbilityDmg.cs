using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cardgame_AbilityDmg : MonoBehaviour
{
    public Cardgame_AbilityController Parent;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided");
        if (other.CompareTag("Cardgame_Field"))
        {
            RaiseField(other.transform.gameObject);
        }
        if (!other.CompareTag("Cardgame_Figure"))
            return;
        var figure = other.transform.gameObject.GetComponent<Cardgame_FigureController>();
        figure.Attack(Parent.Figure);
    }

    private void RaiseField(GameObject field)
    {
        Debug.Log("Raise");
        field.GetComponent<Animator>().SetTrigger("Raise");
    }
}
