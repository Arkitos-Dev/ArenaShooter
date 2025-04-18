using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sens;
    public Transform orientation;
    private Recoil recoilAnim;

    private float xRotation;
    private float yRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        recoilAnim = GetComponentInParent<Recoil>();
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sens;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply recoil offset to camera rotation
        Vector3 finalRotation = new Vector3(xRotation, yRotation) + recoilAnim.currentRecoilOffset;

        // Update the camera's rotation based on the final calculated values
        transform.localRotation = Quaternion.Euler(finalRotation);
        orientation.localRotation = Quaternion.Euler(0, yRotation, 0);
    }
}