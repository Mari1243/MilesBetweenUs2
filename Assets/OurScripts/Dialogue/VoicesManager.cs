using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using Yarn.Markup;
using Yarn.Unity;

public class VoicesManager : ActionMarkupHandler
{
    [Tooltip("All character voices. The handler picks the right one based on who's speaking.")]
    public List<VoiceData> characterVoices = new List<VoiceData>();

    [Tooltip("default voice used when no matching CharacterVoice is found for the speaker.")]
    public VoiceData defaultVoice;
    public AudioSource audioSource;
    private VoiceData currentVoice;
    private int characterCount;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
           
        // prevent the AudioSource from playing on awake or looping
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        print("VOICES MANAGER IS ACTIVE. ");
    }

    // Called at the start of each line, before any characters appear.
    // This is where we figure out who is speaking and load their voice.
    public override void OnPrepareForLine(MarkupParseResult line, TMP_Text text)
{
    characterCount = 0;
    currentVoice = defaultVoice; // fall back to default

    foreach (var attribute in line.Attributes)
    {
        if (attribute.Name == "character")
        {
            if (attribute.Properties.TryGetValue("name", out var nameProp))
            {
                string speakerName = nameProp.StringValue;
                // find a matching VoiceData by character name
                VoiceData found = characterVoices.Find(v => v.characterName == speakerName);
                if (found != null)
                {
                    currentVoice = found;
                }
                break;
            }
        }
    }
}

    // Called by BasicTypewriter every time a character is about to appear.
    // charIndex is the position in the string, line is the full markup result.
    public override async YarnTask OnCharacterWillAppear(int charIndex, MarkupParseResult line, CancellationToken token)
    {
        characterCount++;

        if (currentVoice == null) return;
        if (currentVoice.audioClips == null || currentVoice.audioClips.Length == 0) return;

        // frequency check: only play every Nth character
        if (characterCount % currentVoice.frequency != 0) return;

        // pick a random clip from the voice's clip array
        AudioClip clip = currentVoice.audioClips[Random.Range(0, currentVoice.audioClips.Length)];

        // randomise pitch within the voice's min/max range
        audioSource.pitch = Random.Range(currentVoice.minPitch, currentVoice.maxPitch);
        audioSource.PlayOneShot(clip);
    }
    //keep these empty!
    public override void OnLineDisplayBegin(MarkupParseResult line, TMP_Text text) { }
    public override void OnLineDisplayComplete() { }
    public override void OnLineWillDismiss() { }

}