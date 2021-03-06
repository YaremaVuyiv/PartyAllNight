﻿using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;
using Android.Util;
using Gcm.Client;
using WindowsAzure.Messaging;
using System;
using GoogleMaps;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]

//GET_ACCOUNTS is needed only for Android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]

[BroadcastReceiver(Permission = Gcm.Client.Constants.PERMISSION_GCM_INTENTS)]
[IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_MESSAGE },
    Categories = new string[] { "@PACKAGE_NAME@" })]
[IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK },
    Categories = new string[] { "@PACKAGE_NAME@" })]
[IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_LIBRARY_RETRY },
    Categories = new string[] { "@PACKAGE_NAME@" })]
public class MyBroadcastReceiver : GcmBroadcastReceiverBase<PushHandlerService>
{
    public static string[] SENDER_IDS = new string[] { "642894952915" };

    public const string TAG = "MyBroadcastReceiver-GCM";
}

[Service] // Must use the service tag
public class PushHandlerService : GcmServiceBase
{
    public const string SenderID = "378461629767"; // Google API Project Number
    public const string ListenConnectionString = "Endpoint=sb://techmednotificationstest.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=CdG+F85O0sohaC+2IosMOGtWD3JNgw9t+9wKRMadS5A=";
    public const string NotificationHubName = "AzurePushHubTest";

    public static string RegistrationID { get; private set; }
    private NotificationHub Hub { get; set; }

    public PushHandlerService() : base(SenderID)
    {
        Log.Info(MyBroadcastReceiver.TAG, "PushHandlerService() constructor");
    }

    protected override void OnRegistered(Context context, string registrationId)
    {
        Log.Verbose(MyBroadcastReceiver.TAG, "GCM Registered: " + registrationId);
        RegistrationID = registrationId;

        CreateNotification("PushHandlerService-GCM Registered...",
                            "The device has been Registered!");

        Hub = new NotificationHub(NotificationHubName, ListenConnectionString, context);
        try
        {
            Hub.UnregisterAll(registrationId);
        }
        catch (Exception ex)
        {
            Log.Error(MyBroadcastReceiver.TAG, ex.Message);
        }

        //var tags = new List<string>() { "falcons" }; // create tags if you want
        var tags = new List<string>() { };

        try
        {
            var hubRegistration = Hub.Register(registrationId, tags.ToArray());
        }
        catch (Exception ex)
        {
            Log.Error(MyBroadcastReceiver.TAG, ex.Message);
        }
    }

    protected override void OnMessage(Context context, Intent intent)
    {
        Log.Info(MyBroadcastReceiver.TAG, "GCM Message Received!");

        var msg = new StringBuilder();

        if (intent != null && intent.Extras != null)
        {
            foreach (var key in intent.Extras.KeySet())
                msg.AppendLine(key + "=" + intent.Extras.Get(key).ToString());
        }

        string messageText = intent.Extras.GetString("message");
        if (!string.IsNullOrEmpty(messageText))
        {
            CreateNotification("New hub message!", messageText);
        }
        else
        {
            CreateNotification("Unknown message details", msg.ToString());
        }
    }

    void CreateNotification(string title, string desc)
    {
        //Create notification
        var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

        //Create an intent to show UI
        var uiIntent = new Intent(this, typeof(MainActivity));

        //Create the notification
        var notification = new Notification(Android.Resource.Drawable.SymActionEmail, title)
        {

            //Auto-cancel will remove the notification once the user touches it
            Flags = NotificationFlags.AutoCancel
        };

        //Set the notification info
        //we use the pending intent, passing our ui intent over, which will get called
        //when the notification is tapped.
        notification.SetLatestEventInfo(this, title, desc, PendingIntent.GetActivity(this, 0, uiIntent, 0));

        //Show the notification
        notificationManager.Notify(1, notification);
        //DialogNotify(title, desc);
    }

    protected void DialogNotify(String title, String message)
    {

        MainActivity.instance.RunOnUiThread(() =>
        {
            AlertDialog.Builder dlg = new AlertDialog.Builder(MainActivity.instance);
            AlertDialog alert = dlg.Create();
            alert.SetTitle(title);
            alert.SetButton("Ok", delegate
            {
                alert.Dismiss();
            });
            alert.SetMessage(message);
            alert.Show();
        });
    }

    protected override void OnUnRegistered(Context context, string registrationId)
    {
        Log.Verbose(MyBroadcastReceiver.TAG, "GCM Unregistered: " + registrationId);

        CreateNotification("GCM Unregistered...", "The device has been unregistered!");
    }

    protected override bool OnRecoverableError(Context context, string errorId)
    {
        Log.Warn(MyBroadcastReceiver.TAG, "Recoverable Error: " + errorId);

        return base.OnRecoverableError(context, errorId);
    }

    protected override void OnError(Context context, string errorId)
    {
        Log.Error(MyBroadcastReceiver.TAG, "GCM Error: " + errorId);
    }
}