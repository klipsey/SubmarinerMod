﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SubmarinerMod.Submariner.SkillStates
{
    public class Rest : BaseEmote
    {
        public override void OnEnter()
        {
            base.OnEnter();

            this.PlayEmote("RestEmote", "", 1.5f);
        }
    }
}