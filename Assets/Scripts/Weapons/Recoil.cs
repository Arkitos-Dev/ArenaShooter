using UnityEngine;

public class Recoil : MonoBehaviour
{
    public Vector3 currentRecoilOffset; // This will store the current offset due to recoil

    [SerializeField] private float recoilX = 1.5f;
    [SerializeField] private float recoilY = -1.5f;
    [SerializeField] private float recoilZ = 0.3f;
    
    [SerializeField] private float snappiness = 10f;
    [SerializeField] private float returnSpeed = 3f;

    private Vector3 targetRecoil;

    void Update()
    {
        // Slerp the current recoil towards the target, and then lerp it back to zero
        currentRecoilOffset = Vector3.Slerp(currentRecoilOffset, targetRecoil, snappiness * Time.deltaTime);
        targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, returnSpeed * Time.deltaTime);
    }

    public void RecoilFunc()
    {
        // Simply set the target recoil, which will then be interpolated
        targetRecoil += new Vector3(-recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
}

