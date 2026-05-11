using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NoMoreStuckProjectilesRedux.Patches
{
    [HarmonyPatch(typeof(Mission), nameof(Mission.HandleMissileCollisionReaction))]
    public static class HandleMissileCollisionReactionPatch
    {
        private static readonly FieldInfo MissilesDictionaryField =
            AccessTools.Field(typeof(Mission), "_missilesDictionary");

        private static readonly System.Type MissileType =
            AccessTools.Inner(typeof(Mission), "Missile");

        private static readonly PropertyInfo MissileWeaponProperty =
            MissileType != null ? AccessTools.Property(MissileType, "Weapon") : null;

        private static readonly PropertyInfo MissileEntityProperty =
            MissileType != null ? AccessTools.Property(MissileType, "Entity") : null;

        public static void Prefix(int missileIndex,
                                  ref Mission.MissileCollisionReaction collisionReaction,
                                  ref MatrixFrame attachLocalFrame,
                                  ref bool isAttachedFrameLocal,
                                  Agent attachedAgent,
                                  bool attachedToShield,
                                  ref Vec3 bounceBackVelocity,
                                  ref Vec3 bounceBackAngularVelocity,
                                  Mission __instance)
        {
            if (collisionReaction == Mission.MissileCollisionReaction.BecomeInvisible)
            {
                RewriteToBounce(__instance, missileIndex,
                    ref collisionReaction, ref attachLocalFrame, ref isAttachedFrameLocal,
                    ref bounceBackVelocity, ref bounceBackAngularVelocity);
                return;
            }

            if (collisionReaction != Mission.MissileCollisionReaction.Stick)
                return;

            var settings = MCMSettings.Instance;
            if (settings == null || attachedAgent == null)
                return;

            if (settings.OnlyMainHero && !attachedAgent.IsMainAgent)
                return;

            if (settings.FatalHitsAlwaysStick && attachedAgent.Health <= 0f)
                return;

            if (!TryGetMissile(__instance, missileIndex, out object missile, out MBMissile _))
            {
                RewriteToBounce(__instance, missileIndex,
                    ref collisionReaction, ref attachLocalFrame, ref isAttachedFrameLocal,
                    ref bounceBackVelocity, ref bounceBackAngularVelocity);
                return;
            }

            if (!(MissileWeaponProperty?.GetValue(missile, null) is MissionWeapon weapon))
                return;

            var usage = weapon.CurrentUsageItem;

            if (usage == null)
                return;

            WeaponClass projectileType = usage.WeaponClass;
            int limit;

            if (attachedToShield)
            {
                if (!settings.AffectShields)
                    return;
                limit = settings.GetShieldLimit(projectileType);
            }
            else
            {
                limit = settings.GetCharacterLimit(projectileType);
            }

            if (limit < 0)
                return;

            if (CountAttachedOfClass(attachedAgent, attachedToShield, projectileType) < limit)
            {
                return;
            }

            RewriteToBounce(__instance, missileIndex,
                ref collisionReaction, ref attachLocalFrame, ref isAttachedFrameLocal,
                ref bounceBackVelocity, ref bounceBackAngularVelocity);
        }

        private static void RewriteToBounce(Mission mission, int missileIndex,
                                            ref Mission.MissileCollisionReaction collisionReaction,
                                            ref MatrixFrame attachLocalFrame,
                                            ref bool isAttachedFrameLocal,
                                            ref Vec3 bounceBackVelocity,
                                            ref Vec3 bounceBackAngularVelocity)
        {
            collisionReaction = Mission.MissileCollisionReaction.BounceBack;
            bounceBackAngularVelocity = Vec3.Zero;

            if (!TryGetMissile(mission, missileIndex, out object missile, out MBMissile mbMissile))
            {
                bounceBackVelocity = Vec3.Zero;
                return;
            }

            if (MissileEntityProperty?.GetValue(missile, null) is GameEntity entity)
            {
                attachLocalFrame = entity.GetGlobalFrame();
                isAttachedFrameLocal = false;
            }

            bounceBackVelocity = mbMissile != null ? mbMissile.GetVelocity() * 0.25f : Vec3.Zero;
        }

        private static bool TryGetMissile(Mission mission, int missileIndex, out object missile, out MBMissile mbMissile)
        {
            missile = null;
            mbMissile = null;

            if (MissilesDictionaryField == null)
                return false;

            try
            {
                if (!(MissilesDictionaryField.GetValue(mission) is IDictionary dict)) return false;
                if (!dict.Contains(missileIndex)) return false;

                missile = dict[missileIndex];
                mbMissile = missile as MBMissile;
                return missile != null;
            }
            catch
            {
                missile = null;
                mbMissile = null;
                return false;
            }
        }

        private static int CountAttachedOfClass(Agent agent, bool onShield, WeaponClass weaponClass)
        {
            int counter = 0;

            if (onShield)
            {
                EquipmentIndex offhand = agent.GetOffhandWieldedItemIndex();
                if (offhand == EquipmentIndex.None) return 0;

                MissionWeapon shield = agent.Equipment[offhand];
                int count = shield.GetAttachedWeaponsCount();
                for (int i = 0; i < count; i++)
                {
                    var u = shield.GetAttachedWeapon(i).CurrentUsageItem;
                    if (u != null && u.WeaponClass == weaponClass) counter++;
                }
            }
            else
            {
                int count = agent.GetAttachedWeaponsCount();
                for (int i = 0; i < count; i++)
                {
                    var u = agent.GetAttachedWeapon(i).CurrentUsageItem;
                    if (u != null && u.WeaponClass == weaponClass) counter++;
                }
            }

            return counter;
        }
    }
}