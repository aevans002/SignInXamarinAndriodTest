/*
*****************************************************************************
*****************************************************************************
*  Project: SignInAppAndroid
*  Author: Allan Evans
*  Date Created: 9/19/2020
*  Class: CreationActivity.cs
*  Overview: Adds new user info with password and checks so that there are not
*  repeated users or repeated characters in the password
*  
*****************************************************************************
*****************************************************************************
*/
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Linq;

namespace SignInAppAndroid
{
    [Activity(Label = "CreationActivity")]
    public class CreationActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_creation);

            TextView headerTxt = FindViewById<TextView>(Resource.Id.textView1);
            EditText userTxtBx = FindViewById<EditText>(Resource.Id.userTxtBx);
            EditText passwordTxtBx = FindViewById<EditText>(Resource.Id.passwordTxtBx);
            Button createBtn = FindViewById<Button>(Resource.Id.createBtn);

            createBtn.Click += CreateAccountClick;
        }

        private void CreateAccountClick(object sender, EventArgs eventArgs)
        {
            //Checking for issues with the entries
            TextView headerTxt = FindViewById<TextView>(Resource.Id.textView1);
            EditText userTxtBx = FindViewById<EditText>(Resource.Id.userTxtBx);
            EditText passwordTxtBx = FindViewById<EditText>(Resource.Id.passwordTxtBx);
            View view = (View)sender;
            if (userTxtBx.Length() == 0)
            {
                ToastNotification.ToastMessage(Resources.GetText(Resource.String.name_short));
                return;
            }

            //Due to the info being saved on a simple text file, certain characters are avoided
            if (userTxtBx.Text.Equals(Resources.GetText(Resource.String.seperator)) || userTxtBx.Text.Equals(Resources.GetText(Resource.String.backslash)))
            {
                ToastNotification.ToastMessage(Resources.GetText(Resource.String.forbidden_char));
                return;
            }

            if (passwordTxtBx.Text.Equals(Resources.GetText(Resource.String.seperator)) || passwordTxtBx.Text.Equals(Resources.GetText(Resource.String.backslash)))
            {
                ToastNotification.ToastMessage(Resources.GetText(Resource.String.forbidden_char));
                return;
            }

            //Make things consistent for username and password, so they don't become so large they clip the list view
            if (passwordTxtBx.Length() < 5)
            {
                ToastNotification.ToastMessage(Resources.GetText(Resource.String.password_short));
                return;
            }

            //Keep the user name and password to a certain limit to avoid clipping
            if (passwordTxtBx.Length() > 12 || userTxtBx.Length() > 24)
            {
                ToastNotification.ToastMessage(Resources.GetText(Resource.String.password_long));
                return;
            }

            //Check if the password is all numbers or all characters
            if (passwordTxtBx.Text.All(char.IsDigit) || passwordTxtBx.Text.All(char.IsLetter))
            {
                ToastNotification.ToastMessage(Resources.GetText(Resource.String.password_mix));
                return;
            }

            //Put password into char array to check for sequence
            char[] charSplit = passwordTxtBx.Text.ToCharArray();
            for (int i = 0; i < charSplit.Length; i++)
            {
                if (i < charSplit.Length - 3)
                {
                    if (charSplit[i] == charSplit[i + 1] && charSplit[i + 1] == charSplit[i + 2]) //aaa
                    {
                        ToastNotification.ToastMessage(Resources.GetText(Resource.String.password_repeat));
                        return;
                    }
                }
                if (i < charSplit.Length - 2)
                {
                    if (charSplit[i] == charSplit[i + 1]) //aa
                    {
                        ToastNotification.ToastMessage(Resources.GetText(Resource.String.password_repeat));
                        return;
                    }
                }

                if (i < charSplit.Length - 4)
                {
                    if ((charSplit[i] + charSplit[i + 1]) == (charSplit[i + 2] + charSplit[i + 3])) //abab
                    {
                        ToastNotification.ToastMessage(Resources.GetText(Resource.String.password_repeat));
                        return;
                    }
                }

                if (charSplit.Length >= 6 && i < charSplit.Length - 6)
                {
                    if ((charSplit[i] + charSplit[i + 1] + charSplit[i + 2]) == (charSplit[i + 3] + charSplit[i + 4] + charSplit[i + 5])) //abcabc
                    {
                        ToastNotification.ToastMessage(Resources.GetText(Resource.String.password_repeat));
                        return;
                    }
                }
            }

            SaveNewEntry(userTxtBx.Text, passwordTxtBx.Text);
        }

        private void SaveNewEntry(String user, String pass)
        {
            //Initialize empty string
            String newEntry = "";
            if (System.IO.File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + Resources.GetText(Resource.String.file_path)))
            {
                newEntry = System.IO.File.ReadAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + Resources.GetText(Resource.String.file_path));
                string[] oldArray = newEntry.Split(new string[] { Resources.GetText(Resource.String.new_line) }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < oldArray.Length; i++)
                {   //Need to add array of prohibited chars later
                    string[] splitOld = oldArray[i].Split(new string[] { Resources.GetText(Resource.String.seperator) }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitOld[0] == user)  //Can have same password but not user name
                    {
                        ToastNotification.ToastMessage(Resources.GetText(Resource.String.entry_exists));
                        return;
                    }
                }
            }

            newEntry += user + Resources.GetText(Resource.String.seperator) + pass + Resources.GetText(Resource.String.new_line);  //Put in check to prevent same entry multiple times
            System.IO.File.WriteAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + Resources.GetText(Resource.String.file_path), newEntry);
            ToastNotification.ToastMessage(Resources.GetText(Resource.String.entry_created));
        }

        //Custom Toast notification
        public static class ToastNotification
        {
            public static void ToastMessage(string message)
            {
                var context = Android.App.Application.Context;
                var toastMessage = message;
                var duration = ToastLength.Long;

                //This handles the short messages displayed
                Toast.MakeText(context, toastMessage, duration).Show();
            }
        }
    }
}