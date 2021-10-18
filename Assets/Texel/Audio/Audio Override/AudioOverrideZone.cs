﻿
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Texel
{
    [AddComponentMenu("Texel/Audio/Audio Override Zone")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class AudioOverrideZone : UdonSharpBehaviour
    {
        public ZoneMembership membership;
        public CompoundZoneTrigger zone;

        public AudioOverrideSettings localZoneSettings;
        public bool localZoneEnabled = true;
        public AudioOverrideZone[] linkedZones;
        public AudioOverrideSettings[] linkedZoneSettings;
        public bool[] linkedZoneEnabled;
        public AudioOverrideSettings defaultSettings;
        public bool defaultEnabled = true;

        [NonSerialized]
        public VRCPlayerApi playerArg;

        AudioOverrideManager manager;
        int managedZoneId = -1;
        bool hasManager = false;
        bool hasMembership = false;

        bool hasLocal = false;
        int linkCount = 0;
        bool hasDefault = false;

        void Start()
        {
            if (Utilities.IsValid(zone))
                zone._Register((UdonBehaviour)(Component)this, "_PlayerEnter", "_PlayerLeave", "playerArg");

            if (Utilities.IsValid(linkedZones))
                linkCount = linkedZones.Length;

            hasMembership = Utilities.IsValid(membership);
            hasLocal = Utilities.IsValid(localZoneSettings);
            hasDefault = Utilities.IsValid(defaultSettings);
        }

        public void _Register(AudioOverrideManager overrideManager, int zoneId)
        {
            manager = overrideManager;
            managedZoneId = zoneId;
            hasManager = Utilities.IsValid(manager);
        }

        public int _ZoneId()
        {
            return managedZoneId;
        }

        public void _SetLocalActive(bool state)
        {
            if (localZoneEnabled != state)
            {
                localZoneEnabled = state;
                if (hasManager)
                    manager._RebuildLocal();
            }
        }

        public void _SetDefaultActive(bool state)
        {
            if (defaultEnabled != state)
            {
                defaultEnabled = state;
                if (hasManager)
                    manager._RebuildLocal();
            }
        }

        public void _SetLinkedZoneActive(AudioOverrideZone zone, bool state)
        {
            for (int i = 0; i < linkedZones.Length; i++)
            {
                if (zone == linkedZones[i])
                {
                    if (linkedZoneEnabled[i] != state)
                    {
                        linkedZoneEnabled[i] = state;
                        if (hasManager)
                            manager._RebuildLocal();
                    }
                    break;
                }
            }
        }

        public void _PlayerEnter()
        {
            if (hasMembership)
                membership._AddPlayer(playerArg);
            if (hasManager)
                manager._PlayerEnterZone(this, playerArg);
        }

        public void _PlayerLeave()
        {
            if (hasMembership)
                membership._RemovePlayer(playerArg);
            if (hasManager)
                manager._PlayerLeaveZone(this, playerArg);
        }

        public bool _ContainsPlayer(VRCPlayerApi player)
        {
            if (!hasMembership)
                return true;

            return membership._ContainsPlayer(player);
        }

        public bool _Apply(VRCPlayerApi player)
        {
            if (hasLocal && localZoneEnabled && _ContainsPlayer(player))
            {
                localZoneSettings._Apply(player);
                return true;
            }

            for (int i = 0; i < linkCount; i++)
            {
                AudioOverrideZone zone = linkedZones[i];
                bool zoneEnabled = linkedZoneEnabled[i];
                if (zoneEnabled && zone._ContainsPlayer(player))
                {
                    linkedZoneSettings[i]._Apply(player);
                    return true;
                }
            }

            if (hasDefault && defaultEnabled)
            {
                defaultSettings._Apply(player);
                return true;
            }

            return false;
        }
    }
}
