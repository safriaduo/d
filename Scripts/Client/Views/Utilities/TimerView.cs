using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerView : MonoBehaviour
{
    private const int SIMULATION_TIME_FACTOR = 2;
    private const float SHOW_TIMER_VIEW_COUNTDOWN = 30f;
    private float turnTimerMax = 0f;
    private bool isTimerShown = false;
    private float timer;
    private bool isPlayerTurn = false;
    [SerializeField] private ParticleSystem timerParticle;
    [SerializeField] private StudioEventEmitter emitter;

    private void Update()
    {
        if (turnTimerMax == 0 || !isPlayerTurn)
            return;
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        if (timer < SHOW_TIMER_VIEW_COUNTDOWN && !isTimerShown)
        {
            ShowTimer();
            isTimerShown = true;
        }
        if (timer < 0)
        {
            timer = turnTimerMax;
            StopTimer();
        }
    }

    public void SetMaxTimer(float maxTimer)
    {
        turnTimerMax = maxTimer;
        timer = turnTimerMax;
        var particleMain = timerParticle.main;
        particleMain.simulationSpeed = 1 / (SIMULATION_TIME_FACTOR * SHOW_TIMER_VIEW_COUNTDOWN);
    }

    private void ShowTimer()
    {
        timerParticle.Play();
        emitter.Play();
    }

    public void ToggleTimer(bool playerTurn)
    {
        isTimerShown = false;
        if (playerTurn)
            StartTimer();
        else
            StopTimer();
    }

    private void StopTimer()
    {
        if (!isPlayerTurn)
            return;
        if (timerParticle.isPlaying)
            timerParticle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        isPlayerTurn = false;
        timer = turnTimerMax;
    }

    private void StartTimer()
    {
        if (isPlayerTurn)
            return;
        timer = turnTimerMax;
        isPlayerTurn = true;
    }
}
