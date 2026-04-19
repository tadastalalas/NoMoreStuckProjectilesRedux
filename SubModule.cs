using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace NoMoreStuckProjectilesRedux
{
    public class SubModule : MBSubModuleBase
    {
        private const string HarmonyId = "NoMoreStuckProjectilesRedux";

        private Harmony? _harmony;
        private bool _patched;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            // Intentionally do NOT PatchAll here.
            // Patching Agent methods this early triggers the "meat bullet" projectile
            // bug introduced around Bannerlord 1.3.0. Defer to OnBeforeInitialModuleScreenSetAsRoot.
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            if (_patched)
            {
                return;
            }

            _harmony = new Harmony(HarmonyId);
            _harmony.PatchAll();
            _patched = true;
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();

            if (_patched)
            {
                _harmony?.UnpatchAll(HarmonyId);
                _harmony = null;
                _patched = false;
            }
        }
    }
}