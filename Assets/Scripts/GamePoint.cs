using UnityEngine;

namespace IMTC505.starter.SampleGame
{
    [RequireComponent(typeof(Collider))]
    public class GamePoint : MonoBehaviour
    {
        [Tooltip("Points scored by touching this object.")]
        public float points = 10;

        [Tooltip("Event to trigger when controller interacts with point object.")]
        public System.Action<GamePoint> OnTriggerEnterAction;

        void Start()
        {
            // Make sure non of the colliders in child objects are active
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }

            // Make sure the root collider is a trigger and enabled
            Collider rootCollider = GetComponent<Collider>();
            rootCollider.enabled = true;
            rootCollider.isTrigger = true;
        }

        void OnTriggerEnter(Collider collider)
        {
            OnTriggerEnterAction?.Invoke(this);
        }
    }
}
