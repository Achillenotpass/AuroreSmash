using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

public class CustomRenderPassFeature : ScriptableRendererFeature
{
    class CustomRenderPass : CustomPass
    {
        public RenderTargetIdentifier source;
        private Material material;
        private RenderTargetHandle tempRenderHandler;

        public CustomRenderPass(Material material)
        {
            this.material = material;
            tempRenderHandler.Init("_TemporaryColorTexture");
        }
        protected override void Setup(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // Setup code here
        }

        protected override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer commandBuffer = CommandBuffer.Get();

            commandBuffer.GetTemporaryRT(tempRenderHandler.id, renderingData.cameraData.cameraTargetDescriptor);
            Blit(commandBuffer, source, tempRenderHandler.Identifier(), material);
            Blit(commandBuffer, tempRenderHandler.Identifier(), source);

            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }

        protected override void Cleanup(CommandBuffer cmd)
        {
            // Cleanup code
        }
    }

    [System.Serializable]

    public class Settings
    {
        public Material material = null;
    }

    public Settings settings = new Settings();

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(settings.material);

        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.source = renderer.cameraColorTarget;
        renderer.EnqueuePass(m_ScriptablePass);
    }

}