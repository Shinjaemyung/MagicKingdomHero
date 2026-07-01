using System.Collections;
using System.Collections.Generic;
using TowerDefense.UI;
using UnityEngine;

public class VirtualTower : MonoBehaviour
{
    /// <summary>
    /// 실제로 배치할 타워
    /// </summary>
    public Tower MyTower { get; private set; }

    Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    Material invalidPlacementMaterial;

    public bool isPlacementValid;

    public void Initialize(Tower tower, Material invalidPlacementMaterial)
    {
        MyTower = tower;
        this.invalidPlacementMaterial = invalidPlacementMaterial;

        ApplyHatMaterial();

        SkinnedMeshRenderer[] meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (var renderer in meshRenderers)
        {
            originalMaterials[renderer] = renderer.sharedMaterials;
        }

        RadiusVisualizerController.Instance.SetupRadiusVisualizers(MyTower, transform);

        isPlacementValid = true;
        SetInvalidPlacementState();
    }

    /// <summary>
    /// MyTower의 TowerData.DamageType에 맞게 WizardHat 머티리얼 적용
    /// </summary>
    void ApplyHatMaterial()
    {
        if (MyTower == null || MyTower.towerData == null)
            return;

        WizardHatMaterialApplier.ApplyHatMaterial(transform, MyTower.towerData.damageType);
    }

    public void Move(Vector3 worldPosition, bool placementPossible)
    {
        transform.position = worldPosition;

        if (placementPossible)
        {
            SetValidPlacementState();
        }
        else
        {
            SetInvalidPlacementState();
        }
    }

    public void SetValidPlacementState()
    {
        if (isPlacementValid)
            return;

        isPlacementValid = true;
        SetOriginalMaterial();
    }
    
    public void SetInvalidPlacementState()
    {
        if (!isPlacementValid)
            return;

        isPlacementValid = false;
        SetInvalidPlacementMaterial();
    }

    void SetOriginalMaterial()
    {
        foreach (var entry in originalMaterials)
        {
            var renderer = entry.Key;
            var originMat = entry.Value;

            renderer.sharedMaterials = originMat;
        }
    }

    void SetInvalidPlacementMaterial()
    {
        foreach (var entry in originalMaterials)
        {
            var renderer = entry.Key;
            var materials = renderer.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = invalidPlacementMaterial;
            }

            renderer.sharedMaterials = materials;
        }
    }


}
