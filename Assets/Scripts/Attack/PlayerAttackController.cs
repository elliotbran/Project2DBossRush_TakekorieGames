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

        if (Input.GetMouseButton(0))        
        {
            //Debug.Log("ATTACK");
            //StartCoroutine (attacktimeing());
            //Ray rayattack = new Ray(transform.position, Input.mousePosition);
            //Debug.DrawRay(rayattack.origin, rayattack.direction * 2f, Color.red);


            Vector3 mouseWorldPos = maincamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = mouseWorldPos - transform.position;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            attacker.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            attacking = false;
        }
    }

        IEnumerator attacktimeing()
        {
            attacking = true;
            yield return new WaitForSeconds(2);
        }
}
