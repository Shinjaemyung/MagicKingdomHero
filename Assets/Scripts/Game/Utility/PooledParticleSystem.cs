using System.Collections;
using UnityEngine;

/// <summary>
/// PoolManager로 재사용되는 ParticleSystem.
/// 타격 이펙트처럼 짧게 재생되고 사라지는 파티클을
/// 매번 새로 생성/파괴하지 않고 풀링하기 위한 컴포넌트.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class PooledParticleSystem : Poolable
{
    ParticleSystem _particleSystem;

    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        var main = _particleSystem.main;
        main.playOnAwake = false;
    }

    /// <summary>
    /// 지정한 위치/회전에서 파티클 재생
    /// 재생이 끝나면 자동으로 풀에 반환
    /// </summary>
    public void Play(Vector3 position)
    {
        Play(position, transform.rotation);
    }

    /// <summary>
    /// 지정한 위치/회전으로 파티클 재생
    /// 재생이 끝나면 자동으로 풀에 반환
    /// </summary>
    public void Play(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        //transform.LookAt(position + rotation);
        _particleSystem.Clear();
        _particleSystem.Play();

        StopAllCoroutines();
        StartCoroutine(ReturnAfterPlayback(GetPlaybackDuration()));
    }

    /// <summary>
    /// 재생 시간(파티클이 방출되는 시간 + 방출된 파티클이 사라지는 데 걸리는 최대 시간)을 계산
    /// </summary>
    float GetPlaybackDuration()
    {
        var main = _particleSystem.main;
        var startLifetime = main.startLifetime;

        float maxStartLifetime = startLifetime.mode switch
        {
            ParticleSystemCurveMode.Constant => startLifetime.constant,
            ParticleSystemCurveMode.TwoConstants => startLifetime.constantMax,
            _ => startLifetime.constantMax,
        };

        return main.duration + maxStartLifetime;
    }

    IEnumerator ReturnAfterPlayback(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool();
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
