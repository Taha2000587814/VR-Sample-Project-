﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using vrCampusCourseware;

public class gazeTimerVisual : MonoBehaviour {
    public event Action OnSelectionComplete;                                                // This event is triggered when the bar has filled.

    [SerializeField] private float m_SelectionDuration = 2f;                                // How long it takes for the bar to fill.
    [SerializeField] private bool m_HideOnStart = true;                                     // Whether or not the bar should be visible at the start.
    [SerializeField] private Image m_Selection;                                             // Reference to the image who's fill amount is adjusted to display the bar.
    [SerializeField] private AudioClip gazeAudio;
    [SerializeField] private AudioSource headAudioSource;


    private Coroutine m_SelectionFillRoutine;                                               // Used to start and stop the filling coroutine based on input.
    private bool m_IsSelectionRadialActive;                                                    // Whether or not the bar is currently useable.
    private bool m_RadialFilled;                                                               // Used to allow the coroutine to wait for the bar to fill.


    public float SelectionDuration { get { return m_SelectionDuration; } }


    private void Start()
    {
        // Setup the radial to have no fill at the start and hide if necessary.
        m_Selection.fillAmount = 0f;

        if (m_HideOnStart)
            Hide();
    }


    public void Show()
    {
        m_Selection.gameObject.SetActive(true);
        m_IsSelectionRadialActive = true;
        m_SelectionFillRoutine = StartCoroutine(FillSelectionRadial());
        headAudioSource.PlayOneShot(gazeAudio);
    }

    IEnumerator delay(float x)
    {
        yield return new WaitForSeconds(x);
    }

    public void Hide()
    {
        m_Selection.gameObject.SetActive(false);
        m_IsSelectionRadialActive = false;

        headAudioSource.Stop();
        // This effectively resets the radial for when it's shown again.
        m_Selection.fillAmount = 0f;
        if (m_SelectionFillRoutine != null)
            StopCoroutine(m_SelectionFillRoutine);

        m_Selection.fillAmount = 0f;
        StartCoroutine(delay(0.9f));
    }


    private IEnumerator FillSelectionRadial()
    {
        // At the start of the coroutine, the bar is not filled.
        m_RadialFilled = false;

        // Create a timer and reset the fill amount.
        float timer = 0f;
        m_Selection.fillAmount = 0f;

        // This loop is executed once per frame until the timer exceeds the duration.
        while (timer < m_SelectionDuration)
        {
            // The image's fill amount requires a value from 0 to 1 so we normalise the time.
            m_Selection.fillAmount = timer / m_SelectionDuration;

            // Increase the timer by the time between frames and wait for the next frame.
            timer += Time.deltaTime;
            yield return null;
        }

        // When the loop is finished set the fill amount to be full.
        m_Selection.fillAmount = 1f;

        // Turn off the radial so it can only be used once.
        m_IsSelectionRadialActive = false;

        // The radial is now filled so the coroutine waiting for it can continue.
        m_RadialFilled = true;

        // If there is anything subscribed to OnSelectionComplete call it.
        if (OnSelectionComplete != null)
            OnSelectionComplete();
    }


    public IEnumerator WaitForSelectionRadialToFill()
    {
        // Set the radial to not filled in order to wait for it.
        m_RadialFilled = false;

        // Make sure the radial is visible and usable.
        Show();

        // Check every frame if the radial is filled.
        while (!m_RadialFilled)
        {
            yield return null;
        }

        // Once it's been used make the radial invisible.
        Hide();
    }
    /*

    private void HandleDown()
    {
        // If the radial is active start filling it.
        if (m_IsSelectionRadialActive)
        {
            m_SelectionFillRoutine = StartCoroutine(FillSelectionRadial());
        }
    }


    private void HandleUp()
    {
        // If the radial is active stop filling it and reset it's amount.
        if (m_IsSelectionRadialActive)
        {
            if (m_SelectionFillRoutine != null)
                StopCoroutine(m_SelectionFillRoutine);

            m_Selection.fillAmount = 0f;
        }
    }
    */
}
