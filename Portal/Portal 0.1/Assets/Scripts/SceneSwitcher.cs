using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    public Transform player;
    public float radius;
    public Image textImage;
    public Text text;
    public string sceneName;

    Transform transform;

    void Start()
    {
        transform = gameObject.transform;
        textImage.enabled = false;
        text.enabled = false;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance < radius) {
            textImage.enabled = true;
            text.enabled = true;
            if (Input.GetKeyDown(KeyCode.E)) {
                SceneManager.LoadScene(sceneName);
            }
        } else {
            textImage.enabled = false;
            text.enabled = false;
        }
    }
}
