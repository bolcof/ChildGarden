using System;
using UnityEngine;

namespace VortexStudios.PostProcessing {
    [Serializable]
    public class ScanlineProfile : PostProcessingProfile {
        [SerializeField]
        public Texture pattern = null;
        [SerializeField]
        public int lineCount = 224;
        [SerializeField]
        public float magnetude = 0.75f;

        public override void MaterialUpdate() {
            if (materials == null || materials.Length == 0) {
                materials = new Material[1];
            }

            if (materials[0] != null) {
                materials[0].SetTexture("_PatternTex", pattern);
                materials[0].SetFloat("_Magnitude", 1.0f - magnetude);
                materials[0].SetInt("_LineCount", lineCount); 
            } else {
                materials[0] = new Material(Shader.Find("Vortex Game Studios/Filters/OLD TV Filter/Scanline"));
                if (pattern == null)
                    pattern = materials[0].GetTexture("_PatternTex");
            }
        }
    }
}