// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; set; }

    public bool SHOW_COLLIDER = true; // for the test to see the colliders. 


    // Level Spawning
    private const float DISTANCE_BEFORE_SPAWN = 100.0f;
    private const int INITIAL_SEGMENTS = 10;
    private const int INITIAL_TRANSITION_SEGMETS = 2;
    private const int MAX_SEGMENTS_ON_SCREEN = 15;
    private Transform cameraContainer;
    private int amountOfACtiveSegments;
    private int continuousSegments;
    private int currentSpawnZ;
    private int currentLevel;
    private int y1, y2, y3;

    // List of pieces. 
    public List<Piece> ramps = new List<Piece>();
    public List<Piece> longblocks = new List<Piece>();
    public List<Piece> jumps = new List<Piece>();
    public List<Piece> slides = new List<Piece>();
    [HideInInspector]
    public List<Piece> pieces = new List<Piece>(); // all active pieces in pool. 

    // List of segments
    public List<Segment> availableSegments = new List<Segment>();
    public List<Segment> availableTransitions = new List<Segment>(); // segment that has either something that can't kill the player
                                                                     // or nothing at all, just to give the player some breathing room to continue with the running. 

    [HideInInspector]
    public List<Segment> _activeSegments = new List<Segment>();

    // Gameplay
    private bool isMoving = false;

    private void Awake()
    {
        Instance = this;
        cameraContainer = Camera.main.transform;
        // you can still use Camera.current.transform incase your camera is nottagged main. 
        currentSpawnZ = 0;
        currentLevel = 0;
    }

    private void Start()
    {
        for (int i = 0; i < INITIAL_SEGMENTS; i++)
        {
            if (i < INITIAL_TRANSITION_SEGMETS)
                SpawnTransition();
            else
                GenerateSegment();
        }
    }

    private void GenerateSegment()
    {
        SpawnSegment();

        //just a mechanic that will certainly spawn a Transition in a 1/5 chance. 
        if (Random.Range(0f, 1f) < (continuousSegments * 0.25f))
        {
            // spawn transition segment
            continuousSegments = 0;
            SpawnTransition();
        }
        else
        {
            continuousSegments++;
        }
    }

    private void SpawnTransition()
    {
        List<Segment> possibleTransition = availableTransitions.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleTransition.Count);

        Segment s = GetSegment(id, true);
        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += s.length;
        amountOfACtiveSegments++;
        s.Spawn();
    }

    private void SpawnSegment()
    {
        List<Segment> possibleSeg = availableSegments.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleSeg.Count);

        Segment s = GetSegment(id, false);
        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += s.length;
        amountOfACtiveSegments++;
        s.Spawn();
    }

    private void Update()
    {
        if (currentSpawnZ - cameraContainer.position.z < DISTANCE_BEFORE_SPAWN)
            GenerateSegment();

        if (amountOfACtiveSegments >= MAX_SEGMENTS_ON_SCREEN)
        {
            _activeSegments[amountOfACtiveSegments - 1].DeSpawn();
            amountOfACtiveSegments--;
        }
    }

    public Segment GetSegment(int id, bool transition)
    {
        Segment s = null;
        s = _activeSegments.Find(x => x.SegID == id &&
        x.transition == transition && !x.gameObject.activeSelf);

        if (s == null)
        {
            GameObject go = Instantiate((transition) ? availableTransitions[id].gameObject : availableSegments[id].gameObject);
            s = go.GetComponent<Segment>();

            s.SegID = id;
            s.transition = transition;

            _activeSegments.Insert(0, s);
        }
        else
        {
            _activeSegments.Remove(s);
            _activeSegments.Insert(0, s);
        }
        return s;
    }
    public Piece GetPiece(PieceType _pieceType, int _visualIndex)
    {
        Piece _piece = pieces.Find(x => x.type == _pieceType &&
        x.visualIndex == _visualIndex && !x.gameObject.activeSelf);

        if (_piece == null)
        {
            GameObject go = null; // initialize incase none of the conditions occur. 
            if (_pieceType == PieceType.ramp)
                go = ramps[_visualIndex].gameObject;
            else if (_pieceType == PieceType.longblock)
                go = longblocks[_visualIndex].gameObject;
            else if (_pieceType == PieceType.jump)
                go = jumps[_visualIndex].gameObject;
            else if (_pieceType == PieceType.slide)
                go = slides[_visualIndex].gameObject;

            go = Instantiate(go);
            _piece = go.GetComponent<Piece>();
            pieces.Add(_piece);
        }
        return _piece;
    }
}
