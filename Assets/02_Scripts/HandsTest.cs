using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HandsTest : MonoBehaviour
{
    enum HandSide { Left,Right}
    public OVRHand LHand;
    public OVRHand RHand;
    public bool LlastPinched,RlastPinched;
    HandSide nowDominantSide;

    public Transform trackingSpace;

    public Transform LIndicator;
    public Transform RIndicator;

    public Transform LIndicatorT;
    public Transform RIndicatorT;

    public TextMeshProUGUI debugTmp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        string d="[L_P_";
        OVRHand hand = LHand;
        Transform id = LIndicator;
        Transform idt = LIndicatorT;
        for (int i = 0; i < 2; i++) 
        {
            if (i == 1) {
                id = RIndicator;
                idt = RIndicatorT;
                hand = RHand;
                d += "\n[R_P_";
            }

                d += hand.GetFingerIsPinching(OVRHand.HandFinger.Index).ToString()
                    + "_" + hand.GetFingerPinchStrength(OVRHand.HandFinger.Index).ToString()+"]";
           
            if (hand.IsPointerPoseValid) {
                Vector3 pp = hand.PointerPose.position;

                id.position = pp;
                id.rotation = hand.PointerPose.rotation;

                idt.position = trackingSpace.TransformPoint(pp);
                idt.rotation = hand.PointerPose.rotation;
            }

        }
        d += "\n";

        if (OVRInput.IsControllerConnected(OVRInput.Controller.Hands)) //터치컨트롷ㄹ러가 하나라도 들어오면 false임
            d += "[H_C]";
        else d += "[H_N]";

        if (OVRInput.IsControllerConnected(OVRInput.Controller.Touch))
            d += "[T_C]";
        else d += "[T_N]";

        debugTmp.text = d;

        return;

        OVRHand dominantHand = LHand;
        nowDominantSide = HandSide.Left;
        for (int i = 0; i < 2; i++)
        {
            if (i == 1) { dominantHand = RHand; nowDominantSide = HandSide.Right; }
            /*
            if (LHand.IsDominantHand) { dominantHand = LHand; nowDominantSide = HandSide.Left; }
            else if (RHand.IsDominantHand) { dominantHand = RHand; nowDominantSide = HandSide.Right; }*/
            if (dominantHand == null || IsTrackingGood(dominantHand) == false)
            {
                SetLastPinched(nowDominantSide, false);

                if (nowDominantSide == HandSide.Left)
                {
                    LIndicator.gameObject. SetActive(false);
                }
                else
                {
                    RIndicator.gameObject.SetActive(false);
                }

                continue ;
            }
            
            if(nowDominantSide== HandSide.Left)
            {
                LIndicator.gameObject.SetActive(true);
                LIndicator.transform.SetPositionAndRotation(dominantHand.transform.position, dominantHand.transform.rotation);
            }
            else
            {
                RIndicator.gameObject.SetActive(true);
                RIndicator.transform.SetPositionAndRotation(dominantHand.transform.position, dominantHand.transform.rotation);
            }


            //핀치
            if (dominantHand.GetFingerIsPinching(OVRHand.HandFinger.Index) && dominantHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) == 1)
            {
                if (GetLastPinched(nowDominantSide) == false)
                {
                    RaisePinchDown(dominantHand);
                    SetLastPinched(nowDominantSide, true);
                }
            }
            else
            {
                SetLastPinched(nowDominantSide, false);
            }
        }
    }

    void RaisePinchDown(OVRHand hand)
    {
        Ray ray = new Ray(hand.transform.position , hand.transform.forward);
        GetComponent<OVRInputManager>().ShowPipeData(ray);
        /*
        GameObject id = LIndicator;
        if (nowDominantSide == HandSide.Right)
        {
            id = RIndicator;
        }

            id.transform.SetPositionAndRotation(hand.PointerPose.position, hand.PointerPose.rotation);*/
        
    }

    bool IsTrackingGood(OVRHand hand)
    {
        return hand.IsTracked && hand.IsDataValid&& hand.IsPointerPoseValid && hand.GetFingerConfidence(OVRHand.HandFinger.Index) == OVRHand.TrackingConfidence.High;
    }
    bool GetLastPinched(HandSide side)
    {
        return side == HandSide.Left ? LlastPinched : RlastPinched;
    }
    void SetLastPinched(HandSide side,bool pinch)
    {
        if (side == HandSide.Left) LlastPinched = pinch;else RlastPinched = pinch;
    }
}
