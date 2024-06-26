﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OutlandGenes
{
    public class Hediff_XenotypeAscender : HediffWithComps
    {
        public float currentExp = 0f;

        public bool CanAscend => !HasAnyEarnedAscensionGene || currentExp >= 1f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref currentExp, "currentExp", 0f);
            Scribe_Defs.Look(ref targetXenotype, "targetXenotype");
        }

        public XenotypeDef targetXenotype = null;

        public List<XenotypeDef> PotentialXenotypes
        {
            get
            {
                List<XenotypeDef> potentialXenotypes = new List<XenotypeDef>();
                foreach (Gene gene in pawn.genes.GenesListForReading)
                {
                    if (gene.def.defName.Contains("Outland_XenotypeAscension_"))
                    {
                        XenotypeDef potentialXenotype = DefDatabase<XenotypeDef>.GetNamed(gene.def.defName.Replace("Outland_XenotypeAscension_", ""));
                        if (potentialXenotype != null && !potentialXenotypes.Contains(potentialXenotype))
                        {
                            potentialXenotypes.Add(potentialXenotype);
                        }
                    }
                }
                return potentialXenotypes;
            }
        }

        public bool HasAnyEarnedAscensionGene
        {
            get
            {
                return pawn.GeneActive(OutlandGenesDefOf.Outland_EasyEarnedAscension) || pawn.GeneActive(OutlandGenesDefOf.Outland_EarnedAscension) || pawn.GeneActive(OutlandGenesDefOf.Outland_HardEarnedAscension);
            }
        }

        public void GiveExp(float exp)
        {
            exp *= OutlandGenesMod.settings.earnedAscensionExperienceFactor;

            bool flag = false;
            if (pawn.GeneActive(OutlandGenesDefOf.Outland_HardEarnedAscension))
            {
                exp *= 0.5f;
                flag = true;
            }
            else if (pawn.GeneActive(OutlandGenesDefOf.Outland_EasyEarnedAscension))
            {
                exp *= 2.0f;
                flag = true;
            }
            else if (pawn.GeneActive(OutlandGenesDefOf.Outland_EarnedAscension))
            {
                flag = true;
            }

            if (flag)
            {
                if (currentExp + exp > 1f)
                {
                    currentExp = 1f;
                }
                else
                {
                    currentExp += exp;
                }
            }
        }

        public void SetRandomTargetXenotype()
        {
            targetXenotype = PotentialXenotypes.RandomElement();
        }

        public void Ascend()
        {
            if(targetXenotype == null) { SetRandomTargetXenotype(); }
            List<Gene> list3 = pawn.genes.Endogenes;
            for (int num = list3.Count - 1; num >= 0; num--)
            {
                Gene gene2 = list3[num];
                if (gene2.def.endogeneCategory != EndogeneCategory.Melanin && gene2.def.endogeneCategory != EndogeneCategory.HairColor)
                {
                    pawn.genes.RemoveGene(gene2);
                }
            }
            pawn.genes.SetXenotype(targetXenotype);

            pawn.health.RemoveHediff(this);
        }

        public bool HasAllRequiredSkills(List<SkillRange> skillRanges)
        {
            foreach (SkillRange skillRange in skillRanges)
            {
                if (!skillRange.Range.Includes(pawn.skills.GetSkill(skillRange.Skill).Level))
                {
                    return false;
                }
            }
            return true;
        }

        public bool HasAnyRequiredTrait(List<TraitDef> traits)
        {
            foreach (TraitDef trait in traits)
            {
                if (pawn.story.traits.HasTrait(trait))
                {
                    return true;
                }
            }
            return false;
        }
        
        public IEnumerable<FloatMenuOption> GetXenotypePotentialTargets()
        {
            foreach (XenotypeDef xeno in PotentialXenotypes)
            {
                yield return new FloatMenuOption(xeno.LabelCap, delegate ()
                {
                    targetXenotype = xeno;
                });
            }
            yield break;
        }
    }
}
