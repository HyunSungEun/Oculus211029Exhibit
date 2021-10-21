using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GraphDrawer gd;
    [ContextMenu("�׷����̵�")]
    public void gmt()
    {
        gd.SetGraphOrigin(transform);
    }

    [ContextMenu("�׷����׸���")]
    public void gmd()
    {
        gd.DrawGraph();
    }

    public int pointCount;
    [ContextMenu("�׷��� ���� �� �߰�")]
    public void gmpr()
    {
        float x = gd.GraphMetadata.Width.Min;
        Debug.Log(x);
        float gap = (gd.GraphMetadata.Width.Max - gd.GraphMetadata.Width.Min) / (pointCount - 1);
        gd.points = new LinkedList<Vector2>();
        string debug = "�߰��׸�_";
        for(int i = 0; i < pointCount; i++)
        {
            float posX = x + (gap * i);
            float posY = Random.Range(gd.GraphMetadata.Height.Min, gd.GraphMetadata.Height.Max);
            Vector2 pos = new Vector2(posX, posY);
            gd.points.AddLast(pos);
            debug += pos.ToString();
        }
        Debug.Log(debug + "_��");
    }

    public bool updateGraph;
    float lastChanged;
    public float term;
    private void Update()
    {
        if (!updateGraph || lastChanged+term >Time.time) return;
        float gap = (gd.GraphMetadata.Width.Max - gd.GraphMetadata.Width.Min) / (pointCount - 1);
        gd.points.RemoveFirst();
        gd.GraphMetadata.Width.Min = gd.GraphMetadata.Width.Min + gap;
        gd.GraphMetadata.Width.Max = gd.GraphMetadata.Width.Max + gap;



        gd.points.AddLast(new Vector2(gd.GraphMetadata.Width.Max, Random.Range(gd.GraphMetadata.Height.Min, gd.GraphMetadata.Height.Max)));

        gd.DrawGraph();
        lastChanged = Time.time;

    }

    public PieGraphDrawer pgd;
    public int pieCount;
    [ContextMenu("���̱׷���_�� �ֱ�")]
    public void PieInsert()
    {
        pgd.nodes = new List<PieGraphNode>();
        pgd.nodes.Add(new PieGraphNode(0.34f));
        pgd.nodes.Add(new PieGraphNode(0.06f));
        pgd.nodes.Add(new PieGraphNode(0.1f));
    //    pgd.nodes.Add(new PieGraphNode(0.4f));
        
        pgd.nodes.Add(new PieGraphNode(0.02f));
        pgd.nodes.Add(new PieGraphNode(0.08f));
        /*
        pgd.nodes.Add(new PieGraphNode(0.18f));
        pgd.nodes.Add(new PieGraphNode(0.5f));
        pgd.nodes.Add(new PieGraphNode(0.3f));*/
        Debug.Log(pgd.nodes.IsSumOfRatioZeroToOne());
        pgd.DrawGraph();
    }

}
