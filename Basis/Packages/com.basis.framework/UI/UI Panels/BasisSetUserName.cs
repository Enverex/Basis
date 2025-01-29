using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Basis.Scripts.Drivers;
using Basis.Scripts.BasisSdk.Players;
using Basis.Scripts.Common;
using Basis.Scripts.Networking;
using System.Threading.Tasks;
namespace Basis.Scripts.UI.UI_Panels
{
    public class BasisSetUserName : MonoBehaviour
    {
        public TMP_InputField UserNameTMP_InputField;
        public Button Ready;
        public static string LoadFileName = "CachedUserName.BAS";
        public bool UseAddressables;
        public Button AdvancedSettings;
        public GameObject AdvancedSettingsPanel;
        [Header("Advanced Settings")]
        public TMP_InputField IPaddress;
        public TMP_InputField Port;
        public TMP_InputField Password;
        public Button UseLocalhost;
        public Toggle HostMode;
        public void Start()
        {
            UserNameTMP_InputField.text = BasisDataStore.LoadString(LoadFileName, string.Empty);
            Ready.onClick.AddListener(HasUserName);
            if (AdvancedSettingsPanel != null)
            {
                AdvancedSettings.onClick.AddListener(ToggleAdvancedSettings);
                UseLocalhost.onClick.AddListener(UseLocalHost);
            }
            BasisNetworkManagement.OnEnableInstanceCreate += LoadCurrentSettings;
        }

        public void OnDestroy()
        {
            if (AdvancedSettingsPanel != null)
            {
                AdvancedSettings.onClick.RemoveListener(ToggleAdvancedSettings);
                UseLocalhost.onClick.RemoveListener(UseLocalHost);
            }
        }
        public void UseLocalHost()
        {
            IPaddress.text = "localhost";
        }

        public void LoadCurrentSettings()
        {
            IPaddress.text = BasisNetworkManagement.Instance.Ip;
            Port.text = BasisNetworkManagement.Instance.Port.ToString();
            Password.text = BasisNetworkManagement.Instance.Password;
            HostMode.isOn = BasisNetworkManagement.Instance.IsHostMode;
        }
        public async void HasUserName()
        {
            // Set button to non-interactable immediately after clicking
            Ready.interactable = false;

            if (!string.IsNullOrEmpty(UserNameTMP_InputField.text))
            {
                BasisLocalPlayer.Instance.DisplayName = UserNameTMP_InputField.text;
                BasisDataStore.SaveString(BasisLocalPlayer.Instance.DisplayName, LoadFileName);
                if (BasisNetworkManagement.Instance != null)
                {
                    BasisNetworkManagement.Instance.Ip = IPaddress.text;
                    BasisNetworkManagement.Instance.Password = Password.text;
                    BasisNetworkManagement.Instance.IsHostMode = HostMode.isOn;
                    ushort.TryParse(Port.text, out BasisNetworkManagement.Instance.Port);
                    await CreateAssetBundle();
                    BasisNetworkManagement.Instance.Connect();
                    Ready.interactable = false;
                }
            }
            else
            {
                BasisDebug.LogError("Name was empty, bailing");
                // Re-enable button interaction if username is empty
                Ready.interactable = true;
            }
        }

        public async Task CreateAssetBundle()
        {
            BasisDebug.Log("connecting to default");
            if (BundledContentHolder.Instance.UseAddressablesToLoadScene)
            {
                await BasisSceneLoadDriver.LoadSceneAddressables(BundledContentHolder.Instance.DefaultScene.BasisRemoteBundleEncrypted.BundleURL);
            }
            else
            {
                await BasisSceneLoadDriver.LoadSceneAssetBundle(BundledContentHolder.Instance.DefaultScene);
            }
            Destroy(this.gameObject);
        }

        public void ToggleAdvancedSettings()
        {
            if (AdvancedSettingsPanel != null)
            {
                AdvancedSettingsPanel.SetActive(!AdvancedSettingsPanel.activeSelf);
            }
        }
    }
}
