using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public struct WallPair
    {
        public Wall UpperWall;
        public Wall LowerWall;
        public float GapVerticalCenter;
    }


    public bool IsMoving;
    public bool HasStarted;
    public int Points;

    public Vector2 Center;
    public Vector2 ShipSpawnPoint;

    public Ship ShipPrefab;
    public Text Score;
    public EndgamePopup Popup;
    public AudioClip PassClip;

    public LevelData Data;

    private float groundOffsetFromScreenBottom { get; set; } = 0.15F;
    private float groundTopPosition { get; set; }
    private float screenTopPosition { get; set; }
    private float wallHorizExtent { get; set; }

    private Dictionary<string, Queue<SpriteRenderer>> TerrainQueues { get; set; }
    private LinkedList<WallPair> Walls { get; set; }

    private Ship spawnedShip { get; set; }
    public AudioSource Audio { get; set; }

    void Start()
    {
        Audio = GetComponent<AudioSource>();

        groundTopPosition = (GameManager.Hr.CameraWorldBounds.center.y - GameManager.Hr.CameraWorldBounds.extents.y) +
            GameManager.Hr.CameraWorldBounds.size.y * groundOffsetFromScreenBottom;

        TerrainQueues = new Dictionary<string, Queue<SpriteRenderer>>();
        TerrainQueues["LowerBackgrounds"] = new Queue<SpriteRenderer>();
        TerrainQueues["UpperBackgrounds"] = new Queue<SpriteRenderer>();
        TerrainQueues["Soils"] = new Queue<SpriteRenderer>();

        Walls = new LinkedList<WallPair>();

        Wall wall = Instantiate(Data.Wall);
        wallHorizExtent = wall.Renderer.bounds.extents.x;
        Destroy(wall);

        InitializeFirstTime();
    }

    private void InitializeFirstTime()
    {
        InitializeLevel();
        IsMoving = true;
    }

    public void InitializeLevel()
    {
        Data = GameManager.Hr.SettingsManager.CurrentLevel;

        GameManager.Hr.MainCamera.backgroundColor = Data.BackgroundColor;

        TerrainQueues["Soils"].Enqueue(
            Instantiate(Data.Soil, new Vector2(Center.x - Data.Soil.bounds.size.x, groundTopPosition), Quaternion.identity));
        TerrainQueues["Soils"].Enqueue(
            Instantiate(Data.Soil, new Vector2(Center.x, groundTopPosition), Quaternion.identity));
        TerrainQueues["Soils"].Enqueue(
            Instantiate(Data.Soil, new Vector2(Center.x + Data.Soil.bounds.size.x, groundTopPosition), Quaternion.identity));

        TerrainQueues["LowerBackgrounds"].Enqueue(
            Instantiate(Data.LowerBackground, new Vector2(Center.x - Data.LowerBackground.bounds.size.x, groundTopPosition + Data.LowerBackground.bounds.extents.y),
            Quaternion.identity));
        TerrainQueues["LowerBackgrounds"].Enqueue(
            Instantiate(Data.LowerBackground, new Vector2(Center.x, groundTopPosition + Data.LowerBackground.bounds.extents.y), Quaternion.identity));
        TerrainQueues["LowerBackgrounds"].Enqueue(
            Instantiate(Data.LowerBackground, new Vector2(Center.x + Data.LowerBackground.bounds.size.x, groundTopPosition + Data.LowerBackground.bounds.extents.y),
            Quaternion.identity));

        Vector2 screenTop = new Vector2(Center.x, GameManager.Hr.CameraWorldBounds.center.y + GameManager.Hr.CameraWorldBounds.extents.y);
        screenTopPosition = screenTop.y;

        TerrainQueues["UpperBackgrounds"].Enqueue(
            Instantiate(Data.UpperBackground, new Vector2(Center.x - Data.UpperBackground.bounds.size.x, screenTop.y - Data.UpperBackground.bounds.extents.y),
            Quaternion.identity));
        TerrainQueues["UpperBackgrounds"].Enqueue(
            Instantiate(Data.UpperBackground, new Vector2(Center.x, screenTop.y - Data.UpperBackground.bounds.extents.y), Quaternion.identity));
        TerrainQueues["UpperBackgrounds"].Enqueue(
            Instantiate(Data.UpperBackground, new Vector2(Center.x + Data.UpperBackground.bounds.size.x, screenTop.y - Data.UpperBackground.bounds.extents.y),
            Quaternion.identity));
    }

    private void CheckIfWallSurpassedPlayer(WallPair pair)
    {
        if (!pair.UpperWall.wallPassed)
        {
            if (pair.UpperWall.transform.position.x <= spawnedShip.transform.position.x)
            {
                pair.UpperWall.wallPassed = true;
                Points++;
                Audio.PlayOneShot(PassClip, GameManager.Hr.SettingsManager.CurrentVolume);
                UpdateScore();
            }
        }
    }

    private void MoveObjectWithVelocity(GameObject obj, float velToTheLeft)
    {
        obj.transform.position += Time.fixedDeltaTime * new Vector3(-1, 0) * velToTheLeft;
    }

    private void MoveProps()
    {
        foreach (SpriteRenderer prop in TerrainQueues["Soils"])
            MoveObjectWithVelocity(prop.gameObject, Data.LevelSpeed);
        foreach (SpriteRenderer prop in TerrainQueues["LowerBackgrounds"])
            MoveObjectWithVelocity(prop.gameObject, Data.BackGroundSpeed);
        foreach (SpriteRenderer prop in TerrainQueues["UpperBackgrounds"])
            MoveObjectWithVelocity(prop.gameObject, Data.BackGroundSpeed);

        foreach (WallPair wallPair in Walls)
        {
            CheckIfWallSurpassedPlayer(wallPair);

            MoveObjectWithVelocity(wallPair.UpperWall.gameObject, Data.LevelSpeed);
            MoveObjectWithVelocity(wallPair.LowerWall.gameObject, Data.LevelSpeed);
        }
    }

    private void CheckAndRearrangeTerrain()
    {
        float leftmostScreenCoordinate = GameManager.Hr.CameraWorldBounds.center.x - GameManager.Hr.CameraWorldBounds.extents.x;
        float rightmostScreenCoordinate = GameManager.Hr.CameraWorldBounds.center.x + GameManager.Hr.CameraWorldBounds.extents.x;

        foreach (Queue<SpriteRenderer> queue in TerrainQueues.Values)
        {
            SpriteRenderer leftmostObject = queue.Peek();

            if (leftmostObject.transform.position.x + leftmostObject.bounds.extents.x < leftmostScreenCoordinate)
                leftmostObject.transform.position = new Vector2(rightmostScreenCoordinate + leftmostObject.bounds.extents.x,
                    leftmostObject.transform.position.y);

            queue.Dequeue();
            queue.Enqueue(leftmostObject);
        }
    }

    private Vector2 CalculateMinAndMaxHeightsOfNextGapCenter()
    {
        float minGapCenterHeightOnScreen = groundTopPosition + Data.GapSize / 2;
        float maxGapCenterHeightOnScreen = screenTopPosition - Data.GapSize / 2;

        float lastGapHeight;
        if (Walls.Last != null)
            lastGapHeight = Walls.Last.Value.GapVerticalCenter;
        else
            lastGapHeight = GameManager.Hr.CameraWorldBounds.center.y;

        float minGapCenterHeightAccordingToLast = lastGapHeight - Data.MaxVerticalDistanceBetweenGaps;
        float maxGapCenterHeightAccordingToLast = lastGapHeight + Data.MaxVerticalDistanceBetweenGaps;

        float minGapCenterHeightAccordingToLastClamped =
            Mathf.Clamp(minGapCenterHeightAccordingToLast, minGapCenterHeightOnScreen, maxGapCenterHeightOnScreen);
        float maxGapCenterHeightAccordingToLastClamped =
            Mathf.Clamp(maxGapCenterHeightAccordingToLast, minGapCenterHeightOnScreen, maxGapCenterHeightOnScreen);

        return new Vector2(minGapCenterHeightAccordingToLastClamped, maxGapCenterHeightAccordingToLastClamped);
    }

    private void ManageWalls()
    {
        float wallSpawnPointX = GameManager.Hr.CameraWorldBounds.center.x + GameManager.Hr.CameraWorldBounds.extents.x +
            wallHorizExtent;
        float wallDespawnPointx = GameManager.Hr.CameraWorldBounds.center.x - GameManager.Hr.CameraWorldBounds.extents.x -
            wallHorizExtent;

        if (Walls.Count == 0 ||  wallSpawnPointX - Walls.Last.Value.UpperWall.transform.position.x > Data.DistanceBetweenWalls)
        {
            Vector2 minAndMaxHeights = CalculateMinAndMaxHeightsOfNextGapCenter();
            
            float randomGapHeight = Random.Range(minAndMaxHeights.x, minAndMaxHeights.y);

            Walls.AddLast(CreateWallGap(new Vector2(wallSpawnPointX, randomGapHeight)));
        }

        if(Walls.First.Value.UpperWall.transform.position.x < wallDespawnPointx)
        {
            Destroy(Walls.First.Value.LowerWall);
            Destroy(Walls.First.Value.UpperWall);

            Walls.RemoveFirst();
        }
    }

    public void ClearLevel()
    {
        Points = 0;
        UpdateScore();

        Destroy(spawnedShip?.gameObject);
        spawnedShip = null;

        foreach (Queue<SpriteRenderer> queue in TerrainQueues.Values)
        {
            foreach (SpriteRenderer prop in queue)
            {
                Destroy(prop.gameObject);
            }

            queue.Clear();
        }

        foreach (WallPair wallPair in Walls)
        {
            Destroy(wallPair.UpperWall.gameObject);
            Destroy(wallPair.LowerWall.gameObject);
        }

        Walls.Clear();

    }

    private WallPair CreateWallGap(Vector2 WallGapCenter)
    {
        Wall lowerWall = Instantiate(Data.Wall, WallGapCenter, Quaternion.identity).GetComponent<Wall>();
        Wall upperWall = Instantiate(Data.Wall, WallGapCenter, Quaternion.identity).GetComponent<Wall>();

        lowerWall.transform.position = new Vector2(lowerWall.transform.position.x,
            lowerWall.transform.position.y - lowerWall.Renderer.bounds.extents.y - Data.GapSize / 2);
        upperWall.transform.position = new Vector2(upperWall.transform.position.x,
            upperWall.transform.position.y + upperWall.Renderer.bounds.extents.y + Data.GapSize / 2);

        return new WallPair() { LowerWall = lowerWall, UpperWall = upperWall, GapVerticalCenter = WallGapCenter.y };
    }

    private void UpdateScore()
    {
        Score.text = "x" + Points.ToString();
    }

    public void StartLevel()
    {
        spawnedShip = Instantiate(ShipPrefab, ShipSpawnPoint, Quaternion.identity);
        HasStarted = true;
        IsMoving = true;
        Score.gameObject.SetActive(true);
    }

    private IEnumerator EndLevelCoroutine()
    {
        IsMoving = false;
        HasStarted = false;

        yield return new WaitForSeconds(1F);

        Score.gameObject.SetActive(false);

        Popup.gameObject.SetActive(true);
        Popup.SetScoreOnFinalScreen(Points);
    }

    public void EndLevel()
    {
        StartCoroutine(EndLevelCoroutine());
    }

    void FixedUpdate()
    {
        if(IsMoving)
        {
            MoveProps();
            CheckAndRearrangeTerrain();
        }

        if(HasStarted)
        {
            ManageWalls();

        }
    }
}
