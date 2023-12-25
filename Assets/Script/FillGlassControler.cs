using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillGlassControler : MonoBehaviour
{
    public static FillGlassControler Instance;
    public GameObject waterPrefeb;
    public Transform fillPoint;
    // Start is called before the first frame update
    float time;

    public float spawnInterval = 0.01f; // Adjust the interval between spawns
    private float lastSpawnTime;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }


    public IEnumerator FillGlass(float t)

    {
        waterPrefeb.GetComponent<SpriteRenderer>().color = WaterDropManager.Instance.selectedColr;
        time += Time.deltaTime;

        while (time <= t)
        {

            // Spawn a water drop at the mouse position
            GameObject drop = Instantiate(waterPrefeb, (Vector2)fillPoint.position, Quaternion.identity);
            drop.transform.parent = transform;
            yield return new WaitForSeconds(0.1f);

            yield return null;

        }
        time = 0;
    }

}
