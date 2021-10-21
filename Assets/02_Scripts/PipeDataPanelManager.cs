using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PipeDataPanelManager : MonoBehaviour
{
    public static PipeDataPanelManager Instance;

    public PieGraphDrawer UIPieDrawer;
    public PieGraphDrawer Pie3DDrawer;
    public GraphDrawer graphDrawer;
    public TextMeshProUGUI pipeTypeTxt;

    public GameObject[] Panels;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Graph2DPointsReplace(new PipeData());
        Hide();
    }

    [ContextMenu("테스트")]
    public void Test()
    {
        PieGraphDrawer pgd = UIPieDrawer;
        pgd.nodes = new List<PieGraphNode>();
        pgd.nodes.Add(new PieGraphNode(0.34f));
        pgd.nodes.Add(new PieGraphNode(0.06f));
        pgd.nodes.Add(new PieGraphNode(0.1f));
        pgd.nodes.Add(new PieGraphNode(0.4f));

        pgd.nodes.Add(new PieGraphNode(0.02f));
        pgd.nodes.Add(new PieGraphNode(0.08f));
        pgd.DrawGraph();

        pgd = Pie3DDrawer;
        pgd.nodes = new List<PieGraphNode>();
        pgd.nodes.Add(new PieGraphNode(0.2f));
        pgd.nodes.Add(new PieGraphNode(0.16f));
        pgd.nodes.Add(new PieGraphNode(0.1f));
        pgd.nodes.Add(new PieGraphNode(0.31f));

        pgd.nodes.Add(new PieGraphNode(0.12f));
        pgd.nodes.Add(new PieGraphNode(0.11f));
        pgd.DrawGraph();

        Graph2DPointsReplace(new PipeData());
        graphDrawer.DrawGraph();
    }

    public void Show(Vector3 pos,PipeData pipeData)
    {
        gameObject.SetActive(true);
        transform.position = pos;
        transform.LookAt(Camera.main.transform, Vector3.up);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
       // Test();
      //  pipeTypeTxt.text = "PIPE TYPE : " + pipeData.Name;

        for(int i = 0; i < Panels.Length; i++)
        {
            if(i == (int)pipeData.pipeType)
            {
                Panels[i].SetActive(true);
            }
            else
            {
                Panels[i].SetActive(false);
            }
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    float lastChanged;
    float term = 1f;
    private void Update()
    {
        if (lastChanged + term > Time.time || graphDrawer.points.Count<=0) return;
        int pointCount = 20; //테스트 용
        float gap = (graphDrawer.GraphMetadata.Width.Max - graphDrawer.GraphMetadata.Width.Min) / (pointCount - 1);
        graphDrawer.points.RemoveFirst();
        graphDrawer.GraphMetadata.Width.Min = graphDrawer.GraphMetadata.Width.Min + gap;
        graphDrawer.GraphMetadata.Width.Max = graphDrawer.GraphMetadata.Width.Max + gap;

        graphDrawer.points.AddLast(new Vector2(graphDrawer.GraphMetadata.Width.Max, Random.Range(graphDrawer.GraphMetadata.Height.Min, graphDrawer.GraphMetadata.Height.Max)));

        graphDrawer.DrawGraph();
        lastChanged = Time.time;
    }

    void Graph2DPointsReplace(PipeData pipeData)
    {
        int pointCount = 20; //테스트 용
        float x = graphDrawer.GraphMetadata.Width.Min;
       
        float gap = (graphDrawer.GraphMetadata.Width.Max - graphDrawer.GraphMetadata.Width.Min) / (pointCount - 1);
        graphDrawer.points = new LinkedList<Vector2>();
        string debug = "추가항목_";
        for (int i = 0; i < pointCount; i++)
        {
            float posX = x + (gap * i);
            float posY = Random.Range(graphDrawer.GraphMetadata.Height.Min, graphDrawer.GraphMetadata.Height.Max);
            Vector2 pos = new Vector2(posX, posY);
            graphDrawer.points.AddLast(pos);
            debug += pos.ToString();
        }
    }

}
