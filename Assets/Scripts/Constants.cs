using UnityEngine;

public class Constants
{
    // Story
    public static string STORY_PATH = "Assets/StreamingAssets/";
    public static string DEFAULT_STORY_FILE_NAME = "StoryScript";
    public static string EXCEL_FILE_EXTENSION = ".xlsx";
    public static int DEFAULT_START_LINE = 1;

    // Action
    public static string END_OF_STORY = "End of story";
    public static string END_OF_GAME = "End of game";
    public static string CHOICE = "Choice";
    public static string APPEAR_AT = "appearAt";
    public static string APPEAR_AT_INSTANTLY = "appearAtInstantly";
    public static string DISAPPEAR = "disappear";
    public static string MOVE_TO = "moveTo";
    public static string GOTO = "Goto";
    public static int DURATION_TIME = 1;
    public static string INVESTIGATE = "Investigate"; // R
    public static string INVESTIGATE2 = "Investigate2";
    public static string INVESTIGATE3 = "Investigate3";

    // TypewriterEffect
    public static float DEFAULT_TYPING_SECONDS = 0.05f; // 0.05f

    // Dialogue
    public static string AVATAR_PATH = "Avatar/";
    public static string VOCAL_PATH = "Audio/Vocal/";

    // Background
    public static string BACKGROUND_PATH = "Background/";
    public static string MUSIC_PATH = "Audio/BGM/";
    public static string SOUND_EFFECT_PATH = "Audio/SoundEffect/";

    // Character
    public static string CHARACTER_PATH = "Character/";

    // History
    public static string HISTORY_PATH = "History/";

    // AutoPlay
    public static string BUTTON_PATH = "Button/";
    public static string AUTO_ON = "autoPlayOn";
    public static string AUTO_OFF = "autoPlayOff";
    public static float AUTOPLAY_WAITING_SECONDS = 1.0f;

    // Skip
    public static string SKIP_ON = "skipOn";
    public static string SKIP_OFF = "skipOff";
    public static float SKIP_MODE_TYPING_SPEED = 0.02f;
    public static float DEFAULT_SKIP_WAITING_SECONDS = 0.05f;

    // Save & Load
    public static int SAVE_SLOTS = 8;
    public static int TOTAL_SLOTS = 8;
    public static string COLON = ": ";
    public static string SAVE_GAME = "Save Game";
    public static string LOAD_GAME = "Load Game";
    public static string EMPTY_SLOT = "Empty_Slot";
    public static string SAVE_FILE_PATH = "saves";
    public static string SAVE_FILE_EXTENSION = ".json";

    // Gallery
    public static int MAX_LENGTH = 50;
    public static int GALLERY_SLOTS_PER_PAGE = 9;
    public static string GALLERY = "Knowledge Gallery";
    public static string GALLERY_PLACEHOLDER = "Gallery_Placeholder"; // 占位
    public static string SAVE_PLACEHOLDER = "Save_Placeholder"; // 占位
    public static int DEFAULT_START_INDEX = 0;
    public static readonly string[] ALL_HISTORY_IMAGES = { "History1", "History2", "History3", "History4", "History5", "History6", "History7", "History8", "History9", "History10", "History11", "History12", "History13", "History14", "History15", "History16", "History17", "History18", "History19", "History20" };
    public static string UNLOCKED = "Unlocked";
    public static string THUMBNAIL_PATH = "Thumbnail/";
    public static string BIG_HISTORY_PATH = "BigHistory/";
    public static string BIG_IMAGE_LOAD_FAILED = "Failed to load big image";

    // Error
    public static string AUDIO_LOAD_FAILED = "Failed to load audio";
    public static string IMAGE_LOAD_FAILED = "Failed to load image";
    public static string NO_DATA_FOUND = "No data found";
    public static string COORDINATE_MISSING = "Coordinate missing";
    public static string CAMERA_NOT_FOUND = "Camera not found";
}
