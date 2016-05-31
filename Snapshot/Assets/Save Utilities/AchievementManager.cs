using UnityEngine;
using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

/*
    Code borrowed from https://github.com/spgar/Progress under the MIT license.
*/

// AchievementManager contains Achievements, which players are able to earn through performing various actions
// in the game. Each Achievement specifies a name, description, pair of icons, number of reward points, progress
// towards reward and whether or not it is secret.

[Serializable]
public class SerializeableBadgeData {
    public string Name;
    public string Description;
    public string IconIncomplete;
    public string IconComplete;
    public int RewardPoints;
    public float TargetProgress;
    public bool Secret;
    public List<string> Desecrefies;
    public bool Earned;
    public float currentProgress;
}

public class Achievement {
    public SerializeableBadgeData data;
    public Texture2D iconIncomplete;
    public Texture2D iconComplete;

    public Achievement(string name, string desc, string iconIncompletePath, string iconCompletePath, int rewardPoints, float targetProgress, bool secret, List<string> desecrefies) {
        data = new SerializeableBadgeData();
        data.Name = name;
        data.Description = desc;
        data.IconIncomplete = iconIncompletePath;
        data.IconComplete = iconCompletePath;
        data.RewardPoints = rewardPoints;
        data.TargetProgress = targetProgress;
        data.Secret = secret;
        data.Desecrefies = desecrefies;
        data.Earned = false;
        data.currentProgress = 0.0f;

        iconIncomplete = Resources.Load(iconIncompletePath) as Texture2D;
        iconComplete = Resources.Load(iconCompletePath) as Texture2D;
    }

    public Achievement(SerializeableBadgeData badgeData) {
        data = badgeData;

        iconIncomplete = Resources.Load(data.IconIncomplete) as Texture2D;
        iconComplete = Resources.Load(data.IconComplete) as Texture2D;
    }

    // Returns true if this progress added results in the Achievement being earned.
    public bool AddProgress(float progress) {
        if (data.Earned) {
            return false;
        }

        float newTotalProgress = data.currentProgress + progress;

        if (data.TargetProgress >= newTotalProgress) {
          data.currentProgress = newTotalProgress;
        } else data.currentProgress = data.TargetProgress;

        if (data.currentProgress >= data.TargetProgress) {
            data.Earned = true;
            return true;
        }

        return false;
    }

    // Returns true if this progress set results in the Achievement being earned.
    public bool SetProgress(float progress) {
        if (data.Earned) {
            return false;
        }

        if (data.TargetProgress >= progress) {
          data.currentProgress = progress;
        } else data.currentProgress = data.TargetProgress;
        
        if (progress >= data.TargetProgress) {
            data.Earned = true;
            return true;
        }

        return false;
    }

    // Basic GUI for displaying an achievement. Has a different style when earned and not earned.
    public void OnGUI(Rect position, GUIStyle GUIStyleAchievementEarned, GUIStyle GUIStyleAchievementNotEarned,
                      Texture2D complete, Texture2D incomplete) {
        GUIStyle style = GUIStyleAchievementNotEarned;
        if (data.Earned) {
            style = GUIStyleAchievementEarned;
        }

        GUI.BeginGroup(position);
        GUI.Box(new Rect(0.0f, 0.0f, position.width, position.height), "");

        if (data.Earned) {
            GUI.Box(new Rect(0.0f, 0.0f, position.height, position.height), complete);
        } else {
            GUI.Box(new Rect(0.0f, 0.0f, position.height, position.height), incomplete);
        }

        GUI.Label(new Rect(80.0f, 5.0f, position.width - 80.0f - 50.0f, 25.0f), data.Name, style);

        if (data.Secret && !data.Earned) {
            GUI.Label(new Rect(80.0f, 25.0f, position.width - 80.0f, 25.0f), "???", style); // Description
            //GUI.Label(new Rect(position.width - 50.0f, 5.0f, 25.0f, 25.0f), "???", style); // Reward Points (not used)
            GUI.Label(new Rect(position.width - 250.0f, 50.0f, 250.0f, 25.0f), "???", style); // Progress
        } else {
            GUI.Label(new Rect(80.0f, 25.0f, position.width - 80.0f, 25.0f), data.Description, style);
            //GUI.Label(new Rect(position.width - 50.0f, 5.0f, 25.0f, 25.0f), data.RewardPoints.ToString(), style);
            GUI.Label(new Rect(position.width - 250.0f, 50.0f, 250.0f, 25.0f), "Progress: " + data.currentProgress.ToString("0.#") + " out of " + data.TargetProgress.ToString("0.#"), style);
        }

        GUI.EndGroup();
    }
}

public class AchievementManager : MonoBehaviour {
    public List<Achievement> Achievements;
    public AudioSource EarnedSound;
    public GUIStyle GUIStyleAchievementEarned;
    public GUIStyle GUIStyleAchievementNotEarned;

    private int currentRewardPoints = 0;
    private int potentialRewardPoints = 0;
    private string savePath;
    private bool achievementsLoaded = false;

    void Awake() {
        savePath = Application.persistentDataPath + "/cheevos/";
        //Debug.Log(savePath);

        Achievements = new List<Achievement>();
    }

    void Start() {
        loadAchievements();
        ValidateAchievements();
        UpdateRewardPointTotals();
    }

    // Make sure some assumptions about achievement data setup are followed.
    private void ValidateAchievements() {
        ArrayList usedNames = new ArrayList();
        foreach (Achievement achievement in Achievements) {
            if (achievement.data.RewardPoints < 0) {
                Debug.LogError("AchievementManager::ValidateAchievements() - Achievement with negative RewardPoints! " + achievement.data.Name + " gives " + achievement.data.RewardPoints + " points!");
            }

            if (usedNames.Contains(achievement.data.Name)) {
                Debug.LogError("AchievementManager::ValidateAchievements() - Duplicate achievement names! " + achievement.data.Name + " found more than once!");
            }
            usedNames.Add(achievement.data.Name);
        }
    }

    private Achievement GetAchievementByName(string achievementName) {
        return Achievements.FirstOrDefault(achievement => achievement.data.Name == achievementName);
    }

    private void UpdateRewardPointTotals() {
        currentRewardPoints = 0;
        potentialRewardPoints = 0;

        foreach (Achievement achievement in Achievements) {
            if (achievement.data.Earned) {
                currentRewardPoints += achievement.data.RewardPoints;
            }

            potentialRewardPoints += achievement.data.RewardPoints;
        }
    }

    private void AchievementEarned(Achievement achievement) {
        // Handle removing secret property as needed
        foreach (string name in achievement.data.Desecrefies) {
            Achievement desecrefied = GetAchievementByName(name);

            if (desecrefied != null) {
                desecrefied.data.Secret = false;
                saveAchievement(desecrefied);
            }
            else
                Debug.Log("Hey like not to bother you or anything but '" + achievement.data.Name + "' is trying to desecrefy '" + name + "', which doesn't exist.");
        }

        UpdateRewardPointTotals(); // Gamer Score analog - we don't really use it.
        //EarnedSound.Play(); // Need better sound effect, maybe add popup, too?
    }

    public void AddProgressToAchievement(string achievementName, float progressAmount) {
        Achievement achievement = GetAchievementByName(achievementName);
        if (achievement == null) {
            Debug.LogWarning("AchievementManager::AddProgressToAchievement() - Trying to add progress to an achievemnet that doesn't exist: " + achievementName);
            return;
        }

        if (achievement.AddProgress(progressAmount)) {
            AchievementEarned(achievement);
        }
        saveAchievement(achievement);
    }

    public void SetProgressToAchievement(string achievementName, float newProgress) {
        Achievement achievement = GetAchievementByName(achievementName);
        if (achievement == null) {
            Debug.LogWarning("AchievementManager::SetProgressToAchievement() - Trying to add progress to an achievemnet that doesn't exist: " + achievementName);
            return;
        }

        if (achievement.SetProgress(newProgress)) {
            AchievementEarned(achievement);
        }
        saveAchievement(achievement);
    }

    /*
        New Utilities
    */
    public void initializeAchievements() {
        List<string> dayTripDesecrefies = new List<string>();
        dayTripDesecrefies.Add("Marathoner");
        dayTripDesecrefies.Add("Mountaineer");
        Achievements.Add(new Achievement(
            "Day Tripper",
            "Walk a total of 1 kilometer.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            1000f,
            false,
            dayTripDesecrefies
        ));

        List<string> marathonerDesecrefies = new List<string>();
        marathonerDesecrefies.Add("Seasoned Hiker");
        Achievements.Add(new Achievement(
            "Marathoner",
            "Walk an entire marathon... 42.19 kilometers!",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            42190f,
            true,
            marathonerDesecrefies
        ));

        List<string> seasonedHikerDesecrefies = new List<string>();
        seasonedHikerDesecrefies.Add("Master Backpacker");
        Achievements.Add(new Achievement(
            "Seasoned Hiker",
            "Walk a total of 100 kilometers. That's the distance from Washington, DC to Chicago!",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            100000f,
            true,
            seasonedHikerDesecrefies
        ));

        List<string> masterBackpackerDesecrefies = new List<string>();
        Achievements.Add(new Achievement(
            "Master Backpacker",
            "Walk a total of 500 kilometers. That's the same as the distance between San Francisco and LA!",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            500000f,
            true,
            masterBackpackerDesecrefies
        ));

        List<string> journeymanPhotographerDesecrefies = new List<string>();
        journeymanPhotographerDesecrefies.Add("Experienced Photographer");
        Achievements.Add(new Achievement(
            "Journeyman Photographer",
            "Post a photo to your blog.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            1f,
            false,
            journeymanPhotographerDesecrefies
        ));

        List<string> experiencedPhotographerDesecrefies = new List<string>();
        journeymanPhotographerDesecrefies.Add("Experienced Photographer");
        Achievements.Add(new Achievement(
            "Experienced Photographer",
            "Post 50 photos to your blog.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            50f,
            true,
            experiencedPhotographerDesecrefies
        ));

        List<string> expertPhotographerDesecrefies = new List<string>();
        Achievements.Add(new Achievement(
            "Expert Photographer",
            "Post 200 photos to your blog.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            200f,
            true,
            expertPhotographerDesecrefies
        ));

        List<string> balancedBreakfastDesecrefies = new List<string>();
        balancedBreakfastDesecrefies.Add("Perfection Incarnate");
        Achievements.Add(new Achievement(
            "Balanced Breakfast",
            "Post a perfectly balanced photo to your blog.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            1f,
            false,
            balancedBreakfastDesecrefies
        ));

        List<string> theMostInterestingPhotoDesecrefies = new List<string>();
        theMostInterestingPhotoDesecrefies.Add("Perfection Incarnate");
        Achievements.Add(new Achievement(
            "The Most Interesting Photo in The World",
            "Post a perfectly interesting photo to your blog.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            1f,
            false,
            theMostInterestingPhotoDesecrefies
        ));

        List<string> theFinalFrontierDesecrefies = new List<string>();
        theFinalFrontierDesecrefies.Add("Perfection Incarnate");
        Achievements.Add(new Achievement(
            "The Final Frontier",
            "Post a perfectly well-spaced photo to your blog.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            1f,
            false,
            theMostInterestingPhotoDesecrefies
        ));

        List<string> perfectionIncarnateDesecrefies = new List<string>();
        Achievements.Add(new Achievement(
            "Perfection Incarnate",
            "Post an absolutely perfect photo to your blog.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            1f,
            true,
            perfectionIncarnateDesecrefies
        ));

        List<string> TelemachusDesecrefies = new List<string>();
        Achievements.Add(new Achievement(
            "Telemachus",
            "Post a good photo taken with a telephoto lens to your blog.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            1f,
            false,
            TelemachusDesecrefies
        ));

        List<string> WideAwakeDesecrefies = new List<string>();
        Achievements.Add(new Achievement(
            "Wide Awake",
            "Post a good photo taken using a wide-angle lens to your blog.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            1f,
            false,
            WideAwakeDesecrefies
        ));

        /*List<string> HipstergramDesecrefies = new List<string>();
        Achievements.Add(new Achievement(
            "Hipstergram",
            "Post a good photo taking using a lens filter to your blog.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            1f,
            false,
            HipstergramDesecrefies
        ));*/

        List<string> doeDesecrefies = new List<string>();
        doeDesecrefies.Add("Say Cheese");
        Achievements.Add(new Achievement(
            "Doe, a Deer, a Female Deer",
            "Post a good photo containing a deer to your blog.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            1f,
            false,
            doeDesecrefies
        ));

        List<string> foxDesecrefies = new List<string>();
        foxDesecrefies.Add("Say Cheese");
        Achievements.Add(new Achievement(
            "What the Fox",
            "Post a good photo containing a fox to your blog.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            1f,
            false,
            foxDesecrefies
        ));

        List<string> owlDesecrefies = new List<string>();
        owlDesecrefies.Add("Say Cheese");
        Achievements.Add(new Achievement(
            "Owl Have What She's Having",
            "Post a good photo containing an owl to your blog.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            1f,
            false,
            owlDesecrefies
        ));

        List<string> MountaineerDesecrefies = new List<string>();;
        Achievements.Add(new Achievement(
            "Mountaineer",
            "Reach the highest point in the park.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            1f,
            true,
            MountaineerDesecrefies
        ));

        /*List<string> MementoDesecrefies = new List<string>(); ;
        Achievements.Add(new Achievement(
            "Memento",
            "Unlock the largest memory card.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            1f,
            false,
            MementoDesecrefies
        ));*/

        /*List<string> cheeseDesecrefies = new List<string>(); ;
        Achievements.Add(new Achievement(
            "Say Cheese",
            "Post a photo of an animal striking an unusual pose to your blog.",
            "badge_icons/AchievementIncompleteIcon",
            "badge_icons/AchievementCompleteIcon",
            0,
            1f,
            true,
            cheeseDesecrefies
        ));*/
    }

    public void saveAchievements() {
        //Debug.Log("Saving cheevos to " + savePath);
        BinaryFormatter binForm = new BinaryFormatter();
        string fullCheevoPath;

        foreach (Achievement cheevo in Achievements) {
            fullCheevoPath = savePath + cheevo.data.Name + ".cheevo";
            FileStream cheevoFile;
            if (File.Exists(fullCheevoPath)) {
                cheevoFile = File.Open(fullCheevoPath, FileMode.Open);
            } else cheevoFile = File.Create(fullCheevoPath);

            binForm.Serialize(cheevoFile, cheevo.data);
            cheevoFile.Close();
        }
    }

    public void saveAchievement(Achievement cheevo) {
        //Debug.Log("Saving cheevo to " + savePath);
        BinaryFormatter binForm = new BinaryFormatter();
        string fullCheevoPath = savePath + cheevo.data.Name + ".cheevo";
        FileStream cheevoFile;
        if (File.Exists(fullCheevoPath)) {
            cheevoFile = File.Open(fullCheevoPath, FileMode.Open);
        } else cheevoFile = File.Create(fullCheevoPath);

        binForm.Serialize(cheevoFile, cheevo.data);
        cheevoFile.Close();
    }

    public void loadAchievements() {
        DirectoryInfo dir = new DirectoryInfo(savePath);

        if (!dir.Exists) {
            dir.Create();
        }

        FileInfo[] info = dir.GetFiles("*.cheevo");
        BinaryFormatter binForm = new BinaryFormatter();
        string fullCheevoPath;

        // If info.Length is 0, then there is no cheevo data saved on disk, so we should set some up!
        if (info.Length == 0) {
            initializeAchievements();
            saveAchievements();
        } else if (!achievementsLoaded) {   // Make sure we don't attempt to load the achievements twice
            foreach (FileInfo file in info) {
                fullCheevoPath = savePath + file.Name;
                FileStream cheevoFile = File.Open(fullCheevoPath, FileMode.Open);
                
                SerializeableBadgeData badgeData = (SerializeableBadgeData)binForm.Deserialize(cheevoFile);
                Achievement cheevo = new Achievement(badgeData);

                cheevoFile.Close();
                Achievements.Add(cheevo);
            }
            achievementsLoaded = true;
        }
    }
}
