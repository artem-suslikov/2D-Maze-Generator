using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maze;

[RequireComponent(typeof(GUI))]
[RequireComponent(typeof(CameraController))]

public class MainController : MonoBehaviour
{
    private GUI guiCtrl;
    private CameraController camCtrl;
    private MazeGenerator mazeGen;
    private (int w, int h) size;

    private void OnEnable()
    {
        guiCtrl = GetComponent<GUI>();
        camCtrl = GetComponent<CameraController>();
    }

    private void Update()
    {
        if (guiCtrl.sizeReady)
        {
            guiCtrl.sizeReady = false;
            GenerateMaze();
            camCtrl.SetCamera(size.w, size.h);
            guiCtrl.ShowMaze(mazeGen.GenerateMaze(), mazeGen.GetPath());
        }
    }
    void GenerateMaze()
    {
        size = guiCtrl.GetSize();
        mazeGen = new MazeGenerator(size.w, size.h, 1, size.h / 2, size.w - 2, size.h / 2);
    }


}
