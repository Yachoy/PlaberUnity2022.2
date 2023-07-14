using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMovement : MonoBehaviour
{
    public float angle = 0f;
    private float radius = 0.5f;
    public float y = 5f;
    [Header("Speed")]
    public float speedRadius = 15f;
    public float speedRotate = 5f;
    [Header("Limits")]
    public float minRadius = 2f;
    public float maxRadius = 15f;
    public float minZoom = 3f;
    public float maxZoom = 25f;

    

    // запоминать свое нахождение и делать его центром окружности
    private Vector3 cachedCenter;
    public Transform center;

    private void Start()
    {
        angle = 2.323596f;
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.W) && radius > minRadius && radius - Time.deltaTime > minRadius)
        {
            cachedCenter.x -= Time.deltaTime * speedRadius;
            radius -= Time.deltaTime * speedRadius;
        }
        if (Input.GetKey(KeyCode.S) && radius < maxRadius && radius + Time.deltaTime < maxRadius)
        {
            cachedCenter.x += Time.deltaTime * speedRadius;
            radius += Time.deltaTime * speedRadius;
        }
        if (Input.GetKey(KeyCode.A))
            angle -= Time.deltaTime * speedRotate;

        if (Input.GetKey(KeyCode.D))
            angle += Time.deltaTime * speedRotate;

        if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Camera.main.orthographicSize--;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize++;
        }


        var x = Mathf.Cos(angle) * radius;
        var z = Mathf.Sin(angle) * radius;
        transform.position = new Vector3(x, y, z) + cachedCenter - new Vector3(radius, 0, 0);

        transform.LookAt(center);
    }
}
