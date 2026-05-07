using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace ProyectoVacioUWP_Base
{
    public sealed partial class CombatePage : Page
    {
        private const double REGEN_ENERGIA = 0.8;
        private iPokemon P1, P2;
        private Dictionary<VirtualKey, bool> cooldownsActivos = new Dictionary<VirtualKey, bool>();
        private Dictionary<VirtualKey, double> tiemposRestantes = new Dictionary<VirtualKey, double>();
        private DispatcherTimer gameLoop;
        private bool p1Herido, p1Cansado, p2Herido, p2Cansado;
        private bool p1Protegido, p2Protegido, p1Ocupado, p2Ocupado;
        private bool juegoTerminado = false;
        private bool p1PocionVidaUsada, p1PocionEnergiaUsada, p2PocionVidaUsada, p2PocionEnergiaUsada;
        private bool _esModoIA = false;
        private DispatcherTimer _iaTimer;
        private Random _random = new Random();

        public CombatePage()
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.KeyDown += OnKeyDown;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            GestionarMenuLateral(false);
            CargarFondoAleatorio();
            ConfigurarMusica();

            if (e.Parameter is object[] parametros && parametros.Length == 3)
            {
                P1 = (iPokemon)Activator.CreateInstance((Type)parametros[0]);
                P2 = (iPokemon)Activator.CreateInstance((Type)parametros[1]);
                _esModoIA = (bool)parametros[2];

                ConfigurarVisualPokemon(P1);
                ConfigurarVisualPokemon(P2);

                lblNameP1.Text = P1.Nombre.ToUpper();
                lblNameP2.Text = P2.Nombre.ToUpper();

                ContenedorP1.Content = P1;
                ContenedorP2.Content = P2;

                P1.Vida = 100; P1.Energia = 100;
                P2.Vida = 100; P2.Energia = 100;

                InicializarSistemas();

                if (_esModoIA)
                {
                    _iaTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
                    _iaTimer.Tick += (s, ev) => EjecutarAtaqueIA();
                    _iaTimer.Start();
                }
            }
        }

        // --- SISTEMA DE AUDIO Y LIMPIEZA AL SALIR ---
        private void ConfigurarMusica()
        {
            try
            {
                musicaCombate.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/musica_combate.mp3"));
                var player = musicaCombate.MediaPlayer;
                player.IsLoopingEnabled = true;
                player.Volume = 0.2;
                player.Play();
            }
            catch { }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // 1. Detener todo lo que esté corriendo
            juegoTerminado = true;
            gameLoop?.Stop();
            _iaTimer?.Stop();

            // 2. Parar música de fondo
            if (musicaCombate.MediaPlayer != null)
                musicaCombate.MediaPlayer.Pause();

            // 3. Silenciar sonidos internos de los Pokémon (Gengar, Ditto, etc.)
            SilenciarPokemon(P1);
            SilenciarPokemon(P2);

            // 4. Desvincular evento de teclado para no interferir en otras páginas
            Window.Current.CoreWindow.KeyDown -= OnKeyDown;

            base.OnNavigatedFrom(e);
        }

        private void SilenciarPokemon(iPokemon poke)
        {
            if (poke == null) return;
            try
            {
                // Buscamos campos privados (como mediaPlayers o timers de sonido) dentro del UserControl del Pokémon
                var campos = poke.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                foreach (var campo in campos)
                {
                    // Si es un MediaPlayer, lo paramos y muteamos
                    if (campo.FieldType == typeof(MediaPlayer))
                    {
                        var mp = campo.GetValue(poke) as MediaPlayer;
                        if (mp != null) { mp.Pause(); mp.Source = null; }
                    }
                    // Si es un MediaElement (versión antigua), lo paramos
                    if (campo.FieldType == typeof(MediaElement))
                    {
                        var me = campo.GetValue(poke) as MediaElement;
                        me?.Stop();
                    }
                    // Si es un DispatcherTimer (como el de los sonidos aleatorios de Ditto), lo paramos
                    if (campo.FieldType == typeof(DispatcherTimer) || campo.Name.ToLower().Contains("timer"))
                    {
                        var timer = campo.GetValue(poke) as DispatcherTimer;
                        timer?.Stop();
                    }
                }
            }
            catch { /* Evitar errores si la reflexión falla */ }
        }

        // --- RESTO DE LÓGICA DE COMBATE ---
        private void CargarFondoAleatorio()
        {
            string[] fondos = { "fondo_bosque.jpg", "fondo_cueva.jpg", "fondo_estadio.jpg", "fondo_volcan.jpg" };
            int indice = _random.Next(fondos.Length);
            imgFondo.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{fondos[indice]}"));
        }

        private void GestionarMenuLateral(bool mostrar)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(this);
            while (parent != null && !(parent is SplitView)) parent = VisualTreeHelper.GetParent(parent);
            if (parent is SplitView sv)
            {
                sv.IsPaneOpen = false;
                sv.DisplayMode = mostrar ? SplitViewDisplayMode.CompactInline : SplitViewDisplayMode.Overlay;
                sv.CompactPaneLength = mostrar ? 48 : 0;
            }
        }

        private void ConfigurarVisualPokemon(iPokemon p)
        {
            p.verFilaVida(true);
            p.verFilaEnergia(true);
            p.verFondo(false);
            p.verNombre(false);
            p.verPocionVida(false);
            p.verPocionEnergia(false);
            p.activarAniIdle(true);
        }

        private void InicializarSistemas()
        {
            VirtualKey[] teclas = { VirtualKey.W, VirtualKey.A, VirtualKey.S, VirtualKey.D, VirtualKey.Up, VirtualKey.Left, VirtualKey.Down, VirtualKey.Right };
            foreach (var t in teclas) { cooldownsActivos[t] = true; tiemposRestantes[t] = 0; }

            gameLoop = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            gameLoop.Tick += (s, ev) => {
                if (juegoTerminado) return;
                GestionarEstadoVisual(P1, ref p1Herido, ref p1Cansado, p1Ocupado);
                GestionarEstadoVisual(P2, ref p2Herido, ref p2Cansado, p2Ocupado);
                if (P1.Energia < 100) P1.Energia += REGEN_ENERGIA;
                if (P2.Energia < 100) P2.Energia += REGEN_ENERGIA;
                ActualizarLabelsUI();
            };
            gameLoop.Start();
        }

        private void GestionarEstadoVisual(iPokemon p, ref bool h, ref bool c, bool ocupado)
        {
            if (ocupado || juegoTerminado) return;
            if (p.Vida < 50 && !h) { h = true; p.animacionHerido(); }
            else if (p.Vida >= 50 && h) { h = false; p.animacionNoHerido(); }
            if (p.Energia < 30 && !c) { c = true; p.animacionCansado(); }
            else if (p.Energia >= 50 && c) { c = false; p.animacionNoCansado(); }
        }

        private void OnKeyDown(CoreWindow sender, KeyEventArgs e)
        {
            if (juegoTerminado) return;
            var k = e.VirtualKey;
            if (k == VirtualKey.W) ProcesarAccion(P1, P2, k, "flojo", 15, 0.8, txtW);
            if (k == VirtualKey.A) ProcesarAccion(P1, P2, k, "fuerte", 40, 3.0, txtA);
            if (k == VirtualKey.S) ProcesarAccion(P1, P2, k, "escudo", 15, 0, txtS);
            if (k == VirtualKey.D) ProcesarAccion(P1, P2, k, "relax", 0, 5.0, txtD);
            if (k == VirtualKey.Q) UsarPocion(P1, "vida");
            if (k == VirtualKey.E) UsarPocion(P1, "energia");

            if (!_esModoIA)
            {
                if (k == VirtualKey.Up) ProcesarAccion(P2, P1, k, "flojo", 15, 0.8, txtUp);
                if (k == VirtualKey.Left) ProcesarAccion(P2, P1, k, "fuerte", 40, 3.0, txtLeft);
                if (k == VirtualKey.Down) ProcesarAccion(P2, P1, k, "escudo", 15, 0, txtDown);
                if (k == VirtualKey.Right) ProcesarAccion(P2, P1, k, "relax", 0, 5.0, txtRight);
                if (k == VirtualKey.O) UsarPocion(P2, "vida");
                if (k == VirtualKey.P) UsarPocion(P2, "energia");
            }
        }

        private void EjecutarAtaqueIA()
        {
            if (juegoTerminado || p2Ocupado) return;
            int accion = _random.Next(0, 6);
            switch (accion)
            {
                case 0: ProcesarAccion(P2, P1, VirtualKey.Up, "flojo", 15, 0.8, txtUp); break;
                case 1: ProcesarAccion(P2, P1, VirtualKey.Left, "fuerte", 40, 3.0, txtLeft); break;
                case 2: ProcesarAccion(P2, P1, VirtualKey.Down, "escudo", 15, 0, txtDown); break;
                case 3: ProcesarAccion(P2, P1, VirtualKey.Right, "relax", 0, 5.0, txtRight); break;
                case 4: if (!p2PocionVidaUsada && P2.Vida < 40) UsarPocion(P2, "vida"); break;
                case 5: if (!p2PocionEnergiaUsada && P2.Energia < 30) UsarPocion(P2, "energia"); break;
            }
        }

        private async void ProcesarAccion(iPokemon user, iPokemon target, VirtualKey k, string tipo, double coste, double cd, TextBlock txt)
        {
            bool ocupado = (user == P1) ? p1Ocupado : p2Ocupado;
            if (ocupado || !cooldownsActivos[k] || user.Energia < coste) return;
            SetOcupado(user, true);
            user.Energia -= coste;
            switch (tipo)
            {
                case "flojo": user.animacionAtaqueFlojo(); AplicarDano(user, target, 10); break;
                case "fuerte": user.animacionAtaqueFuerte(); AplicarDano(user, target, 25); break;
                case "escudo":
                    if (user == P1) p1Protegido = true; else p2Protegido = true;
                    user.verEscudo(true);
                    txt.Opacity = 0.4;
                    cooldownsActivos[k] = false;
                    break;
                case "relax": user.Vida += 15; user.Energia += 25; user.animacionDescasar(); break;
            }
            if (tipo != "escudo") IniciarCooldownIndividual(k, cd, txt);
            await Task.Delay(1000);
            if (!juegoTerminado) SetOcupado(user, false);
        }

        private async void AplicarDano(iPokemon user, iPokemon target, double cantidad)
        {
            bool prot = (target == P1) ? p1Protegido : p2Protegido;
            if (prot) { RomperEscudo(target); }
            else
            {
                target.Vida -= cantidad;
                if (target.Vida <= 0) { target.Vida = 0; FinalizarCombate(target, user); return; }
                SetOcupado(target, true);
                await Task.Delay(600);
                if (!juegoTerminado) SetOcupado(target, false);
            }
        }

        private void RomperEscudo(iPokemon target)
        {
            if (target == P1) { p1Protegido = false; txtS.Opacity = 1.0; cooldownsActivos[VirtualKey.S] = true; }
            else { p2Protegido = false; txtDown.Opacity = 1.0; cooldownsActivos[VirtualKey.Down] = true; }
            target.verEscudo(false);
        }

        private async void FinalizarCombate(iPokemon perdedor, iPokemon ganador)
        {
            juegoTerminado = true;
            gameLoop?.Stop();
            _iaTimer?.Stop();
            if (musicaCombate.MediaPlayer != null) musicaCombate.MediaPlayer.Pause();

            SilenciarPokemon(perdedor);

            perdedor.animacionDerrota();
            txtConsola.Text = "¡K.O.!";
            await Task.Delay(1500);

            txtNombreGanador.Text = ganador.Nombre.ToUpper();
            var visualGanador = (iPokemon)Activator.CreateInstance(ganador.GetType());
            visualGanador.verFilaVida(false);
            visualGanador.verFilaEnergia(false);
            visualGanador.verFondo(false);
            visualGanador.activarAniIdle(true);
            ContenedorGanador.Content = visualGanador;

            GridVictoria.Visibility = Visibility.Visible;

            await Task.Delay(5000);
            GestionarMenuLateral(true);
            if (this.Frame.CanGoBack) this.Frame.GoBack();
        }

        private void UsarPocion(iPokemon p, string tipo)
        {
            if (p == P1)
            {
                if (tipo == "vida" && !p1PocionVidaUsada) { P1.Vida += 40; p1PocionVidaUsada = true; txtQ.Opacity = 0.2; }
                else if (tipo == "energia" && !p1PocionEnergiaUsada) { P1.Energia += 50; p1PocionEnergiaUsada = true; txtE.Opacity = 0.2; }
            }
            else
            {
                if (tipo == "vida" && !p2PocionVidaUsada) { P2.Vida += 40; p2PocionVidaUsada = true; txtO.Opacity = 0.2; }
                else if (tipo == "energia" && !p2PocionEnergiaUsada) { P2.Energia += 50; p2PocionEnergiaUsada = true; txtP.Opacity = 0.2; }
            }
            if (P1.Vida > 100) P1.Vida = 100; if (P2.Vida > 100) P2.Vida = 100;
            if (P1.Energia > 100) P1.Energia = 100; if (P2.Energia > 100) P2.Energia = 100;
        }

        private void SetOcupado(iPokemon p, bool estado) { if (p == P1) p1Ocupado = estado; else p2Ocupado = estado; }

        private async void IniciarCooldownIndividual(VirtualKey k, double seg, TextBlock txt)
        {
            cooldownsActivos[k] = false; tiemposRestantes[k] = seg; txt.Opacity = 0.4;
            while (tiemposRestantes[k] > 0 && !juegoTerminado)
            {
                await Task.Delay(100);
                tiemposRestantes[k] -= 0.1;
            }
            if (!juegoTerminado) { cooldownsActivos[k] = true; txt.Opacity = 1.0; }
        }

        private void ActualizarLabelsUI()
        {
            ActualizarTexto(VirtualKey.W, txtW, "[W] FLOJO");
            ActualizarTexto(VirtualKey.A, txtA, "[A] FUERTE");
            ActualizarTexto(VirtualKey.S, txtS, "[S] ESCUDO");
            ActualizarTexto(VirtualKey.D, txtD, "[D] RELAX");
            ActualizarTexto(VirtualKey.Up, txtUp, "[↑] FLOJO");
            ActualizarTexto(VirtualKey.Left, txtLeft, "[←] FUERTE");
            ActualizarTexto(VirtualKey.Down, txtDown, "[↓] ESCUDO");
            ActualizarTexto(VirtualKey.Right, txtRight, "[→] RELAX");
        }

        private void ActualizarTexto(VirtualKey k, TextBlock txt, string baseT)
        {
            if (tiemposRestantes.ContainsKey(k) && tiemposRestantes[k] > 0)
                txt.Text = $"{baseT} | {Math.Max(0, tiemposRestantes[k]):F1}s";
            else txt.Text = baseT;
        }
    }
}