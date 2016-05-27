using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlaneCamera))]
public class PlaneCameraInput : MonoBehaviour
{
    protected new PlaneCamera camera;

    public bool scrollZoom;
    [Range(-8, 8)]
    [Tooltip("Multiply the effect scrolling has on the camera depth target by 2^x")]
    public float scrollDepthPower;

    public bool keyboardPan;
    public bool scalePan;
    public bool keyboardRotate;

    [Range(0, 5)]
    public int magnitude;

    [Range(0, 16)]
    public float panSpeed;
    [Range(0, 16)]
    public float depthSpeed;
    [Range(0, 4)]
    public float rotationPeriod;

    protected virtual void Awake()
    {
        camera = GetComponent<PlaneCamera>();
    }

    protected virtual void Update()
    {
        float m = Mathf.Pow(10, magnitude);

        if (scrollZoom)
        {
            camera.depthTarget -= Input.GetAxis("Mouse ScrollWheel") * depthSpeed * m * Mathf.Pow(2, scrollDepthPower) * Time.deltaTime;
        }

        Vector2 pan = Vector2.zero;

        if (keyboardPan)
        {
            pan += camera.forward * Input.GetAxis("Vertical")   * panSpeed * m * Time.deltaTime
                +  camera.right   * Input.GetAxis("Horizontal") * panSpeed * m * Time.deltaTime;
        }

        if (keyboardRotate)
        {
            camera.angleTarget += Input.GetAxis("Rotation") * (360 / rotationPeriod) * Time.deltaTime;
        }

        if (scalePan)
        {
            pan *= camera.depth / camera.maxDepth;
        }

        camera.focusTarget += pan;
    }
}
