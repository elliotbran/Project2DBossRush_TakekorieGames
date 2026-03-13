using System.Collections;
using UnityEngine;

public class TutorialAA : MonoBehaviour
{
    public GameObject izelStart;
    private Animator animIzelStart;

    public GameObject izelGetsHit;
    private Animator animIzelHit;

    public GameObject hudHits;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animIzelStart = izelStart.GetComponent<Animator>();
        animIzelHit = izelGetsHit.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void startAnimAttack()
    {
        StartCoroutine(startAnimCoroutine());
    }
    public IEnumerator startAnimCoroutine()
    {
        animIzelStart.Play("Disappear");
        yield return new WaitForSeconds(1f);
        izelStart.SetActive(false);
        yield return new WaitForSeconds(1f);
        izelGetsHit.SetActive(true);
        hudHits.SetActive(true);
    }
}
