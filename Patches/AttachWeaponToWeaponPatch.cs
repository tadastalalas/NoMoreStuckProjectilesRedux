using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NoMoreStuckProjectilesRedux.Patches
{
    [HarmonyPatch(typeof(Agent), nameof(Agent.AttachWeaponToWeapon))]
    public static class AttachWeaponToWeaponPatch
    {
        public static bool Prefix(EquipmentIndex slotIndex, MissionWeapon weapon, GameEntity weaponEntity, ref MatrixFrame attachLocalFrame, Agent __instance)
        {
            var settings = MCMSettings.Instance;
            if (settings == null || !settings.AffectShields)
            {
                return true;
            }

            if (settings.OnlyMainHero && !__instance.IsMainAgent)
            {
                return true;
            }

            var usage = weapon.CurrentUsageItem;
            if (usage == null)
            {
                return true;
            }

            WeaponClass projectileType = usage.WeaponClass;
            int limit = settings.GetShieldLimit(projectileType);

            if (limit < 0)
            {
                return true;
            }

            if (limit == 0)
            {
                return false;
            }

            MissionWeapon shield = __instance.Equipment[slotIndex];
            int projectileCounter = 0;
            int attachedWeaponCount = shield.GetAttachedWeaponsCount();
            for (int i = 0; i < attachedWeaponCount; i++)
            {
                var attachedUsage = shield.GetAttachedWeapon(i).CurrentUsageItem;
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