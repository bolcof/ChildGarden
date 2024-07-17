using System;
using UnityEngine;

namespace VortexStudios.PostProcessing {
    [System.Serializable]
    public class PostProcessingProfile {
        protected RenderBuffer _BUFFER;

        private static bool _CURRENTBUFFER = false;
        private static RenderTexture[] _TEMPBUFFER = new RenderTexture[2];// = null;

        public static void SWAPBUFFER() {
            _CURRENTBUFFER = !_CURRENTBUFFER;
        }

        public static RenderTexture SOURCEBUFFER {
            get {
                return _TEMPBUFFER[ _CURRENTBUFFER ? 0 : 1 ];
            }

            set {
                _TEMPBUFFER[ _CURRENTBUFFER ? 0 : 1 ] = value;
            }
        }

        public static RenderTexture DESTBUFFER {
            get {
                return _TEMPBUFFER[ !_CURRENTBUFFER ? 0 : 1 ];
            }

            set {
                _TEMPBUFFER[ !_CURRENTBUFFER ? 0 : 1 ] = value;
            }
        }

#pragma warning disable 0414
        [ SerializeField]
        private bool _foldout = false;

        [SerializeField]
        public Material[] materials;

        [SerializeField]
        protected bool _enabled = true;
        public bool enabled {
            get { return _enabled; }
            set {
                _enabled = value;

                if ( value )
                    OnValidate();
            }
        }

        public PostProcessingProfile() { _foldout = false; }
        public virtual void MaterialUpdate() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnEnable() { }
        public virtual void OnValidate() { }
        public virtual void OnReset() { }
        public virtual RenderTexture OnRenderImage( RenderTexture source ) {
            return source;
        }
    }
}
