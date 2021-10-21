using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GraphDrawer : MonoBehaviour
{
    [SerializeField]
    GraphMetadata graphMetadata;
    public GraphMetadata GraphMetadata { get { return graphMetadata; } set { graphMetadata = value; } }
    /// <summary>
    /// Left_Bottom of Graph Rect
    /// </summary>
    Transform graphOrigin;
    public LinkedList<Vector2> points;
    private void Start()
    {
        graphMetadata = new GraphMetadata(new GraphAxisData(3.5f, 6.8f, 0.85f, 0.03f,0), 
            new GraphAxisData(7.1f, 15.8f,0.3f, 0.03f, 0));

        graphOrigin = transform;
    }

    public void SetGraphOrigin(Transform wOrigin) => SetGraphOrigin(new Pose(wOrigin.position,wOrigin.rotation));
    public void SetGraphOrigin(Pose pose)
    {
        graphOrigin.SetPositionAndRotation(pose.position, pose.rotation);
    }
    public GameObject markerPrefab;
    public Transform markers;
    public Transform wAxisLabel;
    public Transform hAxisLabel;
    public void DrawGraph()
    {
        for(int j = markers.childCount-1; j >= 0; j--)
        {
            Destroy(markers.GetChild(j).gameObject);
        }

        foreach(Vector2 point in points)
        {
            GameObject marker = Instantiate<GameObject>(markerPrefab);
            marker.transform.parent = markers;
            marker.transform.position = transform.TransformPoint(GetGraphPositionOfPoint(point))  ;
            marker.transform.rotation = graphOrigin.rotation;
        }
        //���̺� ���� ����
        wAxisLabel.localScale = new Vector3(graphMetadata.Width.AxisLength, graphMetadata.Width.labelMargin, 1f);
        hAxisLabel.localScale = new Vector3( graphMetadata.Height.labelMargin, graphMetadata.Height.AxisLength, 1f);

        for (int m = 0; m < markers.childCount; m++)
        {
            if(m+1 == markers.childCount)
            {

            }
            else
            {
                //�� marker�� linerenderer ����
                markers.GetChild(m).GetComponent<GraphMarker>().DrawLine(markers.GetChild(m + 1).transform);
            }
        }
    }
    /// <summary>
    /// Graph Origin ����(����) x,y �� ���, 0~1�� ��ȯ �� �׷��� �� ���� ���� ������ ��ǥ ���
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    Vector3 GetGraphPositionOfPoint(Vector2 point)
    {
        Vector3 zeroToOnePosition = default(Vector3);
        zeroToOnePosition.x = ((point.x - graphMetadata.Width.Min) / (graphMetadata.Width.Max - graphMetadata.Width.Min))* graphMetadata.Width.AxisLength;
        zeroToOnePosition.y = ((point.y - graphMetadata.Height.Min) /( graphMetadata.Height.Max - graphMetadata.Height.Min))* graphMetadata.Height.AxisLength;
        return zeroToOnePosition;
    }
}
[System.Serializable]
public class GraphMetadata
{
    public GraphAxisData Width, Height;

    public GraphMetadata(GraphAxisData width, GraphAxisData height)
    {
        Width = width;
        Height = height;
    }
}
[System.Serializable]
public class GraphAxisData
{

    public float Min, Max,AxisLength,labelMargin;
    int division;

    public GraphAxisData(float min, float max, float axisLength, float labelMargin, int division)
    {
        Min = min;
        Max = max;
        AxisLength = axisLength;
        this.labelMargin = labelMargin;
        this.division = division;
    }

    public int Division 
    { 
        set
        {
            if (value < 0)
            {
                division = 0;
                return; 
            }
            division = value; 
        } 
        get 
        { 
            return division;
        } 
    }
}