using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    public event Action OnDialogFinished;

    [SerializeField] private Speaker[] speakers; // 대화에 참여하는 캐릭터들의 UI 배열
    [SerializeField] private DialogData[] dialogs; // 현재 분기의 대사 목록 배열
    [SerializeField] private float typingSpeed = 0.1f; // 텍스트 타이핑 효과의 재생 속도

    [SerializeField] private int currentDialogIndex = -1; // 현재 대사 순번
    [SerializeField] private int currentSpeakerIndex = 0; // 현재 말을 하는 화자(Speaker)의 speakers 배열 순번
    [SerializeField] private bool isTypingEffect = false;
    [SerializeField] private bool isDialogCompleted = false;

    private Coroutine typingCoroutine;

    private void Start()
    {
        Setup();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 텍스트 타이핑 효과를 재생중일때 마우스 왼쪽 클릭하면 타이핑 효과 종료
            if (isTypingEffect)
            {
                isTypingEffect = false;
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                }
                speakers[currentSpeakerIndex].textDialogue.text = dialogs[currentDialogIndex].dialogue;
            }
        }
    }

    public void StartDialog()
    {
        Setup();
        isDialogCompleted = false;
        StartCoroutine(DisplayText());
    }

    // 모든 대화 관련 게임오브젝트 비활성화
    private void Setup()
    {
        foreach (Speaker speaker in speakers)
        {
            SetActiveObjects(speaker, false);
        }
    }

    // 배열만큼 반복 실행
    private IEnumerator DisplayText()
    {
        while (currentDialogIndex < dialogs.Length - 1)
        {
            while (isTypingEffect)
            {
                yield return null;
            }
            SetNextDialog();
        }

        // 마지막 대사를 출력한 후 isTypingEffect가 false가 되면 isDialogCompleted를 true로 설정
        while (isTypingEffect)
        {
            yield return null;
        }

        isDialogCompleted = true;
        OnDialogFinished?.Invoke();
    }
    public bool IsDialogCompleted()
    {
        return isDialogCompleted;
    }

    // 배열안의 대화랑 화자를 설정함
    private void SetNextDialog()
    {
        // 이전 화자의 대화 관련 오브젝트 비활성화
        if (currentDialogIndex >= 0)
        {
            SetActiveObjects(speakers[currentSpeakerIndex], false);
        }

        // 다음 대사를 진행하도록
        currentDialogIndex++;

        // 현재 화자 순번 설정
        currentSpeakerIndex = currentDialogIndex % speakers.Length; // 대사 인덱스를 화자 인덱스로 변환

        // 현재 화자의 대화 관련 오브젝트 활성화
        SetActiveObjects(speakers[currentSpeakerIndex], true);

        // 텍스트 타이핑 효과 재생
        typingCoroutine = StartCoroutine(OnTypingText());
    }

    // 기존 대화 초기화
    private void SetActiveObjects(Speaker speaker, bool visible)
    {
        speaker.imageDialog.gameObject.SetActive(visible);
        speaker.textDialogue.gameObject.SetActive(visible);
    }

    // 타이핑 효과
    private IEnumerator OnTypingText()
    {
        int index = 0;
        isTypingEffect = true;

        // 텍스트를 한글자씩 타이핑치듯 재생
        while (index < dialogs[currentDialogIndex].dialogue.Length)
        {
            speakers[currentSpeakerIndex].textDialogue.text = dialogs[currentDialogIndex].dialogue.Substring(0, index + 1);
            index++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingEffect = false;
        typingCoroutine = null;
    }
}

[System.Serializable]
public struct Speaker
{
    public Image imageDialog; // 대화창 Image UI
    public TextMeshProUGUI textDialogue; // 현재 대사 출력 Text UI
}

[System.Serializable]
public struct DialogData
{
    public int speakerIndex; // 대사를 출력할 현재 DialogSystem의 speakers 배열 순번
    [TextArea(3, 5)] public string dialogue; // 대사
}
