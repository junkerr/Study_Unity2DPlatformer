using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets.Script;

public class GameManager : MonoBehaviour
{
    public PlayerMove player;

    public Image[] UIhealth;
    public TextMeshProUGUI UIPoint;
    public TextMeshProUGUI UIStage;
    public GameObject UIButtonRetry;

    private AudioManager audioManager;

    private Canvas canvas;

    private Color heartColor;

    private void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        GlobalVariables.stageIndex = 0;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(canvas);
        heartColor = UIhealth[0].color;
    }

    void Update()
    {
        UIPoint.text = (GlobalVariables.totalPoint + GlobalVariables.stagePoint).ToString();

    }

    public void NextStage()
    {
        if (GlobalVariables.stageIndex < 2)
        {
            // Health 원복
            AllHealthOn();

            GlobalVariables.stageIndex++;

            SceneManager.LoadScene(GlobalVariables.stageIndex);

            UIStage.text = $"Stage {GlobalVariables.stageIndex + 1}";
        }
        else
        {
            // Game Clear

            // Player Control Lock
            Time.timeScale = 0;

            // Result UI
            UIButtonRetry.SetActive(true);
            TextMeshProUGUI btnText = UIButtonRetry.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = "Clear!";
        }

        // Caculate Point
        GlobalVariables.totalPoint += GlobalVariables.stagePoint;
        GlobalVariables.stagePoint = 0;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Player health Down
            HealthDown();

            // Player Reposition
            if (GlobalVariables.health > 0)
            {
                collision.attachedRigidbody.velocity = Vector2.zero; // 낙하속도 0으로
                collision.transform.position = new Vector3(0, 0, -1);
                audioManager.PlaySound(AudioManager.EPlayerAction.Damaged);
            }
            else if (GlobalVariables.health <= 0)
            {
                // Reposition 안함
                collision.attachedRigidbody.velocity = Vector2.zero; // 낙하속도 0으로
            }
        }
    }

    public void HealthDown()
    {
        if (GlobalVariables.health > 0)
        {
            GlobalVariables.health--;
            UIhealth[GlobalVariables.health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            // All Health UI Off
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);
        }

        if (GlobalVariables.health <= 0)
        {
            // Player Die Effect
            player.OnDie();

            // Retry Button UI
            UIButtonRetry.SetActive(true);
        }

    }

    public void Restart()
    {

        AllHealthOn();

        // UI Retry Button false
        UIButtonRetry.SetActive(false);

        Time.timeScale = 1;
        //SceneManager.LoadScene(GlobalVariables.stageIndex);
        player.Resurrection();
    }

    private void AllHealthOn()
    {
        GlobalVariables.health = 3;
        foreach (Image heart in UIhealth)
        {
            Debug.Log(heartColor);
            heart.color = heartColor;
        }
    }

}
