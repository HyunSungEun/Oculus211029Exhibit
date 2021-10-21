using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public GameObject[] pipeParts;
    public PipeData pipeData;
    public Color originColor;
    public Color highlightColor;

    public bool IsHighlighted;
    public bool wasHighlighted;
    public Material pipeMat;
    float highlightTime;

    private void Awake()
    {
        foreach(GameObject g in pipeParts)
        {
            Outline o =  g.AddComponent<Outline>();
            o.OutlineMode = Outline.Mode.OutlineAll;
            o.OutlineWidth = 6f;

            if(g.name== "WATER") o.OutlineWidth = 10f;

            o.OutlineColor = Color.red;
            o.enabled = false;
        }
    }
    private void Start()
    {
        originColor = pipeMat.color;
    }
    public void ShowOutline(bool outline)
    {
        foreach (GameObject g in pipeParts)
            {
                Outline o = g.GetComponent<Outline>();
                o.enabled = outline;
            }
    }
    private void Update()
    {
        if (wasHighlighted && IsHighlighted == false)
        {
            pipeMat.color = originColor;
            wasHighlighted = false;
            return;
        }
        if(!wasHighlighted && IsHighlighted == false)
        {
            pipeMat.color = originColor;
            wasHighlighted = false;
            return;
        }
        if (false==wasHighlighted && IsHighlighted == true)
        {
            highlightTime = Time.time;
            wasHighlighted = true;
            return;
        }

        pipeMat.color = 
           // highlightColor;  
        Color.Lerp(originColor, highlightColor, Mathf.Sin((Time.time - highlightTime)*3f));


    }
}
[System.Serializable]
public struct PipeData
{
    public enum PIPE_TYPE
    {
        POTABLE_WATER,
        ELECTRIC,
        COMMUNICATION
    }
    public string Name;
    public PIPE_TYPE pipeType;
}
