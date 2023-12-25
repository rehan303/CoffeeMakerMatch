using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetect : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
            if (collision.collider.gameObject.CompareTag("Water"))
            {
                GridManager.Instance.ActiveColl(false);

                Destroy(collision.collider.gameObject.transform.parent.gameObject, .2f);
                Destroy(gameObject, .5f);
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                WaterDropManager.Instance.isParantt = false;
            StartCoroutine(FillGlassControler.Instance.FillGlass(1f));
            }
        }
       
    }

   

    

