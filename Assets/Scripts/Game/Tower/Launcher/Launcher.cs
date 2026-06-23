using ActionGameFramework.Health;
using ActionGameFramework.Projectiles;
using Core.Utilities;
using System.Collections.Generic;
using UnityEngine;


namespace TowerDefense.Towers.TowerLaunchers
{
    public abstract class Launcher : MonoBehaviour, ILauncher
    {
        public abstract void Launch(Targetable enemy, GameObject projectile, Transform firingPoint);

        public virtual void Launch(List<Targetable> enemies, GameObject projectile, Transform[] firingPoints)
        {
            int count = enemies.Count;
            int currentFiringPointIndex = 0;
            int firingPointLength = firingPoints.Length;
            for (int i = 0; i < count; i++)
            {
                Targetable enemy = enemies[i];
                Transform firingPoint = firingPoints[currentFiringPointIndex];
                currentFiringPointIndex = (currentFiringPointIndex + 1) % firingPointLength;

                GameObject poolObject = PoolManager.Instance.GetObject(projectile);
                poolObject.GetComponent<Poolable>().Init(projectile);
                Launch(enemy, poolObject, firingPoint);
            }
        }

        public virtual void Launch(Targetable enemy, GameObject projectile, Transform[] firingPoints)
        {
            GameObject poolObject = PoolManager.Instance.GetObject(projectile);
            poolObject.GetComponent<Poolable>().Init(projectile);
            Launch(enemy, poolObject, GetRandomTransform(firingPoints));
        }

        public virtual void LaunchAtPosition(Vector3 position, GameObject projectile, Transform firingPoint)
        {

        }

        public virtual void LaunchAtPosition(Vector3 position, GameObject projectile, Transform[] firingPoints)
        {
            GameObject poolObject = PoolManager.Instance.GetObject(projectile);
            poolObject.GetComponent<Poolable>().Init(projectile);
            LaunchAtPosition(position, poolObject, GetRandomTransform(firingPoints));
        }

        public void PlayParticles(ParticleSystem particleSystemToPlay, Vector3 origin, Vector3 lookPosition)
        {
            if (particleSystemToPlay == null)
            {
                return;
            }
            particleSystemToPlay.transform.position = origin;
            particleSystemToPlay.transform.LookAt(lookPosition);
            particleSystemToPlay.Play();
        }


        public Transform GetRandomTransform(Transform[] launchPoints)
        {
            int index = Random.Range(0, launchPoints.Length);
            return launchPoints[index];
        }
    }
}