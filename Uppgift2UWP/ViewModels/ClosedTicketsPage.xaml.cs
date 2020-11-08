using System;
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
using Uppgift2UWP.Models;
using System.Collections.ObjectModel; 

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Uppgift2UWP.ViewModels
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClosedTicketsPage : Page
    {
        public ObservableCollection<Ticket> Tickets { get; set; }
        public ClosedTicketsPage()
        {
            this.InitializeComponent();

            Ticket.TicketViewModel ticketViewModel = new Ticket.TicketViewModel();

            ticketViewModel.GetClosed();
            Tickets = ticketViewModel.Tickets;

        }
    }
}
