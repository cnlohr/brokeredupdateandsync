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

1. Add the BrokeredUpdateManager object to your scene, it doesn't matter where.
2. Note: You must use exactly `BrokeredUpdateManager` name for the manager object.
3. Add the BrokeredSync object to your scene.

To make more objects able to be sync'd, on your object you want brokered sync on,
1. "Add Component" "VRC Pickup"
2. "Add Component" "BrokeredSync"

I recommend using a box collider. If you want the blocks to behave fun, uncheck
"Use Gravity" and check "Is Kinematic" and uncheck "Is Trigger"

NOTE: This relies upon BrokeredUpdateManager, which is included in the
unitypackage.

**NOTE:** This is for kinematic objects, not objects which can roll around.  Though
you may have some limited success, I have not yet made this disable objects once
moving for the master.


To use the Sync Manager, after being added to your world, you could do something like this:
```
	void Start()
	{
		GameObject.Find( "BrokeredUpdateManager" ).GetComponent<BrokeredUpdateManager>().RegisterSnailUpdate( this );
	}
	...
	public void SnailUpdate()
	{
		// This gets called sometimes.
	}
``

## BrokeredUpdateManager

BrokeredSync allows Udon scripts to:

* Call GetIncrementingID() to get a unique ID, just a global counter.
* Call `RegisterSubscription( this )` / `UnregisterSubscription( this )` in order to get 'BrokeredUpdate()` called every frame during the period between both calls.
* Call `RegisterSlowUpdate( this )` / `UnregisterSlowUpdate( this )` in order to get `SlowUpdate()` called every several frames. (Between the two calls.) All slow updates are called round-robin.
* Call `RegisterSnailUpdate( this )` / `UnregisterSnailUpdate( this )` in order to get `SnailUpdate()` called every several frames. (Between the two calls.) All snail updates are called round-robin, but with a delay.
	
			
# General Instructions for running test map:
 * Install VRC Worlds SDK 3.0
 * Install Udon Sharp
 * Install CyanEmu
 * Install AudioLink
 * Install TXL Player
