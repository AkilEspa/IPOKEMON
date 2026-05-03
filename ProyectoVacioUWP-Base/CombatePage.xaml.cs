using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ProyectoVacioUWP_Base
{
    public sealed partial class CombatePage : Page
    {
        private const double REGEN_ENERGIA = 1.2;
        private iPokemon P1, P2;
        private Dictionary<VirtualKey, bool> cooldownsActivos = new Dictionary<VirtualKey, bool>();
        private Dictionary<VirtualKey, double> tiemposRestantes = new Dictionary<VirtualKey, double>();
        private DispatcherTimer gameLoop, uiTimer;
        private bool p1Herido, p1Cansado, p2Herido, p2Cansado;
        private bool p1Protegido, p2Protegido, p1Ocupado, p2Ocupado;
        private bool juegoTerminado = false;
        private bool p1PocionVidaUsada, p1PocionEnergiaUsada, p2PocionVidaUsada, p2PocionEnergiaUsada;
        private bool _esModoIA = false;
        private DispatcherTimer _iaTimer;
        private Random _iaRandom = new Random();

        public CombatePage()
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.KeyDown += OnKeyDown;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            GestionarMenuLateral(false);

            // NUEVA FORMA DE RECIBIR DATOS:
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

                // INICIALIZAR IA
                if (_esModoIA)
                {
                    txtJugador2.Text = "INTELIGENCIA ARTIFICIAL";
                    _iaTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                    _iaTimer.Tick += (s, ev) => EjecutarAtaqueIA();
                    _iaTimer.Start();
                }
            }
        }

        private void GestionarMenuLateral(bool mostrar)
        {
            // Intentamos encontrar el SplitView en la estructura de la App
            DependencyObject parent = VisualTreeHelper.GetParent(this);
            while (parent != null && !(parent is SplitView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            if (parent is SplitView sv)
            {
                if (!mostrar)
                {
                    sv.IsPaneOpen = false;
                    sv.DisplayMode = SplitViewDisplayMode.Overlay;
                    sv.CompactPaneLength = 0; // Esto hace que el menú lateral desaparezca por completo
                }
                else
                {
                    sv.DisplayMode = SplitViewDisplayMode.CompactInline;
                    sv.CompactPaneLength = 48; // O el ancho que tengas por defecto
                }
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

            gameLoop = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            gameLoop.Tick += (s, ev) => {
                if (juegoTerminado) return;
                GestionarEstadoVisual(P1, ref p1Herido, ref p1Cansado, p1Ocupado);
                GestionarEstadoVisual(P2, ref p2Herido, ref p2Cansado, p2Ocupado);
                if (P1.Energia < 100) P1.Energia += REGEN_ENERGIA;
                if (P2.Energia < 100) P2.Energia += REGEN_ENERGIA;
            };
            gameLoop.Start();

            uiTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            uiTimer.Tick += (s, ev) => ActualizarLabelsUI();
            uiTimer.Start();
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

            // Controles J1 (Siempre activos)
            if (k == VirtualKey.W) ProcesarAccion(P1, P2, k, "flojo", 15, 0.8, txtW);
            if (k == VirtualKey.A) ProcesarAccion(P1, P2, k, "fuerte", 40, 3.0, txtA);
            if (k == VirtualKey.S) ProcesarAccion(P1, P2, k, "escudo", 15, 0, txtS);
            if (k == VirtualKey.D) ProcesarAccion(P1, P2, k, "relax", 0, 5.0, txtD);
            if (k == VirtualKey.Q) UsarPocion(P1, "vida");
            if (k == VirtualKey.E) UsarPocion(P1, "energia");

            // Controles J2 (Sólo si NO es IA)
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
            // Si el juego acabó o la IA está ocupada en animación, no hace nada
            if (juegoTerminado || p2Ocupado) return;

            // Número aleatorio del 0 al 5
            int accion = _iaRandom.Next(0, 6);

            switch (accion)
            {
                case 0: ProcesarAccion(P2, P1, VirtualKey.Up, "flojo", 15, 0.8, txtUp); break;
                case 1: ProcesarAccion(P2, P1, VirtualKey.Left, "fuerte", 40, 3.0, txtLeft); break;
                case 2: ProcesarAccion(P2, P1, VirtualKey.Down, "escudo", 15, 0, txtDown); break;
                case 3: ProcesarAccion(P2, P1, VirtualKey.Right, "relax", 0, 5.0, txtRight); break;
                case 4: UsarPocion(P2, "vida"); break;
                case 5: UsarPocion(P2, "energia"); break;
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
                case "escudo": if (user == P1) p1Protegido = true; else p2Protegido = true; user.verEscudo(true); txt.Opacity = 0.4; cooldownsActivos[k] = false; break;
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
                await Task.Delay(500);
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
            juegoTerminado = true; gameLoop?.Stop(); uiTimer?.Stop();
            _iaTimer?.Stop();
            perdedor.animacionDerrota();
            txtConsola.Text = $"¡GANADOR: {ganador.Nombre.ToUpper()}!";
            await Task.Delay(3000);
            GestionarMenuLateral(true); // Vuelve al acabar
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
            while (tiemposRestantes[k] > 0 && !juegoTerminado) { await Task.Delay(100); tiemposRestantes[k] -= 0.1; }
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
            if (tiemposRestantes.ContainsKey(k) && tiemposRestantes[k] > 0) txt.Text = $"{baseT} | {Math.Max(0, tiemposRestantes[k]):F1}s";
            else txt.Text = baseT;
        }
    }
}