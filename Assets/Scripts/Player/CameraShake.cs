using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude)
    {
        Quaternion originalRotation = transform.localRotation;

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            
            Quaternion randomRotation = Quaternion.Euler(x, y, 0);
            transform.localRotation = originalRotation * randomRotation;
            magnitude = Mathf.Lerp(magnitude, 0, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }
        
        while (transform.localRotation != originalRotation)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, Time.deltaTime * 10);
            yield return null;
        }
    }
}