using Core.Health;
using Core.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefense.Affectors;
using TowerDefense.Towers;
using TowerDefense.Towers.Placement;
using TowerDefense.UI;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    public VirtualTower virtualTower;

    public IntVector2 dimensions;

    public TowerData towerData;
    int totalCost = 0;
    public int TotalCost => totalCost;

    public float refundRatio = 0.75f;

    IntVector2 gridPosition;
    IPlacementArea placementArea;

    public LayerMask enemyLayerMask;

    public LayerMask Mask { get; protected set; }

    Affector[] _affectors;

    protected Affector[] Affectors
    {
        get
        {
            if (_affectors == null || _affectors.Length == 0)
            {
                _affectors = GetComponentsInChildren<Affector>(true);
            }
            return _affectors;
        }
    }

    private void Awake()
    {
        Initialize(enemyLayerMask);
        ApplyHatMaterial();
    }

    /// <summary>
    /// TowerData의 DamageType에 맞게 WizardHat 머티리얼(색상)을 적용
    /// </summary>
    private void ApplyHatMaterial()
    {
        if (towerData == null)
            return;

        WizardHatMaterialApplier.ApplyHatMaterial(transform, towerData.damageType);
    }

    public virtual void Initialize(IPlacementArea placementArea, IntVector2 destination, int cost)
    {
        if (placementArea == null || destination == null)
            return;

        this.placementArea = placementArea;
        gridPosition = destination;

        transform.position = placementArea.GridToWorld(destination, dimensions);
        //transform.rotation = placementArea.transform.rotation;
        placementArea.Occupy(destination, dimensions);

        totalCost = cost;
    }
    
    public virtual void Initialize(LayerMask enemyMask)
    {
        Mask = enemyMask;

        foreach (Affector effect in Affectors)
        {
            effect.Initialize(Mask);
        }
    }
    
    public void OnClicked()
    {
        GameUIManager.Instance.ShowTowerInfo(this);
    }

    /// <summary>
    /// 업그레이드 후 생성된 타워 인스턴스를 반환
    /// </summary>
    public Tower UpgradeTower(Tower upgradeTower)
    {
        if (!towerData.upgradeTowers.Contains(upgradeTower))
            return null;

        Tower spawnedTower = Instantiate(upgradeTower);
        int upgradeCost = upgradeTower.towerData.cost;
        spawnedTower.Initialize(placementArea, gridPosition, totalCost + upgradeCost);
        GamePlayManager.Instance.UpdatePlayerGold(-upgradeCost);
        Remove();

        return spawnedTower;
    }

    /// <summary>
    /// ITowerRadiusProvider를 구현한 모든 Affector들을 반환
    /// </summary>
    /// <returns>타워에 적용된 모든 ITowerRadiusProvider 리스트</returns>
    public List<ITowerRadiusProvider> GetRadiusVisualizers()
    {
        List<ITowerRadiusProvider> visualizers = new List<ITowerRadiusProvider>();
        foreach (Affector affector in Affectors)
        {
            var visualizer = affector as ITowerRadiusProvider;
            if (visualizer != null)
            {
                visualizers.Add(visualizer);
            }
        }
        return visualizers;
    }

    public void Sell()
    {
        int sellCost = Mathf.RoundToInt(totalCost * refundRatio);
        GamePlayManager.Instance.UpdatePlayerGold(sellCost);
        placementArea.Clear(gridPosition, dimensions);

        Remove();
    }

    public void Remove()
    {
        RadiusVisualizerController.Instance.HideRadiusVisualizers();
        Destroy(gameObject);
    }
}
