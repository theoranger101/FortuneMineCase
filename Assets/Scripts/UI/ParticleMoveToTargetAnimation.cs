using System.Collections;
using Promises;
using UnityEngine;

public class ParticleMoveToTargetAnimation : MonoBehaviour
{
    public Transform TargetTransform;
    public ParticleSystem ParticleSystem;
    public Camera TargetCamera;

    private bool m_MoveToTarget = false;

    private Promise<bool> m_AnimPromise;

    public void Initialize(Transform target, ParticleSystem ps, Camera cam)
    {
        TargetTransform = target;
        ParticleSystem = ps;
        TargetCamera = cam;
    }

    public Promise<bool> OnPlay(float delay, float animDur)
    {
        m_AnimPromise = Promise<bool>.Create();

        ParticleSystem.Play();

        StartCoroutine(MoveToPositionDelay(delay, animDur));

        return m_AnimPromise;
    }

    private IEnumerator MoveToPositionDelay(float delay, float animDur)
    {
        yield return new WaitForSeconds(delay);

        m_MoveToTarget = true;

        yield return new WaitForSeconds(animDur);

        m_MoveToTarget = false;
        ParticleSystem.Stop();
        ParticleSystem.Clear();

        m_AnimPromise.Complete(true);
    }

    private void Update()
    {
        if (!m_MoveToTarget)
        {
            return;
        }

        Vector3 worldPosition = TargetTransform.position;
        Vector3 viewportPoint = TargetCamera.ScreenToViewportPoint(worldPosition);
        viewportPoint = new Vector3(viewportPoint.x, viewportPoint.y, 20);

        var screenPosition = TargetCamera.ViewportToWorldPoint(viewportPoint);
        screenPosition = new Vector3(screenPosition.x, screenPosition.y, 0f);

        int numParticles = ParticleSystem.main.maxParticles;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
        ParticleSystem.GetParticles(particles);

        for (int i = 0; i < numParticles; i++)
        {
            particles[i].position = Vector3.MoveTowards(particles[i].position, screenPosition, Time.deltaTime * 20);
        }

        ParticleSystem.SetParticles(particles);
    }
}