using System;
using UnityEngine;

namespace VortexStudios.PostProcessing {
    [System.Serializable]
    public class TelevisionProfile : PostProcessingProfile {
        [SerializeField]
        public int lineCount = 224;

        [SerializeField]
        public Vector2 sync = new Vector2( 0.0f, 0.0f );
        private Vector2 _sync = new Vector2( 0.0f, 0.0f );
        [SerializeField]
        public float brightness = 0.0f;
        [SerializeField]
        public float contrast = 0.0f;
        [SerializeField]
        public float saturation = 0.5f;
        [SerializeField]
        public float sharpness = -1.0f;

        public override void MaterialUpdate() {
            if (materials == null || materials.Length == 0) {
                materials = new Material[1];
            }

            if (materials[0] != null) {
                materials[0].SetVector("_Sync", sync);
                materials[0].SetFloat("_Brightness", brightness);
                materials[0].SetFloat("_Contrast", (1.016f * (contrast + 1.0f)) / (1.016f * (1.016f - contrast)));
                materials[0].SetFloat("_Saturation", (saturation * 2.0f));
                materials[0].SetFloat("_Sharpness", sharpness);
            } else {
                materials[0] = new Material(Shader.Find("Vortex Game Studios/Filters/OLD TV Filter/Television"));
            }
        }
    }
}
