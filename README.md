# IT201-009
 
**Code Features:**

1.  **Speed Boost Power-Up System with Visual Trail****:** Implements a temporary speed boost with visual trail effect when collecting pickups.
2.  **Timer System with Time Bonuses:** Adds a countdown timer that decreases over time and extends when collecting pickups.
3.  **Death and Respawn Mechanics:** Handles player death with animation states and respawn functionality.
4.  **Fall Detection System:** Automatically respawns player when falling below a threshold.
5.  **Animator Setup:** Implements smooth movement with normalized velocity and character rotation. Three animation states: `IsMoving (bool)`, `Die (trigger)`, `Victory (trigger)`.

**Editor Features:**

These can be changed in the inspector.

1.  **Power-Up Configuration:**
    * Speed Boost Multiplier
    * Speed Boost Duration
    * Trail Color
    * Trail Width
2.  **Respawn System Configuration:** 
    * Respawn Point
    * Fall Threshold
    * Respawn Delay
3.   **Movement Settings:**
    * Movement Speed
    * Rotation Speed
4.  **UI References**
    * Count Text
    * Win Text Object
    * Timer Text
5.  **Animator Setup and Controller:**   
    * Added parameters (IsMoving, Die, Victory) to the Animator component
    * Created transitions between animation states   
    * Set up the default animation state
    * Changed from default animator to TinyHeroController
    * Set up animation states for Idle, Run, Death, and Victory