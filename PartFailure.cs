using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
	[KSPAddon(KSPAddon.Startup.Flight, false)]

	public class PartDamage : MonoBehaviour {
		private bool configLoaded = false;
		public PluginConfiguration config;
		private double checkInterval = 10.0;
		private double checkThreshold = 0.9; // chance per interval seconds damage happens
		private int randomTries = 5;
		private double pollTime = 0.0;
		private System.Random random = new System.Random();

		public void Awake() {
			loadConfig();
		}

		public void OnDisable() {
			saveConfig();
		}

        public void OnDestroy() {
        }

		public void FixedUpdate() {
			if(canRun) {
				double timeNow = Planetarium.GetUniversalTime();
				double timeDiff = timeNow - pollTime;
				if(timeDiff > checkInterval) {
					pollTime = timeNow;
					if(random.NextDouble() < checkThreshold) {
						doDamage();
					}
				}
			}
		}

		public static void markGUIDirty(Part p) {
			foreach(UIPartActionWindow window in FindObjectsOfType(typeof(UIPartActionWindow))) {
				if(window.part == p) {
					window.displayDirty = true;
				}
			}
		}

/*
		private void doDamage() {
			List<Part> parts = FlightGlobals.ActiveVessel.parts;
			int numParts = parts.Count;
			Debug.Log("Number of parts: " + numParts);
			// try to find a part randomly.
			bool found = false;
			for(int k = 0; k < randomTries; k++) {
				Part p = parts[random.Next(0, numParts - 1)];
				if(found = canDamage(p)) {
					break;
				}
			}

			// if we haven't found one randomly, grab the first one we can fuck up.
			if(!found) {
				foreach(Part p in parts) {
					if(canDamage(p)) {
						break;
					}
				}
			}
		}
*/

        public bool doDamage(string typeModule) {
            
        }

        private List<Part> findEligibleParts(string typeModule, List<Part> pCol) {
            List<Part> eligible = new List<Part>();
            foreach(Part p in pCol) {
                bool has = false;
                foreach(PartModule pm in p.Modules) {
                    if(pm.ClassName == typeModule) {
                        has = true;
                    }
                }
                if(has) { eligible.Add(p); }
            }
            return eligible;
        }

        private List<Part> findEligibleResourceParts(List<Part> pCol) {
            List<Part> eligible = new List<Part>();
            foreach(Part p in pCol) {
    			if(p.Resources.Count > 0) {
                    eligible.Add(p);
                }
            }
            return eligible;
        }

		private bool canDamage(Part p) {
			if(hasModule(p, "ModuleGimbal")) {
				// engine gimbal damage
				p.AddModule("DamageTestModule");
				return true;
			}
			if(hasModule(p, "ModuleEngines")) {
				// engine damage
				p.AddModule("DamageTestModule");
				return true;
			}
			if(hasModule(p, "ModuleParachute")) {
				// parachute damage
				p.AddModule("DamageTestModule");
				return true;
			}
			if(hasModule(p, "ModuleDecouple") || hasModule(p, "ModuleAnchoredDecoupler")) {
				// decoupler damage
				p.AddModule("DamageTestModule");
				return true;
			}
			if(hasModule(p, "ModuleDockingNode")) {
				// docking port damage
				p.AddModule("DamageTestModule");
				return true;
			}
			if(p.Resources.Count > 0) {
				// fuel tank damage
				Debug.Log("Found a fuel tank");
				addPartModule(p, "DamageTestModule");
				return true;
			}
			if(hasModule(p, "ModuleResourceIntake")) {
				// intake damage
				p.AddModule("DamageTestModule");
				return true;
			}
			if(hasModule(p, "ModuleWheel")) {
				// wheel damage
				p.AddModule("DamageTestModule");
				return true;
			}
			if(hasModule(p, "ModuleEnviroSensor")) {
				// sensor damage
				p.AddModule("DamageTestModule");
				return true;
			}
			if(hasModule(p, "ModuleDeployableSolarPanel")) {
				// solar panel damage
				p.AddModule("DamageTestModule");
				return true;
			}
			if(hasModule(p, "ModuleGenerator")) {
				// generator damage
				p.AddModule("DamageTestModule");
				return true;
			}
			return false;
		}

		private void addPartModule(Part p, string module) {
			try {
				ConfigNode node = new ConfigNode("MODULE");
				node.AddValue("name", module);
				p.AddModule(node);
			} catch(Exception e) {
				Debug.Log("PartDamage: Failed to add PartModule" + module + "--> " + e.Message + "\n" + e.StackTrace);
			}
		}

/*
		private bool hasModule(Part p, string className) {
			foreach(PartModule pm in p.Modules) {
				if(pm.ClassName == className) {
					return true;
				}
			}
			return false;
		}
*/

		private bool canRun {
			get {
				return FlightGlobals.fetch != null && FlightGlobals.ActiveVessel != null;
			}
		}

		private void loadConfig() {
			Debug.Log("Loading PartDamage settings.");
			if(!configLoaded) {
				config = KSP.IO.PluginConfiguration.CreateForType<PartDamage>(null);
				config.load();
				configLoaded = true;

				try {
					checkInterval = config.GetValue<double>("checkInterval", 10.0);
					checkThreshold = config.GetValue<double>("checkThreshold", 0.9);
					randomTries = config.GetValue<int>("randomTries", 5);
				} catch(ArgumentException) {
					// do nothing here, the defaults are already set
				}
			}
		}

		private void saveConfig() {
			Debug.Log("Saving PartDamage settings.");
			if(!configLoaded) {
				config = KSP.IO.PluginConfiguration.CreateForType<PartDamage>(null);
			}

			config["checkInterval"] = checkInterval;
			config["checkThreshold"] = checkThreshold;
			config["randomTries"] = randomTries;

			config.save();
		}
	}
}
