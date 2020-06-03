using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClear : MonoBehaviour
{
    public float detectionRadius;
    public int stage;
    private bool isStageClear;

    void Update()
    {
        isStageClear = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Player"));
        if (isStageClear)
        {
            LoadNextStage();
        }
    }

    private void LoadNextStage() {
        SceneManager.LoadScene(stage + 1);
    }

    private void OnDrawGizmos()
    { Gizmos.DrawWireSphere(transform.position, detectionRadius); }
}
