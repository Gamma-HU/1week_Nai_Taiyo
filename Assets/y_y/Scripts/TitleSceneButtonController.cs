using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneButtonController : MonoBehaviour
{
    [SerializeField] private string gameScene = "GameScene_yy";
    
    public void LoadGameScene()
    {
        SceneManager.LoadScene(gameScene);
    }
}
