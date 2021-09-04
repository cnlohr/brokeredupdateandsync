# Brokered Update And Sync

Map is live: https://vrchat.com/home/world/wrld_04bba9b6-cc9d-4abe-a44c-1d1046f8ce16

![image](https://user-images.githubusercontent.com/2748168/132091759-6dec5fca-e4e6-4a3a-8589-d18ab70005e4.png)

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
using BrokeredUpdates;
...
	void Start()
	{
		GameObject.Find( "BrokeredUpdateManager" ).GetComponent<BrokeredUpdateManager>()._RegisterSnailUpdate( this );
	}
	...
	public void _SnailUpdate()
	{
		// This gets called sometimes.
	}
```

## BrokeredUpdateManager

BrokeredSync allows Udon scripts to:

* Call _GetIncrementingID() to get a unique ID, just a global counter.
* Call `_RegisterSubscription( this )` / `_UnregisterSubscription( this )` in order to get '_BrokeredUpdate()` called every frame during the period between both calls.
* Call `_RegisterSlowUpdate( this )` / `_UnregisterSlowUpdate( this )` in order to get `_SlowUpdate()` called every several frames. (Between the two calls.) All slow updates are called round-robin.
* Call `_RegisterSnailUpdate( this )` / `_UnregisterSnailUpdate( this )` in order to get `_SnailUpdate()` called every several frames. (Between the two calls.) All snail updates are called round-robin, but with a delay.

Note SlowObjectSync functionality is internal - and used by the brokered sync object.

# General Instructions for running test map:
 * Install VRC Worlds SDK 3.0
 * Install Udon Sharp
 * Install CyanEmu
 * Install AudioLink
 * Install TXL Player

### Future work:

1. Further investigate the quirks of not getting deserialization events.
2. Try more traditionally attaching the object to a player when it's moving instead of just smoothing the lerp {though I personally prefer the smoothed lerp that I'm doing now to player attachment}.
3. Handle disabling motion when object actually stops moving for it's master.
