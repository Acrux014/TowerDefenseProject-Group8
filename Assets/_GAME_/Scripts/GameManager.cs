using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public float dayLength = 60f;
    [SerializeField] public float nightLength = 90f;

    public bool isNight;
    public int nightNumber;
    public float timeLeft;

    private void Start()
    {
        StartDay();
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            if (isNight) StartDay();
            else StartNight();
        }
    }

    public void StartDay()
    {
        isNight = false;
        timeLeft = dayLength;
    }

    public void StartNight()
    {
        isNight = true;
        nightNumber++;
        timeLeft = nightLength;
    }

    private void OnGUI()
    {
        string phase = isNight ? "Night" : "Day";
        GUI.Label(new Rect(10, 10, 300, 25), phase + " " + nightNumber + " - " + Mathf.CeilToInt(timeLeft));
    }
}
