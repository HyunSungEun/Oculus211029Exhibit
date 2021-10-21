using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PieGraphDrawer : MonoBehaviour
{
    [SerializeField]
    Transform model3dPieParent;
    [SerializeField]
    Material[] pie3DModelMat;
    [SerializeField]
    GameObject piePrefab;
    [SerializeField]
    Transform pieGraphPlate;

    
    public List<PieGraphNode> nodes;
    public bool SetToZeroToOneRatio;

    public bool animatedDraw;
    public float animationTime;

    public bool PieAs3DModel;
    public float Pie3DBasicHeight;
    public float Pie3DHeightGap;
    public float Pie3DBasicRadius;
    public float Pie3DRunningRatio;

    bool IsDrawing;

    public Color[] pieColors;

    public void DrawGraph()
    {
        IsDrawing = false;
        //���� ratio������ ���� ([0]=���� ���� ratio)
        nodes.Sort((a, b) =>
        {
            if (a.ratio > b.ratio)
                return -1;
            if (a.ratio == b.ratio)
                return 0;
            return 1;
        });
        if (pieGraphPlate != null)
        {
            for (int j = pieGraphPlate.childCount - 1; j >= 0; j--)
            {
                DestroyImmediate(pieGraphPlate.GetChild(j).gameObject);
            }
        }
        if (model3dPieParent != null)
        {
            for (int j = model3dPieParent.childCount - 1; j >= 0; j--)
            {
                DestroyImmediate(model3dPieParent.GetChild(j).gameObject);
            }
        }
        if (SetToZeroToOneRatio) nodes.MatchRatiosZeroToOne();


        if (PieAs3DModel)
        {
            if (animatedDraw == false)
            {
                for(int i = 0; i < nodes.Count; i++)
                {
                    GameObject pie = new GameObject("pie3D" + nodes[i].ratioDrawerUse);
                    pie.transform.parent = model3dPieParent;
                    MeshFilter mf =  pie.AddComponent<MeshFilter>();
                    mf.mesh = CakeMeshCreator.GetCake(Pie3DBasicHeight+(Pie3DHeightGap* (nodes.Count-i)), Pie3DBasicRadius, nodes[i].ratioDrawerUse * 360f, 1f);
                    pie.transform.Rotate(new Vector3(0f, (nodes.GetRatioDrawerUseIncludingSumOfEarliers(i) - nodes[i].ratio) * 360f, 0f),Space.Self);
                    pie.transform.localScale = new Vector3(1f, 1f, -1f);
                    MeshRenderer mr = pie.AddComponent<MeshRenderer>();
                    mr.material = pie3DModelMat[i%nodes.Count];
                }
            }
            else
            {
                Pie3DRunningRatio = 0f;
                for (int i = 0; i < nodes.Count; i++)
                {
                    GameObject pie = new GameObject("pie3D" + nodes[i].ratioDrawerUse);
                    pie.transform.parent = model3dPieParent;
                    MeshFilter mf = pie.AddComponent<MeshFilter>();
                   // mf.mesh = CakeMeshCreator.GetCake(Pie3DBasicHeight + (Pie3DHeightGap * (nodes.Count - i)), Pie3DBasicRadius, nodes[i].ratioDrawerUse * 360f, 1f);
                    pie.transform.localScale = new Vector3(1f, 1f, -1f);
                    MeshRenderer mr = pie.AddComponent<MeshRenderer>();
                    mr.material = pie3DModelMat[i % nodes.Count];

                    pie.transform.localPosition = Vector3.zero;
                    pie.transform.localRotation = Quaternion.identity;
                    //�̸� ��ġ�� rotate���ѵ�
                    pie.transform.Rotate(new Vector3(0f, (nodes.GetRatioDrawerUseIncludingSumOfEarliers(i) - nodes[i].ratioDrawerUse) * 360f, 0f), Space.Self);
                }
                IsDrawing = true;
            }
            return;
        }


        for(int i = 0; i < nodes.Count; i++)
        {
            GameObject temp = Instantiate<GameObject>(piePrefab,pieGraphPlate);
        }
        if(animatedDraw == false)
        {
            int nodeCursur = 0;
            for(int i= nodes.Count - 1; i >= 0; i--)
            {
                Image pie = pieGraphPlate.GetChild(i).GetComponent<Image>();
                pie.fillAmount = nodes.GetRatioDrawerUseIncludingSumOfEarliers(nodeCursur);
                pie.color = pieColors[nodeCursur%pieColors.Length];
                nodeCursur++;
            }
        }
        else
        {
            IsDrawing = true;
            int nodeCursur = 0;
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                Image pie = pieGraphPlate.GetChild(i).GetComponent<Image>();
                pie.fillAmount =0f;
                pie.color = pieColors[nodeCursur % pieColors.Length];
                nodeCursur++;
            }
        }
    }
    private void Update()
    {
        //�׸��� �ִϸ��̼�
        if(animatedDraw && IsDrawing && !PieAs3DModel)
        {
            int nodeCursur = 0;
            bool allDone=true;
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                Image pie = pieGraphPlate.GetChild(i).GetComponent<Image>();

                if (pie.fillAmount >= nodes.GetRatioDrawerUseIncludingSumOfEarliers(nodeCursur))
                {
                    pie.fillAmount = nodes.GetRatioDrawerUseIncludingSumOfEarliers(nodeCursur);
                }
                else
                {
                    pie.fillAmount += Time.deltaTime * animationTime;
                    allDone = false;
                }
                nodeCursur++;
            }
            if (allDone)
            {
                IsDrawing = false;
            }
        }
        else if (animatedDraw && IsDrawing && PieAs3DModel)
        {

            Pie3DRunningRatio+=Time.deltaTime * animationTime;


            for (int i = 0; i < nodes.Count; i++)
            {
                if(nodes.GetRatioDrawerUseIncludingSumOfEarliers(i) < Pie3DRunningRatio)
                {
                    continue;
                }
                GameObject pie = model3dPieParent.GetChild(i).gameObject;
                MeshFilter mf = pie.GetComponent<MeshFilter>();

                float wholeRatio = nodes.GetRatioDrawerUseIncludingSumOfEarliers(i);
                
                if(Pie3DRunningRatio < wholeRatio - nodes[i].ratioDrawerUse)
                {
                    continue;
                }

                if (Pie3DRunningRatio >= wholeRatio)
                {
                    mf.mesh = CakeMeshCreator.GetCake(Pie3DBasicHeight + (Pie3DHeightGap * (nodes.Count - i)), Pie3DBasicRadius, nodes[i].ratioDrawerUse * 360f, 1f);
                    continue;
                }

                //�������� ratio
                mf.mesh = CakeMeshCreator.GetCake(Pie3DBasicHeight + (Pie3DHeightGap * (nodes.Count - i)), Pie3DBasicRadius, 
                  (Pie3DRunningRatio - (wholeRatio -  nodes[i].ratioDrawerUse)) * 360f, 
                    1f);
                
               // pie.transform.Rotate(new Vector3(0f, (nodes.GetRatioDrawerUseIncludingSumOfEarliers(i) - nodes[i].ratio) * 360f, 0f), Space.Self);
            }

            if(Pie3DRunningRatio > 1f/animationTime)
            {
                IsDrawing = false;
            }

        }
    }

}
[System.Serializable]
public struct PieGraphNode
{
    public float ratio;
    public string description;
    public float ratioDrawerUse;

    public PieGraphNode(float ratio, string description="") : this()
    {
        this.ratio = ratio;
        this.description = description;
        ratioDrawerUse = ratio;
    }

    public PieGraphNode(float ratio, string description, float ratioDrawerUse) : this(ratio, description)
    {
        this.ratioDrawerUse = ratioDrawerUse;
    }
}
public static class MyPieGraphExtension
{
    /// <summary>
    /// ratio�� ���� 1 ���� (0~1 �� ������ ���� �޾Ҵ���) ����
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static bool IsSumOfRatioZeroToOne(this List<PieGraphNode> list)
    {
        float result = 0f;
        foreach(var node in list)
        {
            result += node.ratio;
        }
        return result == 1f;
    }
    /// <summary>
    /// 0~1 �� �ƴ� ������ �޾��� �� 0~1�� ������ ����
    /// </summary>
    /// <param name="list"></param>
    public static void MatchRatiosZeroToOne(this List<PieGraphNode> list)
    {
        bool already = IsSumOfRatioZeroToOne(list);
        if (already)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ratio != list[i].ratioDrawerUse)
                    list[i] = new PieGraphNode(list[i].ratio, list[i].description, list[i].ratioDrawerUse);
            }
        }
        else
        {
            float result = 0f;
            foreach (var node in list)
            {
                result += node.ratio;
            }
            Debug.Log("����" + result);
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = new PieGraphNode(list[i].ratio,list[i].description, list[i].ratio / result);
            }
        }
    }
    /// <summary>
    /// ���� �ε��� = ���� ratio�� ���� �Ǿ� ���� �� Ư�� �ε����� ��ġ������ ratio�� �� ���� ���� ratio���� ���� ���� ��
    /// </summary>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static float GetRatioDrawerUseIncludingSumOfEarliers(this List<PieGraphNode> list, int index)
    {
        if (index >= list.Count) return 0f;
        float result=0f;
        for(int i = 0; i <= index; i++)
        {
            result += list[i].ratioDrawerUse;
        }
        return result;
    }
}