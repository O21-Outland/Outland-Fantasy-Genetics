﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace OutlandGenes
{
    public class Gene_AsexualReproduction : Gene
    {
        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (!base.GetGizmos().EnumerableNullOrEmpty())
            {
                foreach (Gizmo gizzy in base.GetGizmos())
                {
                    yield return gizzy;
                }
            }
            if(!pawn.Spawned || pawn.ageTracker.AgeBiologicalYears < 16 || !pawn.IsColonistPlayerControlled)
            {
                yield break;
            }

            //bool fatigued = pawn.health.hediffSet.HasHediff(OutlandGenesDefOf.Outland_EggFatigue);
            //XenotypeDef targetXeno = DefDatabase<XenotypeDef>.GetNamed(def.defName.Replace("Outland_XenoEggLayer_", ""));
            //string eggTexturePath = targetXeno?.GetModExtension<DefModExt_GeneSpecifics>()?.xenoEggTexturePath ?? "Outland/Items/XenoEgg_Baseliner";
            //yield return new Command_Action()
            //{
            //    defaultLabel = "Outland.GoLayEggLabel".Translate(targetXeno.LabelCap),
            //    defaultDesc = "Outland.GoLayEggDesc".Translate(targetXeno.LabelCap),
            //    icon = ContentFinder<Texture2D>.Get(eggTexturePath),
            //    disabled = fatigued,
            //    disabledReason = fatigued ? "Outland.EggFatigueReason".Translate() : null,
            //    action = delegate
            //    {
            //        ThingDef eggDef = DefDatabase<ThingDef>.GetNamed(targetXeno.defName + "_XenoEgg");
            //        Thing eggLayingSpot = EggUtil.GetBestEggBox(pawn, eggDef);
            //        if (eggLayingSpot == null)
            //        {
            //            Messages.Message("Outland.NoEggBox".Translate(), MessageTypeDefOf.NeutralEvent);
            //            Thing egg = ThingMaker.MakeThing(eggDef);
            //            Comp_XenotypeHatcher comp = egg.TryGetComp<Comp_XenotypeHatcher>();
            //            if(comp != null)
            //            {
            //                comp.hatcheeParent = pawn;
            //                comp.progressSpeed = GeneUtil.GetPregnancySpeed(pawn);
            //            }
            //            GenSpawn.Spawn(egg, pawn.Position, pawn.Map);
            //            pawn.health.AddHediff(OutlandGenesDefOf.Outland_EggFatigue);
            //        }
            //        ((Hediff_EggPouch)pawn.health.hediffSet.GetFirstHediffOfDef(OutlandGenesDefOf.Outland_EggPouch)).curEggDef = eggDef;
            //        pawn.jobs.TryTakeOrderedJob(new Job(OutlandGenesDefOf.Outland_LayEgg, eggLayingSpot), JobTag.Misc);
            //    }
            //};
            if (!pawn.Sterile())
            {
                bool fatigued = pawn.health.hediffSet.HasHediff(OutlandGenesDefOf.Outland_EggFatigue);
                bool currentlyPregnant = pawn.health.hediffSet.HasPregnancyHediff();
                yield return new Command_Action()
                {
                    defaultLabel = "Outland.BeginPregnancy".Translate(),
                    defaultDesc = "Outland.BeginPregnancyDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("Outland/GeneIcons/Gene_AsexualReproduction"),
                    disabled = fatigued || currentlyPregnant,
                    disabledReason = fatigued ? "Outland.EggFatigueReason".Translate() : (currentlyPregnant ? "Outland.AlreadyPregnantReason".Translate() : null),
                    action = delegate
                    {
                        bool success;
                        GeneSet inheritedGeneSet = PregnancyUtility.GetInheritedGeneSet(pawn, pawn, out success);
                        Hediff_Pregnant hediff_Pregnant = (Hediff_Pregnant)HediffMaker.MakeHediff(HediffDefOf.PregnantHuman, pawn);
                        hediff_Pregnant.SetParents(pawn, null, inheritedGeneSet);
                        pawn.health.AddHediff(hediff_Pregnant);
                    }
                };
            }
        }
    }
}
