﻿using BattleMounts.Storage;
using BattleMounts.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace BattleMounts.Jobs
{
    public class JobDriver_Mount_Battlemount : JobDriver
    {
        public override bool TryMakePreToilReservations()
        {
            return true;
        }
        private Pawn Mount { get { return job.targetA.Thing as Pawn; } }

        protected override IEnumerable<Toil> MakeNewToils()
        {

            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnDowned(TargetIndex.A);
            this.FailOnNotCasualInterruptible(TargetIndex.A);


            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_Interpersonal.WaitToBeAbleToInteract(this.pawn);
            yield return TalkToAnimal(TargetIndex.A);
        }

        private Toil TalkToAnimal(TargetIndex tameeInd)
        {
            Toil toil = new Toil();
            toil.initAction = delegate
            {
                Pawn actor = toil.GetActor();
                actor.interactions.TryInteractWith(Mount, InteractionDefOf.AnimalChat);
            };
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.defaultDuration = 150;
            toil.AddFinishAction(delegate {
                Log.Message("finishAction mountee");
                Pawn actor = toil.GetActor();
                var extendedDataStore = Base.Instance.GetExtendedDataStorage();
                var pawnData = extendedDataStore.GetExtendedDataFor(actor);
                pawnData.mount = (Pawn)((Thing)actor.CurJob.GetTarget(tameeInd));
                TextureUtility.setDrawOffset(pawnData);
            });
            return toil;
        }



        
    }
}
