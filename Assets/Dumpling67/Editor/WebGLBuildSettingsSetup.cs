#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Dumpling67.EditorTools
{
    /// <summary>
    /// Меню: Dumpling67 → Apply WebGL Build Settings
    /// Настраивает WebGL Player Settings под Яндекс Игры/VK: компрессия, стриппинг, IL2CPP, шаблон.
    /// </summary>
    public static class WebGLBuildSettingsSetup
    {
        [MenuItem("Dumpling67/Apply WebGL Build Settings")]
        public static void Apply()
        {
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
            PlayerSettings.WebGL.decompressionFallback = false;
            PlayerSettings.WebGL.dataCaching = true;

            PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.WebGL, ManagedStrippingLevel.High);
            PlayerSettings.stripEngineCode = true;
            PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.WebGL, ScriptingImplementation.IL2CPP);
            PlayerSettings.WebGL.linkerTarget = WebGLLinkerTarget.Wasm;

            PlayerSettings.WebGL.threadsSupport = false;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.WebGL.template = "PROJECT:Dumpling67";

            Debug.Log("[WebGLBuildSettingsSetup] Player Settings применены. " +
                      "Осталось вручную: создать/назначить URP Asset и его Renderer " +
                      "(Assets → Create → Rendering → URP Asset), значения — см. docs/WEBGL_PERFORMANCE_BUDGET.md.");
        }
    }
}
#endif
