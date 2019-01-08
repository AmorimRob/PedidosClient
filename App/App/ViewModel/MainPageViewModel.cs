using Microsoft.AspNetCore.SignalR.Client;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace App.ViewModel
{
    public class MainPageViewModel : BaseViewModel
    {
        private Pedido _pedido;

        public Pedido Pedido
        {
            get { return _pedido; }
            set { _pedido = value; OnPropertyChanged(); }
        }

        private ObservableRangeCollection<Pedido> _pedidos;

        public ObservableRangeCollection<Pedido> Pedidos
        {
            get { return _pedidos; }
            set { _pedidos = value; OnPropertyChanged(); }
        }

        public ICommand EnviarPedidoCommand { get; set; }

        HubConnection hubConnection;

        public MainPageViewModel()
        {
            Pedidos = new ObservableRangeCollection<Pedido>();
            Pedido = new Pedido();

            var ip = "localhost";

            if (Device.RuntimePlatform == Device.Android)
                ip = "10.0.2.2";

            hubConnection = new HubConnectionBuilder()
                .WithUrl($"http://{ip}:5000/pedidosHub")
                .Build();


            hubConnection.On<string, Pedido>("ReceberPedido", (user, pedido) =>
            {
                if(user == "Test")
                    Pedidos.Add(pedido);
            });

            EnviarPedidoCommand = new Command(async () =>
            {
                await EnviarPedido(Pedido.Nome, Pedido);
            });
        }

        
        public async Task EnviarPedido(string user, Pedido pedido)
        {
            try
            {
                await hubConnection.InvokeAsync("EnviarPedido", user, pedido);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task Connect()
        {
            try
            {
                await hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task Disconnect()
        {
            try
            {
                await hubConnection.StopAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); ;
            }
         
        }
    }
}
