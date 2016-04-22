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
public class SerializeableCheevoData {
    public string Name;
    public string Description;
    public string IconIncomplete;
    public string IconComplete;
    public int RewardPoints;
    public float TargetProgress;
    public bool Secret;
    public string[] deSecrefies = new string[10];
    public bool Earned = false;
    public float currentProgress = 0.0f;
}

public class Achievement {
    public SerializeableCheevoData data;

    // Returns true if this progress added results in the Achievement being earned.
    public bool AddProgress(float progress) {
        if (data.Earned) {
            return false;
        }

        data.currentProgress += progress;
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

        data.currentProgress = progress;
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
            GUI.Label(new Rect(80.0f, 25.0f, position.width - 80.0f, 25.0f), "Description Hidden!", style);
            GUI.Label(new Rect(position.width - 50.0f, 5.0f, 25.0f, 25.0f), "???", style);
            GUI.Label(new Rect(position.width - 250.0f, 50.0f, 250.0f, 25.0f), "Progress Hidden!", style);
        } else {
            GUI.Label(new Rect(80.0f, 25.0f, position.width - 80.0f, 25.0f), data.Description, style);
            GUI.Label(new Rect(position.width - 50.0f, 5.0f, 25.0f, 25.0f), data.RewardPoints.ToString(), style);
            GUI.Label(new Rect(position.width - 250.0f, 50.0f, 250.0f, 25.0f), "Progress: [" + data.currentProgress.ToString("0.#") + " out of " + data.TargetProgress.ToString("0.#") + "]", style);
        }

        GUI.EndGroup();
    }
}

public class AchievementManager : MonoBehaviour {
    public List<Achievement> Achievements;
    public AudioClip EarnedSound;
    public GUIStyle GUIStyleAchievementEarned;
    public GUIStyle GUIStyleAchievementNotEarned;
    public Texture2D complete;
    public Texture2D incomplete;

    private int currentRewardPoints = 0;
    private int potentialRewardPoints = 0;
    private string savePath;

    void Start() {
        savePath = Application.persistentDataPath + "/cheevos/";

        //Debug.Log(savePath);

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
        dayTripper.data = new SerializeableCheevoData();
        dayTripper.data.Name = "Day Tripper";
        dayTripper.data.Description = "Walk a total of 1 kilometer.";
        dayTripper.data.IconComplete = null;
        dayTripper.data.IconIncomplete = null;
        dayTripper.data.RewardPoints = 0;
        dayTripper.data.TargetProgress = 1000f;
        dayTripper.data.Secret = false;
        dayTripper.data.deSecrefies[0] = "Marathoner";
        dayTripper.data.deSecrefies[1] = "Mountaineer";
        Achievements.Add(dayTripper);

        Achievement marathoner = new Achievement();
        marathoner.data = new SerializeableCheevoData();
        marathoner.data.Name = "Marathoner";
        marathoner.data.Description = "Walk an entire marathon... 42.19 kilometers!";
        marathoner.data.IconComplete = null;
        marathoner.data.IconIncomplete = null;
        marathoner.data.RewardPoints = 0;
        marathoner.data.TargetProgress = 42190f;
        marathoner.data.Secret = true;
        marathoner.data.deSecrefies[0] = "Seasoned Hiker";
        Achievements.Add(marathoner);

        Achievement seasonedHiker = new Achievement();
        seasonedHiker.data.Name = "Seasoned Hiker";
        seasonedHiker.data.Description = "Walk a total of 100 kilometers. That's the distance from Washington, DC to Chicago!";
        seasonedHiker.data.IconComplete = null;
        seasonedHiker.data.IconIncomplete = null;
        seasonedHiker.data.RewardPoints = 0;
        seasonedHiker.data.TargetProgress = 100000f;
        seasonedHiker.data.Secret = true;
        seasonedHiker.data.deSecrefies[0] = "Master Backpacker";
        Achievements.Add(seasonedHiker);

        Achievement masterBackpacker = new Achievement();
        masterBackpacker.data.Name = "Master Backpacker";
        masterBackpacker.data.Description = "Walk a total of 500 kilometers. That's the same as the distance between San Francisco and LA!";
        masterBackpacker.data.IconComplete = null;
        masterBackpacker.data.IconIncomplete = null;
        masterBackpacker.data.RewardPoints = 0;
        masterBackpacker.data.TargetProgress = 500000f;
        masterBackpacker.data.Secret = true;
        Achievements.Add(masterBackpacker);

        Achievement journeymanPhotographer = new Achievement();
        journeymanPhotographer.data.Name = "Journeyman Photographer";
        journeymanPhotographer.data.Description = "Post a photo to your blog.";
        journeymanPhotographer.data.IconComplete = null;
        journeymanPhotographer.data.IconIncomplete = null;
        journeymanPhotographer.data.RewardPoints = 0;
        journeymanPhotographer.data.TargetProgress = 1f;
        journeymanPhotographer.data.Secret = false;
        journeymanPhotographer.data.deSecrefies[0] = "Experienced Photographer";
        Achievements.Add(journeymanPhotographer);

        Achievement experiencedPhotographer = new Achievement();
        experiencedPhotographer.data.Name = "Experienced Photographer";
        experiencedPhotographer.data.Description = "Post 50 photos to your blog.";
        experiencedPhotographer.data.IconComplete = null;
        experiencedPhotographer.data.IconIncomplete = null;
        experiencedPhotographer.data.RewardPoints = 0;
        experiencedPhotographer.data.TargetProgress = 50f;
        experiencedPhotographer.data.Secret = true;
        experiencedPhotographer.data.deSecrefies[0] = "Expert Photographer";
        Achievements.Add(experiencedPhotographer);

        Achievement expertPhotographer = new Achievement();
        expertPhotographer.data.Name = "Expert Photographer";
        expertPhotographer.data.Description = "Post 200 photos to your blog";
        expertPhotographer.data.IconComplete = null;
        expertPhotographer.data.IconIncomplete = null;
        expertPhotographer.data.RewardPoints = 0;
        expertPhotographer.data.TargetProgress = 200f;
        expertPhotographer.data.Secret = true;
        Achievements.Add(expertPhotographer);

        Achievement balancedBreakfast = new Achievement();
        balancedBreakfast.data.Name = "Balanced Breakfast";
        balancedBreakfast.data.Description = "Post a perfectly balanced photo to your blog.";
        balancedBreakfast.data.IconComplete = null;
        balancedBreakfast.data.IconIncomplete = null;
        balancedBreakfast.data.RewardPoints = 0;
        balancedBreakfast.data.TargetProgress = 1f;
        balancedBreakfast.data.Secret = false;
        balancedBreakfast.data.deSecrefies[0] = "Perfection Incarnate";
        Achievements.Add(balancedBreakfast);

        Achievement theMostInterestingPhoto = new Achievement();
        theMostInterestingPhoto.data.Name = "The Most Interesting Photo in The World";
        theMostInterestingPhoto.data.Description = "Post a perfectly interesting photo to your blog.";
        theMostInterestingPhoto.data.IconComplete = null;
        theMostInterestingPhoto.data.IconIncomplete = null;
        theMostInterestingPhoto.data.RewardPoints = 0;
        theMostInterestingPhoto.data.TargetProgress = 1f;
        theMostInterestingPhoto.data.Secret = false;
        theMostInterestingPhoto.data.deSecrefies[0] = "Perfection Incarnate";
        Achievements.Add(theMostInterestingPhoto);

        Achievement theFinalFrontier = new Achievement();
        theFinalFrontier.data.Name = "The Final Frontier";
        theFinalFrontier.data.Description = "Post a perfectly well-spaced photo to your blog.";
        theFinalFrontier.data.IconComplete = null;
        theFinalFrontier.data.IconIncomplete = null;
        theFinalFrontier.data.RewardPoints = 0;
        theFinalFrontier.data.TargetProgress = 1f;
        theFinalFrontier.data.Secret = false;
        theFinalFrontier.data.deSecrefies[0] = "Perfection Incarnate";
        Achievements.Add(theFinalFrontier);

        Achievement perfectionIncarnate = new Achievement();
        perfectionIncarnate.data.Name = "Perfection Incarnate";
        perfectionIncarnate.data.Description = "Post an absolutely perfect photo to your blog.";
        perfectionIncarnate.data.IconComplete = null;
        perfectionIncarnate.data.IconIncomplete = null;
        perfectionIncarnate.data.RewardPoints = 0;
        perfectionIncarnate.data.TargetProgress = 1f;
        perfectionIncarnate.data.Secret = true;
        Achievements.Add(perfectionIncarnate);

        Achievement Telemachus = new Achievement();
        Telemachus.data.Name = "Telemachus";
        Telemachus.data.Description = "Post a good photo taken with a telephoto lens to your blog.";
        Telemachus.data.IconComplete = null;
        Telemachus.data.IconIncomplete = null;
        Telemachus.data.RewardPoints = 0;
        Telemachus.data.TargetProgress = 1f;
        Telemachus.data.Secret = false;
        Achievements.Add(Telemachus);

        Achievement WideAwake = new Achievement();
        WideAwake.data.Name = "Wide Awake";
        WideAwake.data.Description = "Post a good photo taken using a wide-angle lens to your blog.";
        WideAwake.data.IconComplete = null;
        WideAwake.data.IconIncomplete = null;
        WideAwake.data.RewardPoints = 0;
        WideAwake.data.TargetProgress = 1f;
        WideAwake.data.Secret = false;
        Achievements.Add(WideAwake);

        Achievement Hipstergram = new Achievement();
        Hipstergram.data.Name = "Hipstergram";
        Hipstergram.data.Description = "Post a good photo taking using a lens filter to your blog.";
        Hipstergram.data.IconComplete = null;
        Hipstergram.data.IconIncomplete = null;
        Hipstergram.data.RewardPoints = 0;
        Hipstergram.data.TargetProgress = 1f;
        Hipstergram.data.Secret = false;
        Achievements.Add(Hipstergram);

        Achievement OwlHaveWhatShesHaving = new Achievement();
        OwlHaveWhatShesHaving.data.Name = "Owl Have What She's Having";
        OwlHaveWhatShesHaving.data.Description = "Post a good photo containing an owl to your blog.";
        OwlHaveWhatShesHaving.data.IconComplete = null;
        OwlHaveWhatShesHaving.data.IconIncomplete = null;
        OwlHaveWhatShesHaving.data.RewardPoints = 0;
        OwlHaveWhatShesHaving.data.TargetProgress = 1f;
        OwlHaveWhatShesHaving.data.Secret = false;
        OwlHaveWhatShesHaving.data.deSecrefies[0] = "Say Cheese";
        Achievements.Add(OwlHaveWhatShesHaving);

        Achievement doe = new Achievement();
        doe.data.Name = "Doe, a Deer, a Female Deer";
        doe.data.Description = "Post a good photo containing a deer to your blog.";
        doe.data.IconComplete = null;
        doe.data.IconIncomplete = null;
        doe.data.RewardPoints = 0;
        doe.data.TargetProgress = 1f;
        doe.data.Secret = false;
        doe.data.deSecrefies[0] = "Say Cheese";
        Achievements.Add(doe);

        Achievement fox = new Achievement();
        fox.data.Name = "What the Fox";
        fox.data.Description = "Post a good photo containing a fox to your blog.";
        fox.data.IconComplete = null;
        fox.data.IconIncomplete = null;
        fox.data.RewardPoints = 0;
        fox.data.TargetProgress = 1f;
        fox.data.Secret = false;
        fox.data.deSecrefies[0] = "Say Cheese";
        Achievements.Add(fox);

        Achievement Mountaineer = new Achievement();
        Mountaineer.data.Name = "Mountaineer";
        Mountaineer.data.Description = "Reach the highest point in the park.";
        Mountaineer.data.IconComplete = null;
        Mountaineer.data.IconIncomplete = null;
        Mountaineer.data.RewardPoints = 0;
        Mountaineer.data.TargetProgress = 1f;
        Mountaineer.data.Secret = true;
        Achievements.Add(Mountaineer);

        Achievement Memento = new Achievement();
        Memento.data.Name = "Memento";
        Memento.data.Description = "Unlock the largest memory card.";
        Memento.data.IconComplete = null;
        Memento.data.IconIncomplete = null;
        Memento.data.RewardPoints = 0;
        Memento.data.TargetProgress = 1f;
        Memento.data.Secret = false;
        Achievements.Add(Memento);

        Achievement cheese = new Achievement();
        cheese.data.Name = "Say Cheese";
        cheese.data.Description = "Post a photo of an animal striking an unusual pose to your blog.";
        cheese.data.IconComplete = null;
        cheese.data.IconIncomplete = null;
        cheese.data.RewardPoints = 0;
        cheese.data.TargetProgress = 1f;
        cheese.data.Secret = true;
        Achievements.Add(cheese);
    }

    public void saveAchievements() {
        Debug.Log("Saving cheevos to " + savePath);
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
        Debug.Log("Saving cheevo to " + savePath);
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

                Achievement cheevo = new Achievement();
                SerializeableCheevoData cheevoData = (SerializeableCheevoData)binForm.Deserialize(cheevoFile);
                cheevo.data = cheevoData;

                cheevoFile.Close();
                Achievements.Add(cheevo);
            }
        }
    }
}
