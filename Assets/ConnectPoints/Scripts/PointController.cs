using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class PointController : MonoBehaviour
{
    //Sprite Renderer reference.
    private SpriteRenderer _spriteRend;

    /// <summary>
    /// Return this point data
    /// </summary>
    public PointData CurrentPointData {get; private set;}

    /// <summary>
    /// Set up this point and it's data.
    /// </summary>
    /// <param name="d">Point Data</param>
    public void SetupPoint(PointData d)
    {
        if(_spriteRend == null)
            _spriteRend = GetComponent<SpriteRenderer>();
        _spriteRend.color = d.c;
        CurrentPointData = d;
    }
}
