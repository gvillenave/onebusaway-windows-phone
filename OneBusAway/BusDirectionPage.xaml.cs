﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using OneBusAway.WP7.ViewModel;
using OneBusAway.WP7.ViewModel.BusServiceDataStructures;
using System.Windows.Navigation;
using System.Collections.Specialized;

namespace OneBusAway.WP7.View
{
    public partial class BusDirectionPage : PhoneApplicationPage
    {
        private BusDirectionVM viewModel;
        private bool informationLoaded;

        public BusDirectionPage()
        {
            InitializeComponent();

            viewModel = Resources["ViewModel"] as BusDirectionVM;

            informationLoaded = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            viewModel.RegisterEventHandlers();

            // This prevents us from refreshing the data when someone comes back to this page
            // since the bus directions aren't going to change
            if (informationLoaded == false)
            {
                viewModel.LoadRouteDirections(ViewState.CurrentRoute);
                informationLoaded = true;
            }
            else
            {
                // If the information was already loaded clear the selection
                // This way if they navigated back to this page one entry
                // won't already be selected
                BusDirectionListBox.SelectedIndex = -1;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            viewModel.UnregisterEventHandlers();
        }

        private void BusDirectionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ViewState.CurrentRouteDirection = (RouteStops)e.AddedItems[0];

                ViewState.CurrentStop = ViewState.CurrentRouteDirection.stops[0];
                foreach (Stop stop in ViewState.CurrentRouteDirection.stops)
                {
                    if (ViewState.CurrentStop.CalculateDistanceInMiles(MainPage.CurrentLocation) > stop.CalculateDistanceInMiles(MainPage.CurrentLocation))
                    {
                        ViewState.CurrentStop = stop;
                    }
                }

                NavigationService.Navigate(new Uri("/DetailsPage.xaml", UriKind.Relative));
            }
        }

    }
}