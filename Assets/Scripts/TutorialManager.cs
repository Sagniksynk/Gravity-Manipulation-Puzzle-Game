using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject tutorialPanel;
    public TMP_Text instructionText;

    [Header("Settings")]
    public float stepDelay = 1.5f;

    private int currentStep = 0;
    private bool isStepActive = false;

    void Start()
    {
        StartCoroutine(TutorialSequence());
    }

    void Update()
    {
        if (!isStepActive) return;

        bool holdingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        switch (currentStep)
        {
            case 0: // Look Around
                if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0.5f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.5f)
                {
                    NextStep();
                }
                break;

            case 1: // Movement
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                {
                    NextStep();
                }
                break;

            case 2: // Jump
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    NextStep();
                }
                break;

            case 3: // Basic Walls (Arrow Keys)
                if (!holdingShift && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
                {
                    NextStep();
                }
                break;

            case 4:// Execute (Enter)
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    NextStep();
                }
                break;
        }
    }

    void NextStep()
    {
        isStepActive = false;
        currentStep++;
        StartCoroutine(TutorialSequence());
    }

    IEnumerator TutorialSequence()
    {
        yield return new WaitForSeconds(0.5f);

        switch (currentStep)
        {
            case 0:
                ShowInstruction("Move the Mouse to look around.");
                break;
            case 1:
                ShowInstruction("Use W, A, S, D to move.");
                break;
            case 2:
                ShowInstruction("Press SPACE to Jump.");
                break;
            case 3:
                ShowInstruction("Press ARROW KEYS to select Wall Gravity.\nHold SHIFT + UP ARROW to select Ceiling Gravity.");
                break;
            case 4:
                ShowInstruction("Press ENTER to switch gravity.");
                break;
            case 5:
                CompleteTutorial();
                yield break;
        }

        isStepActive = true;
    }

    void ShowInstruction(string text)
    {
        tutorialPanel.SetActive(true);
        instructionText.text = text;
    }

    void CompleteTutorial()
    {
        tutorialPanel.SetActive(false);
        Debug.Log("Tutorial Completed!");
    }
}