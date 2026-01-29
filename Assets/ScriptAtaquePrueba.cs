using System.Collections;
using UnityEngine;

public class ScriptAtaquePrueba : MonoBehaviour
{
    public GameObject colliderPrueba;
    public bool atacando = false;
    PlayerControllerElliot player;
    public bool miBombo = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlayerControllerElliot>();
        //atacando = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(miBombo == true)
            StartCoroutine(TimerCollider());
    }

    public IEnumerator TimerCollider()
    {
        miBombo = false;
        atacando = !atacando;
        Debug.Log(atacando);
        colliderPrueba.SetActive(atacando);
        yield return new WaitForSeconds(2f);
        atacando = !atacando;
        Debug.Log(atacando);
        yield return new WaitForSeconds(2f);
        colliderPrueba.SetActive(atacando);

        miBombo = true;
    }

    private void OnDrawGizmos()
    {
        if(atacando == false)
        {
            CircleCollider2D sphere = colliderPrueba.GetComponent<CircleCollider2D>();
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(colliderPrueba.transform.position, Vector3.back, sphere.radius);
        }
        if(atacando == true)
        {
            CircleCollider2D sphere = colliderPrueba.GetComponent<CircleCollider2D>();
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(colliderPrueba.transform.position, Vector3.back, sphere.radius);
        }
    }

    public void MiBomboBombez()
    {
        player.TomarDaño(1f);
        Debug.Log("Me cago en satanas "+player.Life);
    }
}

