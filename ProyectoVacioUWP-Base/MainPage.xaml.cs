using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace ProyectoVacioUWP_Base
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            // Ajuste de ventana profesional
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(400, 400));

            // Carga inicial
            frCentral.Navigate(typeof(HomePage), this);

            NavView.SelectedItem = NavView.MenuItems[0];

            SystemNavigationManager.GetForCurrentView().BackRequested += OpcionVolver;
            frCentral.Navigated += FrCentral_Navigated;
        }

        private void FrCentral_Navigated(object sender, NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                frCentral.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private void OpcionVolver(object sender, BackRequestedEventArgs e)
        {
            if (frCentral.CanGoBack)
            {
                e.Handled = true;
                frCentral.GoBack();
            }
        }

        private void NavViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var item = sender as NavigationViewItem;
            if (item == null || frCentral == null) return;

            var tag = item.Tag.ToString();
            Type destino = null;
            object parametro = this;

            switch (tag)
            {
                case "Inicio": destino = typeof(HomePage); break;
                case "Pokedex": destino = typeof(PokedexPage); break;
                case "Combate": destino = typeof(SeleccionPage); parametro = false; break;
                case "CombateIA": destino = typeof(SeleccionPage); parametro = true; break;
                case "AcercaDe": destino = typeof(AcercaDePage); break;
            }

            if (destino != null && frCentral.CurrentSourcePageType != destino)
            {
                frCentral.Navigate(destino, parametro);

                // Si estamos en móvil/compacto, cerramos el panel tras elegir
                if (NavView.PaneDisplayMode == NavigationViewPaneDisplayMode.Left)
                    NavView.IsPaneOpen = false;
            }
        }
    }
}