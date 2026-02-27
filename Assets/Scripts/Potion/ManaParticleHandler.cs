using UnityEngine;

public class ManaParticleHandler : MonoBehaviour
{
    public ParticleSystem ps;
    public Transform playerTarget;
    public ManaController manaController;

    private ParticleSystem.Particle[] particles;
    private bool _shouldChase = false;
    private bool _manaYaSumado = false;
    private float _currentSpeed = 0f;
    private const float INITIAL_SPEED = 8f;
    private const float ACCELERATION = 20f;

    public void SpawnMana(int amount)
    {
        _manaYaSumado = false;
        _currentSpeed = INITIAL_SPEED; 
        ps.Emit(amount);
        _shouldChase = false;
        Invoke("EnableChasing", 0.8f);
    }

    void EnableChasing() { _shouldChase = true; }

    void LateUpdate()
    {
        if (!_shouldChase) return;

        if (particles == null || particles.Length < ps.main.maxParticles)
            particles = new ParticleSystem.Particle[ps.main.maxParticles];
        int numParticlesAlive = ps.GetParticles(particles);
        _currentSpeed += ACCELERATION * Time.deltaTime;
        for (int i = 0; i < numParticlesAlive; i++)
        {
            particles[i].position = Vector3.MoveTowards(
                particles[i].position,
                playerTarget.position,
                _currentSpeed * Time.deltaTime
            );
            if (Vector3.Distance(particles[i].position, playerTarget.position) < 0.3f)
            {
                particles[i].remainingLifetime = -1f; 

                if (!_manaYaSumado && manaController != null)
                {
                    manaController.RefillMana(1f);
                    _manaYaSumado = true;
                    Debug.Log("Maná absorbido con éxito");
                }
            }
        }
        ps.SetParticles(particles, numParticlesAlive);
    }
}