﻿using System;
using BlApi;
using BO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Windows
{
    /// <summary>
    /// Interaction logic for Package.xaml
    /// </summary>
    public partial class Package : Window
    {
        IBL bl = BlFactory.GetBl();

        public Window Sender { get; set; }
        public PO.Package POPackage { get; set; }
        public Model Model { get; } = PL.Model.Instance;

        /// <summary>
        /// Consructor for drone display window.
        /// </summary>
        /// <param name="sender">The element that activates the function</param>
        public Package(Window sender)
        {
            this.Sender = sender;
            InitializeComponent();


            MainGrid.ShowGridLines = true;
            AddDownGrid.Visibility = Visibility.Visible;
            senderCustomerInPackage.Visibility = Visibility.Visible;
            targetCustomerInPackage.Visibility = Visibility.Visible;
            Weight.Visibility = Visibility.Visible;
            priority.Visibility = Visibility.Visible;

        }

        /// <summary>
        /// constructor to window add drone.
        /// </summary>
        /// <param name="sender">The element that activates the function</param>
        public Package(Window sender, PO.Package package)
        {
            this.Sender = sender;
            this.POPackage = package;

            InitializeComponent();

            AddDownInfoGrid.Visibility = Visibility.Visible;
            PackageInfoDownGrid.Visibility = Visibility.Visible;
            SenderCustomerInPackageInfo.Visibility = Visibility.Visible;
            TargetCustomerInPackageInfo.Visibility = Visibility.Visible;
            WeightInfo.Visibility = Visibility.Visible;
            PriorityInfo.Visibility = Visibility.Visible;
            DeleteGrid.Visibility = Visibility.Visible;

            this.Height = 550;
            this.Width = 1300;

            this.Sender.Closed += Sender_Closed;

        }

        private void Sender_Closed(object sender, EventArgs e)
        {
            cancel_Click(sender, null);
        }

        /// <summary>
        /// A button that alerts if the user has entered characters rather than numbers.
        /// </summary>
        /// <param name="sender">The element that activates the function</param>
        /// <param name="e"></param>
        private void IntTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //Checks that entered numbers only
            if (e.Handled = !(int.TryParse(e.Text, out int d)) && e.Text != "" && d <= 0)
                MessageBox.Show("Please enter only positive numbers.");

        }

        /// <summary>
        /// Window Close Button.
        /// </summary>
        /// <param name="sender">The element that activates the function</param>
        /// <param name="e"></param>
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Add drone button
        /// </summary>
        /// <param name="sender">The element that activates the function</param>
        /// <param name="e"></param>
        private void add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (senderCustomerInPackage.SelectedItem != null && targetCustomerInPackage.SelectedItem != null && Weight.SelectedItem != null && priority.SelectedItem != null)
                {
                    int senderId = ((CustomerToList)senderCustomerInPackage.SelectedItem).CustomerId;
                    int targetId = ((CustomerToList)targetCustomerInPackage.SelectedItem).CustomerId;

                    try
                    {
                        bl.AddPackage(new()
                        {
                            SenderCustomerInPackage = new() { CustomerId = senderId },
                            TargetCustomerInPackage = new() { CustomerId = targetId },
                            Weight = (Weight)Weight.SelectedItem,
                            Priority = (Priorities)priority.SelectedItem,
                        });
                    }
                    catch (NotValidTargetException ex) { MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error); }


                    Model.UpdatePackages();

                        Model.POCustomers.Find(cus => cus.Id == senderId)?.CopyFromBOCustomer(bl.GetCustomer(senderId));
                        Model.POCustomers.Find(cus => cus.Id == targetId)?.CopyFromBOCustomer(bl.GetCustomer(targetId));

                    Model.UpdateCustomers();

                    MessageBox.Show("Adding the package was completed successfully!", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                    MessageBox.Show("There are unfilled fields", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (NoNumberFoundException ex) { MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (ExistsNumberException ex) { MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error); }


        }


        /// <summary>
        /// Sets that by double-clicking a skimmer from the list it will see the data on the skimmer.
        /// </summary>
        /// <param name="sender">The element that activates the function</param>
        /// <param name="e"></param>
        private void DroneInPackageInfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.DataContext != null)
            {
                if ((textBox.DataContext as BO.DroneInPackage) != null)
                {
                    BO.Drone BODrone = bl.GetDrone((textBox.DataContext as DroneInPackage).Id);
                    PO.Drone PODrone = Model.PODrones.Find(dr => dr.Id == BODrone.Id);
                    if (PODrone == null)
                        Model.PODrones.Add(PODrone = new PO.Drone().CopyFromBODrone(BODrone));

                    new Drone(this, PODrone).Show();
                }
                else
                    MessageBox.Show("No element exists", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void CustomerInPackageInfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (((TextBox)sender).DataContext != null)
            {
                BO.Customer BOCustomer = bl.GetCustomer((((TextBox)sender).DataContext as BO.CustomerInPackage).CustomerId);
                PO.Customer POCustomer = Model.POCustomers.Find(cus => cus.Id == BOCustomer.Id);
                if (POCustomer == null)
                    Model.POCustomers.Add(POCustomer = new PO.Customer().CopyFromBOCustomer(BOCustomer));

                new Customer(this, POCustomer).Show();
            }
            else
                MessageBox.Show("No element exists", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                bl.DeletePackage(POPackage.Id);


                int senderId = POPackage.SenderCustomerInPackage.CustomerId;
                int targetId = POPackage.TargetCustomerInPackage.CustomerId;
                Model.UpdatePackages();



                Model.POCustomers.Find(cus => cus.Id == senderId)?.CopyFromBOCustomer(bl.GetCustomer(senderId));
                Model.POCustomers.Find(cus => cus.Id == targetId)?.CopyFromBOCustomer(bl.GetCustomer(targetId));

                Model.UpdateCustomers();

            }
            catch (NoNumberFoundException ex) { MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (PakcageConnectToDroneException ex) { MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error); }

            this.Close();
        }

   

        ///// <summary>
        ///// Button for sending drone for charging and release from charging according to the status of the drone.
        ///// </summary>
        ///// <param name="sender">The element that activates the function</param>
        ///// <param name="e"></param>
        //private void Charge_Click(object sender, RoutedEventArgs e)
        //{
        //    switch (drone.DroneStatus)
        //    {
        //        case DroneStatuses.Available:
        //            try
        //            {
        //                bl.SendDroneForCharge(drone.Id);
        //                drone.CopyFromBODrone(bl.GetDrone(drone.Id));
        //                this.sender.Filtering();
        //                MessageBox.Show("Sent for charging", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);

        //            }
        //            catch (NotEnoughBattery ex)
        //            {
        //                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }

        //            break;
        //        case DroneStatuses.Maintenance:
        //            bl.RealeseDroneFromCharge(drone.Id);
        //            drone.CopyFromBODrone(bl.GetDrone(drone.Id));
        //            this.sender.Filtering();
        //            MessageBox.Show("Released from charging", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);


        //            break;
        //    }
        //}
        ///// <summary>
        ///// A button that handles the delivery of the package according to the status of the drone.
        ///// </summary>
        ///// <param name="sender">The element that activates the function</param>
        ///// <param name="e"></param>
        //private void Delivery_Click(object sender, RoutedEventArgs e)
        //{
        //    switch (drone.DroneStatus)
        //    {
        //        case DroneStatuses.Available:
        //            try
        //            {
        //                bl.packageAssigning(drone.Id);
        //                drone.CopyFromBODrone(bl.GetDrone(drone.Id));
        //                this.sender.Filtering();
        //                MessageBox.Show("The package was successfully associated", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);


        //            }
        //            catch (NoSuitablePackageForScheduledException ex) { MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error); }
        //            break;
        //        case DroneStatuses.Sendering:
        //            if (drone.PackageInProgress.IsCollected)
        //            {
        //                bl.Deliver(drone.Id);
        //                drone.CopyFromBODrone(bl.GetDrone(drone.Id));
        //                this.sender.Filtering();
        //                MessageBox.Show("The package was delivered to its destination, good day", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);

        //            }
        //            else
        //            {
        //                bl.PickUp(drone.Id);
        //                drone.CopyFromBODrone(bl.GetDrone(drone.Id));
        //                this.sender.Filtering();
        //                MessageBox.Show("The package was successfully collected by the drone", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);

        //            }
        //            break;
        //    }
        //}
    }
}
