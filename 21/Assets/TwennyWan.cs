using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TwennyWan : MonoBehaviour {

    public KMSelectable[] buttons;
    public KMBombModule module;
    public KMAudio Audio;
    public KMSelectable DisplayTwennyWan;

    public GameObject display;
    public GameObject screenTextGameObject;
    public TextMesh screenText;
    public TextMesh basethingsubmitwhatever;

    private static int moduleIdCounter = 1;
    private int moduleId;

    private int number;
    private string numberin21;
    private string numberInBinary;

    private bool buttonMoving = false;
    private bool holding = false;
    private Coroutine holdCoro;

    private bool submitting = false;
    private bool solved = false;

    int PowerForSubmittingTwennyWan = 1;
    int submissionnumber = 0;

    void Awake()
    {
        moduleId = moduleIdCounter++;

        buttons[0].OnInteract += delegate { ButtonPressed(0); return false; };
        buttons[1].OnInteract += delegate { ButtonPressed(1); return false; };

        DisplayTwennyWan.OnInteract += delegate { DisplayPress(); return false; };

        buttons[0].OnInteractEnded += delegate { ButtonReleased(0); };
        buttons[1].OnInteractEnded += delegate { ButtonReleased(1); };

    }

	void Start() {
        GenerateNumber();
    }

    void DisplayPress(){
      DisplayTwennyWan.AddInteractionPunch();
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, DisplayTwennyWan.transform);
      PowerForSubmittingTwennyWan += 1;
      if (PowerForSubmittingTwennyWan > 4) {
        PowerForSubmittingTwennyWan = 1;
      }
      basethingsubmitwhatever.text = PowerForSubmittingTwennyWan.ToString();
    }

    void ButtonPressed(int buttonNum) {
        StartCoroutine(ButtonMove(buttonNum, "down"));
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, buttons[buttonNum].transform);
        buttons[buttonNum].AddInteractionPunch();
        if (solved || submitting) return;

        if (holdCoro != null)
        {
            holding = false;
            StopCoroutine(holdCoro);
            holdCoro = null;
        }

        holdCoro = StartCoroutine(HoldChecker());
    }

    IEnumerator HoldChecker() {
        yield return new WaitForSeconds(.6f);
        holding = true;
        screenText.text = "";
    }

    void ButtonReleased(int buttonNum) {
        StartCoroutine(ButtonMove(buttonNum, "up"));
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonRelease, buttons[buttonNum].transform);
        StopCoroutine(holdCoro);

        if (solved || submitting) return;

        if (holding)
        {
            if (buttonNum == 0)
            {
                StartCoroutine(StartGlitchySubmit());
            }
            else
            {
                submissionnumber = 0;
                screenText.text = numberin21;
            }
        }
        else
        {
          if (buttons[buttonNum] == buttons[0]) {
            submissionnumber += (int)Math.Pow(9, PowerForSubmittingTwennyWan);
          }
          else {
            submissionnumber += (int)Math.Pow(10, PowerForSubmittingTwennyWan);
          }
          screenText.text = submissionnumber.ToString();
        }
    }

    IEnumerator ButtonMove(int buttonNum, string direction) {
        yield return new WaitUntil(() => !buttonMoving);
        buttonMoving = true;

        var buttonToMove = buttons[buttonNum].transform;
        var x = buttonToMove.localPosition.x;
        var z = buttonToMove.localPosition.z;

        switch (direction) {
            case "down":
                buttonToMove.localPosition = new Vector3(x, 0.009f, z);
                break;
            case "up":
                buttonToMove.localPosition = new Vector3(x, 0.01f, z);
                yield return new WaitForSeconds(.01f);
                buttonToMove.localPosition = new Vector3(x, 0.012f, z);
                yield return new WaitForSeconds(.01f);
                buttonToMove.localPosition = new Vector3(x, 0.014f, z);
                yield return new WaitForSeconds(.01f);
                buttonToMove.localPosition = new Vector3(x, 0.016f, z);
                break;
        }

        buttonMoving = false;
    }

    void GenerateNumber() {
        number = Random.Range(9261, 194480);
        numberin21 = DecimalToArbitrarySystem(number, 21);
        numberInBinary = DecimalToArbitrarySystem(number, 10);

        screenText.text = numberin21;

        Debug.LogFormat("[21 #{0}] The displayed number is {1}, which is {2} in decimal.", moduleId, numberin21, number);
    }

    public static string DecimalToArbitrarySystem(long decimalNumber, int radix) {
        const int BitsInLong = 21;
        string Digits = radix == 21 ? "0123456789ABCDEFGHIJK" : "0123456789";

        if (radix < 2 || radix > Digits.Length)
            throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

        if (decimalNumber == 0)
            return "0";

        int index = BitsInLong - 1;
        long currentNumber = Math.Abs(decimalNumber);
        char[] charArray = new char[BitsInLong];

        while (currentNumber != 0)
        {
            int remainder = (int)(currentNumber % radix);
            charArray[index--] = Digits[remainder];
            currentNumber = currentNumber / radix;
        }

        string result = new String(charArray, index + 1, BitsInLong - index - 1);
        if (decimalNumber < 0)
        {
            result = "-" + result;
        }

        return result;
    }

    IEnumerator StartGlitchySubmit() {
      submitting = true;
      Audio.PlaySoundAtTransform("nineplusten", transform);
      StartCoroutine(TextGlitch());
      StartCoroutine(CycleNumbers());
      yield return new WaitForSeconds(2.5f);
      submitting = false;
      StartCoroutine(ANSWERCONFIRMCONFIRMANSWER());
    }

    IEnumerator ANSWERCONFIRMCONFIRMANSWER(){
      StopAllCoroutines();
      screenText.transform.localPosition = new Vector3(0, 0, .0011f);
      if (number == submissionnumber) {
        GetComponent<KMBombModule>().HandlePass();
        Debug.LogFormat("[21 #{0}] You submitted the right number! Twenny Wan!", moduleId);
        screenText.text = "Twenny One!";
        basethingsubmitwhatever.text = "21";
        screenText.transform.localScale = new Vector3(0.00000341364f, 0.0000229182f, 69f);
        Audio.PlaySoundAtTransform("twennyone", transform);
      }
      else {
        GetComponent<KMBombModule>().HandleStrike();
        basethingsubmitwhatever.text = "1";
        PowerForSubmittingTwennyWan = 1;
        Debug.LogFormat("[21 #{0}] You submitted {1}, Twenny Nan!", moduleId, submissionnumber);
        StartCoroutine(StrikeAnimation());
      }
      yield return null;
    }

    IEnumerator StrikeAnimation() {
      submitting = true;
      Audio.PlaySoundAtTransform("youstupid", transform);
      StartCoroutine(DoCrazyStrikeStuff());
      yield return new WaitForSeconds(1.352f);
      submissionnumber = 0;
      submitting = false;
    }

    IEnumerator DoCrazyStrikeStuff()
    {
        List<GameObject> instantiatedText = new List<GameObject>();
        var possibleText = "x!*?-/";

        screenText.text = "";

        while (submitting)
        {
            var x = Random.Range(0f, .0008f);
            var y = Random.Range(0f, .0005f);

            x = Random.Range(0, 2) == 0 ? x *= -1 : x;
            y = Random.Range(0, 2) == 0 ? y *= -1 : y;

            var newTextMesh = Instantiate(screenTextGameObject, display.transform);
            instantiatedText.Add(newTextMesh);

            newTextMesh.transform.localPosition = new Vector3(x, y, .0011f);
            newTextMesh.GetComponent<TextMesh>().color = Color.red;
            newTextMesh.GetComponent<TextMesh>().text = possibleText[Random.Range(0, possibleText.Length)].ToString();
            yield return new WaitForSeconds(.05f);
        }

        foreach (GameObject obj in instantiatedText)
        {
            Destroy(obj);
        }

        screenText.transform.localPosition = new Vector3(0, 0, .0011f);
    }

    IEnumerator TextGlitch() {
        float maxAmountX = 0;

        var amountX = 0f;
        var amountY = 0f;
        var maxAmountY = .0005f;

        while (submitting)
        {
            // Find the amount the message can move using the amount of 'W's.
            switch (numberin21.Where(ix => "Ww".Contains(ix.ToString())).Count())
            {
                case 0:
                    maxAmountX = .0003f;
                    break;
                case 1:
                    maxAmountX = .0002f;
                    break;
                case 2:
                    maxAmountX = .00015f;
                    break;
                case 3:
                    maxAmountX = .0001f;
                    break;
                case 4:
                    maxAmountX = .0003f;
                    break;
            }

            // Adds some to the amount if there are smaller characters.
            switch (numberin21.Where(ix => "iljt1I/".Contains(ix.ToString())).Count())
            {
                case 0:
                    maxAmountX += 0f;
                    break;
                case 1:
                    maxAmountX += .00015f;
                    break;
                case 2:
                    maxAmountX += .0002f;
                    break;
                case 3:
                    maxAmountX = .0002f;
                    break;
                case 4:
                    maxAmountX = .00025f;
                    break;
            }

            var x = Random.Range(0, amountX);
            var y = Random.Range(0, amountY);
            screenText.transform.localPosition = new Vector3(x, y, .0011f);

            // Only increment if the coordinates are smaller than its max.
            amountX = amountX >= maxAmountX ? amountX : amountX + .0001f / 5f;
            amountY = amountY >= maxAmountY ? amountY : amountY + .0001f / 5f;

            // Randomly multiply by -1.
            amountX = Random.Range(0, 2) == 0 ? amountX *= -1 : amountX;
            amountY = Random.Range(0, 2) == 0 ? amountY *= -1 : amountY;

            yield return new WaitForSeconds(.0001f);
        }
    }

    IEnumerator CycleNumbers() {
        while (submitting)
        {
            var tempNumber = Random.Range(0, 194480);
            var tempNumberIn64 = DecimalToArbitrarySystem(tempNumber, 21);
            screenText.text = tempNumberIn64;

            yield return new WaitForSeconds(.0001f);
        }
    }
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} 9/10/display to press that corresponding button. Use !{0} reset/submit to do the corresponding action. Chain using spaces.";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command) {
        command = command.Trim();
        string[] parameters = command.Split(' ');
        for (int i = 0; i < parameters.Length; i++) {
          if (parameters[i].ToString().ToLower() != "9" && parameters[i].ToString().ToLower() != "10" && parameters[i].ToString().ToLower() != "submit" && parameters[i].ToString().ToLower() != "display" && parameters[i].ToString().ToLower() != "reset") {
            yield return "sendtochaterror Invalid command!";
            yield break;
          }
          if (i != parameters.Length - 1 && parameters[i].ToString().ToLower() == "submit") {
            yield return "sendtochaterror You can't submit mid-command!";
            yield break;
          }
          if (i != parameters.Length - 1 && parameters[i].ToString().ToLower() == "reset") {
            yield return "sendtochaterror You can't reset mid-command!";
            yield break;
          }
          if (parameters[i].ToString().ToLower() == "9") {
            buttons[0].OnInteract();
            buttons[0].OnInteractEnded();
            yield return new WaitForSeconds(.1f);
          }
          else if (parameters[i].ToString().ToLower() == "10") {
            buttons[1].OnInteract();
            buttons[1].OnInteractEnded();
            yield return new WaitForSeconds(.1f);
          }
          else if (parameters[i].ToString().ToLower() == "display") {
            DisplayPress();
            yield return new WaitForSeconds(.1f);
          }
          else if (parameters[i].ToString().ToLower() == "submit") {
            yield return "trycancel";
            yield return "solve";
            yield return "strike";
            buttons[0].OnInteract();
            yield return new WaitForSeconds(1f);
            buttons[0].OnInteractEnded();
          }
          else if (parameters[i].ToString().ToLower() == "reset") {
            yield return "trycancel";
            buttons[1].OnInteract();
            yield return new WaitForSeconds(1f);
            buttons[1].OnInteractEnded();
          }
        }
    }
}
