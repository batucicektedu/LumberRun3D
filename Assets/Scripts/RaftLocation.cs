using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftLocation : MonoBehaviour
{
    public enum LocationType
    {
        Start,
        End
    }

    public LocationType locationType;

    private GameObject raft;
    private ParticleSystem waterEffects;

    private Animator playerAnimator;

    private void Awake()
    {
        InitConnections();
    }
    private void Start()
    {
        //Diğer script raft'ı alamadan disable etmemek için connectionları Awake'te, bunu burda yapıyorum
        raft.SetActive(false);
    }

    private void InitConnections()
    {
        raft = GameObject.FindGameObjectWithTag("Raft");
        waterEffects = GameObject.FindGameObjectWithTag("RaftWaterEffect").GetComponent<ParticleSystem>();
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (locationType)
            {
                case LocationType.Start:

                    GameManager.Instance.DisableWoods(GameManager.Instance.raftWoodCount);

                    other.GetComponentInParent<CharacterController>().cutting = false;

                    raft.SetActive(true);

                    playerAnimator.SetTrigger("SurfStart");

                    StartCoroutine(PlayWithDelay());

                    break;

                case LocationType.End:

                    GameObject spawnedRaft = Instantiate(raft, raft.transform.position, raft.transform.rotation);
                    spawnedRaft.GetComponent<Animator>().enabled = false;

                    playerAnimator.SetTrigger("SurfFinished");

                    raft.SetActive(false);
                    StartCoroutine(StopWithDelay());

                    break;
            }
        }
    }

    IEnumerator PlayWithDelay()
    {
        yield return new WaitForSeconds(1);
        waterEffects.Play();
    }

    IEnumerator StopWithDelay()
    {
        yield return new WaitForSeconds(1);
        waterEffects.Stop();
    }
}
