using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraEffects : MonoBehaviour
{
    private Camera cam;
    private float baseSize;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        baseSize = cam.orthographic ? cam.orthographicSize : cam.fieldOfView;
    }

    public void Shake(float duration = 0.2f, float strength = 0.3f)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, strength));
    }

    public void Zoom(float amount, float duration = 0.2f)
    {
        StopAllCoroutines();
        StartCoroutine(ZoomRoutine(baseSize - amount, duration));
    }

    public void ResetZoom(float duration = 0.2f)
    {
        StopAllCoroutines();
        StartCoroutine(ZoomRoutine(baseSize, duration));
    }

    private IEnumerator ShakeRoutine(float duration, float strength)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            // Wait for CameraFollow to finish positioning the camera this frame
            yield return new WaitForEndOfFrame();

            Vector3 offset = Random.insideUnitSphere * strength;
            if (cam.orthographic) offset.z = 0;

            transform.position += offset;
        }
    }

    private IEnumerator ZoomRoutine(float target, float duration)
    {
        float start = cam.orthographic ? cam.orthographicSize : cam.fieldOfView;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float val = Mathf.Lerp(start, target, t / duration);
            if (cam.orthographic) cam.orthographicSize = val;
            else cam.fieldOfView = val;
            yield return null;
        }
        if (cam.orthographic) cam.orthographicSize = target;
        else cam.fieldOfView = target;
    }
}