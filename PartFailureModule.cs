using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using KSP.IO;

/******************************************************************************
 * Copyright (c) 2013, Justin Bengtson
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met: 
 * 
 * 1. Redistributions of source code must retain the above copyright notice,
 * this list of conditions and the following disclaimer.
 * 
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 ******************************************************************************/

namespace RegexKSP {

	/*	
	 * This is a basic PartModule that must be added to a part within KSP.
	 * The plan here was to use ModuleManager to do all the heavy lifting
	 * in order to accomodate mods and such, since general cases will handle
	 * most every part and ModuleManager can already figure out what
	 * modules a part has.  That makes our job of finding a valid part moot.
	 *
	 * In the ModuleManager script you would, for instance, look for a part
	 * that contained a resource and then add a "leak" module that looked
	 * like the skeleton below.  The same copuld be done for engine gimbal,
	 * engines, parachutes, whatever.
	 * 
	 * ModuleMananger can also be used to assign specific values to our
	 * damage module on a per-part basis which gives us much more flexibility.
	 */
	public class PartFailureModule : PartModule {
		private static System.Random random = new System.Random();

		// this is the name of the damage that is displayed to the user on right-click. For instance,
		// if you had a part with multiple resources and one had a leak, you'd set this to
		// "LiquidFuel leak", maybe with a severity note.
		[KSPField(isPersistant = false, guiActive = true, guiName = "Damage", guiUnits = "", guiFormat = "G")]
		public string displayDamage = "";

		[KSPField(isPersistant = true, guiActive = false)]
		public double lastPollTime = 0.0;

		// this was intended to be used for scaling damage.
		[KSPField(isPersistant = true, guiActive = false)]
		public double damageChange = 0.0;

		[KSPField(isPersistant = true, guiActive = false)]
		public double cascadeChance = 0.0;

		[KSPField(isPersistant = true, guiActive = false)]
		public double interval = 0;

		// this was intended to be used for scaling damage.  For instance, you could have small
		// leaks, large leaks, and catastrophic leaks that all dumped resources at varying rates.
		[KSPField(isPersistant = true, guiActive = false)]
		public int severity = 0;

		// To be used with the Enums below.  I think i was going to scrap this and just write a
		// PartModule for each damage type that ModuleManager could then load.  Of course, it
		// could be retained so other PartFailureModules could easily find their cascadable
		// counterparts...
		[KSPField(isPersistant = true, guiActive = false)]
		public int type = 0;

		private PartRepairModule repairModule;

		public override void OnFixedUpdate() {
			if(canRun && type != 0) {
				if(severity < 1) {
					// run checks for damaging this part
					double timeNow = Planetarium.GetUniversalTime();
					double timeDiff = timeNow - lastPollTime;
					if(timeDiff > interval) {
						// write the damage routine here
						// remember that you'll want to add the repairModule to the
						// part if its damaged by this routine and can be repaired.
					}
				} else if(cascadeChance > 0.0) {
					// run checks for cascading damage
					double timeNow = Planetarium.GetUniversalTime();
					double timeDiff = timeNow - lastPollTime;
					if(timeDiff > interval) {
						// write the cascade damage routine here.
						// This was intended for electrical shorts, where a damaged
						// electrical part would damage other electrical parts, but
						// it could be used for all sorts of other things.
					}
				}
			}
		}

		public override void OnLoad(ConfigNode node) {
			repairModule = new PartRepairModule(this);
			// should probably add the repairModule to the part if it's damaged.
		}

		public void repair() {
			severity = 0;
			this.part.Modules.Remove(repairModule);
			markGUIDirty(this.part);
			broadcast(this.part.partInfo.title + " has been repaired.");
		}

		public static void markGUIDirty(Part p) {
			foreach(UIPartActionWindow window in FindObjectsOfType(typeof(UIPartActionWindow))) {
				if(window.part == p) {
					window.displayDirty = true;
				}
			}
		}

		public static void broadcast(string msg) {
			ScreenMessages.PostScreenMessage(msg, 5.0f, ScreenMessageStyle.UPPER_CENTER);
		}

		private void buildDamageTypes() {
			// not entirely sure what I was doing here, maybe remove it?
		}

		private List<PartResource> getEligibleLeakResources() {
			List<PartResource> retval = new List<PartResource>();
			foreach(PartResource pr in this.part.Resources) {
				if(pr.resourceName != "ElectricCharge") {
					retval.Add(pr);
				}
			}
			return retval;
		}

		private bool hasElectricalResource() {
			foreach(PartResource pr in this.part.Resources) {
				if(pr.resourceName == "ElectricCharge") {
					return true;
				}
			}
		}

		private bool canRun {
			get {
				return FlightGlobals.fetch != null && FlightGlobals.ActiveVessel != null;
			}
		}

		public enum Types {
			NONE, ENGINEGIMBAL, ENGINECOOLANT, PARACHUTE, DECOUPLER, DOCKINGPORT, LEAK, INTAKE,
			WHEEL, ENVIRO, SOLARPANEL, GENERATOR, EXPLOSION, ELECTRICAL
		};
	}

	public class PartRepairModule : PartModule {
		private PartFailureModule parent;

		public PartRepairModule(PartFailureModule p) {
			parent = p;
		}

		[KSPEvent(guiName = "Repair Damage", externalToEVAOnly = true, guiActiveUnfocused = true, unfocusedRange = 2.0f)]
		public void Repair() {
			parent.repair();
		}
	}
}
