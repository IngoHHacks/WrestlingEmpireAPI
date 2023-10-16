using System.Globalization;

namespace WECCL.Patches;

[HarmonyPatch]
public class LogPatch
{
    /*
     * This patch is used to enable the game's unity logger if the user has enabled it in the config.
     */
    [HarmonyPatch(typeof(UnmappedGlobals), nameof(UnmappedGlobals.FCNKHBIIJLG))]
    [HarmonyPostfix]
    public static void Globals_FCNKHBIIJLG()
    {
        Debug.unityLogger.logEnabled = Plugin.EnableGameUnityLog.Value;
        Debug.unityLogger.filterLogType = Plugin.GameUnityLogLevel.Value.ToLower() == "error" ? LogType.Error :
            Plugin.GameUnityLogLevel.Value.ToLower() == "warning" ? LogType.Warning : LogType.Log;

        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");
    }
}