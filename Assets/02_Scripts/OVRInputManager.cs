using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRInputManager : MonoBehaviour
{
    enum HandSide { Left=0, Right }
    public GameObject LT;
    public GameObject RT;

    public Transform LTA;
    public Transform RTA;

    public Transform LControllerAvater;
    public Transform RControllerAvater;

    public OVRHand[] hands;
    public Transform[] handAvatars;
    public Transform trackingSpace;
    bool[] lastPinched;
    private void Awake()
    {
        lastPinched = new bool[2] { false, false };
    }

    // Update is called once per frame
    void Update()
    {
        /*파이프 하이라이트 설정 초기화(pipe 쪽에서 update 함수에서 하이라이트 처리,
         * bool 변수로 관리하므로 현재 update문에서 다시  true로 변경하면 하이라이트 애니메이션 유지 가능)
        */
        SetPipeHighLight(false);

        //인풋으로 컨트롤러 사용 시
        if (OVRInput.IsControllerConnected(OVRInput.Controller.Touch))
        {
            //Hand 인풋 변수 초기화(숨김)
            for (int i = 0; i < lastPinched.Length; i++)
            {
                lastPinched[i] = false;
                handAvatars[i].gameObject.SetActive(false);
            }

            //터치 컨트롤러 통해 input
            if (OVRInput.GetControllerPositionTracked(OVRInput.Controller.LTouch))
            {
                if (LControllerAvater.gameObject.activeSelf == false) LControllerAvater.gameObject.SetActive(true);
                LControllerAvater.SetPositionAndRotation(LTA.position, LTA.rotation);

                if (OVRInput.GetDown(OVRInput.Button.Three, OVRInput.Controller.Touch))
                {
                    Ray ray = new Ray(LTA.position, LTA.forward);
                    ShowPipeData(ray);
                }
            }
            else
            {
                LControllerAvater.gameObject.SetActive(false);
            }

            if (OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch))
            {
                if (RControllerAvater.gameObject.activeSelf == false) RControllerAvater.gameObject.SetActive(true);
                RControllerAvater.SetPositionAndRotation(RTA.position, RTA.rotation);

                if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.Touch))
                {
                    Ray ray = new Ray(RTA.position, RTA.forward);
                    ShowPipeData(ray);
                }
            }
            else
            {
                RControllerAvater.gameObject.SetActive(false);
            }
        }
        //Hand Input
        else if (OVRInput.IsControllerConnected(OVRInput.Controller.Hands))
        {
            LControllerAvater.gameObject.SetActive(false);
            RControllerAvater.gameObject.SetActive(false);

            for(int i = 0; i < hands.Length; i++)
            {
                OVRHand hand = hands[i];
                //핸드 트래킹 Pointer Pose 사용 가능 조건 체크
                if(hand.IsDataHighConfidence && hand.IsPointerPoseValid)
                {
                    if(!handAvatars[i].gameObject.activeSelf) handAvatars[i].gameObject.SetActive(true);
                    LineRenderer avatarLine = handAvatars[i].GetComponent<LineRenderer>();
                    //Pinch 정도에 따라 ray Indicator 길이 반영
                    avatarLine.SetPosition(1, Vector3.forward *( 1f + hand.GetFingerPinchStrength(OVRHand.HandFinger.Index)));

                    Vector3 pointerPosition = trackingSpace.TransformPoint(hand.PointerPose.position);
                    Vector3 forwardDir = trackingSpace.TransformDirection(hand.PointerPose.forward);

                    handAvatars[i].position = pointerPosition;
                    handAvatars[i].LookAt(pointerPosition + forwardDir, Vector3.up);

                    Ray ray = new Ray(pointerPosition, handAvatars[i].forward);
                    Pipe pipe=null;

                    //포커스 (ray hit) 된 배관 하이라이트 설정 
                    if (RaycastPipe(ray, out pipe))
                    {
                        pipe.IsHighlighted = true;
                    }
                    
                    if (hand.GetFingerIsPinching(OVRHand.HandFinger.Index))
                        {
                        
                            if (false == lastPinched[i])
                            {
                                lastPinched[i] = true;
                                ShowPipeData(ray);
                            }
                        }
                        else
                        {
                            lastPinched[i] = false;
                        }
                    
                }
                else
                {
                    lastPinched[i] = false;
                    handAvatars[i].gameObject.SetActive(false);
                }

            }

        }
        //어느쪽의 인풋도 없을 때
        else
        {
            LControllerAvater.gameObject.SetActive(false);
            RControllerAvater.gameObject.SetActive(false);
            for (int i = 0; i < lastPinched.Length; i++)
            {
                lastPinched[i] = false;
                handAvatars[i].gameObject.SetActive(false);
            }
        }

    }

    [ContextMenu("L Test")]
    public void lT()
    {
        Ray ray = new Ray(RTA.position, RTA.forward);
        ShowPipeData(ray);
    }

    public void ShowPipeData(Ray ray)
    {
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("PIPE"))) return;
        GameObject hitted = hit.collider.gameObject;
        
        if(hitted.name== "PipeDataPanelExitButton")
        {
            PipeDataPanelManager.Instance.Hide();
            foreach (Pipe p in PipeManager.Instance.pipes)
            {
                p.ShowOutline(false);
            }
            return;
        }
        Pipe pipe = hitted.GetComponent<Pipe>();
        if (pipe == null) return;
        Vector3 hitPose = hit.point;
        //패널 위치 Lerp 0.3f
        Vector3 offset = Vector3.Lerp(ray.origin, hitPose, 0.3f);
        PipeDataPanelManager.Instance.Show(offset, pipe.pipeData);
        //Pipe Outline 설정
        foreach(Pipe p in PipeManager.Instance.pipes)
        {
            if (p == pipe)
            {
                p.ShowOutline(true);
            }
            else
            {
                p.ShowOutline(false);
            }
        }
    }
    public void SetPipeHighLight(bool hl,Pipe pipe=null)
    {
        if (pipe == null)
        {
            foreach(Pipe p in PipeManager.Instance.pipes)
            {
                p.IsHighlighted = hl;
            }
            return;
        }
        pipe.IsHighlighted = hl;
    }
    bool RaycastPipe(Ray ray,out Pipe pipe)
    {
        pipe = null;

        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("PIPE"))) return false;
        pipe = hit.collider.gameObject.GetComponent<Pipe>();

        return pipe!=null ;
    }
}
