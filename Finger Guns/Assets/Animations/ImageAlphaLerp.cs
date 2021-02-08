using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAlphaLerp : MonoBehaviour
{
    // Components
    private Image image;

    // Inspector Options
    [SerializeField]
    [Tooltip("Alpha value at start of the forward transition, with 0 being transparent, and 1 being fully opaque.")]
    private float alphaStart = 0.0f;
    [SerializeField]
    [Tooltip("Alpha value at end of the forward transition, with 0 being transparent, and 1 being fully opaque.")]
    private float alphaEnd = 1.0f;
    [SerializeField]
    [Tooltip("If true, the game object's alpha will be set to \"Alpha End\" on Awake.")]
    private bool awakenBackwards = false;
    [SerializeField]
    private float transitionDuration = 1.0f;

    // Private variables for lerp calculations
    private float lerpStart;
    private float lerpEnd;
    private float lerpDuration;
    private bool isLerping = false;
    private float timeElapsed = 0.0f;
    private float targetAlpha = 0.0f;
    private Color targetColor;

    private void Awake()
    {
        // Debug
        if (transitionDuration <= 0)
        {
            Debug.Log("The Image Alpha Lerp script on " + gameObject + " has a duration of 0, which will result in no transition.");
        }
        
        // References
        image = GetComponent<Image>();
        
        // Set alpha on Awake
        if (!awakenBackwards)
        {
            if (image.color.a != alphaStart)
            {
                targetColor = image.color;
                targetColor.a = alphaStart;
                image.color = targetColor;
            }
        }
        else
        {
            if (image.color.a != alphaEnd)
            {
                targetColor = image.color;
                targetColor.a = alphaEnd;
                image.color = targetColor;
            }
        }
    }

    void Update()
    {
        // Apply Lerp only if we're lerping
        if (isLerping)
        {
            if (timeElapsed < lerpDuration)
            {
                targetAlpha = Mathf.Lerp(lerpStart, lerpEnd, timeElapsed / lerpDuration);
                targetColor.a = targetAlpha;
                image.color = targetColor;

                timeElapsed += Time.deltaTime;
            }
            else
            {
                targetColor.a = lerpEnd;
                image.color = targetColor;
                isLerping = false;
            }
        }
    }

    public void LerpForward()
    {
        lerpStart = alphaStart;
        lerpEnd = alphaEnd;
        lerpDuration = transitionDuration;
        timeElapsed = 0.0f;
        isLerping = true;
    }

    public void SafeLerpForward()
    {
        if (image.color.a == alphaStart)
        {
            LerpForward();
        }
    }

    public void LerpBackward()
    {
        lerpStart = alphaEnd;
        lerpEnd = alphaStart;
        lerpDuration = transitionDuration;
        timeElapsed = 0.0f;
        isLerping = true;
    }

    public void SafeLerpBackward()
    {
        if (image.color.a == alphaEnd)
        {
            LerpBackward();
        }
    }

    public void CustomLerp(float from = 0.0f, float to = 1.0f, float duration = 1.0f)
    {
        lerpStart = from;
        lerpEnd = to;
        lerpDuration = duration;
        timeElapsed = 0.0f;
        isLerping = true;
    }
}
