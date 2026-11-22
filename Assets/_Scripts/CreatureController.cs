using UnityEngine;

public class CreatureController : MonoBehaviour
{
    bool isPlayerInRange = false;
    bool isDealingDamage = false;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(GameManager.PlayerTag)) {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag(GameManager.PlayerTag)) {
            isPlayerInRange = false;
        }
    }

    public void SetDealingDamage(bool isDealing) => this.isDealingDamage = isDealing;
    //public void DealDamage() => this.isDealingDamage = true;
    //public void StopDealingDamage() => this.isDealingDamage = false;

    private void FixedUpdate() {
        if (isPlayerInRange && isDealingDamage && !GameManager.Instance.IsPlayerDead) {
            GameManager.Instance.OnPlayerKilled();
        }
    }
}
