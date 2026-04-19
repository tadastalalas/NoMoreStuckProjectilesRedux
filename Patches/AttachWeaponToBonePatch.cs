using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NoMoreStuckProjectilesRedux.Patches
{
    [HarmonyPatch(typeof(Agent), nameof(Agent.AttachWeaponToBone))]
    public static class AttachWeaponToBonePatch
    {
        public static bool Prefix(MissionWeapon weapon, GameEntity weaponEntity, sbyte boneIndex, ref MatrixFrame attachLocalFrame, Agent __instance)
        {
            var settings = MCMSettings.Instance;
            if (settings == null)
            {
                return true;
            }

            // Restrict to main agent if configured.
            if (settings.OnlyMainHero && !__instance.IsMainAgent)
            {
                return true;
            }

            // Fatal hits always stick if configured.
            if (settings.FatalHitsAlwaysStick && __instance.Health <= 0f)
            {
                return true;
            }

            var usage = weapon.CurrentUsageItem;
            if (usage == null)
            {
                return true;
            }

            WeaponClass projectileType = usage.WeaponClass;
            int limit = settings.GetCharacterLimit(projectileType);

            // -1 means this weapon class is not managed by the mod: leave vanilla behavior.
            if (limit < 0)
            {
                return true;
            }

            // Fast path: limit 0 means never stick.
            if (limit == 0)
            {
                return false;
            }

            int projectileCounter = 0;
            int attachedWeaponCount = __instance.GetAttachedWeaponsCount();
            for (int i = 0; i < attachedWeaponCount; i++)
            {
                var attachedUsage = __instance.GetAttachedWeapon(i).CurrentUsageItem;
                if (attachedUsage != null && attachedUsage.WeaponClass == projectileType)
                {
                    projectileCounter++;
                    if (projectileCounter >= limit)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}