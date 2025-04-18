using UnityEngine;

public class RecoilAnim: MonoBehaviour
{
    [SerializeField] private float recoilKickBack = 0.1f;
    [SerializeField] private float recoilKickRot = 5f;

    [SerializeField] private float returnSpeed = 5f;
    [SerializeField] private float returnRotSpeed = 10f; 

    private Vector3 originalPos;
    private Quaternion originalRot;

    private Vector3 targetPos;
    private Quaternion targetRot;
    
    public Transform recoilGunTransform;

    void Start()
    {
        originalPos = recoilGunTransform.localPosition;
        originalRot = recoilGunTransform.localRotation;
    }

    void Update()
    {
        // calculate the smooth factor based on return time
        float positionSmoothing = 1 - Mathf.Exp(-Time.deltaTime * returnSpeed);
        float rotationSmoothing = 1 - Mathf.Exp(-Time.deltaTime * returnRotSpeed);

        // interpolate position and rotation back to original
        recoilGunTransform.localPosition = Vector3.Lerp(recoilGunTransform.localPosition, originalPos, positionSmoothing);
        recoilGunTransform.localRotation = Quaternion.Slerp(recoilGunTransform.localRotation, originalRot, rotationSmoothing);
    }

    public void RecoilAnimation()
    {
        // calculate new position and rotation with recoil applied
        targetPos = originalPos + Vector3.back * recoilKickBack;
        targetRot = Quaternion.Euler(originalRot.eulerAngles + new Vector3(-recoilKickRot, Random.Range(-recoilKickRot, recoilKickRot) / 2, 0));

        // apply the recoil instantly
        recoilGunTransform.localPosition = targetPos;
        recoilGunTransform.localRotation = targetRot;
    }
}