﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AnimDL.WinUI.Contracts.Services;
using AnimDL.WinUI.ViewModels;
using MalApi;
using Microsoft.UI.Xaml;

namespace AnimDL.WinUI.Activation;

public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    private readonly INavigationService _navigationService;
    private readonly ILocalSettingsService _localSettingsService;

    public DefaultActivationHandler(INavigationService navigationService, 
                                    ILocalSettingsService localSettingsService)
    {
        _navigationService = navigationService;
        _localSettingsService = localSettingsService;
    }

    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        // None of the ActivationHandlers has handled the activation.
        return _navigationService.Frame.Content == null;
    }

    protected async override Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        var token = _localSettingsService.ReadSetting<OAuthToken>("MalToken");
        if (token is null || string.IsNullOrEmpty(token.AccessToken)) 
        {
            _navigationService.NavigateTo(typeof(SettingsViewModel).FullName, new Dictionary<string, object> { ["IsAuthenticated"] = true });
        }
        else
        {
            _navigationService.NavigateTo(typeof(UserListViewModel).FullName);
        }

        await Task.CompletedTask;
    }
}
