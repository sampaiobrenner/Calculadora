using Calculadora;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Trabalho2.Properties;
using Trabalho2.Services;

namespace Trabalho2.Views
{
    public sealed partial class Calculadora : INotifyPropertyChanged
    {
        private readonly CalculadoraServices _calculadoraServices;
        private bool _conectadoAoServidor;
        private string _display;
        private string _displayedImagePath;
        private string _statusConexao;

        public Calculadora()
        {
            InitializeComponent();
            DataContext = this;
            _calculadoraServices = new CalculadoraServices();
            TestarConexaoComServidor();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool ConectadoAoServidor
        {
            get => _conectadoAoServidor;
            set
            {
                _conectadoAoServidor = value;
                OnPropertyChanged(nameof(ConectadoAoServidor));
            }
        }

        public string Display
        {
            get => _display;
            set
            {
                _display = value;
                OnPropertyChanged(nameof(Display));
            }
        }

        public string DisplayedImagePath
        {
            get => _displayedImagePath;
            set
            {
                _displayedImagePath = value;
                OnPropertyChanged(nameof(DisplayedImagePath));
            }
        }

        public string StatusConexao
        {
            get => _statusConexao;
            set
            {
                _statusConexao = value;
                OnPropertyChanged(nameof(StatusConexao));
            }
        }

        private decimal? Numero1 { get; set; }

        private decimal? Numero2 { get; set; }

        private decimal Resultado { get; set; }

        private CalculadoraUtils.Operacao TipoOperacao { get; set; }

        private void AtualizarDisplay(string valor) => Display += valor;

        private void BtnDividir_Click(object sender, RoutedEventArgs e) => DefinirOperacao(CalculadoraUtils.Operacao.Dividir);

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            LimparNumeros();
            LimparDisplay();
        }

        private void BtnMultiplicar_Click(object sender, RoutedEventArgs e) => DefinirOperacao(CalculadoraUtils.Operacao.Multiplicar);

        private void BtnNumero0_Click(object sender, RoutedEventArgs e) => AtualizarDisplay("0");

        private void BtnNumero1_Click(object sender, RoutedEventArgs e) => AtualizarDisplay("1");

        private void BtnNumero2_Click(object sender, RoutedEventArgs e) => AtualizarDisplay("2");

        private void BtnNumero3_Click(object sender, RoutedEventArgs e) => AtualizarDisplay("3");

        private void BtnNumero4_Click(object sender, RoutedEventArgs e) => AtualizarDisplay("4");

        private void BtnNumero5_Click(object sender, RoutedEventArgs e) => AtualizarDisplay("5");

        private void BtnNumero6_Click(object sender, RoutedEventArgs e) => AtualizarDisplay("6");

        private void BtnNumero7_Click(object sender, RoutedEventArgs e) => AtualizarDisplay("7");

        private void BtnNumero8_Click(object sender, RoutedEventArgs e) => AtualizarDisplay("8");

        private void BtnNumero9_Click(object sender, RoutedEventArgs e) => AtualizarDisplay("9");

        private void BtnResultado_Click(object sender, RoutedEventArgs e)
        {
            DefinirNumero();
            ProcessarResultado();
            LimparNumeros();
        }

        private void BtnSomar_Click(object sender, RoutedEventArgs e) => DefinirOperacao(CalculadoraUtils.Operacao.Somar);

        private void BtnSubtrair_Click(object sender, RoutedEventArgs e) => DefinirOperacao(CalculadoraUtils.Operacao.Subtrair);

        private void BtnVirgula_Click(object sender, RoutedEventArgs e) => AtualizarDisplay(",");

        private void DefinirNumero()
        {
            var display = Display?.Replace(",", ".");
            if (string.IsNullOrEmpty(display)) return;

            decimal.TryParse(Display, out var numeroAtual);

            if (Numero2.HasValue)
            {
                Numero1 = Resultado;
                Numero2 = null;
            }

            if (Numero1 is null)
                Numero1 = numeroAtual;
            else
                Numero2 = numeroAtual;
        }

        private void DefinirOperacao(CalculadoraUtils.Operacao operacao)
        {
            DefinirNumero();
            TipoOperacao = operacao;
            LimparDisplay();
        }

        private void LimparDisplay() => Display = string.Empty;

        private void LimparNumeros()
        {
            Numero1 = null;
            Numero2 = null;
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void ProcessarResultado()
        {
            if (Numero1 is null || Numero1 is 0 ||
                Numero2 is null || Numero2 is 0) return;

            var numero1 = Numero1.Value;
            var numero2 = Numero2.Value;

            new Task(CarregarResultadoAsync).Start();

            void CarregarResultadoAsync()
            {
                Display = "Carregando...";
                Resultado = _calculadoraServices.ProcessarResultado(numero1, numero2, TipoOperacao);
                Display = $"{Resultado}".Replace(",00", "");
            }
        }

        private void TestarConexaoComServidor()
        {
            new Task(TestarConexaoComServidorAsync).Start();

            void TestarConexaoComServidorAsync()
            {
                while (true)
                {
                    ConectadoAoServidor = _calculadoraServices.TestarConexaoComServidor();

                    StatusConexao = ConectadoAoServidor
                        ? Properties.Resources.ConectadoAoServidor
                        : Properties.Resources.SemConexaoComOServidor;

                    DisplayedImagePath = ConectadoAoServidor
                        ? Directory.GetCurrentDirectory() + "\\Resources\\conectado.ico"
                        : Directory.GetCurrentDirectory() + "\\Resources\\desconectado.ico";
                }
            }
        }
    }
}