﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

using SRConnect.Models;
using SRConnect.Services;
using MvvmCross.ViewModels;

namespace SRConnect.ViewModels
{
    public class BaseViewModel : MvxViewModel
    {
        //public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        string title = string.Empty;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                RaisePropertyChanged(() => Title);
            }
        }
    }
}
