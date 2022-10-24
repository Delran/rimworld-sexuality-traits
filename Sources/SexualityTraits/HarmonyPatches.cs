using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SexualityTraits
{
    [DefOf]
    public static class ST_DefOf
    {
        public static TraitDef ST_Straight;
    }

    [StaticConstructorOnStartup]
    internal static class HarmonyInit
    {
        static HarmonyInit()
        {
            new Harmony("SexualityTraits.Mod").PatchAll();
            SexualityTraitsMod.settings.sexualityTraits = new List<TraitDef>
            {
                ST_DefOf.ST_Straight,
                TraitDefOf.Gay,
                TraitDefOf.Bisexual,
                TraitDefOf.Asexual
            };
        }
    }
    [HarmonyPatch(typeof(InteractionWorker_RomanceAttempt), nameof(InteractionWorker_RomanceAttempt.RandomSelectionWeight))]
    public static class RandomSelectionWeight_Patch
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(Pawn initiator, Pawn recipient, ref float __result)
        {
            if (SexualityTraitsMod.settings.romanceTweaksEnabled)
            {
                if (initiator.HasTrait(TraitDefOf.Gay) && (initiator.gender != recipient.gender || !recipient.HasTrait(TraitDefOf.Bisexual) && !recipient.HasTrait(TraitDefOf.Gay)))
                {
                    __result = 0;
                }
                else if (recipient.HasTrait(TraitDefOf.Gay) && (initiator.gender != recipient.gender || !initiator.HasTrait(TraitDefOf.Bisexual) && !initiator.HasTrait(TraitDefOf.Gay)))
                {
                    __result = 0;
                }
                else if (initiator.HasTrait(TraitDefOf.Bisexual) && initiator.gender != recipient.gender && recipient.HasTrait(TraitDefOf.Gay))
                {
                    __result = 0;
                }
                else if (initiator.HasTrait(TraitDefOf.Asexual) || recipient.HasTrait(TraitDefOf.Asexual))
                {
                    __result = 0;
                }
                else if ((initiator.HasTrait(ST_DefOf.ST_Straight) || recipient.HasTrait(ST_DefOf.ST_Straight)) && recipient.gender == initiator.gender)
                {
                    __result = 0;
                }
            }
        }

        public static bool HasTrait(this Pawn pawn, TraitDef traitDef)
        {
            if (traitDef != null && (pawn?.story?.traits?.HasTrait(traitDef) ?? false))
            {
                return true;
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PawnGenerator), "GenerateTraits")]
    public static class GenerateTraits_Patch
    {
        public static bool ignoreThis;
        public static void Prefix(Pawn pawn, PawnGenerationRequest request)
        {
            foreach (var def in SexualityTraitsMod.settings.sexualityTraits)
            {
                if (DefDatabase<TraitDef>.GetNamedSilentFail(def.defName) != null)
                {
                    DefDatabase<TraitDef>.Remove(def);
                }
            }
        }
        public static void Postfix(Pawn pawn, PawnGenerationRequest request)
        {

            if (!pawn.story.traits.allTraits.Any(x => SexualityTraitsMod.settings.sexualityTraits.Contains(x.def)))
            {
                var partner = pawn.relations.DirectRelations.Find((DirectPawnRelation x) => LovePartnerRelationUtility.IsLovePartnerRelation(x.def)
                    || LovePartnerRelationUtility.IsExLovePartnerRelation(x.def));
                Func<TraitDef, bool> CanHaveTrait = (TraitDef t) =>
                {
                    if (t == TraitDefOf.Gay)
                    {
                        if (partner != null && partner.otherPawn.gender != pawn.gender)
                        {
                            return false;
                        }
                    }
                    else if (t == TraitDefOf.Asexual)
                    {
                        if (partner != null)
                        {
                            return false;
                        }
                    }
                    else if (t == ST_DefOf.ST_Straight)
                    {
                        if (partner != null && partner.otherPawn.gender == pawn.gender)
                        {
                            return false;
                        }
                    }
                    return true;
                };

                if (SexualityTraitsMod.settings.sexualityTraits.Where(x => CanHaveTrait(x))
                    .TryRandomElementByWeight(x => SexualityTraitsMod.settings.GetCommonalityFor(x), out var traitDef))
                {
                    Trait trait = new Trait(traitDef, PawnGenerator.RandomTraitDegree(traitDef));
                    pawn.story.traits.GainTrait(trait);
                }
            }

            if (!ignoreThis && pawn.story.traits.HasTrait(TraitDefOf.Gay))
            {
                var partner = pawn.relations.DirectRelations.Find((DirectPawnRelation x) => LovePartnerRelationUtility.IsLovePartnerRelation(x.def)
                     || LovePartnerRelationUtility.IsExLovePartnerRelation(x.def));
                if (partner is null)
                {
                    request.AllowGay = true;
                    PawnRelations.GeneratePawnRelations(pawn, ref request);
                }
            }
            foreach (var def in SexualityTraitsMod.settings.sexualityTraits)
            {
                if (DefDatabase<TraitDef>.GetNamedSilentFail(def.defName) is null)
                {
                    DefDatabase<TraitDef>.Add(def);
                }
            }
        }

        //public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        //{
        //    var get_RaceProps = AccessTools.Method(typeof(PawnGenerationRequest), "get_AllowGay");
        //    var codes = instructions.ToList();
        //    bool patched = false;
        //    for (var i = 0; i < codes.Count; i++)
        //    {
        //        var instr = codes[i];
        //        if (!patched && i > 1 && codes[i].opcode == OpCodes.Ldarga_S && codes[i + 1].Calls(get_RaceProps))
        //        {
        //            patched = true;
        //            var brIndex = codes.FirstIndexOf(x => x.opcode == OpCodes.Br && codes.IndexOf(x) > i);
        //            i += (brIndex - 1) - i;
        //                     // we skip this block of the code
        //                     //if (request.AllowGay && (LovePartnerRelationUtility.HasAnyLovePartnerOfTheSameGender(pawn) || LovePartnerRelationUtility.HasAnyExLovePartnerOfTheSameGender(pawn)))
        //                     //{
        //                     //    Trait trait = new Trait(TraitDefOf.Gay, RandomTraitDegree(TraitDefOf.Gay));
        //                     //    pawn.story.traits.GainTrait(trait);
        //                     //}
        //        }
        //        else
        //        {
        //            yield return instr;
        //        }
        //    }
        //}
    }
}
