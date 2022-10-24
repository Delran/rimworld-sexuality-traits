using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SexualityTraits
{
    public static class PawnRelations
	{
		private static PawnRelationDef[] relationsGeneratableBlood = DefDatabase<PawnRelationDef>.AllDefsListForReading.Where((PawnRelationDef rel) => rel.familyByBloodRelation && rel.generationChanceFactor > 0f).ToArray();

		private static PawnRelationDef[] relationsGeneratableNonblood = DefDatabase<PawnRelationDef>.AllDefsListForReading.Where((PawnRelationDef rel) => !rel.familyByBloodRelation && rel.generationChanceFactor > 0f).ToArray();
        public static void GeneratePawnRelations(Pawn pawn, ref PawnGenerationRequest request)
        {
            if (!pawn.RaceProps.Humanlike)
            {
                return;
            }
            PawnGenerationRequest localReq = request;
            if (pawn.kindDef.generateInitialNonFamilyRelations && Rand.Chance(0.3f))
            {
                var array = new Pawn[10];
                GenerateTraits_Patch.ignoreThis = true;
                for (var i = 0; i < 10; i++)
                {
                    array[i] = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawn.kindDef, pawn.Faction, canGeneratePawnRelations: true, allowGay: true, fixedGender: pawn.gender));
                }
                GenerateTraits_Patch.ignoreThis = false;

                var samples = GenerateSamples(array, relationsGeneratableNonblood, 40);
                if (samples.TryRandomElementByWeight((Pair<Pawn, PawnRelationDef> x) =>x.Second.generationChanceFactor * x.Second.Worker.GenerationChance(pawn, x.First, localReq), 
                    out var pair2) && pair2.First != null)
                {
                    Find.WorldPawns.PassToWorld(pair2.First);
                    pair2.Second.Worker.CreateRelation(pawn, pair2.First, ref request);
                    if (pawn.Faction.IsPlayer)
                    {
                        pawn.relations.everSeenByPlayer = true;
                        pair2.First.relations.everSeenByPlayer = true;
                    }
                }
            }
        }

        private static Pair<Pawn, PawnRelationDef>[] GenerateSamples(Pawn[] pawns, PawnRelationDef[] relations, int count)
        {
            Pair<Pawn, PawnRelationDef>[] array = new Pair<Pawn, PawnRelationDef>[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = new Pair<Pawn, PawnRelationDef>(pawns[Rand.Range(0, pawns.Length)], relations[Rand.Range(0, relations.Length)]);
            }
            return array;
        }
	}
}
