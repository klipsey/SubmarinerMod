/*
using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using SubmarinerMod.Submariner.Components;
using SubmarinerMod.Submariner.Content;

namespace SubmarinerMod.Submariner.Components
{
    public class ConvictHudController : MonoBehaviour
    {
        public HUD targetHUD;
        public SubmarinerController SubmarinerController;

        public LanguageTextMeshController targetText;
        public GameObject durationDisplay;
        public Image durationBar;
        public Image durationBarColor;

        private void Start()
        {
            this.SubmarinerController = this.targetHUD?.targetBodyObject?.GetComponent<SubmarinerController>();
            this.SubmarinerController.onConvictDurationChange += SetDisplay;

            this.durationDisplay.SetActive(false);
            SetDisplay();
        }

        private void OnDestroy()
        {
            if (this.SubmarinerController) this.SubmarinerController.onConvictDurationChange -= SetDisplay;

            this.targetText.token = string.Empty;
            this.durationDisplay.SetActive(false);
            GameObject.Destroy(this.durationDisplay);
        }

        private void Update()
        {
            if(targetText.token != string.Empty) { targetText.token = string.Empty; }

            if(this.SubmarinerController && this.SubmarinerController.convictTimer > 0f)
            {
                float fill = this.SubmarinerController.convictTimer;

                if (this.durationBarColor)
                {
                    if (fill >= 1f) this.durationBarColor.fillAmount = 1f;
                    this.durationBarColor.fillAmount = Mathf.Lerp(this.durationBarColor.fillAmount, fill, Time.fixedDeltaTime * 2f);
                }

                this.durationBar.fillAmount = fill;
            }
            else if(this.durationDisplay.activeSelf == true && this.SubmarinerController.convictTimer <= 0f)
            {
                this.durationDisplay.SetActive(false);
            }
        }

        private void SetDisplay()
        {
            if (this.SubmarinerController)
            {
                this.durationDisplay.SetActive(true);
                this.targetText.token = string.Empty;

                this.durationBar.color = SubmarinerAssets.SubmarinerColor;
            }
            else
            {
                this.durationDisplay.SetActive(false);
            }
        }
    }
}
*/