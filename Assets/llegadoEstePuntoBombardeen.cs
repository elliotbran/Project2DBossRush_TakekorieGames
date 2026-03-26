using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class llegadoEstePuntoBombardeen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Bombardeo());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Bombardeo()
    {
        yield return new WaitForSeconds(20f);
        SceneManager.LoadScene(0);
    }
}
