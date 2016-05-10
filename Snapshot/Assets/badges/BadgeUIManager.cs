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

		if (ParkPrepUIManager.src != null) {
			ParkPrepUIManager.src.Play();
		}
    }

    // Sets up a scrollview and fills it out with each Achievement.
    void OnGUI() {
        float yValue = 5.0f;
        float achievementGUIWidth = Screen.width / 2;

        Rect scrollViewPosition = new Rect(0, 0, Screen.width / 2, Screen.height * 0.80f);
        scrollViewPosition.center = new Vector2(Screen.width / 2, Screen.height / 2);
        Rect scrollViewRect = new Rect(0, 0, achievementGUIWidth - 16.0f, cheevoMgr.Achievements.Count * 80.0f);
        Rect labelTextRect = new Rect(0, 0, 200.0f, 25.0f);
        labelTextRect.center = new Vector2(Screen.width / 2, Screen.height / 2);

        //GUI.Label(labelTextRect, "Your Badges");

        achievementScrollviewLocation = GUI.BeginScrollView(scrollViewPosition,
                                                            achievementScrollviewLocation,
                                                            scrollViewRect);

        foreach (Achievement achievement in cheevoMgr.Achievements) {
            Rect position = new Rect(5.0f, yValue, achievementGUIWidth, 75.0f);
            achievement.OnGUI(position, cheevoMgr.GUIStyleAchievementEarned, cheevoMgr.GUIStyleAchievementNotEarned,
                              achievement.iconComplete, achievement.iconIncomplete);
            yValue += 80.0f;
        }

        GUI.EndScrollView();
    }
}
