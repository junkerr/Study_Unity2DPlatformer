using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioGetItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    public AudioClip audioResurrection;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public enum EPlayerAction
    {
        None = 0,
        Jump,
        Attack,
        Damaged,
        GetItem,
        Die,
        Resurrection,
        Finish
    }

    public void PlaySound(EPlayerAction action)
    {
        switch (action)
        {
            case EPlayerAction.Jump:
                audioSource.clip = audioJump;
                break;

            case EPlayerAction.Attack:
                audioSource.clip = audioAttack;
                break;

            case EPlayerAction.Damaged:
                audioSource.clip = audioDamaged;
                break;

            case EPlayerAction.GetItem:
                audioSource.clip = audioGetItem;
                break;

            case EPlayerAction.Die:
                audioSource.clip = audioDie;
                break;

            case EPlayerAction.Finish:
                audioSource.clip = audioFinish;
                break;

            case EPlayerAction.Resurrection:
                audioSource.clip = audioResurrection;
                break;

            default:
                break;
        }

        audioSource.Play();
    }
}
