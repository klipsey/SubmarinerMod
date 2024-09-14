using EntityStates;
using RoR2;
using SubmarinerMod.SubmarinerCharacter.Components;
using SubmarinerMod.SubmarinerCharacter.Content;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace SubmarinerMod.Modules.BaseStates
{
    public abstract class BaseSubmarinerState : BaseState
    {
        protected SubmarinerController SubmarinerController;

        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            RefreshState();
        }
        protected void RefreshState()
        {
            if (!SubmarinerController)
            {
                SubmarinerController = base.GetComponent<SubmarinerController>();
            }
        }
    }
}
