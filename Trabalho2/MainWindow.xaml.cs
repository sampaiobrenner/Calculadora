using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Trabalho2;
using Trabalho2.Annotations;

namespace Trabalho
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly CalculadoraServices _calculadoraServices;

        private string _display;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            _calculadoraServices = new CalculadoraServices();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Display
        {
            get => _display;
            set
            {
                _display = value;
                OnPropertyChanged(nameof(Display));
            }
        }

        private decimal? Numero1 { get; set; }
        private decimal? Numero2 { get; set; }
        private decimal Resultado { get; set; }
        private CalculadoraServices.Operacao? TipoOperacao { get; set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void AtualizarDisplay(string valor) => Display += valor;

        private void BtnDividir_Click(object sender, RoutedEventArgs e) => DefinirOperacao(CalculadoraServices.Operacao.Dividir);

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            LimparNumeros();
            LimparDisplay();
        }

        private void BtnMultiplicar_Click(object sender, RoutedEventArgs e) => DefinirOperacao(CalculadoraServices.Operacao.Multiplicar);

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

        private void BtnSomar_Click(object sender, RoutedEventArgs e) => DefinirOperacao(CalculadoraServices.Operacao.Somar);

        private void BtnSubtrair_Click(object sender, RoutedEventArgs e) => DefinirOperacao(CalculadoraServices.Operacao.Subtrair);

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

        private void DefinirOperacao(CalculadoraServices.Operacao operacao)
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

        private void ProcessarResultado()
        {
            if (Numero1 is null || Numero1 is 0 ||
                Numero2 is null || Numero2 is 0) return;

            var numero1 = Numero1.Value;
            var numero2 = Numero2.Value;

            if (TipoOperacao.HasValue) Task.Factory.StartNew(CarregarResultadoAsync);

            void CarregarResultadoAsync()
            {
                Display = "Carregando...";
                Resultado = _calculadoraServices.ProcessarResultado(numero1, numero2, TipoOperacao.Value);
                Display = $"{Resultado}".Replace(",00", "");
            }
        }
    }
}