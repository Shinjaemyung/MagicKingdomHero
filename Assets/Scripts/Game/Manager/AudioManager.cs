using Core.Utilities;
using System.Xml.Serialization;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }


    [SerializeField, Tooltip("효과음 재생에 사용할 AudioSource 프리팹")]
    GameObject audioSourcePrefab;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        var audioObj = PoolManager.Instance.GetObject(audioSourcePrefab);
        audioObj.GetComponent<Poolable>().Init(audioSourcePrefab);
        audioObj.GetComponent<PooledAudioSource>().Play(clip, position);
    }
}
