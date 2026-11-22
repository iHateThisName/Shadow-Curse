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
    [SerializeField] private PlayerAnimation anim;

    private float lightIntensity = 1f;

    private void OnEnable() {
        this.mirrorFlash.Enable();
        this.mirrorFlash.performed += OnMirrorFlash;
    }

    private void OnDisable() {
        this.mirrorFlash.performed -= OnMirrorFlash;
        this.mirrorFlash.Disable();
    }

    private void Start() {
        // Un-parent the mirror light
        this.mirrorLight.transform.SetParent(null);
    }

    private void OnMirrorFlash(InputAction.CallbackContext context) {
        if (mirrorLight.intensity >= 0f) {
            // Face the same direction as the player, default the light is facing right.
            if (playerMovment.previousDirection == PlayerMovment.DirectionEnum.Right) {
                mirrorLight.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                this.anim.SetTrigger(PlayerAnimation.MirrorRightTrigger);

            } else if (playerMovment.previousDirection == PlayerMovment.DirectionEnum.Left) {
                mirrorLight.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                this.anim.SetTrigger(PlayerAnimation.MirrorLeftTrigger);

            } else if(playerMovment.previousDirection == PlayerMovment.DirectionEnum.Up) {
                mirrorLight.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                this.anim.SetTrigger(PlayerAnimation.MirrorBackTrigger);

            } else if(playerMovment.previousDirection == PlayerMovment.DirectionEnum.Down || playerMovment.previousDirection == PlayerMovment.DirectionEnum.Down) {
                mirrorLight.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, -180f);
                this.anim.SetTrigger(PlayerAnimation.MirrorFrontTrigger);
            }

            Debug.Log($"Mirror Flash! Player Direction: {playerMovment.currentDirection}, Mirror Light Rotation: {mirrorLight.gameObject.transform.rotation.eulerAngles}");

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
