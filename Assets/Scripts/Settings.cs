using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {

    public string participantID = "default";
    public Hand hand = Hand.RIGHT;
    public bool rotating = true;

    public static Settings Instance;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public enum Hand
    {
        RIGHT, LEFT
    };
}
