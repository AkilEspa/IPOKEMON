using ProyectoUWPenBlanco;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace ProyectoVacioUWP_Base
{
    public sealed partial class SeleccionPage : Page
    {
        private List<PokemonSeleccion> _catalogo;
        private List<Border> _listaBordes = new List<Border>();
        private int _indexJ1 = 0, _indexJ2 = 0;
        private bool _listoJ1 = false, _listoJ2 = false;
        private Random _rnd = new Random();
        private Type _randomP1 = null, _randomP2 = null;
        private bool _esModoIA = false;

        private readonly int[,] _grid = new int[5, 5]
        {
            { -1, -1,  0, -1, -1 },
            { -1,  1,  2,  3, -1 },
            {  4,  5,  6,  7,  8 },
            {  9, 10, 11, 12, 13 },
            { -1, 14, 15, 16, -1 }
        };

        private int _f1 = 0, _c1 = 2, _f2 = 0, _c2 = 2;

        public SeleccionPage()
        {
            this.InitializeComponent();
            CargarDatos();
            Window.Current.CoreWindow.KeyDown += OnKeyDown;
            this.Loaded += (s, e) => { this.Focus(FocusState.Programmatic); };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(1100, 850));

            if (e.Parameter is bool esIA) _esModoIA = esIA;

            if (_esModoIA)
            {
                txtP2.Text = "IA: [FLECHAS] + ENTER";
            }
            else
            {
                txtP2.Text = "P2: [FLECHAS] + ENTER";
            }

            ActualizarVisual();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 320));
            Window.Current.CoreWindow.KeyDown -= OnKeyDown; // Importante desuscribirse
        }

        private void CargarDatos()
        {
            _catalogo = new List<PokemonSeleccion>
            {
                new PokemonSeleccion("?", "ms-appx:///Assets/random.png", null),
                new PokemonSeleccion("Computerror", "ms-appx:///Assets/computerror.png", typeof(ComputerrorDLM)),
                new PokemonSeleccion("Ditto", "ms-appx:///Assets/ditto.png", typeof(DittoAEJ)),
                new PokemonSeleccion("Dragonite", "ms-appx:///Assets/dragonite.png", typeof(DragoniteDGM)),
                new PokemonSeleccion("Empoleon", "ms-appx:///Assets/empoleon.png", typeof(EmpoleonARS)),
                new PokemonSeleccion("Fennekin", "ms-appx:///Assets/fennekin.png", typeof(FennekinRRM)),
                new PokemonSeleccion("Gengar", "ms-appx:///Assets/gengar.png", typeof(GengarMBGCS)),
                new PokemonSeleccion("Hank", "ms-appx:///Assets/hank.png", typeof(HankRCC)),
                new PokemonSeleccion("Hoothoot", "ms-appx:///Assets/hoothoot.png", typeof(HoothootJDCF)),
                new PokemonSeleccion("Marill", "ms-appx:///Assets/marill.png", typeof(MarillADO)),
                new PokemonSeleccion("Monferno", "ms-appx:///Assets/monferno.png", typeof(MonfernoCRS)),
                new PokemonSeleccion("Psyduck", "ms-appx:///Assets/psyduck.png", typeof(PsyduckADM)),
                new PokemonSeleccion("Rotom", "ms-appx:///Assets/rottom.png", typeof(RotomXYZ)),
                new PokemonSeleccion("Squirtle", "ms-appx:///Assets/squirtle.png", typeof(SquirtleMRGM)),
                new PokemonSeleccion("Torchic", "ms-appx:///Assets/torchic.png", typeof(TorchicJFBR)),
                new PokemonSeleccion("Unown", "ms-appx:///Assets/unown.png", typeof(UnownOVG)),
                new PokemonSeleccion("Zapdos", "ms-appx:///Assets/zapdos.png", typeof(ZapdosRGS))
            };
            DistribuirEnDiamante();
        }

        private void DistribuirEnDiamante()
        {
            for (int i = 0; i < _catalogo.Count; i++)
            {
                var border = new Border
                {
                    BorderThickness = new Thickness(3),
                    BorderBrush = new SolidColorBrush(Colors.Transparent),
                    Background = new SolidColorBrush(Color.FromArgb(35, 255, 255, 255)),
                    CornerRadius = new CornerRadius(10),
                    Margin = new Thickness(2),
                    Width = 75, // Tamaño fijo para que el diamante sea estable
                    Height = 75,
                    Child = new Image { Source = new BitmapImage(new Uri(_catalogo[i].RutaCara)), Stretch = Stretch.UniformToFill }
                };
                _listaBordes.Add(border);

                if (i == 0) Fila1.Items.Add(border);
                else if (i <= 3) Fila2.Items.Add(border);
                else if (i <= 8) Fila3.Items.Add(border);
                else if (i <= 13) Fila4.Items.Add(border);
                else Fila5.Items.Add(border);
            }
        }

        private void OnKeyDown(CoreWindow sender, KeyEventArgs e)
        {
            if (_listoJ1 && _listoJ2) return;
            var k = e.VirtualKey;

            if (!_listoJ1)
            {
                int ant = _indexJ1;
                _indexJ1 = ProcesarMovimiento(k, true);
                if (k == VirtualKey.Space) _listoJ1 = true;
                if (ant != _indexJ1 || k == VirtualKey.Space) ActualizarVisual();
            }

            if (!_listoJ2)
            {
                int ant = _indexJ2;
                _indexJ2 = ProcesarMovimiento(k, false);
                if (k == VirtualKey.Enter) _listoJ2 = true;
                if (ant != _indexJ2 || k == VirtualKey.Enter) ActualizarVisual();
            }

            if (_listoJ1 && _listoJ2)
            {
                Comenzar();
            }
        }

        private int ProcesarMovimiento(VirtualKey key, bool esJ1)
        {
            int f = esJ1 ? _f1 : _f2;
            int c = esJ1 ? _c1 : _c2;
            int nf = f, nc = c;

            bool up = esJ1 ? key == VirtualKey.W : key == VirtualKey.Up;
            bool down = esJ1 ? key == VirtualKey.S : key == VirtualKey.Down;
            bool left = esJ1 ? key == VirtualKey.A : key == VirtualKey.Left;
            bool right = esJ1 ? key == VirtualKey.D : key == VirtualKey.Right;

            if (up) nf--;
            else if (down) nf++;
            else if (left) nc--;
            else if (right) nc++;
            else return _grid[f, c];

            if (nf < 0 || nf > 4 || nc < 0 || nc > 4 || _grid[nf, nc] == -1) return _grid[f, c];
            if (esJ1) { _f1 = nf; _c1 = nc; } else { _f2 = nf; _c2 = nc; }
            return _grid[nf, nc];
        }

        private void ActualizarVisual()
        {
            for (int i = 0; i < _listaBordes.Count; i++)
            {
                if (i == _indexJ1 && i == _indexJ2) _listaBordes[i].BorderBrush = new SolidColorBrush(Colors.Purple);
                else if (i == _indexJ1) _listaBordes[i].BorderBrush = new SolidColorBrush(Colors.Cyan);
                else if (i == _indexJ2) _listaBordes[i].BorderBrush = new SolidColorBrush(Colors.Red);
                else _listaBordes[i].BorderBrush = new SolidColorBrush(Colors.Transparent);
            }
            ActualizarCapaPrevia(1, _indexJ1, ContenedorJ1, txtNombreJ1, txtEstadoJ1, _listoJ1);
            ActualizarCapaPrevia(2, _indexJ2, ContenedorJ2, txtNombreJ2, txtEstadoJ2, _listoJ2);
        }

        private void ActualizarCapaPrevia(int jugador, int index, ContentControl contenedor, TextBlock txtNombre, TextBlock txtEstado, bool listo)
        {
            var pSel = _catalogo[index];
            Type tFinal = pSel.ClasePokemon;

            if (index == 0)
            {
                txtNombre.Text = "ALEATORIO";
                if (jugador == 1) { if (_randomP1 == null) _randomP1 = _catalogo[_rnd.Next(1, _catalogo.Count)].ClasePokemon; tFinal = _randomP1; }
                else { if (_randomP2 == null) _randomP2 = _catalogo[_rnd.Next(1, _catalogo.Count)].ClasePokemon; tFinal = _randomP2; }
            }
            else
            {
                txtNombre.Text = pSel.Nombre.ToUpper();
                if (jugador == 1) _randomP1 = null; else _randomP2 = null;
            }

            contenedor.Content = null;
            if (index == 0 && !listo)
            {
                contenedor.Content = new Viewbox { Child = new TextBlock { Text = "?", FontSize = 100, Foreground = new SolidColorBrush(Colors.White), Opacity = 0.6 } };
            }
            else if (tFinal != null)
            {
                try
                {
                    var uc = (UserControl)Activator.CreateInstance(tFinal);

                    if (uc is iPokemon ip)
                    {
                        // SOLO SILENCIAMOS SI ES DITTO O GENGAR
                        // Esto evita errores si otros pokemons no implementan bien la interfaz aún
                        if (uc is DittoAEJ || uc is GengarMBGCS)
                        {
                            ip.silenciar(true);
                        }

                        ip.verFilaVida(false);
                        ip.verFilaEnergia(false);
                        ip.verFondo(false);
                        ip.verPocionEnergia(false);
                        ip.verPocionVida(false);
                        ip.activarAniIdle(true);
                    }
                    contenedor.Content = new Viewbox { Stretch = Stretch.Uniform, Child = uc };
                }
                catch { /* Fallback */ }
            }
        

            txtNombre.Foreground = listo ? new SolidColorBrush(Colors.SpringGreen) : (jugador == 1 ? new SolidColorBrush(Colors.Cyan) : new SolidColorBrush(Colors.Red));
            string prefijo = (jugador == 2 && _esModoIA) ? "IA" : $"J{jugador}";
            txtEstado.Text = listo ? $"{prefijo}: ELEGIDO" : $"{prefijo}: ELIGIENDO...";
            txtEstado.Foreground = listo ? new SolidColorBrush(Colors.SpringGreen) : new SolidColorBrush(Colors.White);
        }

        private void Comenzar()
        {
            Type p1 = (_indexJ1 == 0) ? _randomP1 : _catalogo[_indexJ1].ClasePokemon;
            Type p2 = (_indexJ2 == 0) ? _randomP2 : _catalogo[_indexJ2].ClasePokemon;
            Frame.Navigate(typeof(CombatePage), new object[] { p1, p2, _esModoIA });
        }
    }

    public class PokemonSeleccion
    {
        public string Nombre { get; set; }
        public string RutaCara { get; set; }
        public Type ClasePokemon { get; set; }
        public PokemonSeleccion(string n, string r, Type c)
        {
            Nombre = n;
            RutaCara = r;
            ClasePokemon = c;
        }
    }
}