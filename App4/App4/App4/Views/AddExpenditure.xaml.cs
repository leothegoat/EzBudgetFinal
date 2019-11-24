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
    public partial class AddExpenditure : ContentPage
    {
        UserModel user;
        public AddExpenditure(int id)
        {
            InitializeComponent();
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<UserModel>();
                user = conn.FindWithQuery<UserModel>("select * from UserModel where id=?", id);
            }
        }
        private void AddExpense_Clicked(object sender, EventArgs e)
        {
            
            if (String.IsNullOrWhiteSpace(EnteredExpense.Text) == true)
                DisplayAlert("Error", "Please Enter A Valid Amount and Selected Category", "Retry");

            else{
                Nullable<double> amountEx = Convert.ToDouble(EnteredExpense.Text);
                Nullable<double> category = Convert.ToDouble(CategoryPicker.SelectedIndex);
                if (category < -.01 || category > 4.1)
                    DisplayAlert("Error", "Please Enter A Valid Amount and Selected Category", "Retry");
             
                else{
                    amountEx = Math.Round(amountEx.Value, 2);
                    var selected = CategoryPicker.SelectedItem;
                    var value = selected.ToString();
                    TransactionTable transaction = new TransactionTable()
                    {
                        amount = Convert.ToDouble(amountEx),
                        type = "Expense",
                        category = value,
                        UserID = user.Id,
                    };
                    transaction.shit = "Type: "+ transaction.type+"        Category: "+ transaction.category;
                    Account acc;
                    using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                    {
                        conn.CreateTable<TransactionTable>();
                        conn.Insert(transaction);
                        acc = conn.FindWithQuery<Account>("Select* From Account Where uId=?", user.Id);
                        acc.spent += Convert.ToDouble(amountEx);
                        acc.bal -= Convert.ToDouble(amountEx);
                        if (value == "Food")                        
                            acc.foodSpent+= Convert.ToDouble(amountEx);

                        else if (value == "Entertainment")
                            acc.entSpent += Convert.ToDouble(amountEx);
                        
                        else if (value == "Transportation")
                            acc.tranSpent += Convert.ToDouble(amountEx);
                        
                        else if (value == "Bills")
                            acc.billSpent += Convert.ToDouble(amountEx);
                        
                        else
                            acc.otherSpent += Convert.ToDouble(amountEx);
                        conn.Update(acc);
                    }
                    
                    DisplayAlert("Added Expense", "$"+amountEx.ToString(), "Okay");
                    Navigation.PushAsync(new Navigation(user.Id));
                }
            }
        }
        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ChooseTransaction(user.Id));
        }
    }
}