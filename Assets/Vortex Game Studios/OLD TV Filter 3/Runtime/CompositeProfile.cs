using System;
using UnityEngine;

namespace VortexStudios.PostProcessing {
    [System.Serializable]
    public class CompositeProfile : PostProcessingProfile {
        [SerializeField]
        public int lineCount = 224;
        [SerializeField]
        public float distortion = 0.5f;
        [SerializeField]
        public float artifact = 0.2f;
        [SerializeField]
        public float fringing = 0.7f;
        [SerializeField]
        public float bleeding = 1.0f;

        public override void MaterialUpdate() {
            if (materials == null || materials.Length < 2) {
                materials = new Material[2];
            }

            if (materials[0] != null) {
                materials[0].SetFloat("_Distortion", distortion);
                materials[0].SetFloat("_Fringing", fringing);
                materials[0].SetFloat("_Artifact", artifact);
                materials[0].SetInt("_LineCount", lineCount);
            } else {
                materials[0] = new Material(Shader.Find("Vortex Game Studios/Filters/OLD TV Filter/Composite"));
            }

            if (materials[1] != null) {
                materials[1].SetFloat("_Magnitude", bleeding);
                materials[1].SetInt("_LineCount", lineCount);
            } else {
                materials[1] = new Material(Shader.Find("Vortex Game Studios/Filters/OLD TV Filter/Bleeding"));
            }
        }
    }
}