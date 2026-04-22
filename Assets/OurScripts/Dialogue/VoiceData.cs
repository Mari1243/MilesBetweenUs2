using UnityEngine;

[CreateAssetMenu(fileName = "VoiceData", menuName = "Dialogue/Character Voice")]
public class VoiceData : ScriptableObject
{
    [Tooltip("Must match the character name exactly as written in your Yarn script")]
    public string characterName;

    [Tooltip("Play a sound every N characters. 1 = every letter, 2 = every other, etc.")]
    public int frequency = 1;

    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    public AudioClip[] audioClips;
}