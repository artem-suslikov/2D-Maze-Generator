using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    [HideInInspector]
    public bool sizeReady = false;
    [HideInInspector]
    public int width = 20;
    [HideInInspector]
    public int height = 10;

    public Object cellWall;
    public Object cellWay;
    public Object cellPath;

    public Slider widthSlider;
    public Slider heightSlider;
    public GameObject menu;
    public Text widthText;
    public Text heightText;
    public GameObject subMenu;

    private List<Object> temp;

    public (int width, int height) GetSize()
    {
        (int width, int height) size;

        size.width = width;
        size.height = height;

        return size;
    }

    private void OnEnable()
    {
        ShowMenu();
        temp = new List<Object>();
    }

    public void ShowMenu()
    {
        subMenu.SetActive(false);
        menu.SetActive(true);
    }

    private void Update()
    {
        width = (int)widthSlider.value;
        widthText.text = width.ToString();
        height = (int)heightSlider.value;
        heightText.text = height.ToString();
    }

    public void GenerateButtonClick()
    {
        menu.SetActive(false);
        sizeReady = true;
    }

    public void RegenerateButtonClick()
    {
        sizeReady = true;
    }

    public void ChangeSizeButtonClick()
    {
        ShowMenu();
        RemoveMaze();
    }

    public void ShowMaze(bool[,] maze, bool[,] path)
    {
        StartCoroutine(DrawMaze(maze, path));
    }

    public void RemoveMaze()
    {
        foreach (Object o in temp)
        {
            Destroy(o);
        }
        temp.Clear();
    }

    private IEnumerator DrawMaze(bool[,] maze, bool[,] path)
    {
        subMenu.SetActive(false);
        RemoveMaze();

        for (int j = height - 1; j >= 0; j--)
        {
            for (int i = 0; i < width; i++)
            {
                if (maze[i, j])
                {
                    temp.Add(Instantiate(cellWay, new Vector3(i, j, 0), Quaternion.identity));
                }
                else
                {
                    temp.Add(Instantiate(cellWall, new Vector3(i, j, 0), Quaternion.identity));
                }

                if (path[i, j])
                {
                    temp.Add(Instantiate(cellPath, new Vector3(i, j, 0), Quaternion.identity));
                }
            }

            yield return new WaitForSeconds(3.0f / height);
        }

        subMenu.SetActive(true);
    }

    public void ExitButtonClick()
    {
        Application.Quit();
    }
}
