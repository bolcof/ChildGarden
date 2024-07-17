using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VortexStudios.PostProcessing;

[ExecuteInEditMode]
public class OLDTVFilter3 : MonoBehaviour {
    [SerializeField]
    private OLDTVPreset _preset = null;
    public OLDTVPreset preset {
        get { return _preset; }
        set { _preset = value; }
    }

    [SerializeField]
    private Camera _camera = null;

    public bool customAspectRatio = false;
    [SerializeField]
    private Vector2 _aspectRatio = new Vector2( 4.0f, 3.0f );
    public Vector2 aspectRatio {
        set {
            _aspectRatio = value;
        }
        get {
            return _aspectRatio;
        }
    }

    public bool timeScale = false;

    void Start() {
        OnValidate();
    }  

    private void OnValidate() {
        aspectRatio = _aspectRatio;
    }

    private void OnPreRender() {
        if ( _camera != null ) {
            if ( customAspectRatio == true ) {
                float screenRatio = (float)Screen.width / (float)Screen.height;                
                float gameRatio = _aspectRatio.x / _aspectRatio.y;

                if ( screenRatio / gameRatio >= 1.0f ) {          //  horizontal
                    float width = Screen.height / _aspectRatio.y * _aspectRatio.x;   
                    float x = ( Screen.width - width ) / 2.0f;
                    _camera.pixelRect = new Rect( x, 0.0f, width, Screen.height );
                } else {
                    float height = Screen.width / _aspectRatio.x * _aspectRatio.y;
                    float y = ( Screen.height - height ) / 2.0f;
                    _camera.pixelRect = new Rect( 0.0f, y, Screen.width, height );
                }
            } else
                _camera.rect = new Rect( 0, 0, 1, 1 );
        }
    }

    void OnRenderImage( RenderTexture source, RenderTexture destination ) {
        if ( _preset == null ) {
            Graphics.Blit( source, destination );
            return;
        }

        source.wrapMode = TextureWrapMode.Repeat;

        if ( PostProcessingProfile.SOURCEBUFFER == null || !PostProcessingProfile.SOURCEBUFFER.IsCreated() ||
            PostProcessingProfile.SOURCEBUFFER.width != source.width || PostProcessingProfile.SOURCEBUFFER.height != source.height ||
            PostProcessingProfile.SOURCEBUFFER.depth != source.depth ) {
            if ( PostProcessingProfile.SOURCEBUFFER != null && PostProcessingProfile.SOURCEBUFFER.IsCreated() )
                PostProcessingProfile.SOURCEBUFFER.DiscardContents();

            PostProcessingProfile.SOURCEBUFFER = new RenderTexture( source.width, source.height, source.depth );
            PostProcessingProfile.SOURCEBUFFER.antiAliasing = 1;
            PostProcessingProfile.SOURCEBUFFER.anisoLevel = 0;
            PostProcessingProfile.SOURCEBUFFER.autoGenerateMips = false;
            PostProcessingProfile.SOURCEBUFFER.filterMode = FilterMode.Bilinear;
        }

        if ( PostProcessingProfile.DESTBUFFER == null || !PostProcessingProfile.DESTBUFFER.IsCreated() ||
            PostProcessingProfile.DESTBUFFER.width != source.width || PostProcessingProfile.DESTBUFFER.height != source.height ||
            PostProcessingProfile.DESTBUFFER.depth != source.depth ) {
            if ( PostProcessingProfile.DESTBUFFER != null && PostProcessingProfile.DESTBUFFER.IsCreated() )
                PostProcessingProfile.DESTBUFFER.DiscardContents();

            PostProcessingProfile.DESTBUFFER = new RenderTexture( source.width, source.height, source.depth );
            PostProcessingProfile.DESTBUFFER.antiAliasing = 1;
            PostProcessingProfile.DESTBUFFER.anisoLevel = 0;
            PostProcessingProfile.DESTBUFFER.autoGenerateMips = false;
            PostProcessingProfile.DESTBUFFER.filterMode = FilterMode.Bilinear;
        }

        Graphics.Blit( source, PostProcessingProfile.SOURCEBUFFER );

        ExecuteFilter(_preset.noiseFilter);                     // noise filter
        ExecuteFilter(_preset.staticFilter);                    // static filter
        ExecuteFilter(_preset.chromaticAberrationFilter);       // scanline filter
        ExecuteFilter(_preset.compositeFilter);                 // composite video filter
        ExecuteFilter(_preset.scanlineFilter);                  // scanline filter
        ExecuteFilter(_preset.televisionFilter);                // television filter
        ExecuteFilter(_preset.tubeFilter);                      // tube filter

        //Graphics.CopyTexture( source, destination );
        Graphics.Blit( PostProcessingProfile.SOURCEBUFFER, destination );
    }


    private void ExecuteFilter(PostProcessingProfile p) {
        if (p.enabled) {
            p.MaterialUpdate();     // update the material settings

            foreach (Material m in p.materials) {
                if (m != null) {
                    Graphics.Blit(PostProcessingProfile.SOURCEBUFFER, PostProcessingProfile.DESTBUFFER, m);
                    PostProcessingProfile.SWAPBUFFER();
                }
            }
        }
    }
}