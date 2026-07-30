using UnityEngine;
namespace PhysicsMaster.Gameplay {
    public sealed class GoalZone : MonoBehaviour {
        public System.Action Entered;
        void OnTriggerEnter2D(Collider2D other) { if (other.attachedRigidbody != null && other.gameObject.name.StartsWith("DynamicBall")) Entered?.Invoke(); }
    }
}
