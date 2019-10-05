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

    public void Init(Cardgame_FigureController f, Cardgame_Controller controller, Cardgame_FieldController[] fields, string message)
    {
        Figure = f;
        transform.parent = Figure.transform;
        animator.enabled = true;
        controller.DoDmgTo(fields, message, Figure);
        //Destroy();
    }

    public void Destroy()
    {
        Debug.Log("Destroy");
        transform.parent = null;
        Destroy(gameObject);
    }
}
