using EntityStates;
using RoR2;
using SubmarinerMod.Interrogator.Components;
using SubmarinerMod.Interrogator.Content;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace SubmarinerMod.Modules.BaseStates
{
    public abstract class BaseInterrogatorSkillState : BaseSkillState
    {
        protected SubmarinerController interrogatorController;

        protected bool isConvicting;
        public virtual void AddRecoil2(float x1, float x2, float y1, float y2)
        {
            this.AddRecoil(x1, x2, y1, y2);
        }
        public override void OnEnter()
        {
            RefreshState();
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
        protected void RefreshState()
        {
            if (!interrogatorController)
            {
                interrogatorController = base.GetComponent<SubmarinerController>();
                isConvicting = interrogatorController.isConvicted;
            }
        }
    }
}
