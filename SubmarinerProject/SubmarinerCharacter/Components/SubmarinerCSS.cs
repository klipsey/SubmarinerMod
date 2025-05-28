using UnityEngine;
using RoR2;

namespace SubmarinerMod.SubmarinerCharacter.Components
{
    public class SubmarinerCSS : MonoBehaviour
    {
        private bool hasPlayed = false;
        private float timer = 0f;
        private void Awake()
        {
        }

        private void Start()
        {
            Util.PlaySound("Play_affix_void_bug_spawn", this.gameObject);
        }

        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;

            if (!hasPlayed && timer >= 0.9f)
            {
                hasPlayed = true;

                Util.PlaySound("Play_acrid_shift_puddle_loop", this.gameObject);
            }
        }
    }
}
