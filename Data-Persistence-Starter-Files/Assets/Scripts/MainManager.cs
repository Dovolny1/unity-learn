using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public bool LevelLoaded;

    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;
    public string playerNameStr;

    [Header("UI")]
    public GameObject Canvas;
    public Text ScoreText;
    public GameObject GameOverText;
    public Text BestScoreText;
    public Text PlayerName;
    
    private bool m_Started = false;
    public int m_Points;
    
    public bool m_GameOver = false;

    
    // Start is called before the first frame update
    void Start()
    {
        //SaveData data = new SaveData();
        //data.score = 0;
        //data.name = "";

        //string json = JsonUtility.ToJson(data);

        //File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);

        //Debug.Log("Score reset");

    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "main")
        {
            if (Canvas == null)
            {
                Canvas = GameObject.FindWithTag("Canvas");
            }
            else
            {
                if (ScoreText == null)
                {
                    ScoreText = Canvas.transform.GetChild(0).GetComponent<Text>();
                }

                if (GameOverText == null)
                {
                    GameOverText = Canvas.transform.GetChild(1).gameObject;
                }

                if (BestScoreText == null)
                {
                    BestScoreText = Canvas.transform.GetChild(2).GetComponent<Text>();
                    Load();
                }

                
            }

            if (!LevelLoaded)
            {
                

                const float step = 0.6f;
                int perLine = Mathf.FloorToInt(4.0f / step);

                int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
                for (int i = 0; i < LineCount; ++i)
                {
                    for (int x = 0; x < perLine; ++x)
                    {
                        Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                        var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                        brick.PointValue = pointCountArray[i];
                        brick.onDestroyed.AddListener(AddPoint);
                    }
                }

                LevelLoaded = true;
            }



            if (Ball != null)
            {
                if (!m_Started)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        m_Started = true;
                        float randomDirection = Random.Range(-1.0f, 1.0f);
                        Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                        forceDir.Normalize();

                        Ball.transform.SetParent(null);
                        Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
                    }
                }
                
            }
            else
            {
                if (!m_Started)
                {
                    Ball = GameObject.FindWithTag("Ball").GetComponent<Rigidbody>();

                    
                }

                
            }

            if (m_GameOver)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SceneManager.LoadScene(1);
                    m_GameOver = false;
                    LevelLoaded = false;
                    m_Started = false;
                    m_Points = 0;
                }
            }
            
        }


        if (SceneManager.GetActiveScene().name == "menu")
        {
            if (PlayerName == null)
            {
                PlayerName = GameObject.FindWithTag("Canvas").transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();
            }
        }
        
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        
        GameOverText.SetActive(true);
        m_GameOver = true;

        Save();
    }



    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        
    }

    public void StartGame()
    {
        playerNameStr = PlayerName.text;

        SceneManager.LoadScene(1);
        
    }

    [System.Serializable]
    class SaveData
    {
        public int score;
        public string name;
    }

    public void Save()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string oldjson = File.ReadAllText(path);
            SaveData olddata = JsonUtility.FromJson<SaveData>(oldjson);

            if (olddata.score < m_Points)
            {
                SaveData data = new SaveData();
                data.score = m_Points;
                data.name = playerNameStr;

                string json = JsonUtility.ToJson(data);

                File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);

                Debug.Log("New high score saved!");
            }

        }
        else
        {
            //First time score save

            SaveData data = new SaveData();
            data.score = m_Points;
            data.name = playerNameStr;

            string json = JsonUtility.ToJson(data);

            File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);

            Debug.Log("Score saved first time!");
        }


        
    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);


            BestScoreText.text = "Best Score: " + data.score + " by " + data.name; 

            Debug.Log("Score Loaded");
        }
    }
}
