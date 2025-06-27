using UnityEngine;

public class ParticleSizeShrink : MonoBehaviour
{
    ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        var sizeModule = ps.sizeOverLifetime;
        sizeModule.enabled = true;

        AnimationCurve curve = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(1f, 0f)
        );

        sizeModule.size = new ParticleSystem.MinMaxCurve(1f, curve);

        ps.Stop();
        ps.Clear();
        ps.Play();
    }
}
