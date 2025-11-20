// Assets/Script/FooterLinkHandler.cs
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FooterLinkHandler : MonoBehaviour, IPointerClickHandler
{
    [Header("Panels (드래그해서 연결)")]
    [SerializeField] private GameObject mainPanel;    // 로그인 패널(예: MainPanel)
    [SerializeField] private GameObject signupPanel;  // 회원가입 패널(예: SignupPanel)

    [Header("UI Camera (선택)")]
    [Tooltip("Canvas가 Screen Space - Camera / World Space면 여기에 UI 카메라를 넣어줘. Overlay면 비워도 됨.")]
    [SerializeField] private Camera uiCamera;

    private TextMeshProUGUI tmpText;
    private Canvas rootCanvas;

    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        rootCanvas = GetComponentInParent<Canvas>();

        if (!tmpText)
            Debug.LogError("[FooterLinkHandler] TextMeshProUGUI가 없습니다.", this);

        if (!rootCanvas)
            Debug.LogError("[FooterLinkHandler] 상위에 Canvas가 없습니다.", this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!tmpText || eventData == null) return;

        // Canvas 모드별 카메라 결정
        Camera cam = null;
        if (rootCanvas && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = eventData.pressEventCamera ?? uiCamera; // Camera/World 모드면 카메라 필요

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmpText, eventData.position, cam);
        if (linkIndex == -1) return; // 클릭한 위치에 링크 없음

        TMP_LinkInfo linkInfo = tmpText.textInfo.linkInfo[linkIndex];
        string linkId = linkInfo.GetLinkID();   // <link="signup">회원가입하기</link> 의 "signup"

        switch (linkId)
        {
            case "signup":
                Debug.Log("[FooterLinkHandler] 회원가입 링크 클릭");
                if (!signupPanel || !mainPanel)
                {
                    Debug.LogError("[FooterLinkHandler] 패널 참조가 비었어. 인스펙터에서 mainPanel/signupPanel을 연결해줘.", this);
                    return;
                }
                signupPanel.SetActive(true);
                mainPanel.SetActive(false);
                break;

            default:
                // URL 같은 외부 링크도 지원하고 싶으면 여기서 처리
                // if (Uri.IsWellFormedUriString(linkId, UriKind.Absolute)) Application.OpenURL(linkId);
                Debug.Log($"[FooterLinkHandler] 처리되지 않은 linkId: {linkId}", this);
                break;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!tmpText) tmpText = GetComponent<TextMeshProUGUI>();
        // 실수 방지: Raycast Target 켜두기
        if (tmpText) tmpText.raycastTarget = true;
    }
#endif
}


