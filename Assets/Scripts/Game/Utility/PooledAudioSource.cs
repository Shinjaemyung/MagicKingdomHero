using System.Collections;
using UnityEngine;

/// <summary>
/// PoolManager로 재사용되는 AudioSource.
/// 효과음처럼 짧게 재생되고 사라지는 사운드를
/// 매번 새로 생성/파괴하지 않고 풀링하기 위한 컴포넌트.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PooledAudioSource : Poolable
{
    AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    /// <summary>
    /// 지정한 위치에서 clip을 재생하고,
    /// 재생이 끝나면 자동으로 풀에 반환한다.
    /// </summary>
    public void Play(AudioClip clip, Vector3 position)
    {
        if (clip == null)
        {
            ReturnToPool();
            return;
        }

        transform.position = position;
        _audioSource.clip = clip;
        _audioSource.Play();

        StopAllCoroutines();
        StartCoroutine(ReturnAfterPlayback(clip.length));
    }

    IEnumerator ReturnAfterPlayback(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool();
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        _audioSource.clip = null;
    }
}
