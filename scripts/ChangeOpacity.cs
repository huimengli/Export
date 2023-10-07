using UnityEngine;

namespace Export.Tools
{
    /// <summary>
    /// 修改透明度
    /// </summary>
    public class ChangeOpacity : MonoBehaviour
    {
        /// <summary>
        /// 透明度
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        public float opacity = 1f;

        private Renderer rend;

        void Awake()
        {
            rend = GetComponent<Renderer>();
            SetOpacity(opacity);
        }

        void Start()
        {
            
        }

        void Update()
        {
            // 你可以在这里加入自己的逻辑，例如根据用户输入来改变 opacity 的值
            // 然后调用 SetOpacity 方法来应用改变
        }

        void SetOpacity(float opacity)
        {
            Color color = rend.material.color;
            color.a = opacity;
            rend.material.color = color;

            if (opacity < 1f)
            {
                rend.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                rend.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                rend.material.SetInt("_ZWrite", 0);
                rend.material.DisableKeyword("_ALPHATEST_ON");
                rend.material.EnableKeyword("_ALPHABLEND_ON");
                rend.material.renderQueue = 3000;
            }
            else
            {
                rend.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                rend.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                rend.material.SetInt("_ZWrite", 1);
                rend.material.EnableKeyword("_ALPHATEST_ON");
                rend.material.DisableKeyword("_ALPHABLEND_ON");
                rend.material.renderQueue = -1;
            }
        }
    } 
}