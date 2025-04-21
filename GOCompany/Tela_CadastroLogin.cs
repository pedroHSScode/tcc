using GO;
using System.Text.RegularExpressions;

namespace WinFormsApp2
{
    // tive a ideia de botar bool na tabela pessoas para dizer se a senha � forte ou fraca - Pedro 23/09
    /*
    -retirei o cpf.length do InitializeComponent e o alterei manualmente nas propriedades
    -o evento cpf.KeyPress foi adicionado para aceitar apenas d�gitos (mais sobre comentado na fun��o)
    -ToolTip adicionado para utilizar no evento MouseHover, mas nn precisa est� nesse evento, basta existir, mas isso � feito para ficar um c�digo did�tico
    -GerarCPF foi adicionado para completar os 2 ultimos digitos. Est� sendo utilizado no evento completarCPF_Click
    -n�o consegui fazer o 9� digito baseado no estado
    -criei a classe ConsultasMySQL para termos mais controles sobre como cada dado � inserido na tabela.
    -Ainda falta o hash na senha

    /*
    13/10/2024 2:30
    -fiz a fun��o de pegar o txt e retornar o nono digito do estado que esta no txt 
    -finalizei o funcionamento do cpf
    -cpf est� acolhendo rg do estado e o calculo do 10� e 11� digito corretamente
    -falta mostrar o tooltip para quando o cpf j� estiver completo ou n�o
    obs: irei me basear no bot�o cadastrar para dizer se a tela � cadastrar ou entrar
    */


    /*
     pr�xima etapa:
     1- ao cadastrar validar se o email j� est� sendo usado ou n�o(tela cadastro)
     2- ao entrar validar se a senha e email est�o corretos, mas precisa ser avaliando a row(tela entrar)
     */

    public partial class Tela_de_Cadastro : Form
    {
        int idade = 0;// variavel publica que � usada no dataNasc_value para inserir a idade no banco de dados

        ToolTip toolTip;
        string conectarAoBanco = @"Server=localhost;Database=gocompany;User ID=root;Password=;";
        ConsultasMySQL consultas;

        bool senhaFraca = false;
        bool senhaMedia = false;
        bool senhaForte = false;
        public Tela_de_Cadastro()
        {
            InitializeComponent();
            generos.DropDownStyle = ComboBoxStyle.DropDownList;

            consultas = new ConsultasMySQL(conectarAoBanco);
            toolTip = new ToolTip();

            string[] estados = new string[]
           {
                "Acre",
                "Alagoas",
                "Amap�",
                "Amazonas",
                "Bahia",
                "Cear�",
                "Distrito Federal",
                "Esp�rito Santo",
                "Goi�s",
                "Maranh�o",
                "Mato Grosso",
                "Mato Grosso do Sul",
                "Minas Gerais",
                "Par�",
                "Para�ba",
                "Paran�",
                "Pernambuco",
                "Piau�",
                "Rio de Janeiro",
                "Rio Grande do Norte",
                "Rio Grande do Sul",
                "Rond�nia",
                "Roraima",
                "Santa Catarina",
                "S�o Paulo",
                "Sergipe",
                "Tocantins",

           };

            // Adiciona os pa�ses ao ComboBox
            pais.Items.AddRange(estados);

        }

        private void label2_Click(object sender, EventArgs e)
        {


            panel1.Visible = false;
            panel2.Visible = false;
            panel4.Visible = false;
            panel3.Location = new Point(273, 192);
            cadastrar.Location = new Point(352, 322);
            cadastrar.Text = "Entrar";

        }
        private void cadastro_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel2.Visible = true;
            panel4.Visible = true;
            panel3.Location = new Point(161, 114);
            cadastrar.Location = new Point(388, 362);
            cadastrar.Text = "Cadastrar";
        }
        private void cpf_Keydown(object sender, KeyEventArgs e)
        {
            string txt = cpf.Text;

            if (e.KeyCode == Keys.Enter && cpf.Text.All(char.IsDigit) && cpf.Text.Length == 11)
            {
                txt = txt.Insert(3, ".");
                txt = txt.Insert(7, ".");
                txt = txt.Insert(11, "-");

                cpf.Text = txt;

                e.Handled = true;
            }

        }

        private void rg_Keydown(object sender, KeyEventArgs e)
        {
            string txt = rg.Text;

            if (e.KeyCode == Keys.Enter && rg.Text.All(char.IsDigit) && rg.Text.Length == 9)
            {
                txt = txt.Insert(2, ".");
                txt = txt.Insert(6, ".");
                txt = txt.Insert(10, "-");

                rg.Text = txt;

                e.Handled = true;
            }
        }

        private void cadastrar_Click(object sender, EventArgs e)
        {
            bool nomeConfirmado = false;
            bool senhaConfirmado = false;
            bool emailConfirmado = false;
            bool cpfConfirmado = false;
            bool rgConfirmado = false;
            bool estadoConfirmado = false;
            bool generoConfirmado = false;
            bool idadeConfirmado = false;
            if (cadastrar.Text == "Cadastrar")
            {
                //para o nome//
                if (nome.Text.Length > 0)
                {
                    nomeConfirmado = true;
                }
                else
                {
                    MessageBox.Show("O campo nome est� vazio",
                       "Houve algum erro",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Stop);
                    nomeConfirmado = false;
                }
                //////////fim//////////
                //para a senha
                string tip = "";
                if (senhaFraca)
                    tip = "Fraca";
                else if (senhaMedia)
                    tip = "Media";
                else if (senhaForte)
                    tip = "Forte";

                if (senha.Text == repetirSenha.Text)
                {
                    if (senha.Text.Length >= 4)
                    {
                        senhaConfirmado = true;
                    }
                    else
                    {
                        MessageBox.Show("A senha precisa ter 4 ou mais caracteres",
                        "Houve algum erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Stop);
                        senhaConfirmado = false;
                    }
                }
                else
                    MessageBox.Show("As senhas precisam ser iguai nos campos 'senha' e 'repetir senha'",
                        "Houve algum erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Stop);
                //////////fim//////////

                //para o email//
                string validarEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!consultas.Verificar_redundancias_de_dados(email.Text))
                {
                    if (Regex.IsMatch(email.Text, validarEmail))
                        emailConfirmado = true;
                    else
                    {
                        MessageBox.Show("O campo email est� em formato incorreto. exemplo de email correto: usuario@email.com",
                            "Dado(s) incorretos",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Stop);
                        emailConfirmado = false;
                    }
                }
                else
                    MessageBox.Show("Este email j� est� em uso",
                                "Houve algum erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Stop);
                //////////fim//////////
                //para cpf//
                if (cpf.Text.Length == 11)
                {
                    cpfConfirmado = true;
                }
                else
                {
                    MessageBox.Show("o campo cpf tem menos ou mais de 11 d�gitos",
                        "Houve algum erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Stop);
                    cpfConfirmado = false;
                }
                /////////fim//////////

                //para o rg//
                if (rg.Text.Length == 12)
                    rgConfirmado = true;
                else
                {
                    MessageBox.Show("o campo rg tem menos ou mais de 12 d�gitos",
                            "Houve algum erro",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Stop);
                    rgConfirmado = false;
                }

                //////////fim//////////

                //para o estado//

                string? estado = (pais.SelectedItem != null) ? pais.SelectedItem.ToString() : string.Empty;
                if (estado != string.Empty)
                    estadoConfirmado = true;
                else
                {
                    estadoConfirmado = false;
                    MessageBox.Show("nenhum estado selecionado",
                     "Houve algum erro",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Stop);

                }

                //////////fim//////////
                //para o genero//
                if (outrosGeneros.Text != string.Empty)
                    generoConfirmado = true;
                else
                {
                    generoConfirmado = false;
                    MessageBox.Show("nenhum g�nero selecionado",
                     "Houve algum erro",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Stop);
                }

                //////////fim//////////
                //para a idade e datanasc//
                if (idade > 17)
                    idadeConfirmado = true;
                else
                {
                    idadeConfirmado = false;
                    MessageBox.Show("Voc� � muito novo(a) para se cadastrar neste site",
                     "Houve algum erro",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Stop);

                }

                /////////fim/////////

                //adicionar a porra toda no banco//

                if ((senhaConfirmado) && (nomeConfirmado) && (emailConfirmado) && (cpfConfirmado) && (rgConfirmado) && (estadoConfirmado)
                     && (generoConfirmado) && (idadeConfirmado))
                {
                    consultas.Inserir_Na_Tabela_Clientes(nome.Text, senha.Text, tip, email.Text, cpf.Text, rg.Text, estado, outrosGeneros.Text,
                        dataNasc.Value, idade);
                    MessageBox.Show("Agradecemos por escolher nossa compania. Enviamos um email de Conscientiza��o",
                    "Cadastro efetuado com sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                }
                /////////fim//////////
            }
            // ou acessar conta(tentar)//

            else if (cadastrar.Text == "Entrar")
            {
                bool validar = false;
                validar = consultas.Acessar_Conta_Cliente(email.Text, senha.Text);
                if (validar)
                {
                    Menu_Principal telaMP = new Menu_Principal();
                    this.Hide();
                    telaMP.Show();
                    ClienteLogado.EmailClienteLogado = email.Text;
                }
                else
                    MessageBox.Show("Email ou senha incorreto(s)",
                        "Houve algum erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
            }
        }


        private void cpf_KeyPress(object sender, KeyPressEventArgs e)
        {//tudo certo
            //limita a quantidade de caracteres para permitir que a fun��o GerarCPF seja executada sem problemas e permite apenas n�meros - Pedro 23/09 
            if (cpf.Text.Length < 8)
            {
                if (!char.IsDigit(e.KeyChar))
                    e.Handled = true;
                else
                {
                    e.Handled = false;
                }
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("Na inten��o de seguir as regras do detran sem a exposi��o de dados pessoais, voc� deve " +
                    "pressionar o bot�o 'OK' para completar os 3 n�meros restantes " +
                    "(baseado no estado de seu rg e o c�lculo do 10� e 11� d�gito) de seu cpf",
                    "manuseamento de dados sens�veis",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }



        private void completarCPF_Click(object sender, EventArgs e)
        {//tudo certo
            string estado = pais.SelectedItem?.ToString() ?? "nulo";
            int valor = nonoDigito(estado);
            bool limitar_entrada = false;//usado para n�o colocar mais de 1 digito no cpf
            bool validar_Estado = false;

            if ((!validar_Estado) && (valor < 10) && (cpf.Text.Length == 8))
            {
                cpf.Text += valor.ToString();
                validar_Estado = true;
            }

            if (!limitar_entrada)
            {
                if (cpf.Text.Length == 9)
                {

                    cpf.Text = ManipularCpf(cpf.Text);
                    cpf.Enabled = false;
                    limitar_entrada = true;
                }
            }
            else
                MessageBox.Show("Este bot�o n�o poder� ser usado novamente",
                    "manuseamento de dados sens�veis",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

        }

        private void completarCPF_MouseHover(object sender, EventArgs e)
        {//tudo certo
            if (cpf.Enabled)
                toolTip.Show("Completa o cpf com base o DDD e determina os 2 �ltimos d�gitos", completarCPF);
        }
        private string ManipularCpf(string txt)
        {//tudo certo
            int d1 = 0;
            int d2 = 0;
            int cont = 10;
            for (int i = 0; i <= 8; i++)
                d1 += (txt[i] - '0') * cont--;//o -'0' � para considerar que quero a string que est� naquele vetor da string, e nn o valor ascii

            d1 = (d1 % 11 < 2) ? 0 : 11 - (d1 % 11);
            txt += d1.ToString();
            cont = 11;
            for (int i = 0; i <= 9; i++)
                d2 += (txt[i] - '0') * cont--;

            d2 = (d2 % 11 < 2) ? 0 : 11 - (d2 % 11);
            txt += d2.ToString();
            return txt;

        }

        private void senha_Leave(object sender, EventArgs e)
        {//tudo certo
            //uma fun��o para dizer se a senha � fraca, m�dia ou forte.
            int val = ValidarSenha(senha.Text);
            switch (val)
            {
                case < 3://fraca
                    senhaFraca = true;
                    senhaMedia = false;
                    senhaForte = false;
                    break;
                case 3://media
                    senhaFraca = false;
                    senhaMedia = true;
                    senhaForte = false;
                    break;
                case 4://forte
                    senhaFraca = false;
                    senhaMedia = false;
                    senhaForte = true;
                    break;
            }



        }
        private int ValidarSenha(string txt)
        {//tudo certo
            // fun��o que ser� usada no Senha_Leave para validar cada caractere
            bool numero = false;
            bool maiusculo = false;
            bool minusculo = false;
            bool especial = false;

            int medidor = 0;
            int tam = txt.Length;
            for (int i = 0; i < tam; i++)
            {
                if ((char.IsDigit(txt[i])) && (!numero))
                {
                    medidor++;
                    numero = true;
                }
                else if ((char.IsLower(txt[i])) && (!minusculo))
                {
                    medidor++;
                    minusculo = true;
                }
                else if ((char.IsUpper(txt[i])) && (!maiusculo))
                {
                    medidor++;
                    maiusculo = true;
                }
                else if ((char.IsPunctuation(txt[i]) || char.IsSymbol(txt[i])) && (!especial))
                {
                    medidor++;
                    especial = true;
                }
            }
            return medidor;
        }
        public int nonoDigito(string txt)
        {
            int nonodg = 0;

            switch (txt)
            {
                case "Distrito Federal":
                case "Goi�s":
                case "Mato Grosso":
                case "Mato Grosso do Sul":
                case "Tocantins":
                    nonodg = 1;
                    break;

                case "Par�":
                case "Amazonas":
                case "Acre":
                case "Amap�":
                case "Rond�nia":
                case "Roraima":
                    nonodg = 2;
                    break;

                case "Cear�":
                case "Maranh�o":
                case "Piau�":
                    nonodg = 3;
                    break;

                case "Pernambuco":
                case "Rio Grande do Norte":
                case "Para�ba":
                case "Alagoas":
                    nonodg = 4;
                    break;


                case "Bahia":
                case "Sergipe":
                    nonodg = 5;
                    break;


                case "Minas Gerais":
                    nonodg = 6;
                    break;


                case "Rio de Janeiro":
                case "Esp�rito Santo":
                    nonodg = 7;
                    break;


                case "S�o Paulo":
                    nonodg = 8;
                    break;


                case "Paran�":
                case "Santa Catarina":
                    nonodg = 9;
                    break;


                case "Rio Grande do Sul":
                    nonodg = 0;
                    break;

                default:
                    MessageBox.Show("Nenhum n�mero foi encontrado para o estado fornecido.",
                        "Erro",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                    nonodg = 111;
                    break;

            }
            if (nonodg != 111)
            {
                return nonodg;
            }
            else
                return nonodg;
        }

        private void cpf_MouseHover(object sender, EventArgs e)
        {
            if (!cpf.Enabled)
            {
                toolTip.Show("N�o � possivel modificar seu cpf provis�rio", cpf);
                toolTip.Show("N�o � possivel modificar seu cpf provis�rio", completarCPF);
            }
        }


        private void generos_SelectedValueChanged(object sender, EventArgs e)
        {

            if (generos.SelectedItem != null)
            {
                if (generos.SelectedItem.ToString() == "Outros")
                {
                    outrosGeneros.Text = string.Empty;
                    outrosGeneros.Enabled = true;
                    outrosGeneros.Visible = true;
                }
                else
                {
                    outrosGeneros.Text = generos.SelectedItem.ToString();
                    outrosGeneros.Enabled = false;
                    outrosGeneros.Visible = false;
                }
            }
        }

        private void dataNasc_Leave(object sender, EventArgs e)
        {
            int anoAtual = DateTime.Now.Year;
            int mesAtual = DateTime.Now.Month, diaAtual = DateTime.Now.Day;
            idade = anoAtual - dataNasc.Value.Year;
            if ((dataNasc.Value.Month >= mesAtual) && (dataNasc.Value.Day > diaAtual))
                idade -= 1;
            lbidade.Text = $"idade:{idade}";
        }

        private void generos_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lbEntrarTelaFuncionario_Click(object sender, EventArgs e)
        {
            Tela_Entrar_Funcionario TelaEF =  new Tela_Entrar_Funcionario();
            this.Hide();
            TelaEF.Show();
        }
    }
}

/*
                if (char.IsUpper(txt[i]))
                    Grande = true;
                if (char.IsDigit(txt[i]))
                    Numero = true;
                if (char.IsLower(txt[i]))
                    Pequeno = true;
                if ((!char.IsLetterOrDigit(txt[i])) && (!char.IsWhiteSpace(txt[i])))
                    Especial = true;
*/

//43.980.544-2

/*if (pais.SelectedItem != null)
 {
     estado = pais.SelectedItem.ToString();
     MessageBox.Show(estado);
 }
e coloca esse c�digo logo abaixo do partial class

 string estado = "";
a� dps tu cria o evento e coloca esse mesmo codigo  dentro

if (pais.SelectedItem != null)
 {
     estado = pais.SelectedItem.ToString();
     MessageBox.Show(estado);
 } 
*/