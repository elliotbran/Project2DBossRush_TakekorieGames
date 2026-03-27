using UnityEngine;

public class CheckShadowIzelDead : MonoBehaviour
{
    public EnemyTutorialController _enemyTutorialController;

    public GameObject bossCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_enemyTutorialController.isDead)
        {
            bossCanvas.SetActive(false);
        }
    }
}
