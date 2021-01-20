using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    #region Variables
    //Public    
    public GameObject target;
    public float followSpeed = 1f;	
	public Vector3 offset;

    //Private
    private float interpVelocity;
    Vector3 targetPos;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
	{
		targetPos = transform.position;
	}

	void FixedUpdate()
	{
		if (target)
		{
			Vector3 posNoZ = transform.position;
			posNoZ.z = target.transform.position.z;

			Vector3 targetDirection = (target.transform.position - posNoZ);

			interpVelocity = targetDirection.magnitude * 5f;

			targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);

			transform.position = Vector3.Lerp(transform.position, targetPos + offset, followSpeed);

		}
	}
    #endregion
}