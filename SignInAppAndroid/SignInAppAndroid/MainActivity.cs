using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace SignInAppAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]

    //Needs a header
    public class MainActivity : AppCompatActivity
    {
        private ArrayAdapter<string> adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //link basic buttons and textboxes
            ListView userListView = FindViewById<ListView>(Resource.Id.userListView);
            Button newEntryBtn = FindViewById<Button>(Resource.Id.newEntryBtn);

            newEntryBtn.Click += nextClick;
            ReadEntries();
        }

        protected override void OnResume()
        {
            base.OnResume();
            //For refreshing the displayed listview
            ReadEntries();
        }



        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void ReadEntries()
        {
            //Reads saved file if it exists and populates listview
            ListView userListView = FindViewById<ListView>(Resource.Id.userListView);
            if (File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/saved.txt"))
            {
                string existingEntries = File.ReadAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/saved.txt");
                string[] existingArray = existingEntries.Split(new string[] { "/r" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < existingArray.Length; i++)
                {
                    string[] splitEntry = existingArray[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    existingArray[i] = "User:  " + splitEntry[0] + "  Password:  " + splitEntry[1];
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

        //Generated code when initially creating the app
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}
