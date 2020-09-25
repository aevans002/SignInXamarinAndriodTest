/*
*****************************************************************************
*****************************************************************************
*  Project: SignInAppAndroid
*  Author: Allan Evans
*  Date Created: 9/19/2020
*  Class: MainActivity.cs
*  Overview: Class for the initial view, displays the user list (if any are
*  saved) and welcomes the user
*  
*****************************************************************************
*****************************************************************************
*/
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using System.IO;

namespace SignInAppAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private ArrayAdapter<string> adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            //link basic buttons and textboxes
            ListView userListView = FindViewById<ListView>(Resource.Id.userListView);
            Button newEntryBtn = FindViewById<Button>(Resource.Id.newEntryBtn);

            newEntryBtn.Click += nextClick;
        }

        protected override void OnResume()
        {
            base.OnResume();
            //For refreshing the displayed listview, this will be called everytime the view appears
            ReadEntries();
        }

        private void ReadEntries()
        {
            string emptySpace = "  ";
            //Reads saved file if it exists and populates listview
            ListView userListView = FindViewById<ListView>(Resource.Id.userListView);
            if (File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + Resources.GetText(Resource.String.file_path)))
            {
                //If the file exists, it is read and added to the list
                string existingEntries = File.ReadAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + Resources.GetText(Resource.String.file_path));
                string[] existingArray = existingEntries.Split(new string[] { Resources.GetText(Resource.String.new_line) }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < existingArray.Length; i++)
                {
                    string[] splitEntry = existingArray[i].Split(new string[] { Resources.GetText(Resource.String.seperator) }, StringSplitOptions.RemoveEmptyEntries);
                    existingArray[i] = Resources.GetText(Resource.String.user) + emptySpace + splitEntry[0] + emptySpace + emptySpace + Resources.GetText(Resource.String.password) + emptySpace + splitEntry[1];
                }

                adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, existingArray);
                userListView.Adapter = adapter;
            }

        }


        private void nextClick(object sender, EventArgs eventArgs)
        {
            //Sends user to creation view
            View view = (View)sender;
            Intent nextActivity = new Intent(this, typeof(CreationActivity));
            StartActivity(nextActivity);
        }

    }
}
