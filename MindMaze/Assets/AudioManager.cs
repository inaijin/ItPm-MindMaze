using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    Transform playerTansform;
    float enemyDistanceFactor;
    AudioFader audioFader;
    int trackIndex = 0;
    CorridorFirstDungeonGenerator mapGenerator;
    void Start()
    {
        playerTansform = GameObject.FindGameObjectWithTag("Player").transform;
        mapGenerator = FindObjectOfType<CorridorFirstDungeonGenerator>();
        enemyDistanceFactor = mapGenerator.enemyDistanceChangeType;
        audioFader = GetComponent<AudioFader>();
        trackIndex = 0;
        audioFader.PlayTrack(trackIndex);
    }
    void Update()
    {
        float distance = Vector3.Distance(playerTansform.position, mapGenerator.playerSpawnPoint);
        int newIndex = Math.Min((int)(distance / enemyDistanceFactor), audioFader.audioSources.Length - 1);
        if(newIndex == trackIndex)
        {
            return;
        }
        Debug.Log("new sound " + newIndex);
        trackIndex = newIndex;
        audioFader.PlayTrack(trackIndex);
    }
}
