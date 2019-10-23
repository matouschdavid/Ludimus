using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cardgame_AbilityController : MonoBehaviour
{
    public Animator animator;
    public Cardgame_FigureController Figure;
    // Start is called before the first frame update
    void Start()
    {
    }

    public IEnumerator Init(Cardgame_FigureController f, Cardgame_Controller controller, Cardgame_FieldController[] fields, string message, Cardgame_Card card)
    {
        Figure = f;
        transform.parent = Figure.transform;
        
        yield return StartCoroutine(controller.DoDmgTo(fields, message, Figure, card));
        animator.enabled = true;
        //Destroy();
    }

    public void Destroy()
    {
        //Debug.Log("Destroy");
        transform.parent = null;
        Destroy(gameObject);
    }
}
