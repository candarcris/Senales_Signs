using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class ShadowSetupTools : EditorWindow
{
    [MenuItem("Tools/Setup SagarLayers Shadows")]
    public static void SetupShadows()
    {
        GameObject sagarLayers = GameObject.Find("SagarLayers");
        if (sagarLayers == null)
        {
            EditorUtility.DisplayDialog("Error", "Object 'SagarLayers' not found in the scene!", "OK");
            return;
        }

        // 1. Create or Load Material
        string materialPath = "Assets/Materials/SpriteLit.mat";
        Material spriteLitMat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

        if (spriteLitMat == null)
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null)
            {
                // Fallback for newer URP versions closest match if exact name changed, 
                // or just standard Lit if URP specific not found (unlikely in URP project).
                litShader = Shader.Find("Universal Render Pipeline/Lit"); 
                if(litShader == null) litShader = Shader.Find("URP/Lit");
            }

            if (litShader == null)
            {
                EditorUtility.DisplayDialog("Error", "Could not find 'Universal Render Pipeline/Lit' shader.", "OK");
                return;
            }

            spriteLitMat = new Material(litShader);
            AssetDatabase.CreateAsset(spriteLitMat, materialPath);
            Debug.Log("Created new Material: " + materialPath);
        }

        // ALWAYS Update Material Properties
        // Fix: Use Opaque Surface with Alpha Clipping to ensure shadows are cast.
        spriteLitMat.SetFloat("_Surface", 0); // Opaque
        spriteLitMat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
        
        spriteLitMat.SetFloat("_AlphaClip", 1); // Enable Alpha Clipping
        spriteLitMat.EnableKeyword("_ALPHATEST_ON");
        spriteLitMat.SetFloat("_Cutoff", 0.5f);
        
        // Set Render Queue to AlphaTest
        spriteLitMat.renderQueue = 2450;

        // 2. Apply to all SpriteRenderers
        SpriteRenderer[] renderers = sagarLayers.GetComponentsInChildren<SpriteRenderer>(true);
        Undo.RecordObjects(renderers, "Setup Shadows SagarLayers");

        int count = 0;
        foreach (var sr in renderers)
        {
            sr.shadowCastingMode = ShadowCastingMode.On;
            sr.receiveShadows = true;
            sr.sharedMaterial = spriteLitMat;
            count++;
        }

        EditorUtility.SetDirty(sagarLayers);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Success", $"Updated {count} SpriteRenderers on '{sagarLayers.name}'.\nMaterial: {materialPath}", "OK");
    }
}
