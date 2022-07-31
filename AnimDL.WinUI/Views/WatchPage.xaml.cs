﻿using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using AnimDL.Core.Models;
using AnimDL.WinUI.Helpers;
using AnimDL.WinUI.ViewModels;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace AnimDL.WinUI.Views;

public class WatchPageBase : ReactivePageEx<WatchViewModel> { }

public sealed partial class WatchPage : WatchPageBase
{
    public WatchPage()
    {
        InitializeComponent();

        // Load video html
        this.ObservableForProperty(x => x.ViewModel.Url, x => x)
            .Subscribe(async x =>
            {
                var html = VideoJsHelper.GetPlayerHtml(x);
                await WebView.EnsureCoreWebView2Async();
                WebView.NavigateToString(html);
            });

        this.ObservableForProperty(x => x.ViewModel.VideoPlayerRequestMessage, x => x)
            .Subscribe(async x =>
            {
                await WebView.EnsureCoreWebView2Async();
                WebView.CoreWebView2.PostWebMessageAsJson(x);
            });

        this.WhenActivated(d =>
        {
            // Suggestion Choosen
            SearchBox.Events().SuggestionChosen
                              .Select(x => x.args.SelectedItem as SearchResult)
                              .InvokeCommand(ViewModel.SearchResultPicked)
                              .DisposeWith(d);

            // Relay messages from webview
            WebView.Events()
                   .WebMessageReceived
                   .Select(x => JsonSerializer.Deserialize<WebMessage>(x.args.WebMessageAsJson))
                   .Subscribe(async x => await ViewModel.OnVideoPlayerMessageRecieved(x))
                   .DisposeWith(d);
        });
    }

}

public enum WebMessageType
{
    Ready,
    TimeUpdate,
    DurationUpdate,
    Ended,
    CanPlay
}

public class WebMessage
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WebMessageType MessageType { get; set; }
    public string Content { get; set; }
}