using System;
using UnityEngine;

public class Cardgame_MoveIndicator : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Transform startPoint;
    public Cardgame_FigureController figureOnField;
    public bool Available;
    private void Start()
    {
    }
    public void Highlight()
    {
        meshRenderer.enabled = true;
        Available = true;
    }

    public void ResetIndicator()
    {
        meshRenderer.enabled = false;
        Available = false;
    }

    public bool IsOccupied()
    {
        Ray ray = new Ray(startPoint.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.transform.gameObject.CompareTag("Cardgame_Figure"))
            {
                figureOnField = hit.transform.gameObject.GetComponent<Cardgame_FigureController>();
                return true;
            }
            else
                figureOnField = null;
        }
        return false;
    }
}