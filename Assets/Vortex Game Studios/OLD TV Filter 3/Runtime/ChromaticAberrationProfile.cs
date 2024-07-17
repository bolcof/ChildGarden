using System;
using UnityEngine;

namespace VortexStudios.PostProcessing {
    [System.Serializable]
    public class ChromaticAberrationProfile : PostProcessingProfile {
        [SerializeField]
        public Texture pattern = null;
        [SerializeField]
        public float magnetude = 0.2f;

        public override void MaterialUpdate() {
            if (materials == null || materials.Length == 0) {
                materials = new Material[1];
            }

            if (materials[0] != null) {
                materials[0].SetTexture("_PatternTex", pattern);
                materials[0].SetFloat("_Magnitude", magnetude);
            } else {
                materials[0] = new Material(Shader.Find("Vortex Game Studios/Filters/OLD TV Filter/ChromaticAberration"));
                if (pattern == null)
                    pattern = materials[0].GetTexture("_PatternTex");
            }
        }
    }
}