using System;
using UnityEngine;

namespace VortexStudios.PostProcessing {
    [System.Serializable]
    public class TubeProfile : PostProcessingProfile {
        [SerializeField]
        public Texture maskPattern = null;
        [SerializeField]
        public Texture reflexPattern = null;
        [SerializeField]
        public float distortionMagnitude = 0.15f;
        [SerializeField]
        public float reflexMagnitude = 0.15f;

        public override void MaterialUpdate() {
            if (materials == null || materials.Length == 0) {
                materials = new Material[1];
            }

            if (materials[0] != null) {
                materials[0].SetTexture("_MaskTex", maskPattern);
                materials[0].SetTexture("_ReflexTex", reflexPattern);
                materials[0].SetFloat("_Distortion", distortionMagnitude);
                materials[0].SetFloat("_Reflex", reflexMagnitude);
            } else {
                materials[0] = new Material(Shader.Find("Vortex Game Studios/Filters/OLD TV Filter/Tube"));
                if (maskPattern == null)
                    maskPattern = materials[0].GetTexture("_MaskTex");
            }
        }
    }
}