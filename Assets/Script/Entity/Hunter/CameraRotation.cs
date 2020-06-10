using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{

    public GameObject player;
    public GameObject center;
    public GameObject mainCamera;
    public Transform cameraStartTransform;
    public Transform cameraCenterTransform;

    private float moveDistance;
    private float moveSpeed = 3;
    public float moveTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        moveDistance = Vector3.Distance(cameraStartTransform.position, cameraCenterTransform.position) * moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (center == null)
            return;

        float rot = Input.GetAxis("Mouse X") * 180 * Time.deltaTime;
        center.transform.Rotate(new Vector3(0, rot, 0));



        if (moveTime > 0.5f)
        {
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, cameraCenterTransform.position, moveDistance * Time.deltaTime);
            mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, cameraCenterTransform.rotation, 90 * moveSpeed * Time.deltaTime);

            if (Vector3.Distance(mainCamera.transform.position, cameraCenterTransform.position) < 0.05f)
            {
                moveTime = 0;
                mainCamera.transform.position = cameraCenterTransform.position;
                mainCamera.transform.rotation = cameraCenterTransform.rotation;
            }


        }
        else if (moveTime < -0.5f) {


            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, cameraStartTransform.position, moveDistance * Time.deltaTime);
            mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, cameraStartTransform.rotation, 90 * moveSpeed * Time.deltaTime);

            if (Vector3.Distance(mainCamera.transform.position, cameraStartTransform.position) < 0.05f)
            {
                moveTime = 0;
                mainCamera.transform.position = cameraStartTransform.position;
                mainCamera.transform.rotation = cameraStartTransform.rotation;
            }


        }
    }


    private void LateUpdate()
    {
        center.transform.position = player.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer==8)
            moveTime = 1f;

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
            moveTime = -1f;


    }




}
