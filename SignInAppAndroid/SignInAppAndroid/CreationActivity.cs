using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace SignInAppAndroid
{
    [Activity(Label = "CreationActivity")]

    //Normally add headers right here
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
                ToastNotification.ToastMessage("Name too short");
                return;
            }

            //Make things consistent for username and password, so they don't become so large they clip the list view
            if (passwordTxtBx.Length() < 5 || userTxtBx.Length() < 5)
            {
                ToastNotification.ToastMessage("Password too short");
                return;
            }

            if (passwordTxtBx.Length() > 12 || userTxtBx.Length() > 12)
            {
                ToastNotification.ToastMessage("Password too long");
                return;
            }

            if (passwordTxtBx.Text.All(char.IsDigit))
            {
                ToastNotification.ToastMessage("Password needs a mix of characters");
                return;
            }

            if (passwordTxtBx.Text.All(char.IsLetter))
            {
                ToastNotification.ToastMessage("Password needs a mix of characters");
                return;
            }

            //Put password into char array to check for sequence
            char[] charSplit = passwordTxtBx.Text.ToCharArray();
            for (int i = 0; i < charSplit.Length; i++)
            {   //Looking back I'm sure theres a better way to check for a sequence
                    if (i < charSplit.Length - 3)
                    {
                        if (charSplit[i] == charSplit[i + 1] && charSplit[i + 1] == charSplit[i + 2]) //aaa
                        {
                            ToastNotification.ToastMessage("Password can't have repeated characters");
                            return;
                        }
                    }
                    if (i < charSplit.Length - 2)
                    {
                        if (charSplit[i] == charSplit[i + 1]) //aa
                        {
                            ToastNotification.ToastMessage("Password can't have repeated characters");
                            return;
                        }
                    }

                    if (i < charSplit.Length - 4)
                    {
                        if ((charSplit[i] + charSplit[i + 1]) == (charSplit[i + 2] + charSplit[i + 3])) //abab
                        {
                            ToastNotification.ToastMessage("Password can't have repeated characters");
                            return;
                        }
                    }
                    
                    if (charSplit.Length >= 6 && i < charSplit.Length - 6)
                    {
                        if ((charSplit[i] + charSplit[i + 1] + charSplit[i + 2]) == (charSplit[i + 3] + charSplit[i + 4] + charSplit[i + 5])) //abcabc
                        {
                            ToastNotification.ToastMessage("Password can't have repeated characters");
                            return;
                        }
                    }
            }

            SaveNewEntry(userTxtBx.Text, passwordTxtBx.Text);
        }

        private void SaveNewEntry(String user, String pass)
        {
            //Save entries to file, missing DB?!
            //Confused a bit by why this needs System.IO
            String newEntry = "";
            if (System.IO.File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/saved.txt"))
            {
                newEntry = System.IO.File.ReadAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/saved.txt");
                string[] oldArray = newEntry.Split(new string[] { "/r" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < oldArray.Length; i++)
                {   //Need to add array of prohibited chars later
                    string[] splitOld = oldArray[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitOld[0] == user)  //Can have same password but not user name
                    {
                        ToastNotification.ToastMessage("Entry already exists");  //Crude confirmation I know....
                        return;
                    }
                }
            }

            newEntry += user + "," + pass + "/r";  //Put in check to prevent same entry multiple times
            System.IO.File.WriteAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/saved.txt", newEntry);
            ToastNotification.ToastMessage("Entry created");
        }

        //Custom Toast notification
        public static class ToastNotification
        {
            public static void ToastMessage(string message)
            {
                var context = Android.App.Application.Context;
                var toastMessage = message;
                var duration = ToastLength.Long;

                //Make global class to add this in?
                Toast.MakeText(context, toastMessage, duration).Show();
            }
        }
    }
}