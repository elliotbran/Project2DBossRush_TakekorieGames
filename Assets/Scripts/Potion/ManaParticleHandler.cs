using UnityEngine;

public class ManaParticleHandler : MonoBehaviour
{
    public ParticleSystem ps;
    public Transform player;
    public ManaController manaController;
    private ParticleSystem.Particle[] particles;
    private bool _shouldChase = false;
    private bool _hasaddedmana = false;
    private float _currentSpeed = 0f;
    private const float speed_initial = 5f;
    private const float acceleration = 20f;

    public void SpawnMana(int amount) //Se activa esta funcion cuando hacemos un parry con exito 
    {
        _hasaddedmana = false; //Se da persmiso para que se haga el parry y no se sume mana al isntante  
        _currentSpeed = speed_initial; //la velocidad de las partiuclas se resetea y empiezan lentos
        ps.Emit(amount); //salen las particulas azules
        _shouldChase = false; //no sigue al jugador 
        Invoke("EnableChasing", 0.2f); //esperamos a que caigan y depues que vuelen al player
    }

    void EnableChasing() { _shouldChase = true; } //activa el movimiento de las particulas

    void LateUpdate()
    {
        if (!_shouldChase) return;

        if (particles == null || particles.Length < ps.main.maxParticles) //almacenamos la informacion de las particulas
            particles = new ParticleSystem.Particle[ps.main.maxParticles];

        int numParticlesAlive = ps.GetParticles(particles); //contamos cuantas particulas hay y se guarda la informacion 

        _currentSpeed += acceleration * Time.deltaTime; //las particas aceleran cada vez mas rapidos

        for (int i = 0; i < numParticlesAlive; i++) //Se revisa cuantas partiuclas una por una 
        {
            particles[i].position = Vector3.MoveTowards( //las particulas van hacia el player 
                particles[i].position,
                player.position,
                _currentSpeed * Time.deltaTime
            );
            if (Vector3.Distance(particles[i].position, player.position) < 0.3f) //cuando la particula toca al jugado desaparece la particula y llena el mana
            {
                particles[i].remainingLifetime = -1f; //desaparece las particulas

                if (!_hasaddedmana && manaController != null) //se llena el mana cuando toca la 1 particula 
                {
                    manaController.RefillMana(1f); //sumamos 1 de mana 
                    _hasaddedmana = true; //las otras particulas no sumen 
                    Debug.Log("Maná absorbido");
                }
            }
        }
        ps.SetParticles(particles, numParticlesAlive); //se actualiza la posicion  real de las particulas 
    }
}