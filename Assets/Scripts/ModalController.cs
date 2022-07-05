using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalController : MonoBehaviour
{
    [SerializeField] private GameObject options, sessionOrigin;
    [SerializeField] private Button cancelButton, proceedButton, resyncButton, optionsButton;
    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] private TextMeshProUGUI _logs;

    public static string assetLinkServer = "http://temarotech.net/testassets/AssetBundle/furnitures";       // temaro server
    public static string assetLinkCloud = "https://bit.ly/furnituresbundle";                                // gdrive
    public static string customAssetLink;                                                                   // user input

    void Start()
    {
        customAssetLink = PlayerPrefs.GetString("Asset Link", assetLinkCloud);

        if (customAssetLink != assetLinkCloud)
        {
            Proceed();
        }
        else
        {
            DisableSession();
        }

        Button cancelBtn = cancelButton.GetComponent<Button>();
        Button proceedBtn = proceedButton.GetComponent<Button>();
        Button resyncBtn = resyncButton.GetComponent<Button>();
        Button optionsBtn = optionsButton.GetComponent<Button>();

        cancelBtn.onClick.AddListener(Cancel);
        proceedBtn.onClick.AddListener(Proceed);
        resyncBtn.onClick.AddListener(ClearCache);
        optionsBtn.onClick.AddListener(ShowOptions);
    }

    void ClearCache()
    {
        if (Caching.ClearCache())
        {
            _logs.text = "Asset cleared";
            Debug.Log("Asset cleared");
        }
        else
        {
            _logs.text = "Cache is being used";
            Debug.Log("Cache is being used");
        }
    }

    void CheckInput()
    {
        if (!string.IsNullOrEmpty(addressInput.text))
        {
            if (!(IsValidURL(addressInput.text)))
            {
                _logs.text = "Invalid link";
                Debug.Log("Invalid link");
                addressInput.text = "";
            }
            else
            {
                if (addressInput.text.Contains("furnitures"))
                {
                    PlayerPrefs.SetString("Asset Link", addressInput.text);
                    SetCustomAddress();
                    HideOptions();
                }
                else
                {
                    _logs.text = "Asset could not be located";
                    Debug.Log("Asset could not be located");
                }
            }
        }
        else
        {
            SetDefaultAddress();
            HideOptions();
        }
    }

    bool IsValidURL(string URL)
    {
        string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
        Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return Rgx.IsMatch(URL);
    }
    void ShowOptions()
    {
        options.SetActive(true);
        DisableSession();
    }
    void HideOptions()
    {
        options.SetActive(false);
        EnableSession();
    }
    void Cancel() => CheckInput();
    void Proceed() => CheckInput();
    void SetDefaultAddress() => customAssetLink = assetLinkCloud;
    void SetCustomAddress() => customAssetLink = addressInput.text;
    void DisableSession() => sessionOrigin.SetActive(false);
    void EnableSession() => sessionOrigin.SetActive(true);
}
