using System;
using System.Threading.Tasks;
using Dawnshard.Menu;
using Dawnshard.Network;
using Nakama.Console;
using NBitcoin.Protocol;
using Nethereum.Web3;
using UnityEngine;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using Satori.TinyJson;
using static AlturaWeb3.AlturaWeb3;

public class Web3AuthAPI : MonoBehaviour
{
    private Web3Auth web3Auth;
    private Action OnLoginSuccess;
    Web3 web3;
    const string rpcURL = "https://rpc.ankr.com/eth";

    public static Web3AuthAPI Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        web3Auth = GetComponent<Web3Auth>();

#if TEST_ENV
        web3Auth.setOptions(new Web3AuthOptions()
        {
            redirectUrl = new Uri("Dawnshard://com.DeepMonolith.Dawnshard/auth"),
            clientId = "BBMtfjRic6e94aJ_VwTlT6hayOLyFRGqt0uEbf2y2RjHfUhXoyUpE3tqv0J8EBMS950RpLDwbf-K0oTEw4uld0I",
            network = Web3Auth.Network.TESTNET,
        });
#else
        web3Auth.setOptions(new Web3AuthOptions()
        {
            redirectUrl = new Uri("Dawnshard://com.DeepMonolith.Dawnshard/auth"),
            clientId = "BL7--40cxThdbdpEPF0QPsKaH9CByk6UBEtzztPHgX3frHsJ84T6uy0oTj7SDPH5Wzngh0mxuVrIez0vvcyrCRI",
            network = Web3Auth.Network.SAPPHIRE_MAINNET,
        });
#endif

        web3Auth.onLogin+= UserLoginReceivedAsync;
        web3 = new Web3(rpcURL);
    }
    public void Login(Provider selectedProvider, string email, Action OnSuccess = null)
    {
        GameController.Instance.ClearURLWithoutRefresh();
        var options = new LoginParams()
        {
            loginProvider = selectedProvider,
            extraLoginOptions = selectedProvider == Provider.EMAIL_PASSWORDLESS ? new ExtraLoginOptions()
            {
                login_hint = email
            } : null,

        };

        OnLoginSuccess = OnSuccess;
        try
        {
            web3Auth.login(options);
        }
        catch (Exception e)
        {
            ErrorPopup.Instance.Open();
            ErrorPopup.Instance.SetErrorText(e.Message);
            LoadingPopup.Instance.Close();
            throw;
        }
    }

    private async void UserLoginReceivedAsync(Web3AuthResponse response)
    {
        var account = new Account(response.privKey);

        PlayerPrefs.SetString(Constants.EthAddress, account.Address);
        PlayerPrefs.SetString(Constants.PrivateKey, account.PrivateKey);
        PlayerPrefs.SetString(Constants.W3Token, response.userInfo.idToken);

        if (GameController.Instance.Session == null)
        {
            try
            {
                await GameController.Instance.WalletLogin(account.Address, response.userInfo.idToken);
            }
            catch (Exception e)
            {
                ErrorPopup.Instance.Open();
                ErrorPopup.Instance.SetErrorText(e.Message);
                LoadingPopup.Instance.Close();
                throw;
            }
            
        }
        else
        {
            try
            {
                await GameController.Instance.LinkAddress();
                if(!TutorialStorageAPI.TutorialStoragePlayer.AlreadyRegistered)
                    await TutorialStorageAPI.SaveTutorialStorage(alreadyRegistered: true);
            }
            catch (Exception e)
            {
                ErrorPopup.Instance.Open();
                ErrorPopup.Instance.SetErrorText(e.Message);
                LoadingPopup.Instance.Close();
                throw;
            }
        }

        OnLoginSuccess?.Invoke();
    }

    public void Logout()
    {
        GameController.Instance.ClearURLWithoutRefresh();
        web3Auth.logout();
    }
    
    public async void LoginWithAltura(string alturaGuardCode, Action OnSuccess = null, Action<Exception> OnException = null)
    {
        GameController.AddressResponse addressResponse=null;
        try
        {
            addressResponse = await GameController.Instance.AddAlturaConnection(alturaGuardCode);
        }
        catch (Exception e)
        {
            ErrorPopup.Instance.Open();
            ErrorPopup.Instance.SetErrorText("Invalid code. Please revoke your connections on marketplace.playdawnshard.com and retry");
            LoadingPopup.Instance.Close();
            return;
        }

        if (addressResponse != null)
        {
            PlayerPrefs.SetString(Constants.EthAddress, addressResponse.Address);
            PlayerPrefs.SetString(Constants.AlturaToken, addressResponse.Token);
            PlayerPrefs.Save();
        }

        if (GameController.Instance.Session != null)
        {
            try
            {
                await GameController.Instance.LinkAddress();
                if(!TutorialStorageAPI.TutorialStoragePlayer.AlreadyRegistered)
                    await TutorialStorageAPI.SaveTutorialStorage(alreadyRegistered: true);
            }
            catch (Exception e)
            {
                OnException?.Invoke(e);
                return;
            }
        }
        else
        {
            try
            {
                await GameController.Instance.WalletLogin(addressResponse.Address, addressResponse.Token);
            }
            catch (Exception e)
            {
                if(addressResponse!=null)
                    await GameController.Instance.RevokeAlturaConnection();
                OnException?.Invoke(e);
                return;
            }
        }

        OnSuccess?.Invoke();
    }
    
}