using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Core;

namespace NoMoreStuckProjectilesRedux
{
    public class MCMSettings : AttributeGlobalSettings<MCMSettings>
    {
        public override string Id => "NoMoreStuckProjectilesRedux";
        public override string DisplayName => "No More Stuck Projectiles Redux";
        public override string FolderName => "NoMoreStuckProjectilesRedux";
        public override string FormatType => "json";

        // ---------- General ----------
        [SettingPropertyBool("Only Affect Main Hero",
            HintText = "If enabled, only the player character (and their shield) is affected. If disabled, the limits apply to every agent in the mission. [Default: false]",
            Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("General", GroupOrder = 0)]
        public bool OnlyMainHero { get; set; } = false;

        [SettingPropertyBool("Affect Shields",
            HintText = "Also apply limits to projectiles sticking into shields. [Default: true]",
            Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("General", GroupOrder = 0)]
        public bool AffectShields { get; set; } = true;

        [SettingPropertyBool("Fatal Hits Always Stick",
            HintText = "A projectile that kills an agent will always stick, regardless of limits. [Default: true]",
            Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("General", GroupOrder = 0)]
        public bool FatalHitsAlwaysStick { get; set; } = true;

        // ---------- Character Limits ----------
        [SettingPropertyInteger("Arrow Limit", 0, 20,
            HintText = "Maximum number of arrows that can stick to a character. [Default: 20]",
            Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Character Limits", GroupOrder = 1)]
        public int ArrowLimit { get; set; } = 20;

        [SettingPropertyInteger("Bolt Limit", 0, 20,
            HintText = "Maximum number of bolts that can stick to a character. [Default: 20]",
            Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Character Limits", GroupOrder = 1)]
        public int BoltLimit { get; set; } = 20;

        [SettingPropertyInteger("Throwing Axe Limit", 0, 20,
            HintText = "Maximum number of throwing axes that can stick to a character. [Default: 0]",
            Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Character Limits", GroupOrder = 1)]
        public int ThrowingAxeLimit { get; set; } = 0;

        [SettingPropertyInteger("Throwing Knife Limit", 0, 20,
            HintText = "Maximum number of throwing knives that can stick to a character. [Default: 20]",
            Order = 3, RequireRestart = false)]
        [SettingPropertyGroup("Character Limits", GroupOrder = 1)]
        public int ThrowingKnifeLimit { get; set; } = 20;

        [SettingPropertyInteger("Javelin Limit", 0, 20,
            HintText = "Maximum number of javelins that can stick to a character. [Default: 0]",
            Order = 4, RequireRestart = false)]
        [SettingPropertyGroup("Character Limits", GroupOrder = 1)]
        public int JavelinLimit { get; set; } = 0;

        // ---------- Shield Limits ----------
        [SettingPropertyInteger("Shield Arrow Limit", 0, 20,
            HintText = "Maximum number of arrows that can stick to a shield. [Default: 20]",
            Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Shield Limits", GroupOrder = 2)]
        public int ShieldArrowLimit { get; set; } = 20;

        [SettingPropertyInteger("Shield Bolt Limit", 0, 20,
            HintText = "Maximum number of bolts that can stick to a shield. [Default: 20]",
            Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Shield Limits", GroupOrder = 2)]
        public int ShieldBoltLimit { get; set; } = 20;

        [SettingPropertyInteger("Shield Throwing Axe Limit", 0, 20,
            HintText = "Maximum number of throwing axes that can stick to a shield. [Default: 20]",
            Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Shield Limits", GroupOrder = 2)]
        public int ShieldThrowingAxeLimit { get; set; } = 20;

        [SettingPropertyInteger("Shield Throwing Knife Limit", 0, 20,
            HintText = "Maximum number of throwing knives that can stick to a shield. [Default: 20]",
            Order = 3, RequireRestart = false)]
        [SettingPropertyGroup("Shield Limits", GroupOrder = 2)]
        public int ShieldThrowingKnifeLimit { get; set; } = 20;

        [SettingPropertyInteger("Shield Javelin Limit", 0, 20,
            HintText = "Maximum number of javelins that can stick to a shield. [Default: 0]",
            Order = 4, RequireRestart = false)]
        [SettingPropertyGroup("Shield Limits", GroupOrder = 2)]
        public int ShieldJavelinLimit { get; set; } = 0;

        // ---------- Accessors ----------
        public int GetCharacterLimit(WeaponClass weaponClass)
        {
            switch (weaponClass)
            {
                case WeaponClass.Arrow: return ArrowLimit;
                case WeaponClass.Bolt: return BoltLimit;
                case WeaponClass.ThrowingAxe: return ThrowingAxeLimit;
                case WeaponClass.ThrowingKnife: return ThrowingKnifeLimit;
                case WeaponClass.Javelin: return JavelinLimit;
                default: return -1; // -1 = unmanaged weapon class; allow stick
            }
        }

        public int GetShieldLimit(WeaponClass weaponClass)
        {
            switch (weaponClass)
            {
                case WeaponClass.Arrow: return ShieldArrowLimit;
                case WeaponClass.Bolt: return ShieldBoltLimit;
                case WeaponClass.ThrowingAxe: return ShieldThrowingAxeLimit;
                case WeaponClass.ThrowingKnife: return ShieldThrowingKnifeLimit;
                case WeaponClass.Javelin: return ShieldJavelinLimit;
                default: return -1;
            }
        }
    }
}