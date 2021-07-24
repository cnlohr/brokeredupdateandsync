# cnvrctools

CN's VRC Tools and Prefabs

# Included Tools

## BrokeredSync

Mechanism to do something similar to a VRCPickup, but much, much faster and
able to easily handle 1,000+ objects per map and still hit 100+ FPS.

Note that this only applies for things where you want players to be able to
pick up objects, place them down and stand on them once placed.  I.e. nothing
that is effected by gravity and nothing that keeps moving after the player has
let go.

(1) Add the BrokeredUpdateManager object to your scene, it doesn't matter where
(2) Add the BrokeredSync object to your scene.

To make more objects able to be sync'd, 
(1) "Add Component" "VRC Pickup"
(2) "Add Component" "BrokeredSync"

NOTE: This relies upon BrokeredUpdateManager, which is included in the
unitypackage.

## BrokeredUpdateManager

BrokeredSync allows Udon scripts to:

* Call GetIncrementingID() to get a unique ID, just a global counter.
* Call `RegisterSubscription( this )` / `UnregisterSubscription( this )` in order to get 'BrokeredUpdate()` called every frame during the period between both calls.
* Call `RegisterSlowUpdate( this )` / `UnregisterSlowUpdate( this )` in order to get `SlowUpdate()` called every several frames. (Between the two calls.) All slow updates are called round-robin.
	
			
# General Instructions for running test map:
 * Install VRC Worlds SDK 3.0
 * Install Udon Sharp
 * Install CyanEmu
 * Install AudioLink
 * Install TXL Player
