﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ATLETIUM_T.Models;
using Microsoft.Maui.Controls;
using System.Timers;
using ATLETIUM_T.api.controllers;
using ATLETIUM_T.api.repositories;

using Timer = System.Timers.Timer;

namespace ATLETIUM_T.components;

public partial class TrainsList : ContentView
{

    private ObservableCollection<TrainMain> _trains { get; set; } = new ObservableCollection<TrainMain>();
    private int? _train_list_view_tapped;
    private Timer _train_list_view_tapped_timer = new Timer(2000);
    private int _dayWeekNumber;

    public event EventHandler<int> ValueChanged;
    private int _amountOfTrains;
    public int AmountOfTrains
    {
        get => _amountOfTrains;
        set
        {
            if (_amountOfTrains != value)
            {
                _amountOfTrains = value;
                OnValueChanged(_amountOfTrains);
            }
        }
    }
    
    private void OnValueChanged(int newValue)
    {
        ValueChanged?.Invoke(this, newValue);
    }

    public int DayWeekNumber
    {
        get
        {
            if (_dayWeekNumber == 0) return (int)DateTime.Now.DayOfWeek;
            return _dayWeekNumber;
        }
        
        set { _dayWeekNumber = value; }
    }

    private DateTime _currentDay;
    public DateTime CurrentDay
    {
        get
        {
            if (_currentDay == null) return DateTime.Now;
            return _currentDay;
        }
        
        set { _currentDay = value; }
    }
    
    private TrainController _controller = new TrainController(new TrainRepository());
    
    public TrainsList()

    {
        InitializeComponent();
        DayWeekNumber = (int)DateTime.Now.DayOfWeek == 0 ? 7 : (int)DateTime.Now.DayOfWeek;
        LoadTrains();
        _train_list_view_tapped_timer.Elapsed += OnTappedTimerEvent;
        TrainsListView.ItemsSource = _trains;
    }
    
    public void ListViewTrainsRefreshing()
    {
        LoadTrains();
    }
    
    public async void LoadTrains()
    {
        var trainsList = await _controller.GetTrainsListAsync(DayWeekNumber, CurrentDay);
        if (_trains != null) _trains.Clear();
        if (trainsList == null) return;
        foreach (var train in trainsList)
        {
            _trains.Add(train);
        }

        AmountOfTrains = _trains.Count;
    }

    private async void TrainsListView_OnItemTapped(object? sender, ItemTappedEventArgs e)
    {
        _train_list_view_tapped_timer.Start();
        if (e.ItemIndex == _train_list_view_tapped)
        {
            _train_list_view_tapped = null;

            var train = _trains[e.ItemIndex];
            train.date = CurrentDay.ToString();
            
            await Shell.Current.GoToAsync($"{nameof(TrainDetail)}",
                new Dictionary<string, object>
                {
                    ["train"] = train
                });

        }
        else
        {
            _train_list_view_tapped = e.ItemIndex;
        }
    }

    private void OnTappedTimerEvent(object sender, ElapsedEventArgs e)
    {
        _train_list_view_tapped = null;
    }
}