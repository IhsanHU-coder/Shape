using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BtnAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Animator animator;
    [SerializeField] private Animator outro;
    // [SerializeField] private Animator isAbout;
    public CubeGridMovement player;
    public SmoothCameraFollow smoothCameraFollow;
    private bool isClicked = false;
    [SerializeField] private int levelID;
    public bool aboutIsOpen = false;
    

    void Start()
    {
        outro.Play("intro");
        // if (player != null)
        //     player.awalSpawn();
        isClicked = false;
        if (smoothCameraFollow != null)
            smoothCameraFollow.UnFreezeCamera();
    }



    public void Level1()
    {
        isClicked = true;
        animator.Play("ShakeBtn");
        // SceneManager.LoadScene("Level 1");
        outro.Play("outro");
        StartCoroutine(level1());
    }

    IEnumerator level1()
    {
        yield return new WaitForSeconds(3.5f);

        SceneManager.LoadScene("Level 1");
        outro.Play("intro");
    }

    public void Level2()
    {
        isClicked = true;
        animator.Play("ShakeBtn");
        outro.Play("outro");
        StartCoroutine(level2());
    }
    IEnumerator level2()
    {
        yield return new WaitForSeconds(3.5f);

        SceneManager.LoadScene("Level 2");
        outro.Play("intro");
    }
    public void Level3()
    {
        isClicked = true;
        animator.Play("ShakeBtn");
        outro.Play("outro");
        StartCoroutine(level3());
    }
    IEnumerator level3()
    {
        yield return new WaitForSeconds(3.5f);

        SceneManager.LoadScene("Level 3");
        outro.Play("intro");
    }
    public void MainMenu()
    {
        isClicked = true;
        animator.Play("ShakeBtn");
        outro.Play("outro");
        StartCoroutine(mainMenu());
    }
    IEnumerator mainMenu()
    {
        yield return new WaitForSeconds(3.5f);

        SceneManager.LoadScene("Main Menu");
        outro.Play("intro");
    }

    public void Retry()
    {
        isClicked = true;
        animator.Play("ShakeBtn");
        outro.Play("outro");
        StartCoroutine(retry());

    }
    IEnumerator retry()
    {
        yield return new WaitForSeconds(3.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        player.awalSpawn();
        smoothCameraFollow.UnFreezeCamera();
        outro.Play("intro");
        
    }

    public void WorldMap()
    {
        isClicked = true;
        animator.Play("ShakeBtn");
        outro.Play("outro");
        StartCoroutine(worldMap());
    }
    IEnumerator worldMap()
{
    yield return new WaitForSeconds(3.5f);

    int currentLevel = SaveManager.LoadCurrentLevel();
    SaveManager.SaveCurrentLevel(currentLevel);

    PlayerPrefs.Save();
    GameFlow.FromMainMenu = false;

    SceneManager.LoadScene("WorldMap");
}

public void MainWorldMap()
    {
        isClicked = true;
        animator.Play("ShakeBtn");
        outro.Play("outro");
        StartCoroutine(mainWorldMap());
    }
    IEnumerator mainWorldMap()
{
    yield return new WaitForSeconds(3.5f);

    int currentLevel = SaveManager.LoadCurrentLevel();
    SaveManager.SaveCurrentLevel(currentLevel);

    PlayerPrefs.Save();
    GameFlow.FromMainMenu = true;

    SceneManager.LoadScene("WorldMap");
}
    



    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isClicked) return;
        animator.Play("FlipBtn");
        Debug.Log("Detect");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // animator.Play("Idle");
    }

    public void PlaySceneTransition(string sceneName)
{
    isClicked = true;
    animator.Play("ShakeBtn");
    outro.Play("outro");
    StartCoroutine(LoadScene(sceneName));
}

IEnumerator LoadScene(string sceneName)
{
    yield return new WaitForSeconds(3.5f);

    SceneManager.LoadScene(sceneName);

    outro.Play("intro");
}

public void aboutOpen()
    {
        outro.Play("AboutAnim");
    }
    public void aboutClose()
    {
        outro.Play("Close");
    }
}