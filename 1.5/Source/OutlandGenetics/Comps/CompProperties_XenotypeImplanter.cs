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
    public class CompProperties_XenotypeImplanter : CompProperties_AbilityEffect
    {
        public CompProperties_XenotypeImplanter()
        {
            compClass = typeof(Comp_XenotypeImplanter);
        }

        public XenotypeDef xenotype;
    }
}
