using System.Collections;
using UnityEngine;

public class RangeZero : MonoBehaviour
{
    BossController _bossController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        _bossController = GameObject.Find("Boss")?.GetComponent<BossController>(); // Get the BossController component attached to the boss

    }
    void Start()
    {
        StartCoroutine(WaitOneSecondRange()); // Start the coroutine to wait for 1 second
    }

    IEnumerator WaitOneSecondRange()
    {
        _bossController.rangeAttackRange = 0; // Set isRangeZero to false after 1 second
        yield return new WaitForSeconds(1.75f); // Wait for 1 second
        _bossController.rangeAttackRange = 20; // Set isRangeZero to false after 1 second
    }
        
}
