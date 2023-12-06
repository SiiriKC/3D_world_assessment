using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Dialogue _currentDialogue;

    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _statementText;
    [SerializeField] private Button _buttonPrefab;
    [SerializeField] private Transform _buttonsPanel;
    [SerializeField] private Transform _mainPanel;

    public AudioClip[] sounds;
    AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void LoadButtons(LineOfDialogue lineToLoad)
    {
        if (lineToLoad == null)
        {
            return;
        }

        for(int i = 0; i < lineToLoad.Responses.Length; i++)
        {
            string response = lineToLoad.Responses[i].statement;

            Button spawnedButton = Instantiate(_buttonPrefab, _buttonsPanel);
            spawnedButton.GetComponentInChildren<TMP_Text>().text = response;

            if (lineToLoad.Responses[i].nextLine == null)
            {
                spawnedButton.onClick.AddListener(() => EndConverastion());
                
            }
            else
            {
                int buttonID = i;
                spawnedButton.onClick.AddListener(() => ButtonPressed(buttonID));
                PlaySound(0);

            }
        }
    }

    private void PlaySound(int soundIndex)
    {
        if (soundIndex >= 0 && soundIndex < sounds.Length)
        {
            _audioSource.clip = sounds[soundIndex];
            _audioSource.Play();
        }

    }

        public void LoadText()
    {
        _nameText.text = _currentDialogue.nameOfCharacter;
        _statementText.text = _currentDialogue.CurrentLine().Statement;
    }

    public void LoadDialogue(Dialogue dialogue)
    {
        _mainPanel.gameObject.SetActive(true);
        _currentDialogue = dialogue;
        _currentDialogue.Reset();

        LoadText();

        ClearButtons();
        LoadButtons(_currentDialogue._StartLine());
        ChangeMouseVisible(true);
        
    }

    public void EndConverastion()
    {
        _mainPanel.gameObject.SetActive(false);
        ChangeMouseVisible(false);
        PlaySound(1);
    }

    public void ButtonPressed(int index)
    {
        ClearButtons();
        LineOfDialogue lod = _currentDialogue.NextLine(index);
        LoadButtons(lod);
        LoadText();
        PlaySound(0);
    }    

    private void ClearButtons()
    {
        for(int i = _buttonsPanel.childCount -1; i>=0; i--)
        {
            Transform child = _buttonsPanel.GetChild(i);

            Destroy(child.gameObject);
        }
    }

    public void ChangeMouseVisible(bool isVisible)
    {
        Cursor.visible = isVisible;

        if (isVisible)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
