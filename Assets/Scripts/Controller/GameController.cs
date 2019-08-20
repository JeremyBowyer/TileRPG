using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : StateMachine
{
    // References
    public CameraController cameraRig;
    public Camera _camera;
    public ProtagonistController protag;

    // Variables
    public List<GameObject> players; // All players on map
    public List<GameObject> enemies; // Enemies in battle
    public List<GameObject> characters; // All enemies and players on map

    void Start()
    {
        // Assign references
        protag = GameObject.FindGameObjectWithTag("Protag").GetComponent<ProtagonistController>();
        cameraRig = GameObject.Find("CameraTarget").GetComponent<CameraController>();
        _camera = GameObject.Find("Camera").GetComponent<Camera>();
    }

    public void PauseGame()
    {
        foreach (GameObject character in characters)
        {
            if (character == null)
                continue;
            CharController controller = character.GetComponent<CharController>();
            controller.Pause();
        }
    }

    public void ResumeGame()
    {
        foreach (GameObject character in characters)
        {
            if (character == null)
                continue;
            CharController controller = character.GetComponent<CharController>();
            controller.Resume();
        }
    }

    public void EnableRBs(bool enabled)
    {
        foreach (GameObject character in characters)
        {
            if (character == null)
                continue;

            Rigidbody rb = character.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = !enabled;
                rb.useGravity = enabled;
            }

        }
    }

    public void FollowTarget(Transform _target)
    {
        cameraRig.FollowTarget = _target;
        cameraRig.isFollowing = true;
    }

    public void InstantiateProtagonist()
    {

    }

}