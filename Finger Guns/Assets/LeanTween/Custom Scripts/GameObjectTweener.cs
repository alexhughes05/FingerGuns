using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectTweener : MonoBehaviour
{
    public string notes;
    [Header("TweenMasterSettings")]
    public TweenSet tweenMasterSettings;
    [SerializeField]
    private bool useMasterSettings = false;
    [Header("Transforms")]
    public TweenSetVector3 move;
    [Tooltip("Due to the way LeanTween calculates rotation, it is not recommended for any action that happens very quickly.")]
    public TweenSetVector3 rotate;
    [SerializeField]
    private bool backwardsTweenReversesRotation = true;
    public TweenSetVector3 scale;

    [Header("Global Settings")]
    [SerializeField]
    private bool tweenInOnEnable = false;
    [SerializeField]
    private bool inactiveOnAwake = false;
    [SerializeField]
    private bool disableScriptAfterTweenBackward = false;
    [SerializeField]
    private bool setGameObjectInactiveAfterTweenBackward = false;
    [SerializeField]
    private bool startInBackwardPosition = false;
    [SerializeField]
    private bool startInForwardPosition = false;
    [SerializeField]
    [Tooltip("If left blank, will default to this game object.")]
    private GameObject gameObjectToTween;

    private void Awake()
    {
        // Fallback for gameObjectToTween
        if (gameObjectToTween == null)
        {
            gameObjectToTween = gameObject;
        }
        // Set original values
        move.originalValue = gameObjectToTween.transform.position;
        rotate.originalValue = gameObjectToTween.transform.rotation.eulerAngles;
        scale.originalValue = gameObjectToTween.transform.localScale;
        // Set object to "forward position" if bool is set
        if (startInForwardPosition)
        {
            // Position
            if (move.to != move.from)
            {
                if (move.isRelative)
                {
                    gameObjectToTween.transform.position += move.to - move.from;
                }
                else
                {
                    gameObjectToTween.transform.position = move.to;
                }
            }
            // Rotation
            if (rotate.to != rotate.from)
            {
                if (rotate.isRelative)
                {
                    gameObjectToTween.transform.rotation *= Quaternion.Euler(rotate.to - rotate.from);
                }
                else
                {
                    gameObjectToTween.transform.rotation = Quaternion.Euler(rotate.to);
                }
            }
            // Scale
            if (scale.to != scale.from)
            {
                if (scale.isRelative)
                {
                    gameObjectToTween.transform.localScale += scale.to - scale.from;
                }
                else
                {
                    gameObjectToTween.transform.localScale = scale.to;
                }
            }
        }

        // Set object to "backward position" if bool is set
        if (startInBackwardPosition)
        {
            // Position
            if (move.to != move.from)
            {
                if (!move.isRelative)
                {
                    gameObjectToTween.transform.position = move.from;
                }
            }
            // Rotation
            if (rotate.to != rotate.from)
            {
                if (!rotate.isRelative)
                {
                    gameObjectToTween.transform.rotation = Quaternion.Euler(rotate.from);
                }
            }
            // Scale
            if (scale.to != scale.from)
            {
                if (!scale.isRelative)
                {
                    gameObjectToTween.transform.localScale = scale.from;
                }
            }
        }

        // tweenMasterSettings Overrides other tweenSettings
        if (useMasterSettings)
        {
            move.copySettings(tweenMasterSettings);
            rotate.copySettings(tweenMasterSettings);
            scale.copySettings(tweenMasterSettings);
        }

        // Inactive on Awake, if called
        if (inactiveOnAwake)
        {
            gameObjectToTween.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (tweenInOnEnable)
        {
            TweenForward();
        }
    }

    private float FindLongestTween()
    {
        // Find the longest delay + duration
        float longestTime = 0f;
        if (move.from != move.to)
        {
            longestTime = move.delay + move.duration;
        }
        if (rotate.from != rotate.to)
        {
            if (rotate.delay + rotate.duration > longestTime)
            {
                longestTime = rotate.delay + rotate.duration;
            }
        }
        if (scale.from != scale.to)
        {
            if (scale.delay + scale.duration > longestTime)
            {
                longestTime = scale.delay + scale.duration;
            }
        }
        return longestTime;
    }

    public void TweenForward()
    {
        if (move.from != move.to)
        {
            TweenForwardMove();
        }
        if (rotate.from != rotate.to)
        {
            TweenForwardRotate();
        }
        if (scale.from != scale.to)
        {
            TweenForwardScale();
        }
    }

    public void TweenBackward()
    {
        if (move.from != move.to)
        {
            TweenBackwardMove();
        }
        if (rotate.from != rotate.to)
        {
            TweenBackwardRotate();
        }
        if (scale.from != scale.to)
        {
            TweenBackwardScale();
        }

        if (disableScriptAfterTweenBackward)
        {
            StartCoroutine(DisableAfterTime(FindLongestTween()));
        }

        if (setGameObjectInactiveAfterTweenBackward)
        {
            StartCoroutine(SetInactiveAfterTime(FindLongestTween()));
        }
    }

    IEnumerator SetInactiveAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObjectToTween.SetActive(false);
    }

    IEnumerator DisableAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        enabled = false;
    }

    private void TweenForwardMove()
    {
        // Relative movement
        if (move.isRelative)
        {
            Vector3 netVector = move.originalValue + (move.from - move.to);
            move.tweenObject = LeanTween.move(gameObjectToTween, netVector, move.duration);
            move.tweenObject.setDelay(move.delay);
            move.tweenObject.setEase(move.easeType);
            if (move.loop)
            {
                move.tweenObject.setLoopClamp();
            }
            if (move.pingPong)
            {
                move.tweenObject.setLoopPingPong();
            }
        }
        // Absolute movement
        else
        {
            gameObjectToTween.transform.position = move.from;
            move.tweenObject = LeanTween.move(gameObjectToTween, move.to, move.duration);
            move.tweenObject.setDelay(move.delay);
            move.tweenObject.setEase(move.easeType);
            if (move.loop)
            {
                move.tweenObject.setLoopClamp();
            }
            if (move.pingPong)
            {
                move.tweenObject.setLoopPingPong();
            }
        }
    }

    private void TweenBackwardMove()
    {
        // Relative movement
        if (move.isRelative)
        {
            move.tweenObject = LeanTween.move(gameObjectToTween, move.originalValue, move.duration);
            move.tweenObject.setDelay(move.delay);
            move.tweenObject.setEase(move.easeType);
            if (move.loop)
            {
                move.tweenObject.setLoopClamp();
            }
            if (move.pingPong)
            {
                move.tweenObject.setLoopPingPong();
            }
        }
        // Absolute movement
        else
        {
            gameObjectToTween.transform.position = move.to;
            move.tweenObject = LeanTween.move(gameObjectToTween, move.from, move.duration);
            move.tweenObject.setDelay(move.delay);
            move.tweenObject.setEase(move.easeType);
            if (move.loop)
            {
                move.tweenObject.setLoopClamp();
            }
            if (move.pingPong)
            {
                move.tweenObject.setLoopPingPong();
            }
        }
    }

    private void TweenForwardRotate()
    {
        // Relative movement
        if (rotate.isRelative)
        {
            Vector3 netValue = rotate.originalValue + (rotate.to - rotate.from);
            rotate.tweenObject = LeanTween.rotate(gameObjectToTween, netValue, rotate.duration);
            rotate.tweenObject.setDelay(rotate.delay);
            rotate.tweenObject.setEase(rotate.easeType);
            if (rotate.loop)
            {
                rotate.tweenObject.setLoopClamp();
            }
            if (rotate.pingPong)
            {
                rotate.tweenObject.setLoopPingPong();
            }
        }
        // Absolute movement
        else
        {
            gameObjectToTween.transform.rotation = Quaternion.Euler(rotate.from);
            rotate.tweenObject = LeanTween.rotate(gameObjectToTween, rotate.to - rotate.from, rotate.duration);
            rotate.tweenObject.setDelay(rotate.delay);
            rotate.tweenObject.setEase(rotate.easeType);
            if (rotate.loop)
            {
                rotate.tweenObject.setLoopClamp();
            }
            if (rotate.pingPong)
            {
                rotate.tweenObject.setLoopPingPong();
            }
        }
    }

    private void TweenBackwardRotate()
    {
        // Calculate the inverse of the rotation because LeanTween doesn't have absolute rotation values - it can only do relative
        Vector3 rotateInverse;
        if (backwardsTweenReversesRotation)
        {
            rotateInverse = rotate.from - rotate.to;
        }
        else
        {
            rotateInverse = rotate.to - rotate.from;
        }

        // Relative movement
        if (rotate.isRelative)
        {
            rotate.tweenObject = LeanTween.rotate(gameObjectToTween, rotateInverse, rotate.duration);
            rotate.tweenObject.setDelay(rotate.delay);
            rotate.tweenObject.setEase(rotate.easeType);
            if (rotate.loop)
            {
                rotate.tweenObject.setLoopClamp();
            }
            if (rotate.pingPong)
            {
                rotate.tweenObject.setLoopPingPong();
            }
        }
        // Absolute movement
        else
        {
            gameObjectToTween.transform.rotation = Quaternion.Euler(rotate.to);
            rotate.tweenObject = LeanTween.rotate(gameObjectToTween, rotateInverse, rotate.duration);
            rotate.tweenObject.setDelay(rotate.delay);
            rotate.tweenObject.setEase(rotate.easeType);
            if (rotate.loop)
            {
                rotate.tweenObject.setLoopClamp();
            }
            if (rotate.pingPong)
            {
                rotate.tweenObject.setLoopPingPong();
            }
        }
    }

    private void TweenForwardScale()
    {
        // Relative movement
        if (scale.isRelative)
        {
            Vector3 netVector = scale.originalValue + (scale.to - scale.from);
            scale.tweenObject = LeanTween.scale(gameObjectToTween, netVector, scale.duration);
            scale.tweenObject.setDelay(scale.delay);
            scale.tweenObject.setEase(scale.easeType);
            if (scale.loop)
            {
                scale.tweenObject.setLoopClamp();
            }
            if (scale.pingPong)
            {
                scale.tweenObject.setLoopPingPong();
            }
        }
        // Absolute movement
        else
        {
            gameObjectToTween.transform.localScale = scale.from;
            scale.tweenObject = LeanTween.scale(gameObjectToTween, scale.to, scale.duration);
            scale.tweenObject.setDelay(scale.delay);
            scale.tweenObject.setEase(scale.easeType);
            if (scale.loop)
            {
                scale.tweenObject.setLoopClamp();
            }
            if (scale.pingPong)
            {
                scale.tweenObject.setLoopPingPong();
            }
        }
    }

    private void TweenBackwardScale()
    {
        // Relative scalement
        if (scale.isRelative)
        {
            scale.tweenObject = LeanTween.scale(gameObjectToTween, scale.originalValue, scale.duration);
            scale.tweenObject.setDelay(scale.delay);
            scale.tweenObject.setEase(scale.easeType);
            if (scale.loop)
            {
                scale.tweenObject.setLoopClamp();
            }
            if (scale.pingPong)
            {
                scale.tweenObject.setLoopPingPong();
            }
        }
        // Absolute movement
        else
        {
            gameObjectToTween.transform.localScale = scale.to;
            scale.tweenObject = LeanTween.scale(gameObjectToTween, scale.from, scale.duration);
            scale.tweenObject.setDelay(scale.delay);
            scale.tweenObject.setEase(scale.easeType);
            if (scale.loop)
            {
                scale.tweenObject.setLoopClamp();
            }
            if (scale.pingPong)
            {
                scale.tweenObject.setLoopPingPong();
            }
        }
    }
}
