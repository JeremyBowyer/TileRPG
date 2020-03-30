using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class GameController : StateMachine
{
    // References
    [HideInInspector]
    public CameraController cameraRig;
    [HideInInspector]
    public Camera _camera;
    [HideInInspector]
    public ProtagonistController protag;

    public NavMeshAgent protagAgent;

    // Variables
    public bool isPaused = false;
    public List<GameObject> players; // All players on map
    public List<GameObject> enemies; // Enemies in battle
    public List<GameObject> characters; // All enemies and players on map

    public virtual void AssignReferences()
    {
        // Assign references
        cameraRig = GameObject.Find("CameraTarget").GetComponent<CameraController>();
        _camera = GameObject.Find("Camera").GetComponent<Camera>();
    }

    public void InstantiateProtagonist()
    {
        GameObject protagGO = Instantiate(Resources.Load("Prefabs/Characters/Players/" + PersistentObjects.protagonist.model)) as GameObject;
        protagGO.tag = "Protag";
        protagGO.name = "Protagonist";

        Transform charLight = protagGO.transform.Find("CharacterLight");
        if (charLight != null)
            charLight.gameObject.SetActive(true);

        protag = protagGO.AddComponent<ProtagonistController>();
        protagAgent = protagGO.AddComponent<NavMeshAgent>();
        protagAgent.baseOffset = 0f;
        protagAgent.angularSpeed = 1200f;
        protagAgent.acceleration = 80f;
        protagAgent.radius = 0.2f;
        protagAgent.height = 1.9f;

        Rigidbody rb = protagGO.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public void PauseGame()
    {
        isPaused = true;
        cameraRig.isPaused = true;
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
        isPaused = false;
        cameraRig.isPaused = false;
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

}