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
public class Achievement {
    public string Name;
    public string Description;
    public string IconIncomplete;
    public string IconComplete;
    public int RewardPoints;
    public float TargetProgress;
    public bool Secret;
    public string[] deSecrefies = new string[10];

    public bool Earned = false;
    private float currentProgress = 0.0f;

    // Returns true if this progress added results in the Achievement being earned.
    public bool AddProgress(float progress) {
        if (Earned) {
            return false;
        }

        currentProgress += progress;
        if (currentProgress >= TargetProgress) {
            Earned = true;
            return true;
        }

        return false;
    }

    // Returns true if this progress set results in the Achievement being earned.
    public bool SetProgress(float progress) {
        if (Earned) {
            return false;
        }

        currentProgress = progress;
        if (progress >= TargetProgress) {
            Earned = true;
            return true;
        }

        return false;
    }

    // Basic GUI for displaying an achievement. Has a different style when earned and not earned.
    public void OnGUI(Rect position, GUIStyle GUIStyleAchievementEarned, GUIStyle GUIStyleAchievementNotEarned,
                      Texture2D complete, Texture2D incomplete) {
        GUIStyle style = GUIStyleAchievementNotEarned;
        if (Earned) {
            style = GUIStyleAchievementEarned;
        }

        GUI.BeginGroup(position);
        GUI.Box(new Rect(0.0f, 0.0f, position.width, position.height), "");

        if (Earned) {
            GUI.Box(new Rect(0.0f, 0.0f, position.height, position.height), complete);
        } else {
            GUI.Box(new Rect(0.0f, 0.0f, position.height, position.height), incomplete);
        }

        GUI.Label(new Rect(80.0f, 5.0f, position.width - 80.0f - 50.0f, 25.0f), Name, style);

        if (Secret && !Earned) {
            GUI.Label(new Rect(80.0f, 25.0f, position.width - 80.0f, 25.0f), "Description Hidden!", style);
            GUI.Label(new Rect(position.width - 50.0f, 5.0f, 25.0f, 25.0f), "???", style);
            GUI.Label(new Rect(position.width - 250.0f, 50.0f, 250.0f, 25.0f), "Progress Hidden!", style);
        } else {
            GUI.Label(new Rect(80.0f, 25.0f, position.width - 80.0f, 25.0f), Description, style);
            GUI.Label(new Rect(position.width - 50.0f, 5.0f, 25.0f, 25.0f), RewardPoints.ToString(), style);
            GUI.Label(new Rect(position.width - 250.0f, 50.0f, 250.0f, 25.0f), "Progress: [" + currentProgress.ToString("0.#") + " out of " + TargetProgress.ToString("0.#") + "]", style);
        }

        GUI.EndGroup();
    }
}

public class AchievementManager : MonoBehaviour {
    public List<Achievement> Achievements;
    public AudioClip EarnedSound;
    public GUIStyle GUIStyleAchievementEarned;
    public GUIStyle GUIStyleAchievementNotEarned;

    private int currentRewardPoints = 0;
    private int potentialRewardPoints = 0;
    private Vector2 achievementScrollviewLocation = Vector2.zero;
    private string savePath;
    private Texture2D complete;
    private Texture2D incomplete;

    void Start() {
        savePath = Application.persistentDataPath + "/cheevos/";

        complete = Resources.Load("badge_icons/AchievementCompleteIcon") as Texture2D;
        incomplete = Resources.Load("badge_icons/AchievementIncompleteIcon") as Texture2D;

        loadAchievements();
        ValidateAchievements();
        UpdateRewardPointTotals();
    }

    // Make sure some assumptions about achievement data setup are followed.
    private void ValidateAchievements() {
        ArrayList usedNames = new ArrayList();
        foreach (Achievement achievement in Achievements) {
            if (achievement.RewardPoints < 0) {
                Debug.LogError("AchievementManager::ValidateAchievements() - Achievement with negative RewardPoints! " + achievement.Name + " gives " + achievement.RewardPoints + " points!");
            }

            if (usedNames.Contains(achievement.Name)) {
                Debug.LogError("AchievementManager::ValidateAchievements() - Duplicate achievement names! " + achievement.Name + " found more than once!");
            }
            usedNames.Add(achievement.Name);
        }
    }

    private Achievement GetAchievementByName(string achievementName) {
        return Achievements.FirstOrDefault(achievement => achievement.Name == achievementName);
    }

    private void UpdateRewardPointTotals() {
        currentRewardPoints = 0;
        potentialRewardPoints = 0;

        foreach (Achievement achievement in Achievements) {
            if (achievement.Earned) {
                currentRewardPoints += achievement.RewardPoints;
            }

            potentialRewardPoints += achievement.RewardPoints;
        }
    }

    private void AchievementEarned() {
        UpdateRewardPointTotals();
        //AudioSource.PlayClipAtPoint(EarnedSound, Camera.main.transform.position);
    }

    public void AddProgressToAchievement(string achievementName, float progressAmount) {
        Achievement achievement = GetAchievementByName(achievementName);
        if (achievement == null) {
            Debug.LogWarning("AchievementManager::AddProgressToAchievement() - Trying to add progress to an achievemnet that doesn't exist: " + achievementName);
            return;
        }

        if (achievement.AddProgress(progressAmount)) {
            AchievementEarned();
        }
    }

    public void SetProgressToAchievement(string achievementName, float newProgress) {
        Achievement achievement = GetAchievementByName(achievementName);
        if (achievement == null) {
            Debug.LogWarning("AchievementManager::SetProgressToAchievement() - Trying to add progress to an achievemnet that doesn't exist: " + achievementName);
            return;
        }

        if (achievement.SetProgress(newProgress)) {
            AchievementEarned();
        }
    }

    /*
        New Utilities
    */
    public void initializeAchievements() {
        Achievement dayTripper = new Achievement();
        dayTripper.Name = "Day Tripper";
        dayTripper.Description = "Walk a total of 1 kilometer.";
        dayTripper.IconComplete = null;
        dayTripper.IconIncomplete = null;
        dayTripper.RewardPoints = 0;
        dayTripper.TargetProgress = 1000f;
        dayTripper.Secret = false;
        dayTripper.deSecrefies[0] = "Marathoner";
        dayTripper.deSecrefies[1] = "Mountaineer";
        Achievements.Add(dayTripper);

        Achievement marathoner = new Achievement();
        marathoner.Name = "Marathoner";
        marathoner.Description = "Walk an entire marathon... 42.19 kilometers!";
        marathoner.IconComplete = null;
        marathoner.IconIncomplete = null;
        marathoner.RewardPoints = 0;
        marathoner.TargetProgress = 42190f;
        marathoner.Secret = true;
        marathoner.deSecrefies[0] = "Seasoned Hiker";
        Achievements.Add(marathoner);

        Achievement seasonedHiker = new Achievement();
        seasonedHiker.Name = "Seasoned Hiker";
        seasonedHiker.Description = "Walk a total of 100 kilometers. That's the distance from Washington, DC to Chicago!";
        seasonedHiker.IconComplete = null;
        seasonedHiker.IconIncomplete = null;
        seasonedHiker.RewardPoints = 0;
        seasonedHiker.TargetProgress = 100000f;
        seasonedHiker.Secret = true;
        seasonedHiker.deSecrefies[0] = "Master Backpacker";
        Achievements.Add(seasonedHiker);

        Achievement masterBackpacker = new Achievement();
        masterBackpacker.Name = "Master Backpacker";
        masterBackpacker.Description = "Walk a total of 500 kilometers. That's the same as the distance between San Francisco and LA!";
        masterBackpacker.IconComplete = null;
        masterBackpacker.IconIncomplete = null;
        masterBackpacker.RewardPoints = 0;
        masterBackpacker.TargetProgress = 500000f;
        masterBackpacker.Secret = true;
        Achievements.Add(masterBackpacker);

        Achievement journeymanPhotographer = new Achievement();
        journeymanPhotographer.Name = "Journeyman Photographer";
        journeymanPhotographer.Description = "Post a photo to your blog.";
        journeymanPhotographer.IconComplete = null;
        journeymanPhotographer.IconIncomplete = null;
        journeymanPhotographer.RewardPoints = 0;
        journeymanPhotographer.TargetProgress = 1f;
        journeymanPhotographer.Secret = false;
        journeymanPhotographer.deSecrefies[0] = "Experienced Photographer";
        Achievements.Add(journeymanPhotographer);

        Achievement experiencedPhotographer = new Achievement();
        experiencedPhotographer.Name = "Experienced Photographer";
        experiencedPhotographer.Description = "Post 50 photos to your blog.";
        experiencedPhotographer.IconComplete = null;
        experiencedPhotographer.IconIncomplete = null;
        experiencedPhotographer.RewardPoints = 0;
        experiencedPhotographer.TargetProgress = 50f;
        experiencedPhotographer.Secret = true;
        experiencedPhotographer.deSecrefies[0] = "Expert Photographer";
        Achievements.Add(experiencedPhotographer);

        Achievement expertPhotographer = new Achievement();
        expertPhotographer.Name = "Expert Photographer";
        expertPhotographer.Description = "Post 200 photos to your blog";
        expertPhotographer.IconComplete = null;
        expertPhotographer.IconIncomplete = null;
        expertPhotographer.RewardPoints = 0;
        expertPhotographer.TargetProgress = 200f;
        expertPhotographer.Secret = true;
        Achievements.Add(expertPhotographer);

        Achievement balancedBreakfast = new Achievement();
        balancedBreakfast.Name = "Balanced Breakfast";
        balancedBreakfast.Description = "Post a perfectly balanced photo to your blog.";
        balancedBreakfast.IconComplete = null;
        balancedBreakfast.IconIncomplete = null;
        balancedBreakfast.RewardPoints = 0;
        balancedBreakfast.TargetProgress = 1f;
        balancedBreakfast.Secret = false;
        balancedBreakfast.deSecrefies[0] = "Perfection Incarnate";
        Achievements.Add(balancedBreakfast);

        Achievement theMostInterestingPhoto = new Achievement();
        theMostInterestingPhoto.Name = "The Most Interesting Photo in The World";
        theMostInterestingPhoto.Description = "Post a perfectly interesting photo to your blog.";
        theMostInterestingPhoto.IconComplete = null;
        theMostInterestingPhoto.IconIncomplete = null;
        theMostInterestingPhoto.RewardPoints = 0;
        theMostInterestingPhoto.TargetProgress = 1f;
        theMostInterestingPhoto.Secret = false;
        theMostInterestingPhoto.deSecrefies[0] = "Perfection Incarnate";
        Achievements.Add(theMostInterestingPhoto);

        Achievement theFinalFrontier = new Achievement();
        theFinalFrontier.Name = "The Final Frontier";
        theFinalFrontier.Description = "Post a perfectly well-spaced photo to your blog.";
        theFinalFrontier.IconComplete = null;
        theFinalFrontier.IconIncomplete = null;
        theFinalFrontier.RewardPoints = 0;
        theFinalFrontier.TargetProgress = 1f;
        theFinalFrontier.Secret = false;
        theFinalFrontier.deSecrefies[0] = "Perfection Incarnate";
        Achievements.Add(theFinalFrontier);

        Achievement perfectionIncarnate = new Achievement();
        perfectionIncarnate.Name = "Perfection Incarnate";
        perfectionIncarnate.Description = "Post an absolutely perfect photo to your blog.";
        perfectionIncarnate.IconComplete = null;
        perfectionIncarnate.IconIncomplete = null;
        perfectionIncarnate.RewardPoints = 0;
        perfectionIncarnate.TargetProgress = 1f;
        perfectionIncarnate.Secret = true;
        Achievements.Add(perfectionIncarnate);

        Achievement Telemachus = new Achievement();
        Telemachus.Name = "Telemachus";
        Telemachus.Description = "Post a good photo taken with a telephoto lens to your blog.";
        Telemachus.IconComplete = null;
        Telemachus.IconIncomplete = null;
        Telemachus.RewardPoints = 0;
        Telemachus.TargetProgress = 1f;
        Telemachus.Secret = false;
        Achievements.Add(Telemachus);

        Achievement WideAwake = new Achievement();
        WideAwake.Name = "Wide Awake";
        WideAwake.Description = "Post a good photo taken using a wide-angle lens to your blog.";
        WideAwake.IconComplete = null;
        WideAwake.IconIncomplete = null;
        WideAwake.RewardPoints = 0;
        WideAwake.TargetProgress = 1f;
        WideAwake.Secret = false;
        Achievements.Add(WideAwake);

        Achievement Hipstergram = new Achievement();
        Hipstergram.Name = "Hipstergram";
        Hipstergram.Description = "Post a good photo taking using a lens filter to your blog.";
        Hipstergram.IconComplete = null;
        Hipstergram.IconIncomplete = null;
        Hipstergram.RewardPoints = 0;
        Hipstergram.TargetProgress = 1f;
        Hipstergram.Secret = false;
        Achievements.Add(Hipstergram);

        Achievement OwlHaveWhatShesHaving = new Achievement();
        OwlHaveWhatShesHaving.Name = "Owl Have What She's Having";
        OwlHaveWhatShesHaving.Description = "Post a good photo containing an owl to your blog.";
        OwlHaveWhatShesHaving.IconComplete = null;
        OwlHaveWhatShesHaving.IconIncomplete = null;
        OwlHaveWhatShesHaving.RewardPoints = 0;
        OwlHaveWhatShesHaving.TargetProgress = 1f;
        OwlHaveWhatShesHaving.Secret = false;
        OwlHaveWhatShesHaving.deSecrefies[0] = "Say Cheese";
        Achievements.Add(OwlHaveWhatShesHaving);

        Achievement doe = new Achievement();
        doe.Name = "Doe, a Deer, a Female Deer";
        doe.Description = "Post a good photo containing a deer to your blog.";
        doe.IconComplete = null;
        doe.IconIncomplete = null;
        doe.RewardPoints = 0;
        doe.TargetProgress = 1f;
        doe.Secret = false;
        doe.deSecrefies[0] = "Say Cheese";
        Achievements.Add(doe);

        Achievement fox = new Achievement();
        fox.Name = "What the Fox";
        fox.Description = "Post a good photo containing a fox to your blog.";
        fox.IconComplete = null;
        fox.IconIncomplete = null;
        fox.RewardPoints = 0;
        fox.TargetProgress = 1f;
        fox.Secret = false;
        fox.deSecrefies[0] = "Say Cheese";
        Achievements.Add(fox);

        Achievement Mountaineer = new Achievement();
        Mountaineer.Name = "Mountaineer";
        Mountaineer.Description = "Reach the highest point in the park.";
        Mountaineer.IconComplete = null;
        Mountaineer.IconIncomplete = null;
        Mountaineer.RewardPoints = 0;
        Mountaineer.TargetProgress = 1f;
        Mountaineer.Secret = true;
        Achievements.Add(Mountaineer);

        Achievement Memento = new Achievement();
        Memento.Name = "Memento";
        Memento.Description = "Unlock the largest memory card.";
        Memento.IconComplete = null;
        Memento.IconIncomplete = null;
        Memento.RewardPoints = 0;
        Memento.TargetProgress = 1f;
        Memento.Secret = false;
        Achievements.Add(Memento);

        Achievement cheese = new Achievement();
        cheese.Name = "Say Cheese";
        cheese.Description = "Post a photo of an animal striking an unusual pose to your blog.";
        cheese.IconComplete = null;
        cheese.IconIncomplete = null;
        cheese.RewardPoints = 0;
        cheese.TargetProgress = 1f;
        cheese.Secret = true;
        Achievements.Add(cheese);
    }

    public void saveAchievements() {
        Debug.Log("Saving cheevos to " + savePath);
        BinaryFormatter binForm = new BinaryFormatter();
        string fullCheevoPath;

        foreach (Achievement cheevo in Achievements) {
            fullCheevoPath = savePath + cheevo.Name + ".cheevo";
            FileStream cheevoFile;
            if (File.Exists(fullCheevoPath)) {
                cheevoFile = File.Open(fullCheevoPath, FileMode.Open);
            } else cheevoFile = File.Create(fullCheevoPath);

            binForm.Serialize(cheevoFile, cheevo);
            cheevoFile.Close();
        }
    }

    public void saveAchievement(Achievement cheevo) {
        Debug.Log("Saving cheevo to " + savePath);
        BinaryFormatter binForm = new BinaryFormatter();
        string fullCheevoPath = savePath + cheevo.Name + ".cheevo";
        FileStream cheevoFile;
        if (File.Exists(fullCheevoPath)) {
            cheevoFile = File.Open(fullCheevoPath, FileMode.Open);
        } else cheevoFile = File.Create(fullCheevoPath);

        binForm.Serialize(cheevoFile, cheevo);
        cheevoFile.Close();
    }

    public void loadAchievements() {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/cheevos/");

        if (!dir.Exists) {
            dir.Create();
        }

        FileInfo[] info = dir.GetFiles("*.cheevo");
        BinaryFormatter binForm = new BinaryFormatter();
        string fullCheevoPath;

        // If info.Length is 0, then there is no cheevo data saved on disk, so we should set some up!
        if (info.Length == 0) {
            initializeAchievements();
        } else {
            foreach (FileInfo file in info) {
                fullCheevoPath = savePath + file.Name;
                FileStream cheevoFile = File.Open(fullCheevoPath, FileMode.Open);

                Achievement cheevo = (Achievement)binForm.Deserialize(cheevoFile);
                cheevoFile.Close();

                Achievements.Add(cheevo);
            }
        }
    }

    // Sets up a scrollview and fills it out with each Achievement.
    // Also displays the total number of reward points earned.
    void OnGUI() {
        float yValue = 5.0f;
        float achievementGUIWidth = 500.0f;

        GUI.Label(new Rect(200.0f, 5.0f, 200.0f, 25.0f), "-- Achievements --");

        achievementScrollviewLocation = GUI.BeginScrollView(new Rect(0.0f, 25.0f, achievementGUIWidth + 25.0f, 400.0f), achievementScrollviewLocation,
                                                            new Rect(0.0f, 0.0f, achievementGUIWidth, Achievements.Count() * 80.0f));

        foreach (Achievement achievement in Achievements) {
            Rect position = new Rect(5.0f, yValue, achievementGUIWidth, 75.0f);
            achievement.OnGUI(position, GUIStyleAchievementEarned, GUIStyleAchievementNotEarned,
                              complete, incomplete);
            yValue += 80.0f;
        }

        GUI.EndScrollView();

        GUI.Label(new Rect(10.0f, 440.0f, 200.0f, 25.0f), "Reward Points: [" + currentRewardPoints + " out of " + potentialRewardPoints + "]");
    }
}
