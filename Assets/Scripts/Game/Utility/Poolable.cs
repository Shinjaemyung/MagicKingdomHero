using Core.Utilities;
using UnityEngine;

public class Poolable : MonoBehaviour
{
    /// <summary>
    /// 스폰될 때마다 증가하는 ID. pool 반환 후 재스폰된 오브젝트를 구별하는 데 사용.
    /// </summary>
    public int SpawnId { get; private set; } = 0;

    protected GameObject prefabReference;

    bool isPoolReturned = false;

    public void Init(GameObject prefab)
    {
        prefabReference = prefab;
    }

    public virtual void OnSpawn()
    {
        SpawnId++;
        isPoolReturned = false;
        gameObject.SetActive(true);
    }

    public virtual void OnDespawn()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    public void ReturnToPool()
    {
        if (isPoolReturned) return;

        isPoolReturned = true;
        PoolManager.Instance.ReturnObject(prefabReference, gameObject);
    }
}
