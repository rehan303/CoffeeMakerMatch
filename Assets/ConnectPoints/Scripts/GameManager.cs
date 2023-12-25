using UnityEngine;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    #region Public Declarations
    [Header("Point Properties")]
    public PointController pointPrefab;
    public int gridSize;
    public int pointVariation = 3;
    public int pointOffset = 2;
    public Color[] pointColors;

    [Header("Point Down Animation")]
    public float animationTime = 0.5f;
    #endregion
    #region Private Declarations
    private LineRenderer _line;
    private List<PointController> allSelectedPoints = new(), allPointsPool = new();
    private Camera mainCam;
    private Material _lineMat;
    private PointController _currentPoint = null;
    public PointData[] pointData;
    private bool _checkInput = true;
    #endregion
    #region Unity Methods
    private void Start()
    {
        //Setup camera
        mainCam = Camera.main;
        //Setup camera ortho size to fit all points in viewport
        //Get Aspect ratio of screen.
        var aspectRatio = (float)Screen.width / (float)Screen.height; 
        //Calculate Distance to fit all points in camera viewport
        var distance = gridSize * pointOffset + pointOffset;
        //Calculate Orthographic size
        var orthoSize = distance / aspectRatio/ 2f;
        mainCam.orthographicSize = orthoSize;
        //Setup camera position
        //Convert to nearest integer to place camera in center of all points.
        var camXY = Mathf.RoundToInt(orthoSize / 2f - pointOffset / 2f);
        mainCam.transform.position = new Vector3(camXY,camXY,-10f);

        //Setup Line Renderer
        _line = GetComponent<LineRenderer>();
        _lineMat = _line.material;

        //Generate Random Point variations with random color.
        //Avoid more variations than point colors.
        pointVariation = pointVariation > pointColors.Length ? pointColors.Length : pointVariation;
        pointData = new PointData[pointVariation];
        for (int i = 0; i < pointVariation; i++)
        {
            var d = new PointData
            {
                id = i,
                c = pointColors[i]
            };
            pointData[i] = d;
        }

        //Generate points grid
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                var pc = Instantiate(pointPrefab, new Vector2(i * pointOffset, j * pointOffset), Quaternion.identity);
                var d = pointData[Random.Range(0, pointVariation)];
                pc.SetupPoint(d);
                allPointsPool.Add(pc);
            }
        }
    }
    private void Update()
    {
        if (!_checkInput)
            return;
        if(Input.GetMouseButtonDown(0))
        {
            var ray = mainCam.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.Raycast(ray.origin, ray.direction, 20);
            if(hit.collider != null)
            {
                Debug.Log("Hit : " + hit.collider.name);
                _line.positionCount = 1;
                _line.SetPosition(0, hit.collider.transform.position);
                _currentPoint = hit.collider.GetComponent<PointController>();
                _lineMat.color = _currentPoint.CurrentPointData.c;
                allSelectedPoints.Add(_currentPoint);
            }
        }
        else if(Input.GetMouseButton(0) && _currentPoint != null)
        {
            var ray = mainCam.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.Raycast(ray.origin, ray.direction, 20);
            if (hit.collider != null)
            {
                PointController c = hit.collider.GetComponent<PointController>();
                if (c != _currentPoint)
                {
                    if (c.CurrentPointData.id == _currentPoint.CurrentPointData.id)
                    {
                        if (allSelectedPoints.Contains(c))
                        {
                            _currentPoint = c;
                            int ind = allSelectedPoints.IndexOf(c);
                            ind++;
                            while(allSelectedPoints.Count != ind)
                            {
                                allSelectedPoints.RemoveAt(allSelectedPoints.Count - 1);
                            }
                        }
                        else
                        {
                            var distance = Vector2.Distance(c.transform.position, _currentPoint.transform.position);
                            if (distance > 3)
                                return;
                            allSelectedPoints.Add(c);
                            _line.positionCount = allSelectedPoints.Count;
                            _currentPoint = c;
                        }
                        SetupLine();
                    }
                }
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            //Check if we have valid selections
            if(allSelectedPoints.Count > 2)
            {
                //Check how many dots needs to destroy and how many rows has been effected.
                //Store information to check dots to make them fall down.
                var dropList = new List<DropInfo>(); 
                var movePoints = new List<MovePointInfo>();
                foreach(var pc in allSelectedPoints)
                {
                    var pos = pc.transform.position;
                    pc.gameObject.SetActive(false);
                    var drop = dropList.Find(x => x.x == pos.x);
                    if (drop == null)
                    {
                        drop = new DropInfo
                        {
                            x = pos.x,
                            dropCount = 1
                        };
                        dropList.Add(drop);
                    }
                    else
                    {
                        drop.dropCount++;
                    }
                    var allpc = allPointsPool.FindAll(x => x.gameObject.activeSelf
                        && x.transform.position.x == pos.x && x.transform.position.y > pos.y);
                    foreach(var p in allpc)
                    {
                        var movep = movePoints.Find(x => x.point == p);
                        if (movep != null)
                        {
                            movep.endY -= pointOffset;
                        }
                        else
                        {
                            movePoints.Add(new MovePointInfo
                            {
                                startY = p.transform.position.y,
                                endY = p.transform.position.y - pointOffset,
                                point = p
                            });
                        }
                    }
                }
                //Reset points
                _line.positionCount = 0;
                allSelectedPoints.Clear();
                StartCoroutine(nameof(DropAnimation), new object[] { dropList, movePoints });
            }
            else
            {
                //Invalid selection
                _line.positionCount = 0;
                allSelectedPoints.Clear();
            }
        }
    }
    #endregion
    #region Co-routines
    private System.Collections.IEnumerator DropAnimation(object[] allInfo)
    {
        _checkInput = false;
        var dropList = (List<DropInfo>)allInfo[0];
        var movePoints = (List<MovePointInfo>)allInfo[1];
        yield return null;
        //Generate new points at the top to fall down;
        foreach(var d in dropList)
        {
            for (int di = 0; di < d.dropCount; di++)
            {
                var pos = new Vector2(d.x, gridSize * pointOffset + di * pointOffset);
                var p1 = GetPoint();
                p1.SetupPoint(pointData[Random.Range(0, pointVariation)]);
                p1.transform.position = pos;
                p1.gameObject.SetActive(true);
                movePoints.Add(new MovePointInfo {
                    startY = pos.y,
                    endY = pos.y - d.dropCount * pointOffset,
                    point = p1
                });
            }
        }

        //Move Points down
        var rate = 1f / animationTime;
        var p = 0f;
        while(p <= 1f)
        {
            p += Time.deltaTime * rate;
            foreach(var moveInfo in movePoints)
            {
                var nextY = Mathf.Lerp(moveInfo.startY, moveInfo.endY, p);
                var pos = moveInfo.point.transform.position;
                moveInfo.point.transform.position = new Vector3(pos.x, nextY);
            }
            yield return null;
        }
        //Set points position properly.
        foreach (var moveInfo in movePoints)
        {
            moveInfo.point.transform.position = new Vector3(moveInfo.point.transform.position.x,
                moveInfo.endY);
        }
        yield return null;
        _checkInput = true;
    }
    #endregion
    #region Private Methods

    /// <summary>
    /// Get the point from points pool.
    /// </summary>
    /// <returns>Return a point which is disabled</returns>
    private PointController GetPoint()
    {
        var p = allPointsPool.Find(x => !x.gameObject.activeSelf);
        if(p == null)
        {
            p = Instantiate(pointPrefab);
            allPointsPool.Add(p);
        }
        return p;
    }
    //Setup line renderer positions
    private void SetupLine()
    {
        _line.positionCount = allSelectedPoints.Count;
        int i = 0;
        foreach(var p in allSelectedPoints)
        {
            _line.SetPosition(i, p.transform.position);
            i++;
        }
    }
    #endregion
    #region Private Classes
    /// <summary>
    /// Helper class to keep x position and drop count of point to fall down.
    /// </summary>
    private class DropInfo
    {
        public float x;
        public float dropCount;
    }
    /// <summary>
    /// Helper class to keep point information and it's starting y position and ending y position.
    /// </summary>
    private class MovePointInfo
    {
        public float startY, endY;
        public PointController point;
    }
    #endregion
}
#region Public classes
/// <summary>
/// Helper class to keep point color variations.
/// </summary>
public class PointData
{
    public int id;
    public Color c;
}
#endregion