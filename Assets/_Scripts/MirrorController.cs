using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class MirrorController : MonoBehaviour
{

    [SerializeField] private InputAction mirrorFlash;
    [SerializeField] private Light2D mirrorLight;

    [SerializeField] private PlayerMovment playerMovment;

    private float lightIntensity = 1f;

    private void OnEnable() {
        this.mirrorFlash.Enable();
        this.mirrorFlash.performed += OnMirrorFlash;
    }

    private void OnDisable() {
        this.mirrorFlash.performed -= OnMirrorFlash;
        this.mirrorFlash.Disable();
    }

    private void OnMirrorFlash(InputAction.CallbackContext context) {
        if (mirrorLight.intensity == 0f) {
            // Face the same direction as the player, default the light is facing right.
            if (playerMovment.IsFacingRight) {
                mirrorLight.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            } else {
                mirrorLight.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            }

            // Turn on the light
            mirrorLight.gameObject.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, mirrorLight.gameObject.transform.position.z);
            mirrorLight.intensity = this.lightIntensity;
            StartCoroutine(LightReduce());
        }
    }

    private IEnumerator LightReduce() {
        while (mirrorLight.intensity > 0f) {
            mirrorLight.intensity -= 0.05f;
            yield return new WaitForSeconds(0.1f);
        }
        this.mirrorLight.intensity = 0f;
    }
}
