using UnityEngine;

public class Constants
{
    // Story
    public static string STORY_PATH = "Assets/StreamingAssets/";
    public static string DEFAULT_STORY_FILE_NAME = "StoryScript";
    public static string EXCEL_FILE_EXTENSION = ".xlsx";
    public static int DEFAULT_START_LINE = 1;

    // Choice
    public static string END_OF_STORY = "End of story";
    public static string CHOICE = "Choice";

    // TypewriterEffect
    public static float DEFAULT_TYPING_SECONDS = 0.5f; // 0.05f

    // Dialogue
    public static string AVATAR_PATH = "Avatar/";
    public static string VOCAL_PATH = "Audio/Vocal/";

    // Background
    public static string BACKGROUND_PATH = "Background/";
    public static string MUSIC_PATH = "Audio/BGM/";

    // Character
    public static string CHARACTER_PATH = "Character/";

    // AutoPlay
    public static string BUTTON_PATH = "Button/";
    public static string AUTO_ON = "autoPlayOn";
    public static string AUTO_OFF = "autoPlayOff";
    public static float AUTOPLAY_WAITING_SECONDS = 1.0f;

    // Skip
    public static string SKIP_ON = "skipOn";
    public static string SKIP_OFF = "skipOff";
    public static float SKIP_MODE_TYPING_SPEED = 0.02f;
    public static float DEFAULT_SKIP_SECONDS = 0.05f;

    // Action
    public static string Appear_At = "appearAt";
    public static string Disappear = "disappear";
    public static string Move_To = "moveTo";
    public static int DURATION_TIME = 1;

    // Save & Load
    public static int SAVE_SLOTS = 8;
    public static int TOTAL_SLOTS = 8;
    public static string COLON = ": ";
    public static string SAVE_GAME = "save_game";
    public static string LOAD_GAME = "load_game";
    public static string EMPTY_SLOT = "empty_slot";
    public static string SAVE_FILE_PATH = "saves";
    public static string SAVE_FILE_EXTENSION = ".json";

    // Error
    public static string AUDIO_LOAD_FAILED = "Failed to load audio";
    public static string IMAGE_LOAD_FAILED = "Failed to load image";
    public static string NO_DATA_FOUND = "No data found";
    public static string COORDINATE_MISSING = "Coordinate missing";
    public static string CAMERA_NOT_FOUND = "Camera not found";
}
