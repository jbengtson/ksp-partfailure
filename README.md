ksp-partfailure
===============

Part Failure for Kerbal Space Program

Provides random part failures in many forms, including outright explosions, fuel leaks, and cascading electrical shorts, depending on what PartModules are loaded into a part.

This is still in a pre-alpha state.  Currently a single testing damage type is available to be assigned and repaired based on whether the part has resources other than electricity.

Future features include:
- Damage persistence.
- Multiple different damage types depending on loaded PartModules.
- User-configurable failure rates.
- Since each failure is a PartModule they can be customized to do anything once assigned.  One possibility is to "cascade" electrical shorts throughout the craft unless repaired.

Although I don't outright copy code, I have learned a lot from the following mod authors' code and have decided to license accordingly:

- ialdabaoth: Deadly Reentry and Modular Fuel System
- TaranisElsu: TAC Life Support

And, of course, thanks to #kspmodders for letting me bug them with dumb newbie questions.
