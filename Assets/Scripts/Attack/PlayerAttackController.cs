using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerAttackController : MonoBehaviour
{
    public enum AttackState { Idle, Attacking, Cooldown }
    PlayerControllerElliot controller;
    public Camera maincamera;
    public bool attacking;
    private Animator playerAnimator;

    public GameObject attacker;
    public AttackState currentState = AttackState.Idle;

    void Start()
    {
        controller = GetComponent<PlayerControllerElliot>();
        playerAnimator = GetComponent<Animator>();
        attacker.SetActive(false);
    }

    void Update()
    {
        UpdateStates();
        Ray ray = new Ray(transform.position, controller.moveDir);
        Debug.DrawRay(ray.origin, ray.direction*5f, Color.red);

        /////////------------NO TOCAR------------/////////
        Vector3 mouseWorldPos = maincamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorldPos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        attacker.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        /////////------------NO TOCAR------------/////////
        
        if (Input.GetMouseButtonDown(0)) 
        {
            Debug.Log("ATAQUE INICIADO");
            StartCoroutine(Attacktimeing());
            //Attack();   
            Debug.Log("ATAQUE FINALIZADO");
        }
    }

    void UpdateStates()
    {
        switch(currentState)
        {
            case AttackState.Idle:
                UpdateIdle();
                break;
        }
    }

    void UpdateIdle()
    {
        //Setear el animator
        //Recuperar stamina
    }

    void UpdateAttack()
    {

    }

    public void SetNewState(AttackState newState) 
    { 
        if( newState == AttackState.Attacking)
        {

        }
        currentState = newState; 
    }

    IEnumerator Attacktimeing()
    { 
        attacker.SetActive(true);//Esto activa HitRange que es la zona de daño, se pondra en el animator
        attacking = true;
        Attack();
        yield return new WaitForSeconds(1f);//Cuanto dura el ataque
        attacker.SetActive(false);
        attacking = false;

    }

    void Attack()
    {
        playerAnimator.SetTrigger("Attack");
          
    }
}
