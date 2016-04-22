using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BadgeUIManager : MonoBehaviour {
    public AchievementManager cheevoMgr;

    private Vector2 achievementScrollviewLocation = Vector2.zero;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void goToMain() {
        SceneManager.LoadScene("main_menu");
    }

    // Sets up a scrollview and fills it out with each Achievement.
    void OnGUI() {
        float yValue = 5.0f;
        float achievementGUIWidth = 500.0f;

        GUI.Label(new Rect(200.0f, 5.0f, 200.0f, 25.0f), "-- Badges --");

        achievementScrollviewLocation = GUI.BeginScrollView(new Rect(0.0f, 25.0f, achievementGUIWidth + 25.0f, 400.0f), achievementScrollviewLocation,
                                                            new Rect(0.0f, 0.0f, achievementGUIWidth, cheevoMgr.Achievements.Count * 80.0f));

        foreach (Achievement achievement in cheevoMgr.Achievements) {
            Rect position = new Rect(5.0f, yValue, achievementGUIWidth, 75.0f);
            achievement.OnGUI(position, cheevoMgr.GUIStyleAchievementEarned, cheevoMgr.GUIStyleAchievementNotEarned,
                              achievement.iconComplete, achievement.iconIncomplete);
            yValue += 80.0f;
        }

        GUI.EndScrollView();
    }
}
