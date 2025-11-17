using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class AuthManager : MonoBehaviour
{
    [Header("Login UI")]
    public TMP_InputField LoginUsernameField;
    public TMP_InputField LoginPasswordField;

    [Header("Register UI")]
    public TMP_InputField RegisterUsernameField;
    public TMP_InputField RegisterPasswordField;

    [Header("Status")]
    public TMP_Text statusText;

    async void Start()
    {
        await UnityServices.InitializeAsync();
    }

    public async void OnLoginButton()
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(
                LoginUsernameField.text.Trim(), LoginPasswordField.text);
            statusText.text = "Login Sucessful!";
            SceneManager.LoadScene("MainMenuScene");
        }
        catch (System.Exception e)
        {
            statusText.text = "Login failed: " + e.Message;
        }
    }

    public async void OnRegisterButton()
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(
                RegisterUsernameField.text.Trim(), RegisterPasswordField.text);
            statusText.text = "Account created!";
            SceneManager.LoadScene("MainMenuScene");
        }
        catch (System.Exception e)
        {
            statusText.text = "Register failed: " + e.Message;
        }
    }

    public async void OnGuestButton()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            statusText.text = "Playing as Guest!";
            SceneManager.LoadScene("MainMenuScene");
        }
        catch (System.Exception e)
        {
            statusText.text = "Guest login failed: " + e.Message;
        }
    }

    public void LoginButtonClick() => OnLoginButton();
    public void RegisterButtonClick() => OnRegisterButton();
    public void GuestButtonClick() => OnGuestButton();
}
