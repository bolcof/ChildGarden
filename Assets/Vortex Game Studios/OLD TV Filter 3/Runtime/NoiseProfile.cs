using System;
using UnityEngine;

namespace VortexStudios.PostProcessing {
    [System.Serializable]
    public class NoiseProfile : PostProcessingProfile {
        [SerializeField]
        public Texture pattern = null;
        [SerializeField]
        public float magnetude = 0.25f;
        [SerializeField]
        public Vector2 scale = Vector2.one;

        public override void MaterialUpdate() {
            if (materials == null || materials.Length == 0) {
                materials = new Material[1];
            }

            if (materials[0] != null) {
                materials[0].SetTexture("_PatternTex", pattern);
                materials[0].SetFloat("_Magnitude", magnetude);
                materials[0].SetFloat("_PatternScaleX", scale.x);
                materials[0].SetFloat("_PatternScaleY", scale.y);
            } else {
                materials[0] = new Material(Shader.Find("Vortex Game Studios/Filters/OLD TV Filter/Noise"));
                if (pattern == null)
                    pattern = materials[0].GetTexture("_PatternTex");
            }
        }

        /*public override void OnEnable() {
            if ( pattern == null )
                pattern = material.GetTexture( "_PatternTex" );
            
        }

        public override void OnFixedUpdate() {
            _offset.x = _offset.y;
            _offset.y = UnityEngine.Random.Range( -1.0f, 1.0f );
        }

        public override RenderTexture OnRenderImage( RenderTexture source ) {
            base.OnRenderImage( source );

            if ( material != null && pattern != null && magnetude != 0.0f ) {
                material.SetTexture( "_PatternTex", pattern );
                material.SetFloat( "_PatternOffsetX", _offset.x );
                material.SetFloat( "_PatternOffsetY", _offset.y );
                material.SetFloat( "_PatternScaleX", scale.x );
                material.SetFloat( "_PatternScaleY", scale.y );
                //material.SetTextureOffset( "_PatternTex", _offset );
                //material.SetTextureScale( "_PatternTex", scale );
                //_PatternOffset
                material.SetFloat( "_Magnitude", magnetude );

                Graphics.Blit( SOURCEBUFFER, DESTBUFFER, material );
                SWAPBUFFER();
            }


            return null;
        }*/
    }
}