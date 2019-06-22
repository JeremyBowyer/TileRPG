using UnityEngine;
using System.Collections;


public class AutoTransparent : MonoBehaviour
{
    private Shader m_OldShader = null;
    private Color m_OldColor = Color.black;
    private MeshRenderer m_Renderer;
    private float m_Transparency = 0.3f;
    private const float m_TargetTransparancy = 0.3f;
    private const float m_FallOff = 0.1f; // returns to 100% in 0.1 sec

    public void Awake()
    {
        m_Renderer = GetComponent<MeshRenderer>();
    }

    public void BeTransparent()
    {
        // reset the transparency;
        m_Transparency = m_TargetTransparancy;
        if (m_OldShader == null)
        {
            // Save the current shader
            m_OldShader = m_Renderer.material.shader;
            m_OldColor = m_Renderer.material.color;
            //m_Renderer.material.shader = Shader.Find("Transparent/Diffuse");
        }
    }

    void Update()
    {
        if (m_Transparency < 1.0f)
        {
            m_Renderer.enabled = false;
            Color C = m_Renderer.material.color;
            C.a = m_Transparency;
            m_Renderer.material.color = C;
        }
        else
        {
            m_Renderer.enabled = true;
            // Reset the shader
            m_Renderer.material.shader = m_OldShader;
            m_Renderer.material.color = m_OldColor;
            // And remove this script
            Destroy(this);
        }
        m_Transparency += ((1.0f - m_TargetTransparancy) * Time.deltaTime) / m_FallOff;
    }

}