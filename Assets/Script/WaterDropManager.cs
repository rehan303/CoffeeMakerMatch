using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDropManager : MonoBehaviour
{
    public static WaterDropManager Instance;
    public GameObject waterDropPrefab;
    public bool isParantt = false;
    GameObject obj;
    public Color selectedColr;
    public Color[] colors = new Color[3];
    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // Check for mouse movement
        if (Input.GetMouseButton(0))
        {


            if (!isParantt)
            {
                obj = new GameObject();
                selectedColr = colors[Random.Range(0, colors.Length)];
                waterDropPrefab.GetComponent<SpriteRenderer>().color = selectedColr;
                FillGlassControler.Instance.waterPrefeb.GetComponent<SpriteRenderer>().color = selectedColr;

                isParantt = true;
            }

       
            if (isParantt)
                SpawnWaterDrop(obj);
            
        }
        if (Input.GetMouseButtonUp(0))
        {
            GridManager.Instance.ActiveColl(true);
        }
           
    }

    void SpawnWaterDrop(GameObject parant)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        GameObject dropḑ = Instantiate(waterDropPrefab, (Vector2)mousePosition + Random.insideUnitCircle * .3f, Quaternion.identity);
        dropḑ.transform.parent = parant.transform;
    }
}
