using UnityEngine;
using UnityEngine.UI;

public class MusicSwitcher : MonoBehaviour
{
    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public AudioSource audioSource3;

    public Button switchButton1;
    public Button switchButton2;
    public Button switchButton3;

    private void Start()
    {
        // Assign button click listeners
        switchButton1.onClick.AddListener(ActivateAudioSource1);
        switchButton2.onClick.AddListener(ActivateAudioSource2);
        switchButton3.onClick.AddListener(ActivateAudioSource3);

        // Initially activate the first audio source
        ActivateAudioSource1();
    }

    private void ActivateAudioSource1()
    {
        audioSource1.gameObject.SetActive(true);
        audioSource2.gameObject.SetActive(false);
        audioSource3.gameObject.SetActive(false);
    }

    private void ActivateAudioSource2()
    {
        audioSource1.gameObject.SetActive(false);
        audioSource2.gameObject.SetActive(true);
        audioSource3.gameObject.SetActive(false);
    }

    private void ActivateAudioSource3()
    {
        audioSource1.gameObject.SetActive(false);
        audioSource2.gameObject.SetActive(false);
        audioSource3.gameObject.SetActive(true);
    }
}
