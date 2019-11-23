﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App4
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddDeposit : ContentPage
    {
        UserModel user;
        public AddDeposit(int id)
        {
            InitializeComponent();
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<UserModel>();
                user = conn.FindWithQuery<UserModel>("select * from UserModel where id=?", id);
            }
        }
        private void AddDeposit_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(EnteredDeposit.Text) == true)
                DisplayAlert("Error", "Please Enter A Valid Amount", "Retry");

            else
            {
                Nullable<double> amountDep = Convert.ToDouble(EnteredDeposit.Text);
                amountDep = Math.Round(amountDep.Value, 2);
                TransactionTable transaction = new TransactionTable()
                {
                    amount = Convert.ToDouble(amountDep),
                    type = "Deposit",
                    category = "",
                    UserID = user.Id,
                };
                transaction.shit = "Type: " + transaction.type; //"        Category: " + transaction.category;
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    conn.CreateTable<TransactionTable>();
                    conn.Insert(transaction);
                    Account acc = conn.FindWithQuery<Account>("Select* From Account Where uId=?", user.Id);
                    acc.dep += Convert.ToDouble(amountDep);
                    acc.bal += Convert.ToDouble(amountDep);
                    conn.Update(acc);
                }

                DisplayAlert("Deposited", amountDep.ToString(), "Okay");
                Navigation.PushAsync(new Navigation(user.Id));
            }
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ChooseTransaction(user.Id));
        }
    }
}