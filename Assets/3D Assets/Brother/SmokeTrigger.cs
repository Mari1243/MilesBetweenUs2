using UnityEngine;

public class SmokeTrigger : MonoBehaviour
{


    public ParticleSystem particles;

    public void PlayParticles()
    {
        if (particles != null)
            particles.Play();
    }
}