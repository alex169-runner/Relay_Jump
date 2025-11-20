using System.Collections.Generic;
using UnityEngine;

internal class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    public static readonly int maxRollBackNum = 10;

    public enum Difficulty { EASY, MEDIUM, HARD };
    public Difficulty difficulty {  get; private set; }

    private static readonly int[] Moves = { 5, 7, 10 };
    public int moves { get; private set; }

    public Map answer;
    private GridMap gridMap;
    private FrogManager situation;
    private MaterialHighlighter mapHighlighter;

    private bool isStarted;
    private int backupId;

    public GridMap mapPrefab;
    public FrogManager frogManagerPrefab;

    public LinkedList<Action> actionStack;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            difficulty = Difficulty.EASY;

            isStarted = false;

            gridMap = Instantiate(mapPrefab);
            mapHighlighter = gridMap.GetComponent<MaterialHighlighter>();

            gridMap.gameObject.transform.position = Vector3.zero;
            gridMap.gameObject.SetActive(false);

            situation = Instantiate(frogManagerPrefab);

            actionStack = new();
        } else {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isStarted) {
            if (Input.GetMouseButtonDown(1)) {
                RollBack();
            }
            else if (Input.GetKeyDown(KeyCode.Return)) {
                HideItem();
                NewGame();
            } else if (Input.GetKeyUp(KeyCode.Space)) {
                RestartGame();
            }
        }
    }

    private void OnEnable()
    {
        Frog.OnFrogClicked += StartJump;
    }

    private void OnDisable()
    {
        Frog.OnFrogClicked -= StartJump;
    }
    private void HideItem()
    {
        gridMap.gameObject.SetActive(false);
        situation.Clear();
    }

    private void NewGame()
    {
        isStarted = false;
        EndGameUI.instance.SetAsleep();
        DifficultySelector.Instance.SetAwake();
    }


    private void RestartGame()
    {
        actionStack.Clear();
        situation.Init();

        situation.frogs[situation.stimulatedId].isStimulated = false;
        situation.stimulatedId = backupId;
        situation.frogs[backupId].isStimulated = true;

        for (int i = 0; i < FrogManager.number; ++i) {
            situation.frogs[i].isHome = answer.IsOccupied(situation.frogs[i].pos);
        }
    }

    public void AddStack(int starId,
                         int planetId,
                  Vector2Int planetPosOld,
                  Vector2Int planetPosNew)
    {
        actionStack.AddLast(new Action
        (
            starId, planetId, planetPosOld, planetPosNew
        ));
        if (actionStack.Count > maxRollBackNum) {
            actionStack.RemoveFirst();
        }
    }

    private void RollBack()
    {
        if (actionStack.Count == 0) {
            return;
        }
        Action cur = actionStack.Last.Value;
        actionStack.RemoveLast();

        situation.map.GetVacant(cur.planetPosNew);
        situation.map.GetOccupied(cur.planetPosOld);

        situation.frogs[cur.starId].isStimulated = false;
        situation.frogs[cur.planetId].isStimulated = true;

        situation.frogs[cur.planetId].isHome = answer.IsOccupied(cur.planetPosOld);

        situation.frogs[cur.planetId].pos = cur.planetPosOld;

        situation.stimulatedId = cur.planetId;
    }

    private void StartJump(Frog frog)
    {
        situation.Jump(frog);
        if (answer.IsSame(situation.map)) {
            EndGame();
        }
    }

    public void SetDifficulty(Difficulty newDifficulty)
    {
        difficulty = newDifficulty;
        InitializeGame();
    }

    private void InitializeGame()
    {
        gridMap.gameObject.SetActive(true);

        situation.Init();
        backupId = situation.stimulatedId;

        answer = new(situation.map);

        actionStack.Clear();

        ApplyDifficultySetting();

        isStarted = true;
    }

    private void ApplyDifficultySetting()
    {
        moves = Moves[(int)difficulty];

        List<Vector2Int> frogs = new();
        for (int i = 0; i < FrogManager.number; ++i) {
            frogs.Add(situation.frogs[i].pos);
        }

        int stimulatedId = situation.stimulatedId;

        while (moves > 0) {

            int next = Random.Range(0, FrogManager.number - 1);
            if (next == stimulatedId) {
                ++next;
            }

            Vector2Int reflectPos = Map.Reflection(frogs[stimulatedId], frogs[next]);

            if (answer.IsOccupied(reflectPos)) {
                continue;
            }

            answer.GetVacant(frogs[stimulatedId]);
            answer.GetOccupied(reflectPos);

            frogs[stimulatedId] = reflectPos;
            stimulatedId = next;

            moves--;
        }

        for (int i = 0; i < FrogManager.number; ++i) {
            situation.frogs[i].isHome = answer.IsOccupied(situation.frogs[i].pos);
        }

        mapHighlighter.HighlightPositions(frogs);
    }

    private void EndGame()
    {
        HideItem();

        EndGameUI.instance.ShowEndGameMessage(difficulty);
        Invoke("NewGame", 2.1f);
    }
}
