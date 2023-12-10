using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Lost_Gorga.Old
{
    public class Game : MonoBehaviour
    {
        public static Game Data { get; private set; }

        public bool Coin { get; private set; }
        public bool Gem { get; private set; }
        public bool Heart { get; private set; }

        public bool isCredits = false;

        private AudioSource gameAudio;

        [SerializeField]
        private AudioSource musicSource;

        [SerializeField]
        GameObject itemBG, coinUI, gemUI, heartUI;

        [SerializeField]
        AudioClip achievement, creditCollect, lost, win, theme;

        private void Awake()
        {
            if (Data != null && Data != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Data = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start()
        {
            gameAudio = GetComponent<AudioSource>();

            UpdateUI();
        }

        private void Update()
        {
            if (!isCredits && SceneManager.GetActiveScene().buildIndex == 14)
            { 
                isCredits = true; 
                UpdateUI();
            }
        
            AchievementCheck(false);
            MusicCheck();
        }

        public void Collect(string collectible)
        {
            switch (collectible)
            {
                case "coin":
                    Coin = true; break;
                case "gem":
                    Gem = true; break;
                case "heart":
                    Heart = true; break;
                default: Debug.Log($"{collectible} is not recognized"); break;
            }

            UpdateUI();
            AchievementCheck(true);
        }

        private void UpdateUI()
        {
            itemBG.SetActive(false);
            if (isCredits) itemBG.SetActive(false);
            else if ((Coin || Gem || Heart) && !isCredits)
            {
                itemBG.SetActive(true);
                coinUI.SetActive(Coin);
                gemUI.SetActive(Gem);
                heartUI.SetActive(Heart);
            }
        }

        void PlaySound(AudioClip sound)
        {
            gameAudio.clip = sound;
            gameAudio.Play();
        }

        void AchievementCheck(bool isFirstFrame)
        {
            if (isFirstFrame)
            {
                if (!isCredits)
                {
                    PlaySound(achievement);
                    musicSource.volume = 0.4f;
                    Time.timeScale = 0.3f;
                }
                else
                {
                    PlaySound(creditCollect);
                    musicSource.volume = 1f;
                    Time.timeScale = 1f;
                }
            }
            else if (Time.timeScale != 1 && gameAudio.clip == achievement && !gameAudio.isPlaying)
            {
                musicSource.volume = 1;
                Time.timeScale = 1;
            }
        }

        void MusicCheck()
        {
            if ((SceneManager.GetActiveScene().buildIndex == 4 || SceneManager.GetActiveScene().buildIndex == 6) && musicSource.clip != lost)
            {
                musicSource.clip = lost;
                musicSource.Play();
            }
            if (SceneManager.GetActiveScene().buildIndex == 11)
            {
                musicSource.Stop();
            }
            if (SceneManager.GetActiveScene().buildIndex == 13 && musicSource.clip != theme)
            {
                musicSource.clip = theme;
                musicSource.Play();
            }
            if (SceneManager.GetActiveScene().buildIndex == 14 && musicSource.clip != win)
            {
                musicSource.clip = win;
                musicSource.loop = false;
                musicSource.Play();
            }
        }
    }
}
