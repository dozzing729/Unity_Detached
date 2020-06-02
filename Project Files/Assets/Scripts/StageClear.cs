using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClear : MonoBehaviour
{
    public PlayerController player;
    public HandController   leftHand;
    public HandController   rightHand;
    public float            detectionRadius;
    public int              stage;
    public bool             isLastStage;
    private bool            isStageClear;

    void Start()
    {
        isStageClear = false;
    }

    void Update()
    {
        isStageClear = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Player"));

        if (isStageClear)
        {
            if (isLastStage) {
                LoadNextStage();
            }
            else {
                RetrieveHands();
            }

            gameObject.SetActive(false);
        }
    }

    private void LoadNextStage() 
    {
        SceneManager.LoadScene(stage + 1);
    }

    private void RetrieveHands() 
    {
        switch (player.getArms())
                {
                    case 1:
                        if (!player.getLeftRetrieving())
                        {
                            player.setLeftRetrieving(true);
                            leftHand.StartRetrieve();
                        }
                        break;
                    case 0:
                        if (!player.getLeftRetrieving())
                        {
                            player.setLeftRetrieving(true);
                            leftHand.StartRetrieve();
                        }
                        if (!player.getRightRetrieving())
                        {
                            player.setRightRetrieving(true);
                            rightHand.StartRetrieve();
                        }
                        break;
                    default:
                        break;
                }
    }

    private void OnDrawGizmos()
    { Gizmos.DrawWireSphere(transform.position, detectionRadius); }
}
