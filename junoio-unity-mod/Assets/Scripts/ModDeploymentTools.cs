namespace Assets.Scripts
{
#if UNITY_EDITOR
    using System;
    using System.IO;
    using System.Threading;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Editor-only helpers for deploying the locally built mod package into Juno's live mods folder.
    /// </summary>
    internal static class ModDeploymentTools
    {
        private const string ModFileName = "JunoIO.sr2-mod";
        private const string ModInfoFileName = "JunoIO.sr2-mod-info";
        private const int CopyRetryCount = 8;
        private const int CopyRetryDelayMs = 300;

        [MenuItem("Tools/JunoIO/Deploy Latest Built Mod To Juno Mods Folder")]
        private static void DeployLatestBuiltMod()
        {
            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName
                ?? throw new InvalidOperationException("Could not determine the Unity project root.");
            string buildOutputDirectory = Path.Combine(projectRoot, "ModAssetBundles");
            string builtModPath = Path.Combine(buildOutputDirectory, ModFileName);
            string builtInfoPath = Path.Combine(buildOutputDirectory, ModInfoFileName);
            string modsDirectory = GetJunoModsDirectory();
            string liveModPath = Path.Combine(modsDirectory, ModFileName);
            string liveInfoPath = Path.Combine(modsDirectory, ModInfoFileName);

            if (!File.Exists(builtModPath))
            {
                EditorUtility.DisplayDialog(
                    "JunoIO Deploy",
                    "No built mod package was found yet.\n\n" +
                    "Build the mod first so that ModAssetBundles\\JunoIO.sr2-mod exists, then run this deploy step.",
                    "OK");
                return;
            }

            Directory.CreateDirectory(modsDirectory);

            try
            {
                CopyWithRetries(builtModPath, liveModPath);

                if (File.Exists(builtInfoPath))
                {
                    CopyWithRetries(builtInfoPath, liveInfoPath);
                }

                Debug.Log($"JunoIO deployed to {liveModPath}");
                EditorUtility.DisplayDialog(
                    "JunoIO Deploy",
                    "Deployed the latest built mod package into the live Juno mods folder.",
                    "OK");
            }
            catch (IOException ex)
            {
                Debug.LogError($"JunoIO deploy failed: {ex}");
                EditorUtility.DisplayDialog(
                    "JunoIO Deploy Failed",
                    "Could not overwrite the live mod package because Windows reported it is in use.\n\n" +
                    $"Built package: {builtModPath}\n" +
                    $"Live target: {liveModPath}\n\n" +
                    "Close Juno: New Origins and any file browser or tool holding the mod file open, then run:\n" +
                    "Tools > JunoIO > Deploy Latest Built Mod To Juno Mods Folder",
                    "OK");
            }
        }

        [MenuItem("Tools/JunoIO/Open Built Mod Output Folder")]
        private static void OpenBuiltModOutputFolder()
        {
            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName
                ?? throw new InvalidOperationException("Could not determine the Unity project root.");
            string buildOutputDirectory = Path.Combine(projectRoot, "ModAssetBundles");
            Directory.CreateDirectory(buildOutputDirectory);
            EditorUtility.RevealInFinder(buildOutputDirectory);
        }

        [MenuItem("Tools/JunoIO/Open Juno Mods Folder")]
        private static void OpenJunoModsFolder()
        {
            string modsDirectory = GetJunoModsDirectory();
            Directory.CreateDirectory(modsDirectory);
            EditorUtility.RevealInFinder(modsDirectory);
        }

        private static string GetJunoModsDirectory()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Low",
                "Jundroo",
                "SimpleRockets 2",
                "Mods");
        }

        private static void CopyWithRetries(string sourcePath, string destinationPath)
        {
            Exception lastException = null;

            for (int attempt = 0; attempt < CopyRetryCount; attempt++)
            {
                try
                {
                    File.Copy(sourcePath, destinationPath, overwrite: true);
                    return;
                }
                catch (IOException ex)
                {
                    lastException = ex;
                }
                catch (UnauthorizedAccessException ex)
                {
                    lastException = ex;
                }

                Thread.Sleep(CopyRetryDelayMs);
            }

            throw new IOException(
                $"Failed to copy {sourcePath} to {destinationPath} after {CopyRetryCount} attempts.",
                lastException);
        }
    }
#endif
}
