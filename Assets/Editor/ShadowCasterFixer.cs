using UnityEngine;
using UnityEditor;

public class ShadowCasterFixer : AssetPostprocessor
{
    // Este script se ejecuta automáticamente en el Editor.
    // Evita que Unity borre el pase "ShadowCaster" de tus materiales Transparentes.

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            if (assetPath.EndsWith(".mat") && assetPath.Contains("SandBox")) 
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                
                if (mat != null && mat.shader.name.Contains("Lit"))
                {
                    // Si el material es transparente Y tiene Alpha Clipping activado
                    if (mat.HasProperty("_Surface") && mat.GetFloat("_Surface") == 1 // 1 = Transparent
                        && mat.HasProperty("_AlphaClip") && mat.GetFloat("_AlphaClip") == 1) // 1 = On
                    {
                        // HACK DE UNITY: Re-encender las sombras
                        bool changed = false;
                        
                        // Eliminar SHADOWCASTER de los pases deshabilitados si Unity lo bloqueó
                        if (mat.GetShaderPassEnabled("ShadowCaster") == false)
                        {
                            mat.SetShaderPassEnabled("ShadowCaster", true);
                            changed = true;
                        }

                        if (changed)
                        {
                            Debug.Log($"[ShadowFixer] Forzando Sombras en material Transparente: {mat.name}");
                            // Guardar el material sin recargar el AssetDatabase en un loop infinito
                            EditorUtility.SetDirty(mat);
                        }
                    }
                }
            }
        }
    }
}
