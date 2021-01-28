using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformColliderCreation : MonoBehaviour
{
    public float heightPadding = -0.1f;
    public float platformThickness = 0.1f;

    private int targetLayer;
    private float targetWidth;
    private float targetYPos;

    // Components
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    
    private void Awake()
    {
        // Set gameObject's layer to "Platform"
        targetLayer = LayerMask.NameToLayer("Platform");
        if (targetLayer < 0)
        {
            Debug.Log("Platforms are trying to assign themselves to the \"Platform\" layer for platforming logic, but no such layer exists.");
        }
        else if (gameObject.layer != targetLayer)
        {
            gameObject.layer = targetLayer;
        }

        // Set up component dependencies
        if(GetComponent<SpriteRenderer>() == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        else
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (GetComponent<BoxCollider2D>() == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        else
        {
            boxCollider = GetComponent<BoxCollider2D>();
        }

        // Set variables to be used in manipulating the BoxCollider2D
        targetWidth = spriteRenderer.size.x;
        targetYPos = (spriteRenderer.size.y / 2);
        targetYPos -= (platformThickness / 2);
        targetYPos += heightPadding;

        // Set BoxCollider2D values to match the SpriteRenderer
        boxCollider.size = new Vector2(targetWidth, platformThickness);
        boxCollider.offset = new Vector2(0.0f, targetYPos);
    }
}
