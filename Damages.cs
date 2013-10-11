using System;
using UnityEngine;
using KSP.IO;

 /******************************************************************************
 * Copyright (c) 2013, Justin Bengtson
 *
 * Part Failure for Kerbal Space Program
 * 
 * This code is licensed under Creative Commons CC BY-NC-SA 3.0
 * Attribution-NonCommercial-ShareAlike 3.0 Unported
 *
 * License text available here:
 * http://creativecommons.org/licenses/by-nc-sa/3.0/legalcode
 ******************************************************************************/

namespace RegexKSP {
    public interface IConfigSave {
        void saveConfigNode(ConfigNode config);
    }

	public class DamageTestModule : PartModule, IConfigSave {
		[KSPField(isPersistant = true, guiActive = true, guiName = "Damage", guiUnits = "", guiFormat = "G")]
		public string displayDamage = "";

		[KSPField(isPersistant = true, guiActive = false]
        public double lastPollTime = 0.0;

		public override void OnAwake() {
			ScreenMessages.PostScreenMessage(this.part.partInfo.title + " has been damaged!", 5.0f, ScreenMessageStyle.UPPER_CENTER);
			this.isEnabled = true;
			displayDamage = "Testing";
		}

		public override void OnFixedUpdate() {
            double timeNow = Planetarium.GetUniversalTime();
            double timeDiff = timeNow - lastPollTime;
            lastPollTime = timeNow;
		}

		[KSPEvent(guiName = "Repair Test Damage", externalToEVAOnly = true, guiActiveUnfocused = true, unfocusedRange = 2.0f)]
		public void Repair() {
			var vessel = FlightGlobals.ActiveVessel;
			if(!vessel.isEVA) { return; }
			this.part.RemoveModule(this);
			PartDamage.markGUIDirty(this.part);
		}

        public void saveConfigNode(ConfigNode config) {
            ConfigNode retval = new ConfigNode("MODULE");
            retval.AddValue("name", "DamageTestModule");
            retval.AddValue("displayDamage", displayDamage);
            retval.AddValue("lastPollTime", lastPollTime);
            retval.AddValue("vesselId", );
            retval.AddValue("partId", );

            config.AddNode(node);
        }
	}
}
