using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseOpenInventory : MonoBehaviour
{

    // to fade in and out inventory
    private static CanvasGroup canvasGroup;

    // fade in and out bools
    private bool fadingIn;
    private bool fadingOut;

    // Timer of fade
    public float fadeTime;

    // bool to check if fading out
    private static bool checkFade;

    // return the cnavas group
    public static CanvasGroup CanvasGroup
    {
        get { return CloseOpenInventory.canvasGroup; }
    }

    // return the check fade bool
    public static bool CheckFade
    {
        get { return CloseOpenInventory.checkFade; }
    }

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = transform.parent.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        // open and close inventory when input key is pressed
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (canvasGroup.alpha > 0)
            {
                checkFade = true;
                StartCoroutine("FadeOut"); // start the coroutine to close inventory

            }
            else
            {
                checkFade = false;
                StartCoroutine("FadeIn"); // start the coroutine to open inventory
            }
        }
    }


    // timer to close inventory
    private IEnumerator FadeOut()
    {
        if (!fadingOut)
        {
            fadingOut = true;
            fadingIn = false;

            // making sure to not fade in and out at the same time
            StopCoroutine("FadeIn");

            // Aptha value of inventory
            float startAlpha = canvasGroup.alpha;

            // the rate to fade out
            float rate = 1.0f / fadeTime;

            // progress of the fade 
            float progress = 0.0f;

            // keep fading out till the inventory is not visible
            while (progress < 1.0)
            {
                // fading out the inventory
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, progress);

                // increase the progress with the rate
                progress += rate * Time.deltaTime;

                yield return null;
            }
            // when done fading
            canvasGroup.alpha = 0;
            fadingOut = false;
        }
    }

    private IEnumerator FadeIn()
    {
        if (!fadingIn)
        {
            fadingOut = false;
            fadingIn = true;

            // making sure to not fade in and out at the same time
            StopCoroutine("FadeOut");

            // Aptha value of inventory
            float startAlpha = canvasGroup.alpha;

            // the rate to fade out
            float rate = 1.0f / fadeTime;

            // progress of the fade 
            float progress = 0.0f;

            // keep fading in till the inventory is visible
            while (progress < 1.0)
            {
                // fading out the inventory
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, progress);

                // increase the progress with the rate
                progress += rate * Time.deltaTime;

                yield return null;
            }

            // when done fading
            canvasGroup.alpha = 1;
            fadingIn = false;
        }
    }
}
