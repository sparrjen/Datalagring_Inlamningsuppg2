﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Data.SqlClient;
using Uppgift2UWP.ViewModels;
using Uppgift2UWP.Models; 
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Uppgift2UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MainPage : Page
    {
        public Ticket.TicketViewModel ViewModel { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            DataAccess.ReadSettingsAsync();

        }

        private void MenuView_Loaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(AddTicketPage));
        }
        private void MenuView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;

                switch (item.Tag.ToString())
                {
                    case "AddTicketsPage":
                        ContentFrame.Navigate(typeof(AddTicketPage));
                        break;

                    case "OpenTicketsPage":
                        ContentFrame.Navigate(typeof(OpenTicketsPage));
                        break;

                    case "ClosedTicketsPage":
                        ContentFrame.Navigate(typeof(ClosedTicketsPage));
                        break;

                }

            }


        }
    }
}
