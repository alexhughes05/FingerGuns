using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformColliderCreation : MonoBehaviour
{
    #region Variables

    //Public
    [SerializeField] float heightPadding = -0.1f;
    [SerializeField] float platformThickness = 0.1f;

    // Private
    private float targetWidth;
    private float targetYPos;

    // Components
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    #endregion

    private void Awake()
    {
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
