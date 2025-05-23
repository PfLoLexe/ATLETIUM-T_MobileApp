﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATLETIUM_T.api.controllers;
using ATLETIUM_T.api.repositories;
using ATLETIUM_T.components;
using ATLETIUM_T.localDatabase.controllers;
using ATLETIUM_T.localDatabase.repositories;
using ATLETIUM_T.Models;
using Microsoft.Maui.Controls;

namespace ATLETIUM_T.views;

public partial class AuthorizationPage : ContentPage
{
    private readonly LocalSettingsController _controller = new LocalSettingsController(new LocalSettingsRepository());
    private PinCodeComponent _pinCodeComponent;
    
    public AuthorizationPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        CheckToken();
    }

    private async void CheckToken()
    {
        var token = await _controller.GetToken();
        if (token == null)
        {
            LoginAuth();
            return;
        }
        
        PinCodeAuth();
    }

    private void PinCodeAuth()
    {
        var _pinCodeComponent = new PinCodeComponent();
        _pinCodeComponent.DataSent += OnDataReceived;
        _pinCodeComponent.ChangeComponent += ChangeComponentToLogin;
        MainLayout.Children.Clear();
        MainLayout.Children.Add(_pinCodeComponent);
    }

    private void ChangeComponentToLogin(object? sender, bool e)
    {
        LoginAuth();
    }
    
    private void LoginAuth()
    {
        MainLayout.Children.Clear();
        MainLayout.Children.Add(new LoginComponent());
    }
    
    private void OnDataReceived(object sender, string data)
    {
        LoginAuth();
    }
}