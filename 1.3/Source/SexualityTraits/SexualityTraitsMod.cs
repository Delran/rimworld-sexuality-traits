using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SexualityTraits
{
    class SexualityTraitsMod : Mod
    {
        public static SexualityTraitsSettings settings;
        public SexualityTraitsMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<SexualityTraitsSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Sexuality Traits";
        }
    }
}
