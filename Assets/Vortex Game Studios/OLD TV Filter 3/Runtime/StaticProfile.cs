using System;
using UnityEngine;

namespace VortexStudios.PostProcessing {
    [System.Serializable]
    public class StaticProfile : PostProcessingProfile {
        [SerializeField]
        public Texture staticPattern = null;
        [SerializeField]
        public float staticMagnitude = 0.1f;
        [SerializeField]
        public float staticScale = 1.0f;
        [SerializeField]
        public float staticOffset = 0.0f;
        [SerializeField]
        public Texture dirtPattern = null;
        [SerializeField]
        public float dirtMagnitude = 0.35f;

        public override void MaterialUpdate() {
            if (materials == null || materials.Length == 0) {
                materials = new Material[1];
            }

            if (materials[0] != null) {
                materials[0].SetTexture("_StaticTex", staticPattern);
                materials[0].SetFloat("_StaticScale", staticScale);
                materials[0].SetFloat("_StaticOffset", staticOffset);
                materials[0].SetFloat("_StaticMagnitude", staticMagnitude);

                materials[0].SetTexture("_DirtTex", dirtPattern);
                materials[0].SetFloat("_DirtMagnitude", dirtMagnitude);
            } else {
                materials[0] = new Material(Shader.Find("Vortex Game Studios/Filters/OLD TV Filter/Static"));
                if (staticPattern == null)
                    staticPattern = materials[0].GetTexture("_StaticTex");
                if (dirtPattern == null)
                    dirtPattern = materials[0].GetTexture("_DirtTex");
            }
        }
    }
}