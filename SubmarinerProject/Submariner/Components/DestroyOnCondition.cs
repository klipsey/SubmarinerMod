using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using TMPro;
using UnityEngine.Networking;

namespace SubmarinerMod.Submariner.Components
{
    public class DestroyOnCondition : MonoBehaviour
    {
        public AnchorConnectionComponent anchor;

        private void FixedUpdate()
        {
            if (!anchor.ownerIsInRange)
            {
                Destroy(this.gameObject);
            }
        }
    }
}