using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeManager : MonoBehaviour
{
    public static PipeManager Instance;
    public Pipe[] pipes;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        pipes = GameObject.FindObjectsOfType<Pipe>();
    }
}
