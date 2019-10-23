using System;
using UnityEngine;

public class Cardgame_MoveIndicator : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Material activatedMat;
    public Material hightlightedMat;
    public Material enemyMat;
    public Transform startPoint;
    public Cardgame_FigureController figureOnField;
    public bool Available;

    public void ActivateIndicator(bool enemy)
    {
        meshRenderer.enabled = true;
        if (enemy)
            meshRenderer.material = enemyMat;
        else
            meshRenderer.material = activatedMat;
        
        Available = true;
    }

    public void ResetIndicator()
    {
        meshRenderer.enabled = false;
        Available = false;
    }

    internal void Highlight()
    {
        meshRenderer.material = hightlightedMat;
    }

    internal void Delight()
    {
        meshRenderer.material = activatedMat;
    }
}