using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    public int gridWidth = 6;
    public int gridHeight = 6;
    public GameObject[] jellyObj;
    [SerializeField]
    public GameObject[,] grid;
    public float moveSpeed;

    bool girdGenerated = false;
    private bool isMoving = false;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MakeGrid());
    }

    // Update is called once per frame
    void Update()
    {

        // Move objects down
        if (girdGenerated)
        {
            FillEmptySpace();
            StartCoroutine(MoveObjectsDownCoroutine());

        }


    }


    IEnumerator MakeGrid()
    {
        grid = new GameObject[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {

            for (int y = 0; y < gridHeight; y++)
            {
                int xPos = (int)(x * 3f);
                int yPos = (int)-(y * 3f);
                GameObject waterJelly = Instantiate(jellyObj[UnityEngine.Random.Range(0, jellyObj.Length)], new Vector3(xPos, yPos, .1f), Quaternion.identity);
                waterJelly.transform.parent = transform;
                grid[x, y] = waterJelly;
                yield return new WaitForSeconds(.01f);
            }
        }
        girdGenerated = true;
    }

    public IEnumerator MoveObjectsDownCoroutine()
    {
        if (isMoving)
        {
            yield break; // Skip the coroutine 
        }

        isMoving = true;


        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 1; y < gridHeight; y++)
            {
                if (grid[x, y] == null)
                {
                    for (int aboveY = y - 1; aboveY >= 0; aboveY--)
                    {
                        if (grid[x, aboveY] != null)
                        {
                            int xPos = (int)(x * 3f);
                            int yPos = (int)-(y * 3f);
                            // grid[x, aboveY].transform.position = new Vector3(xPos, yPos, 0);
                            yield return StartCoroutine(MoveObjectDown(grid[x, aboveY], new Vector3(xPos, yPos, .1f)));
                            grid[x, y] = grid[x, aboveY];
                            grid[x, aboveY] = null;
                            break;
                        }
                    }
                }
            }
        }

        yield return null;

        isMoving = false;

    }



    IEnumerator MoveObjectDown(GameObject obj, Vector3 targetPosition)
    {
        while (Vector3.Distance(obj.transform.position, targetPosition) > 0.01f)
        {
            obj.transform.position = Vector3.Lerp(obj.transform.position, targetPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }
        obj.transform.position = targetPosition;
    }

    public void FillEmptySpace()
    {

        for (int x = 0; x < gridWidth; x++)
        {

            if (grid[x, 0] == null)
            {
                int xPos = (int)(x * 3f);
                GameObject waterJelly = Instantiate(jellyObj[UnityEngine.Random.Range(0, jellyObj.Length)], new Vector3(xPos, 0, .1f), Quaternion.identity);
                waterJelly.transform.parent = transform;
                grid[x, 0] = waterJelly;


            }


        }

    }

    public void ActiveColl(bool active)
    {
        foreach (var obj in grid)
        {
            if (obj != null)
                obj.gameObject.GetComponent<CircleCollider2D>().enabled = active;
        }
    }

}


[Serializable]
public class GridObjAndPos
{
    public float xPos;
    public float yPos;
    public GameObject obj;
}
