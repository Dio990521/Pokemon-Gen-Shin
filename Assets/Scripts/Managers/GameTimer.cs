using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TimerState
{
    NOTWORK,
    WORKING,
    DONE
}
public class GameTimer
{
    private float _startTime;
    private Action _task;
    private bool _isStopTimer;
    private TimerState _timerState;

    public GameTimer()
    {
        ResetTimer();
    }
    public void ResetTimer()
    {
        _startTime = 0;
        _task = null;
        _isStopTimer = true;
        _timerState = TimerState.NOTWORK;
    }
    public void StartTimer(float time, Action task)
    {
        _startTime=time;
        _task = task;
        _isStopTimer = false;
        _timerState = TimerState.WORKING;
    }
    public void UpdateTimer()
    {
        if (_isStopTimer) return;

        _startTime -= Time.deltaTime;
        if(_startTime < 0f)
        {
            _task?.Invoke();
            _timerState = TimerState.DONE;
            _isStopTimer = true;
        }
    }
    public TimerState GetTimerState() => _timerState;
}
