using UnityEngine;

public class StartEruption : MonoBehaviour
{
    //Components
    Eruption eruptionScript;

    //private
    private bool alreadyTriggered;

    private void Awake()
    {
        eruptionScript = FindObjectOfType<Eruption>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !alreadyTriggered)
        {
            alreadyTriggered = true;
            eruptionScript.StartEruption();
        }
    }
}
