using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraMotor : MonoBehaviour
{
    public Transform lookAt;
    public Vector3 offset = new Vector3(0, 4, -10);
    [SerializeField] float smoothness = 2.0f;

    public Vector3 rotation = new Vector3(35, 0, 0); 

    public bool IsMoving { get; set; }

    private void Awake()
    {
        // get the offset from how we set up the Camera in our scene. 
        //offset = Camera.main.transform.position;
    }

    private void LateUpdate()
    {
        if (!IsMoving)
            return;

        Vector3 _cameraTargetPosition = lookAt.position + offset;   
        _cameraTargetPosition.x = 0f;

        // linear interpolation. 
        transform.position = Vector3.Lerp(transform.position, _cameraTargetPosition, smoothness);

        // quad interpolation (using Rotations) 
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), .2f);  
    }
}
