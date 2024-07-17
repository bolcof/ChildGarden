using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VortexStudios.PostProcessing;

public class OLDTVFilterRenderPassFeature : ScriptableRendererFeature {
    class OLDTVFilterRenderPass : ScriptableRenderPass {
        public RenderTargetIdentifier source;
        private OLDTVPreset _preset;
        private RenderTargetHandle _renderTargetHandler;
        private CommandBuffer _commandBuffer;
        public OLDTVFilterRenderPass(OLDTVPreset preset) {
            this._preset = preset;
            _renderTargetHandler.Init("_TemporaryColorTexture");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
            // aqui configura o filtro
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
            // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
            // aqui aplica o filtro

            if (_preset == null) {
                return;
            }

            _commandBuffer = CommandBufferPool.Get();
            _commandBuffer.GetTemporaryRT(_renderTargetHandler.id, renderingData.cameraData.cameraTargetDescriptor);

            //Blit(_commandBuffer, source, _renderTargetHandler.Identifier());

            ExecuteFilter(_preset.noiseFilter);                     // noise filter
            ExecuteFilter(_preset.staticFilter);                    // static filter
            ExecuteFilter(_preset.chromaticAberrationFilter);       // scanline filter
            ExecuteFilter(_preset.compositeFilter);                 // composite video filter
            ExecuteFilter(_preset.scanlineFilter);                  // scanline filter
            ExecuteFilter(_preset.televisionFilter);                // television filter
            ExecuteFilter(_preset.tubeFilter);                      // tube filter

            //Blit(_commandBuffer, _renderTargetHandler.Identifier(), source);

            context.ExecuteCommandBuffer(_commandBuffer);
            CommandBufferPool.Release(_commandBuffer);
        }

        private void ExecuteFilter(PostProcessingProfile p) {
            if (p.enabled) {
                p.MaterialUpdate();     // update the material settings

                foreach (Material m in p.materials) {
                    if (m != null) {
                        //Blit(_commandBuffer, source, source, m);
                        //Blit(_commandBuffer, _renderTargetHandler.Identifier(), _renderTargetHandler.Identifier(), m);
                        Blit(_commandBuffer, source, _renderTargetHandler.Identifier(), m);
                        Blit(_commandBuffer, _renderTargetHandler.Identifier(), source);
                    }
                }
            }
        }

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd) {
            // aqui limpa as coisas que foram alocadas na memória do filtro
        }
    }

    [SerializeField]
    private OLDTVPreset _preset = null;
    public OLDTVPreset preset {
        get { return _preset; }
        set { _preset = value; }
    }

    OLDTVFilterRenderPass m_ScriptablePass;

    public override void Create() {
        m_ScriptablePass = new OLDTVFilterRenderPass(_preset);
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        if (Application.isEditor && (renderingData.cameraData.camera.name == "SceneCamera" || renderingData.cameraData.camera.name == "Preview Scene Camera"))
            return;

        m_ScriptablePass.source = renderer.cameraColorTarget;
        renderer.EnqueuePass(m_ScriptablePass);
    }
}