using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerAttackController : MonoBehaviour
{
    PlayerControllerElliot controller;
    public Camera maincamera;
    public bool attacking;

    public GameObject attacker;

    void Start()
    {
        controller = GetComponent<PlayerControllerElliot>();
    }

    void Update()
    {        
        Ray ray = new Ray(transform.position, controller.moveDir);
        Debug.DrawRay(ray.origin, ray.direction*5f, Color.red);

        if (Input.GetMouseButton(0))//CAMBIAR A DOWN     
        {
            StartCoroutine(Attacktimeing());
        }
    }
    IEnumerator Attacktimeing()
    {
        attacking = true;
        Vector3 mouseWorldPos = maincamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorldPos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        attacker.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        //Debug.Log("ATAQUE INICIADO");
        yield return new WaitForSeconds(2f);//Cuanto dura el ataque
        attacking = false;
        //Debug.Log("ATAQUE FINALIZADO");

    }
}
