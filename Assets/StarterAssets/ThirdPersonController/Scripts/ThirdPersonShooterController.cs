using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Animator animator;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInput;

    private void Awake() {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInput = GetComponent<StarterAssetsInputs>();
    }
    private void Update() {
        Vector3 mouseWorldPosition = WorldPositionByRayCast(); 

        if (starterAssetsInput.aim) {
            WhileAim(mouseWorldPosition);
        } else {
            WhileNotAim();
        }

        if (starterAssetsInput.shoot) {
            Shooting(mouseWorldPosition);
        }
    }


    //Getting World poisiton with reycast for shooring on target
    private Vector3 WorldPositionByRayCast() {
        Vector3 mouseWorldPosition = Vector3.zero;
        //Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask)) {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }
        return mouseWorldPosition;
    }

    // While click on right click camera animation of zoom in / camera switching
    private void WhileAim(Vector3 mouseWorldPosition) {
        aimVirtualCamera.gameObject.SetActive(true);
        thirdPersonController.SetSensitivity(aimSensitivity);
        thirdPersonController.SetRotateOnMove(false);
        animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20);
    }

    //While release the right clict camera animation zoom out / camera switching
    private void WhileNotAim() {
        aimVirtualCamera.gameObject.SetActive(false);
        thirdPersonController.SetSensitivity(normalSensitivity);
        thirdPersonController.SetRotateOnMove(true);
        animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
    }

    // For instantiate the bullet prefab 
    private void Shooting(Vector3 mouseWorldPosition) {
        Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
        Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        starterAssetsInput.shoot = false;
    }
}
