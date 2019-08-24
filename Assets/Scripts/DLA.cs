using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DLA : MonoBehaviour
{
    public static DLA instance;

    [SerializeField]
    private float TimeElapsed = 0;
    private bool finished = false;
    [SerializeField]
    private int biggestChain = 0;
    [SerializeField]
    private float Progress = 0;
    [SerializeField]
    private int highSec = 0;
    [SerializeField]
    private int lowSec = 0;
    [SerializeField]
    private int nullSec = 0;

    [Space]
    public bool hideProcess = false;

    [Space]
    [Header("Settings")]
    public int SpawnSphereSize = 20;
    public float biasTowardCenter = 0.1f;
    public float biasHorizontal = 0.0001f;
    public int WalkSpeed = 20;
    public float MaxWalkers = 100;
    [Space]
    public GameObject walkerPrefab;
    public Material clusterMat;
    [Space]
    public List<Walker> walkers;
    public List<Walker> cluster;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
    }

    private void Start()
    {
#if UNITY_EDITOR
        UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
#endif
        Init();
    }

    private void Init()
    {
        walkers = new List<Walker>();
        cluster = new List<Walker>();

        // ROOT
        Walker root = Instantiate(walkerPrefab).GetComponent<Walker>();
        walkers.Add(root);
        root.Init(true);
        root.gameObject.name = "ROOT";

        for (int i = 0; i < MaxWalkers - 1; i++)
        {
            Walker g = Instantiate(walkerPrefab).GetComponent<Walker>();
            walkers.Add(g);
            g.Init();
        }
    }

    public void FinishWalk(Walker w)
    {
        cluster.Add(w);
        walkers.Remove(w);
        if (w.chainPos > biggestChain) biggestChain = w.chainPos;
        //update completion
        float f = (cluster.Count / MaxWalkers) *100;
        Progress = f;
        if (f == 100) finished = true;
    }

    bool donePostProcessing = false;
    private void Update()
    {
        if (finished && !donePostProcessing)
        {
            cluster[0].transform.localScale = new Vector3(.20f, .20f, .20f);

            Gradient gradient = new Gradient();

            // Populate the color keys at the relative time 0 and 1 (0 and 100%)
            GradientColorKey[] colorKey = new GradientColorKey[5];
            colorKey[0].color = Color.green;
            colorKey[0].time = 0.0f;
            colorKey[1].color = Color.green;
            colorKey[1].time = 0.3f;
            colorKey[2].color = Color.yellow;
            colorKey[2].time = 0.4f;
            colorKey[3].color = Color.yellow;
            colorKey[3].time = .5f;
            colorKey[4].color = Color.red;
            colorKey[4].time = 0.7f;

            // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
            GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 0.0f;
            alphaKey[1].time = 1.0f;

            gradient.SetKeys(colorKey, alphaKey);
            int total = cluster.Count;
            highSec = Mathf.FloorToInt(0.2f * (float)total);
            lowSec = Mathf.FloorToInt(0.15f * (float)total);
            nullSec = total - (highSec + lowSec);

            int highSecCounter = highSec;
            int lowSecCoutner = lowSec;

            var order = cluster.OrderBy(c => c.chainPos);

            foreach (Walker w in order)
            {
                w.transform.localScale = new Vector3(.10f, .10f, .10f);
                if (highSecCounter > 0)
                {
                    w.line.material.color = gradient.Evaluate(0);
                    w.meshrenderer.material.color = gradient.Evaluate(0);
                    highSecCounter--;
                } else if (lowSecCoutner > 0)
                {
                    w.line.material.color = gradient.Evaluate(.45f);
                    w.meshrenderer.material.color = gradient.Evaluate(.45f);
                    lowSecCoutner--;
                } else
                {
                    w.line.material.color = gradient.Evaluate(1);
                    w.meshrenderer.material.color = gradient.Evaluate(1);
                }
                //float f = (float)w.chainPos / (float)(biggestChain - 2);
                //if (w.chainPos > 25) w.line.material.color = gradient.Evaluate(1);
                //else w.line.material.color = gradient.Evaluate(f);
                //w.meshrenderer.material.color = gradient.Evaluate(f);
                //gradient.Evaluate(0.25f);
            }
            donePostProcessing = true;
        }
    }

    private void FixedUpdate()
    {
        if (!finished) TimeElapsed += Time.deltaTime;
    }
}
