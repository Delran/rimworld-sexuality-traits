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
    class SexualityTraitsSettings : ModSettings
    {
        private float straightCommonality = 0.2f;
        private float gayCommonality = 0.2f;
        private float bisexualCommonality = 0.5f;
        private float asexualCommonality = 0.1f;
        public bool romanceTweaksEnabled = true;
        public List<TraitDef> sexualityTraits;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref straightCommonality, "straightCommonality", 0.2f);
            Scribe_Values.Look(ref gayCommonality, "gayCommonality", 0.2f);
            Scribe_Values.Look(ref bisexualCommonality, "bisexualCommonality", 0.5f);
            Scribe_Values.Look(ref asexualCommonality, "asexualCommonality", 0.1f);
            Scribe_Values.Look(ref romanceTweaksEnabled, "romanceTweaksEnabled", true);
        }

        public float GetCommonalityFor(TraitDef traitDef)
        {
            switch (traitDef.defName)
            {
                case "ST_Straight": return straightCommonality;
                case "Bisexual": return bisexualCommonality;
                case "Gay": return gayCommonality;
                case "Asexual": return asexualCommonality;
            }
            return 1f;
        }
        public void DoSettingsWindowContents(Rect inRect)
        {
            Rect rect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            listingStandard.SliderLabeled("ST.StraightCommonality".Translate(), ref straightCommonality, (straightCommonality * 100f).ToStringDecimalIfSmall() + "%", 0f, 1f);
            float valueChange = 0.00001f;

            while (straightCommonality + gayCommonality + bisexualCommonality + asexualCommonality > 1f)
            {
                TryModify(ref gayCommonality, -valueChange);
                TryModify(ref bisexualCommonality, -valueChange);
                TryModify(ref asexualCommonality, -valueChange);
            }
            while (straightCommonality + gayCommonality + bisexualCommonality + asexualCommonality < 1f)
            {
                TryModify(ref gayCommonality, valueChange);
                TryModify(ref bisexualCommonality, valueChange);
                TryModify(ref asexualCommonality, valueChange);
            }

            listingStandard.SliderLabeled("ST.GayCommonality".Translate(), ref gayCommonality, (gayCommonality * 100f).ToStringDecimalIfSmall() + "%", 0f, 1f);
            while (straightCommonality + gayCommonality + bisexualCommonality + asexualCommonality > 1f)
            {
                TryModify(ref straightCommonality, -valueChange);
                TryModify(ref bisexualCommonality, -valueChange);
                TryModify(ref asexualCommonality, -valueChange);
            }
            while (straightCommonality + gayCommonality + bisexualCommonality + asexualCommonality < 1f)
            {
                TryModify(ref straightCommonality, valueChange);
                TryModify(ref bisexualCommonality, valueChange);
                TryModify(ref asexualCommonality, valueChange);
            }

            listingStandard.SliderLabeled("ST.BisexualCommonality".Translate(), ref bisexualCommonality, (bisexualCommonality * 100f).ToStringDecimalIfSmall() + "%", 0f, 1f);
            while (straightCommonality + gayCommonality + bisexualCommonality + asexualCommonality > 1f)
            {
                TryModify(ref straightCommonality, -valueChange);
                TryModify(ref gayCommonality, -valueChange);
                TryModify(ref asexualCommonality, -valueChange);
            }
            while (straightCommonality + gayCommonality + bisexualCommonality + asexualCommonality < 1f)
            {
                TryModify(ref straightCommonality, valueChange);
                TryModify(ref gayCommonality, valueChange);
                TryModify(ref asexualCommonality, valueChange);
            }

            listingStandard.SliderLabeled("ST.AsexualCommonality".Translate(), ref asexualCommonality, (asexualCommonality * 100f).ToStringDecimalIfSmall() + "%", 0f, 1f);
            while (straightCommonality + gayCommonality + bisexualCommonality + asexualCommonality > 1f)
            {
                TryModify(ref straightCommonality, -valueChange);
                TryModify(ref gayCommonality, -valueChange);
                TryModify(ref bisexualCommonality, -valueChange);
            }
            while (straightCommonality + gayCommonality + bisexualCommonality + asexualCommonality < 1f)
            {
                TryModify(ref straightCommonality, valueChange);
                TryModify(ref gayCommonality, valueChange);
                TryModify(ref bisexualCommonality, valueChange);
            }

            if (listingStandard.ButtonText("Reset".Translate()))
            {
                straightCommonality = 0.2f;
                gayCommonality = 0.2f;
                bisexualCommonality = 0.5f;
                asexualCommonality = 0.1f;
            }

            listingStandard.CheckboxLabeled("ST.EnableRomanceTweaks".Translate(), ref romanceTweaksEnabled, "ST.EnableRomanceTweaksTooltip".Translate());
            listingStandard.End();
            base.Write();
        }

        private void TryModify(ref float field, float value)
        {
            if (value > 0)
            {
                if (field < 1) field += value * field;
            }
            else if (field > 0)
            {
                field += value * field;
            }
            field = Mathf.Clamp01(field);
        }
    }
}
