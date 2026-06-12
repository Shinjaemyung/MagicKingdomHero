using System.Collections.Generic;
using TowerDefense.Towers;
using UnityEngine;

namespace TowerDefense.UI
{
    public class RadiusVisualizerController : MonoBehaviour
    {
        public static RadiusVisualizerController Instance { get; private set; }

        /// <summary>
        /// 타워의 범위를 시각화하는 데 사용하는 프리팹
        /// </summary>
        public GameObject radiusVisualizerPrefab;

        public float radiusVisualizerHeight = 0.02f;

        public Vector3 localEuler;

        readonly List<GameObject> _radiusVisualizers = new();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        /// <summary>
        /// 타워 또는 virtual 타워의 범위 시각화를 설정
        /// </summary>
        /// <param name="tower">
        /// 범위 정보를 가져올 타워
        /// </param>
        /// <param name="virtualTower">
        /// 시각화 오브젝트를 부모로 연결할 virtual 타워의 Transform
        /// </param>
        public void SetupRadiusVisualizers(Tower tower, Transform virtualTower = null)
        {
            // 범위 표시 생성
            List<ITowerRadiusProvider> providers = tower.GetRadiusVisualizers();

            int length = providers.Count;
            for (int i = 0; i < length; i++)
            {
                if (_radiusVisualizers.Count < i + 1)
                {
                    _radiusVisualizers.Add(Instantiate(radiusVisualizerPrefab));
                }

                ITowerRadiusProvider provider = providers[i];

                GameObject radiusVisualizer = _radiusVisualizers[i];
                radiusVisualizer.SetActive(true);
                radiusVisualizer.transform.SetParent(virtualTower == null ? tower.transform : virtualTower);
                radiusVisualizer.transform.localPosition = new Vector3(0, radiusVisualizerHeight, 0);
                radiusVisualizer.transform.localScale = Vector3.one * provider.EffectRadius * 2.0f;
                radiusVisualizer.transform.localRotation = new Quaternion { eulerAngles = localEuler };

                var visualizerRenderer = radiusVisualizer.GetComponent<Renderer>();
                if (visualizerRenderer != null)
                {
                    visualizerRenderer.material.color = provider.EffectColor;
                }
            }
        }

        /// <summary>
        /// 범위 시각화 비활성화
        /// </summary>
        public void HideRadiusVisualizers()
        {
            foreach (GameObject radiusVisualizer in _radiusVisualizers)
            {
                radiusVisualizer.transform.parent = transform;
                radiusVisualizer.SetActive(false);
            }
        }
    }
}
