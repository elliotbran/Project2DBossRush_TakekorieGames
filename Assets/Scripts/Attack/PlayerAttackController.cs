using UnityEngine;
using UnityEngine.Scripting;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerAttackController : MonoBehaviour
{
    PlayerController controller;
    public bool attacking;
    public Vector3 attack_range;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, controller.movementInput);
        Debug.DrawRay(ray.origin, ray.direction*5f, Color.red);
    }
    public void Attack()
    {
        attacking = true;
        //DIBUJA GIZMO DE RANGO

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, attack_range);
    }
}
